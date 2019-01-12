var target = Argument("target", "Default");
var configuration = Argument("Configuration", "Release");

Information($"Running target {target} in configuration {configuration}");

var projectName = "AlbaVulpes.API";
var distDirectory = Directory("./dist");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(distDirectory);
    });

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("Build")
	.Does(() =>
	{
		DotNetCoreBuild(".",
			new DotNetCoreBuildSettings()
			{
				Configuration = configuration,
				ArgumentCustomization = args => args.Append("--no-restore"),
			});
	});

Task("PublishDist")
    .Does(() =>
    {
        DotNetCorePublish(
            $"./src/{projectName}.csproj",
            new DotNetCorePublishSettings()
            {
                Configuration = configuration,
                OutputDirectory = distDirectory,
                ArgumentCustomization = args => args.Append("--no-restore"),
            });
    });

Task("FullBuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");

Task("Default")
    .IsDependentOn("FullBuild")
    .IsDependentOn("PublishDist");
	
RunTarget(target);