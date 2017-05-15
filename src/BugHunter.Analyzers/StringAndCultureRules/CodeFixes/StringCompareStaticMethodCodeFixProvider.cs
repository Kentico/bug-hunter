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
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    /// <summary>
    /// Adds <see cref="System.StringComparison"/> argument to the method invocation diagnosed by <see cref="StringCompareStaticMethodAnalyzer"/>
    /// </summary>
    /// <remarks>Respects the ignore case argument if present</remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringCompareStaticMethodCodeFixProvider)), Shared]
    public class StringCompareStaticMethodCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringCompareStaticMethodAnalyzer.DiagnosticId);

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
            var tempInvocation = invocation;
            var stringComparisonOptions = StringComparisonOptions.GetAll();

            // if it was diagnosed and overload with three or six arguments is called it must be:
            // public static int Compare(string strA, string strB, bool ignoreCase)
            // OR
            // public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase)
            // therefore we will apply only suitable fixes
            var originalArguments = invocation.ArgumentList.Arguments;
            if (originalArguments.Count == 3 || originalArguments.Count == 6)
            {
                var ignoreCaseAttribute = originalArguments.Last().Expression.ToString();
                bool ignoreCase;

                if (!bool.TryParse(ignoreCaseAttribute, out ignoreCase))
                {
                    // could not detect value of 'ignoreCase' argument -> no codefix provided
                    return;
                }

                // delete last argument of the previous invocation
                tempInvocation = tempInvocation.WithArgumentList(SyntaxFactory.ArgumentList(originalArguments.RemoveAt(originalArguments.Count - 1)));
                stringComparisonOptions = ignoreCase
                    ? StringComparisonOptions.GetCaseInsensitive()
                    : StringComparisonOptions.GetCaseSensitive();
            }

            foreach (var stringComparisonOption in stringComparisonOptions)
            {
                var newInvocation = tempInvocation.AppendArguments(stringComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessagesProvider.GetReplaceWithMessage(newInvocation),
                        createChangedDocument:
                        c => editor.ReplaceExpressionWith(invocation, newInvocation, c, namespacesToBeReferenced),
                        equivalenceKey: $"{nameof(StringCompareStaticMethodCodeFixProvider)}-{stringComparisonOption}"),
                    context.Diagnostics.First());
            }
        }
    }
}