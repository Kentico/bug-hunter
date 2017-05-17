// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.MethodInvocationHeuristics.Helpers
{
    /// <summary>
    /// This is duplicate version of <see cref="ApiReplacementForMethodAnalyzer"/> BUT withou syntax heuristics optimization.
    ///
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    public class ApiReplacementForMethodAnalyzerNoOptimization
    {
        private readonly ISyntaxNodeAnalyzer _methodInvocationAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiReplacementForMethodAnalyzerNoOptimization"/> class.
        /// </summary>
        /// <param name="config">Configuration of the analysis</param>
        public ApiReplacementForMethodAnalyzerNoOptimization(ApiReplacementConfig config)
        {
            _methodInvocationAnalyzer = new MethodInvocationAnalyzerNoOptimization(config, new MethodInvocationDiagnosticFormatter());
        }

        /// <summary>
        /// Registers the analyzers on <paramref name="analysisContext" />
        ///
        /// Makes sure the analysis is run for all method invocations
        /// and enables concurrent analysis execution and disables analysis of generated code
        /// </summary>
        /// <param name="analysisContext">Contex of the analysis to register the action on</param>
        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            analysisContext.RegisterSyntaxNodeAction(_methodInvocationAnalyzer.Run, SyntaxKind.InvocationExpression);
        }
    }
}