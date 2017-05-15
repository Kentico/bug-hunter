// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes
{
    /// <summary>
    /// Replaces diagnosed syntax node from <see cref="FormsAuthenticationSignOutAnalyzer"/> with a call to <c>CMS.Membership.AuthenticationHelper.SignOut()</c>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormsAuthenticationSignOutCodeFixProvider)), Shared]
    public class FormsAuthenticationSignOutCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(FormsAuthenticationSignOutAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);

            // invoked always statically no need for conditional access filtering
            var invocationExpression = await editor.GetDiagnosedInvocation();

            if (invocationExpression == null || !invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                return;
            }

            var usingNamespace = "CMS.Membership";

            // parenthesis (for method invocation) will be reused from previous code
            var newExpressionBody = SyntaxFactory.ParseExpression("AuthenticationHelper.SignOut");
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newExpressionBody),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocationExpression.Expression, newExpressionBody, c, usingNamespace),
                    equivalenceKey: nameof(FormsAuthenticationSignOutCodeFixProvider)),
                diagnostic);
        }
    }
}