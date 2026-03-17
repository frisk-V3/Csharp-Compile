using System.CommandLine;
using csharpcompile.Core;

var root = new RootCommand("csharpcompile - cross platform builder");

var publish = new Command("publish", "Publish project");

// 複数ターゲット
var targetsOption = new Option<string>(
    "--targets",
    () => "win-x64",
    "Comma separated RIDs"
);

// 出力先
var outputOption = new Option<string>(
    "--output",
    () => "dist",
    "Output directory"
);

// プロジェクト
var projectOption = new Option<string>(
    "--project",
    () => ".",
    "Project path"
);

// AOT
var aotOption = new Option<bool>(
    "--aot",
    () => false,
    "Enable NativeAOT"
);

publish.AddOption(targetsOption);
publish.AddOption(outputOption);
publish.AddOption(projectOption);
publish.AddOption(aotOption);

publish.SetHandler((targets, output, project, aot) =>
{
    Builder.PublishMulti(project, targets, output, aot);
}, targetsOption, outputOption, projectOption, aotOption);

root.AddCommand(publish);

return await root.InvokeAsync(args);
