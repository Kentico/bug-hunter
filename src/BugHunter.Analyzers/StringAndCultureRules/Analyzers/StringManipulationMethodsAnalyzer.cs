// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// The <c>String.ToLower</c> or <c>String.ToUpper</c> method is invoked without specifying <see cref="System.Globalization.CultureInfo"/> argument
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringManipulationMethodsAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringManipulationMethodsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.StringManipulationMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringManipulationMethodsAnalyzer"/> class.
        /// </summary>
        public StringManipulationMethodsAnalyzer()
            : base("ToLower", "ToUpper")
        {
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule
            => StringMethodsRuleBuilder.CreateRuleForManipulationMethods(DiagnosticId);

        /// <inheritdoc />
        protected override bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            => invocationExpression.ArgumentList.Arguments.Count == 0;
    }
}
