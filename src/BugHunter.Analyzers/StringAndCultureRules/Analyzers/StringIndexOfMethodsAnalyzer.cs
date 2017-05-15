// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// The <c>String.IndexOf</c> or <c>String.LastindexOf</c> method is invoked without specifying <see cref="System.StringComparison"/> argument
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringIndexOfMethodsAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringIndexOfMethodsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.StringIndexOfMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIndexOfMethodsAnalyzer"/> class.
        /// </summary>
        public StringIndexOfMethodsAnalyzer()
            : base("IndexOf", "LastIndexOf")
        {
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule
            => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);

        /// <inheritdoc />
        protected override bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            => base.IsForbiddenOverload(context, invocationExpression, methodSymbol) && !IsFirstArgumentChar(methodSymbol);

        private static bool IsFirstArgumentChar(IMethodSymbol methodSymbol)
            => !methodSymbol.Parameters.IsEmpty && methodSymbol.Parameters.First().Type.SpecialType == SpecialType.System_Char;
    }
}
