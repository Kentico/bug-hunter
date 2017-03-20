﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormsAuthenticationSignOutCodeFixProvider)), Shared]
    public class FormsAuthenticationSignOutCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(FormsAuthenticationSignOut.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocationExpression = await editor.GetDiagnosedInvocation();

            if (invocationExpression == null)
            {
                return;
            }

            var usingNamespace = "CMS.Membership";
            // parenthesis (for method invocation) will be reused from previous code
            var newExpressionBody = SyntaxFactory.ParseExpression("AuthenticationHelper.SignOut");
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newExpressionBody),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocationExpression.Expression, newExpressionBody, usingNamespace),
                    equivalenceKey: nameof(FormsAuthenticationSignOutCodeFixProvider)),
                diagnostic);
        }
    }
}