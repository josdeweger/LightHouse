#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target              = Argument("target", "Default");
var configuration       = Argument("configuration", "Release");
var solutionFolderPath  = "./src/";
var solution            = solutionFolderPath + "LightHouse.sln";
var consoleAppPath      = solutionFolderPath + "LightHouse/";
var artifactsFolderPath = "./artifacts/";
var chocolateyApiKey    = EnvironmentVariable("CHOCOLATEY_API_KEY");
var version             = "1.0.1";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory(solutionFolderPath + "bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsFolderPath);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration
        };

        DotNetCoreBuild(solution, settings);
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles(solutionFolderPath + "*.Specs/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true
                });
        }
    });

Task("Create-Executable")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() => {
        var settings = new DotNetCorePublishSettings
        {
            Framework = "netcoreapp2.1",
            Configuration = "Release",
            OutputDirectory = artifactsFolderPath,
            SelfContained = true,
            Runtime = "win10-x64"
        };

        DotNetCorePublish(consoleAppPath, settings);
    });

Task("Create-Chocolatey-Package")
    .IsDependentOn("Create-Executable")
    .Does(() => {
        var settings   = new ChocolateyPackSettings 
        {                                     
            Debug                   = true,
            Verbose                 = true,
            OutputDirectory         = Directory(artifactsFolderPath),
            Version                 = version
        };

        ChocolateyPack("./Lighthouse.nuspec", settings);
    });

Task("Push-Chocolatey-Package")
    .IsDependentOn("Create-Chocolatey-Package")
    .Does(() => {
        var settings = new ChocolateyPushSettings 
        {
            Source                = "https://push.chocolatey.org/",
            ApiKey                = chocolateyApiKey,
            Debug                 = true,
            Verbose               = true
        };
        
        var packages = GetFiles(artifactsFolderPath + "*.nupkg");

        foreach(var package in packages) 
        {            
            ChocolateyPush(package, settings);
        }
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Push-Chocolatey-Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
