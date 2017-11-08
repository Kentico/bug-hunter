using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Extensions
{
    /// <summary>
    /// Helper class containing extensions for <see cref="SymbolAnalysisContext" />.
    /// </summary>
    public static class SymbolAnalysisContextExtensions
    {
        /// <summary>
        /// Reports all provided <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="context">Analysis context where the diagnostics should be reported</param>
        /// <param name="diagnostics">Diagnostics to be reported</param>
        public static void ReportDiagnostics(this SymbolAnalysisContext context, IEnumerable<Diagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}