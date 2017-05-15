// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    /// <summary>
    /// Adds <see cref="System.StringComparison"/> argument to the method invocation diagnosed by one of
    /// <see cref="StringEqualsMethodAnalyzer"/>, <see cref="StringStartAndEndsWithMethodsAnalyzer"/>, or <see cref="StringIndexOfMethodsAnalyzer"/>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringComparisonMethodsWithModifierCodeFixProvider)), Shared]
    public class StringComparisonMethodsWithModifierCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(
                StringEqualsMethodAnalyzer.DiagnosticId,
                StringStartAndEndsWithMethodsAnalyzer.DiagnosticId,
                StringIndexOfMethodsAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocation = await editor.GetDiagnosedInvocation();
            if (invocation == null)
            {
                return;
            }

            const string namespacesToBeReferenced = "System";

            foreach (var stringComparisonOption in StringComparisonOptions.GetAll())
            {
                var newInvocation = invocation.AppendArguments(stringComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessagesProvider.GetReplaceWithMessage(newInvocation),
                        createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation, c, namespacesToBeReferenced),
                        equivalenceKey: $"{nameof(StringComparisonMethodsWithModifierCodeFixProvider)}-{stringComparisonOption}"),
                    context.Diagnostics.First());
            }
        }
    }
}