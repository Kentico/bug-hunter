﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ValidationHelperGetCodeFixProvider)), Shared]
    public class ValidationHelperGetCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ValidationHelperGetAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocationExpression = await editor.GetDiagnosedInvocation();

            if (invocationExpression == null)
            {
                return;
            }

            var argumentsToSustain = invocationExpression.ArgumentList.Arguments.Take(2).ToArray();
            var newInvocation =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.ParseExpression($"{invocationExpression.Expression}System"));
            newInvocation = newInvocation.AppendArguments(argumentsToSustain);

            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newInvocation),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocationExpression, newInvocation, c, "CMS.Helpers"),
                    equivalenceKey: nameof(ValidationHelperGetCodeFixProvider)),
                diagnostic);
        }
    }
}