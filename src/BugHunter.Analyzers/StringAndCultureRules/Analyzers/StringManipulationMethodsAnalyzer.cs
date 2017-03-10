using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'ToLower()' and 'ToUpper()' methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringManipulationMethodsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_MANIPULATION_METHODS;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            category: nameof(AnalyzerCategories.StringAndCulture),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();

        protected override IDiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            RegisterAction(Rule, context, "System.String", "ToLower", "ToUpper");
        }

        // If method is already called with StringComparison argument, no need for diagnostic
        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.ArgumentList.Arguments.Count == 0;
        }
    }
}
