using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'Compare()' static methods called on strings and reports their usage when no overload with StringComparison or CultureInfo argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringCompareStaticMethodAnalyzer : BaseStringComparisonMethodsAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_COMPARE_STATIC_METHOD;

        private static readonly DiagnosticDescriptor Rule = CreateRule(DIAGNOSTIC_ID);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.String", "Compare");
        }
    }
}
