using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Serilog.Log;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    GitHubActions GitHubActions => GitHubActions.Instance;

    string BranchSpec => GitHubActions?.Ref;

    string BuildNumber => GitHubActions?.RunNumber.ToString();

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [GitVersion(Framework = "net10.0", NoFetch = true, NoCache = true)]
    readonly GitVersion GitVersion;

    AbsolutePath ArtifactsDirectory => RootDirectory / "Artifacts";

    AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";

    string SemVer;

    Target CalculateVersion => _ => _
        .Executes(() =>
        {
            SemVer = GitVersion.SemVer;
            if (IsPullRequest)
            {
                Information(
                    "Branch spec {branchspec} is a pull request. Adding build number {buildnumber}",
                    BranchSpec, BuildNumber);

                SemVer = string.Join('.', GitVersion.SemVer.Split('.').Take(3).Union(new[]
                {
                    BuildNumber
                }));
            }

            Information("SemVer = {semver}", SemVer);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .EnableNoCache());
        });

    Target Compile => _ => _
        .DependsOn(CalculateVersion, Restore)
        .Executes(() =>
        {
            ReportSummary(s => s
                .WhenNotNull(SemVer, (summary, semVer) => summary
                    .AddPair("Version", semVer)));

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoLogo()
                .EnableNoRestore()
                .SetVersion(SemVer)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            TestResultsDirectory.CreateOrCleanDirectory();

            var testProjects = Solution.AllProjects
                .Where(p => p.Name.EndsWith(".Specs", StringComparison.OrdinalIgnoreCase))
                .ToList();

            DotNetTest(s => s
                // Run tests in debug mode so FluentAssertions can show variable names
                .SetConfiguration(Configuration.Debug)
                // Prevent machine language from affecting culture-sensitive tests
                .SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
                .SetDataCollector("XPlat Code Coverage")
                .SetResultsDirectory(TestResultsDirectory)
                .EnableNoRestore()
                .CombineWith(testProjects, (ss, project) => ss
                    .SetProjectFile(project)
                    .AddLoggers($"trx;LogFileName={project.Name}.trx")));
        });

    Target GenerateCodeCoverageReport => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            ReportGenerator(s => s
                .AddReports(TestResultsDirectory / "**/coverage.cobertura.xml")
                .AddReportTypes(ReportTypes.lcov, ReportTypes.Html)
                .SetTargetDirectory(TestResultsDirectory / "reports")
                .AddFileFilters("-*.g.cs"));

            string link = TestResultsDirectory / "reports" / "index.html";
            Information($"Code coverage report: \x1b]8;;file://{link.Replace('\\', '/')}\x1b\\{link}\x1b]8;;\x1b\\");
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();

            var functionApps = Solution.AllProjects
                .Where(p => p.Name.EndsWith(".Api", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var project in functionApps)
            {
                DotNetPublish(s => s
                    .SetProject(project)
                    .SetConfiguration(Configuration)
                    .SetOutput(ArtifactsDirectory / project.Name)
                    .EnableNoLogo()
                    .EnableNoRestore());
            }
        });

    Target Default => _ => _
        .DependsOn(GenerateCodeCoverageReport, Publish);

    bool IsPullRequest => GitHubActions?.IsPullRequest ?? false;

    bool IsTag => BranchSpec != null && BranchSpec.Contains("refs/tags", StringComparison.OrdinalIgnoreCase);
}
