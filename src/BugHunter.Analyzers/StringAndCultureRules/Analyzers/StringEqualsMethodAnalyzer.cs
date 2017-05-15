// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// The <c>String.Equals</c> instance or static method is invoked without specifying <see cref="System.StringComparison"/> argument
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringEqualsMethodAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringEqualsMethodAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.StringEqualsMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringEqualsMethodAnalyzer"/> class.
        /// </summary>
        public StringEqualsMethodAnalyzer()
            : base("Equals")
        {
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule
            => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);
    }
}
