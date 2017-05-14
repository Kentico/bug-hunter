// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpResponseRedirectCodeFixProvider)), Shared]
    public class HttpResponseRedirectCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpResponseRedirectAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            if (!diagnostic.IsMarkedAsSimpleMemberAccess())
            {
                // no support yet for complicated cases as conditional access
                return;
            }

            var editor = new MemberInvocationCodeFixHelper(context);
            var invocationExpression = await editor.GetDiagnosedInvocation();

            if (invocationExpression == null || !invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var codeFix1 = SyntaxFactory.InvocationExpression(SyntaxFactory.ParseExpression("URLHelper.Redirect"), invocationExpression.ArgumentList);
            var codeFix2 = SyntaxFactory.InvocationExpression(SyntaxFactory.ParseExpression("URLHelper.LocalRedirect"), invocationExpression.ArgumentList);

            var message1 = $"{CodeFixMessagesProvider.GetReplaceWithMessage(codeFix2)} {CmsApiReplacementsResources.RedirectCodeFixLocal}";
            var message2 = $"{CodeFixMessagesProvider.GetReplaceWithMessage(codeFix2)} {CmsApiReplacementsResources.RedirectCodeFixExternal}";

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: message1,
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocationExpression, codeFix1, c, usingNamespace),
                    equivalenceKey: nameof(HttpResponseRedirectCodeFixProvider)),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: message2,
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocationExpression, codeFix2, c, usingNamespace),
                    equivalenceKey: nameof(HttpResponseRedirectCodeFixProvider) + "Local"),
                    diagnostic);
        }
    }
}