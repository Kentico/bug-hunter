// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    /// <summary>
    /// Adds <see cref="System.Globalization.CultureInfo.CurrentCulture"/> argument to the method invocation diagnosed
    /// by <see cref="StringCompareStaticMethodAnalyzer"/>; or replaces the method with Invariant version
    /// </summary>
    /// <remarks>Respects the ignore case argument if present</remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringManipulationMethodsCodeFixProvider)), Shared]
    public class StringManipulationMethodsCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringManipulationMethodsAnalyzer.DiagnosticId);

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

            var diagnostic = context.Diagnostics.First();

            var newInvocation1 = GetNewInvocationWithInvariantMethod(invocation);
            var newInvocation2 = GetNewInvocationWithCultureInfoParameter(invocation);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newInvocation1),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation1, c),
                    equivalenceKey: $"{nameof(StringManipulationMethodsCodeFixProvider)}-InvariantCulture"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newInvocation2),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation2, c, "System.Globalization"),
                    equivalenceKey: $"{nameof(StringManipulationMethodsCodeFixProvider)}-CurrentCulture"),
                diagnostic);
        }

        private static ExpressionSyntax GetNewInvocationWithCultureInfoParameter(InvocationExpressionSyntax invocation)
            => SyntaxFactory.ParseExpression($"{invocation.Expression}(CultureInfo.CurrentCulture)");

        private static ExpressionSyntax GetNewInvocationWithInvariantMethod(InvocationExpressionSyntax invocation)
            => SyntaxFactory.ParseExpression($"{invocation.Expression}Invariant()");
    }
}