using SpireCLI.Commands;


// 1) Build Command Manager
var manager = CommandManagerBuilder.BuildCommandManager();

if (args.Length == 0)
    args = new string[] { "--interactive" };

// 2) Run
return manager.Run(args).ExitCode;
