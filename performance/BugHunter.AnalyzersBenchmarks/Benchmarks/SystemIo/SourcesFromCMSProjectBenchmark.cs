﻿using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers;
using BugHunter.SystemIO.Analyzers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo
{
    public class SourcesFromCmsProjectBenchmark
    {
        private readonly DiagnosticAnalyzer _systemIoV0 = new SyntaxNodeIdentifierNameBaselineAnalyzer();
        private readonly DiagnosticAnalyzer _systemIoV2 = new V02_IdentifierName_SymbolAnalysis();
        private readonly DiagnosticAnalyzer _systemIoV5 = new V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag();
        private readonly DiagnosticAnalyzer _systemIoV7 = new V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag();
        private readonly DiagnosticAnalyzer _systemIoV8 = new V08_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelAnallysis_WithBag();
        private readonly DiagnosticAnalyzer _systemIoV9 = new V09_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelExecutionAndAnallysis_WithBag();
        private readonly DiagnosticAnalyzer _systemIoV10 = new V10_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysis();
        private readonly DiagnosticAnalyzer _systemIoV11 = new V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults();

        private static async Task<Solution> GetSolution()
        {
            var workspace = MSBuildWorkspace.Create();
            await workspace.OpenProjectAsync("C:\\TFS\\CMS\\MAIN\\CMSSolution\\Base\\Base.csproj");

            return workspace.CurrentSolution;
        }


        //[Benchmark(Baseline = false)]
        //public int FilesCompilation()
        //{
        //    return AnalysisRunner.RunAnalysisAsync(GetSolution().Result).Result;
        //}

        [Benchmark(Baseline = true)]
        public int AnalyzerV0_EmptyCallback()
        {
            return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV0).Result;
        }

        [Benchmark]
        public int V02_IdentifierName_SymbolAnalysis()
        {
            return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV2).Result;
        }

        [Benchmark]
        public int V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag()
        {
            return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV5).Result;
        }

        [Benchmark]
        public int V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag()
        {
            return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV7).Result;
        }

        //[Benchmark]
        //public int V08_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelAnallysis_WithBag()
        //{
        //    return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV8).Result;
        //}

        //[Benchmark]
        //public int V09_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelExecutionAndAnallysis_WithBag()
        //{
        //    return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV9).Result;
        //}

        //[Benchmark]
        //public int V10_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysis()
        //{
        //    return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV10).Result;
        //}

        [Benchmark]
        public int V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults()
        {
            return AnalysisRunner.RunAnalysisAsync(GetSolution().Result, _systemIoV11).Result;
        }
    }
}
