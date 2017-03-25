using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'IndexOf()' and 'LastIndexOf()' etc. methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringIndexOfMethodsAnalyzer : BaseStringComparisonMethodsAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_INDEX_OF_METHODS;
        
        private static readonly DiagnosticDescriptor Rule = CreateRule(DIAGNOSTIC_ID);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            RegisterAction(Rule, context, "System.String", "IndexOf", "LastIndexOf");
        }

        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {

            return base.CheckPostConditions(context, invocationExpression, methodSymbol) && !IsFirstArgumentChar(context, invocationExpression, methodSymbol);
        }

        private static bool IsFirstArgumentChar(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            var firstArgument = invocationExpression
                .ArgumentList
                .Arguments
                .FirstOrDefault()
                ?.Expression;

            if (firstArgument == null)
            {
                return false;
            }

            if (firstArgument.ToString().StartsWith("'"))
            {
                return true;
            }

            // it can be a variable of type char
            var firstArgumentType = context.SemanticModel.GetTypeInfo(firstArgument).Type;

            return firstArgumentType?.SpecialType == SpecialType.System_Char;
        }
    }
}
