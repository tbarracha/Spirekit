using SpireCLI.Commands;


// 1) Build Command Manager
var manager = CommandManagerBuilder.BuildCommandManager();

// 2) Run
return manager.Run(args);
