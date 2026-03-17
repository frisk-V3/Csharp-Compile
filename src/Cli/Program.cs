using System.CommandLine;
using csharpcompile.Core;

var root = new RootCommand("csharpcompile - cross platform builder");

var publish = new Command("publish", "Publish project");

var targetOption = new Option<string>(
    "--target",
    () => "win-x64",
    "Runtime Identifier (RID)"
);

var projectOption = new Option<string>(
    "--project",
    () => ".",
    "Path to project or folder"
);

publish.AddOption(targetOption);
publish.AddOption(projectOption);

publish.SetHandler((target, project) =>
{
    Builder.Publish(project, target);
}, targetOption, projectOption);

root.AddCommand(publish);

return await root.InvokeAsync(args);
