using SpireCLI.Commands.ProjectManagement.Core;
using SpireCore.Commands;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;

namespace SpireCLI.Commands.ProjectManagement.Backend;

/// <summary>
/// Runs the Spire *.Host project and grabs the generated Swagger JSON.
/// Usage:
///   spire get-swagger [outputPath]
/// If <see cref="outputPath"/> is omitted, "swagger.json" is created in the current folder.
/// </summary>
public sealed class GetSwaggerCommand : BaseSpireProjectCommand
{
    public override string Name => "get-swagger";
    public override string Description => "Runs the Host project and downloads /swagger/v1/swagger.json";
    public override IEnumerable<string> Aliases =>
        new[] { "swagger", "swagger-json", "generate-swagger" };

    public override CommandResult Execute(CommandContext context)
    {
        return RunAsync(() => ExecuteAsync(context));
    }

    private async Task<CommandResult> ExecuteAsync(CommandContext context)
    {
        // -------- 1. Load active solution config ---------------------------------
        var config = SpireProjectConfigManager.LoadConfig();
        if (config == null)
            return CommandResult.Error("No active Spire solution configured. Run `spire set-solution` first.");

        var hostProj = config.Projects.FirstOrDefault(p =>
            p.Name.EndsWith(".Host", StringComparison.OrdinalIgnoreCase));
        if (hostProj == null)
            return CommandResult.Error("*.Host project not found in the active solution.");

        // -------- 2. Pick a random free port -------------------------------------
        var port = GetUnusedPort();
        var url = $"http://localhost:{port}";

        // -------- 3. Start `dotnet run` for the Host -----------------------------
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --urls {url}",
            WorkingDirectory = Path.GetDirectoryName(hostProj.Path)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            Environment =
            {
                ["ASPNETCORE_URLS"] = url
            }
        };
        var proc = Process.Start(psi);
        if (proc == null)
            return CommandResult.Error("Failed to start Host project.");

        try
        {
            // -------- 4. Poll /swagger/v1/swagger.json until it responds ----------
            var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var swaggerEndpoint = $"{url}/swagger/v1/swagger.json";
            var maxWait = TimeSpan.FromSeconds(30);
            var start = DateTime.UtcNow;

            while (true)
            {
                if (DateTime.UtcNow - start > maxWait)
                    return CommandResult.Error("Host did not start within 30 s.");

                try
                {
                    var res = await http.GetAsync(swaggerEndpoint);
                    if (res.IsSuccessStatusCode)
                    {
                        var json = await res.Content.ReadAsStringAsync();

                        // -------- 5. Write output -----------------------------------
                        var outPath = context.Args.ElementAtOrDefault(0);

                        if (string.IsNullOrWhiteSpace(outPath))
                        {
                            var hostDir = Path.GetDirectoryName(hostProj.Path)!;
                            outPath = Path.Combine(hostDir, "swagger.json");
                        }

                        await File.WriteAllTextAsync(outPath, JsonDocument
                            .Parse(json)                          // pretty-print
                            .RootElement
                            .GetRawText());

                        try { if (!proc.HasExited) proc.Kill(entireProcessTree: true); } catch { /* ignore */ }

                        return CommandResult.Success($"Swagger JSON saved to {outPath}");
                    }
                }
                catch { /* not ready yet */ }

                await Task.Delay(500);
            }
        }
        finally
        {
            // -------- 6. Ensure the Host process is killed ------------------------
            try { if (!proc.HasExited) proc.Kill(entireProcessTree: true); } catch { /* ignore */ }
        }
    }

    // Grab an unused localhost port
    private static int GetUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
