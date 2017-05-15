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
    /// The <c>String.StartsWith</c> or <c>String.EndsWith</c> method is invoked without specifying <see cref="System.StringComparison"/>
    /// nor <see cref="System.Globalization.CultureInfo"/> argument
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringStartAndEndsWithMethodsAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringStartAndEndsWithMethodsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.StringStartsEndsWithMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringStartAndEndsWithMethodsAnalyzer"/> class.
        /// </summary>
        public StringStartAndEndsWithMethodsAnalyzer()
            : base("StartsWith", "EndsWith")
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
