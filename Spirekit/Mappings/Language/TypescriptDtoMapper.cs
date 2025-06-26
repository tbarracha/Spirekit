// -----------------------------------------------------------------------------
// Author: Tiago Barracha <ti.barracha@gmail.com>
// Created with AI assistance (ChatGPT)
// Description: Converts .NET DTO classes into TypeScript interfaces.
// Useful for generating frontend-friendly models to match backend DTOs,
// ensuring type safety and reducing duplication between client and server.
//
// -----------------------------------------------------------------------------
//
// USAGE:
//
// var zipBytes = TypescriptDtoMapper.GenerateDtosAsZip();
// File.WriteAllBytes("typescript-dtos.zip", zipBytes);
//
// Optionally, provide a root folder path:
// var zip = TypescriptDtoMapper.GenerateDtosAsZip("C:/MyProject/Contracts/Dtos");
//
// This will scan for all *Dto.cs files, convert them into .ts interfaces,
// and bundle them into a downloadable ZIP archive.
//
// -----------------------------------------------------------------------------
//
// NOTES:
// - Nullable C# properties are translated as union types (e.g., null | string).
// - Lists are converted into TypeScript arrays.
// - Unknown types fall back to their original C# name and may need manual review.
// -----------------------------------------------------------------------------


using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Spirekit.Mappings.Language;

/// <summary>
/// Converts .NET DTO classes into TypeScript interfaces.
/// Useful for generating frontend-friendly models to match backend DTOs,
/// ensuring type safety and reducing duplication between client and server.
/// </summary>
public static class TypescriptDtoMapper
{
    private static readonly Dictionary<string, string> TypeMap = new()
    {
        ["string"] = "string",
        ["int"] = "number",
        ["long"] = "number",
        ["double"] = "number",
        ["float"] = "number",
        ["decimal"] = "number",
        ["bool"] = "boolean",
        ["Guid"] = "string",
        ["DateTime"] = "string",
    };

    public static byte[] GenerateDtosAsZip(string? rootOverride = null)
    {
        var basePath = GetDtosRootPath(rootOverride);

        var tempDir = Path.Combine(Path.GetTempPath(), $"ts-dtos-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        var dtoFiles = Directory.GetFiles(basePath, "*Dto.cs", SearchOption.AllDirectories);
        foreach (var file in dtoFiles)
        {
            var relativePath = Path.GetRelativePath(basePath, Path.GetDirectoryName(file)!);
            var tsName = Path.GetFileNameWithoutExtension(file);
            var fileContent = ConvertDtoToTypescript(File.ReadAllLines(file), tsName);

            var destDir = Path.Combine(tempDir, relativePath);
            Directory.CreateDirectory(destDir);
            File.WriteAllText(Path.Combine(destDir, ToKebabCase(tsName) + ".ts"), fileContent);
        }

        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in Directory.GetFiles(tempDir, "*.ts", SearchOption.AllDirectories))
            {
                var entryPath = Path.GetRelativePath(tempDir, file).Replace("\\", "/");
                var entry = archive.CreateEntry(entryPath);

                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(file);
                fileStream.CopyTo(entryStream);
            }
        }

        Directory.Delete(tempDir, true);
        ms.Position = 0;
        return ms.ToArray();
    }

    private static string GetDtosRootPath(string? rootOverride)
    {
        if (!string.IsNullOrWhiteSpace(rootOverride))
        {
            var cleaned = rootOverride.Trim().Trim('"')
                              .Replace('/', Path.DirectorySeparatorChar);
            cleaned = Path.GetFullPath(cleaned);

            if (!Directory.Exists(cleaned))
                throw new DirectoryNotFoundException(
                    $"Provided Dtos rootOverride does not exist: {cleaned}");
            return cleaned;
        }

        // Locate the Contracts assembly on disk
        var assemblyPath = typeof(TypescriptDtoMapper).Assembly.Location;
        var assemblyDir = Path.GetDirectoryName(assemblyPath)!;

        string root;
        if (assemblyDir.Split(Path.DirectorySeparatorChar)
                    .Any(p => p.Equals("bin", StringComparison.OrdinalIgnoreCase)))
        {
            // Running from bin/Debug/netX => climb up to project root
            root = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", ".."));
        }
        else
        {
            // Already at project root
            root = assemblyDir;
        }

        var dtoPath = Path.Combine(root, "Dtos");
        if (!Directory.Exists(dtoPath))
            throw new DirectoryNotFoundException(
                $"Unable to locate Dtos folder at: {dtoPath}");

        return Path.GetFullPath(dtoPath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
    }

    private static string ConvertDtoToTypescript(string[] lines, string className)
    {
        var props = new List<string>();
        var seen = new HashSet<string>(); // To avoid duplicates

        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"public\s+([\w<>\[\]\?]+)\s+(\w+)\s*\{");
            if (!match.Success) continue;

            var csType = match.Groups[1].Value.Replace("?", "").Trim();
            var propName = match.Groups[2].Value;
            var tsPropName = ToCamelCase(propName);

            // Skip if already processed
            if (!seen.Add(tsPropName)) continue;

            var tsType = match.Groups[1].Value.Contains("?") ? "null | " : "";

            if (csType.StartsWith("List<"))
            {
                var inner = Regex.Match(csType, @"List<(\w+)>").Groups[1].Value;
                tsType += MapType(inner) + "[]";
            }
            else
            {
                tsType += MapType(csType);
            }

            props.Add($"  {tsPropName}: {tsType};");
        }

        return $"// Auto-generated from {className}.cs\n" +
               $"export interface {className} {{\n" +
               string.Join("\n", props) +
               "\n}\n";
    }

    private static string MapType(string csType) =>
        TypeMap.TryGetValue(csType, out var ts) ? ts : csType;

    private static string ToCamelCase(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToLower(s[0]) + s[1..];

    private static string ToKebabCase(string s) =>
        Regex.Replace(s, "([a-z])([A-Z])", "$1-$2").ToLower();
}
