// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace SolutionStatistics
{
    /// <summary>
    /// Gathers and prints out statistics about all projects in CMSSolution
    /// If only projects that have analyzers installed shall be inspected, pass '-OnlyProjectsWithInstalledAnalyzer' flag as an argument
    /// </summary>
    internal static class Program
    {
        private static readonly string CMSSolutionPath = @"C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln";
        private static readonly string OnlyProjectsWithInstalledAnalyzerFlag = @"-OnlyProjectsWithInstalledAnalyzer";

        private static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

            MainAsync(args, cts.Token).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args, CancellationToken cancellationToken)
        {
            var onlyProjectsWithInstalledAnalyzer = args.Contains(OnlyProjectsWithInstalledAnalyzerFlag);
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($@"Loading solution {CMSSolutionPath}");
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(CMSSolutionPath, cancellationToken).ConfigureAwait(false);

            Console.WriteLine($@"Loaded solution in {stopwatch.Elapsed:mm\:ss\.ff}");
            stopwatch.Restart();

            var csharpProjects = solution
                .Projects
                .Where(project => project.Language == LanguageNames.CSharp)
                .Where(project => !onlyProjectsWithInstalledAnalyzer || !StatisticsHelper.IsProjectExcluded(project))
                .ToList();

            Console.WriteLine("Number of projects: \t\t\t{0,12:N}", csharpProjects.Count);
            Console.WriteLine("Number of documents:\t\t\t{0,12:N}", csharpProjects.Sum(x => x.DocumentIds.Count));

            var statistics = await StatisticsHelper.GetProjectsStatisticsAsync(csharpProjects, cancellationToken).ConfigureAwait(true);

            StatisticsHelper.PrintStatistics(statistics);

            Console.WriteLine($@"Stats computed in {stopwatch.Elapsed:mm\:ss\.ff}.");
        }
    }
}
