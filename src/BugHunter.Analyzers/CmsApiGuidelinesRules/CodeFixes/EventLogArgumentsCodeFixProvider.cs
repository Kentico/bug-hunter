// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.CodeFixes
{
    /// <summary>
    /// Replaces event type argument diagnosed by <see cref="EventLogArgumentsAnalyzer"/> with equivalent constant from <c>CMS.EventLog.EventType</c>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventLogArgumentsCodeFixProvider)), Shared]
    public class EventLogArgumentsCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(EventLogArgumentsAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var codeFixHelper = new MemberInvocationCodeFixHelper(context);
            var diagnostic = codeFixHelper.GetFirstDiagnostic();
            var invocationExpression = await codeFixHelper.GetDiagnosedInvocation();
            if (invocationExpression == null)
            {
                return;
            }

            var oldArgument = invocationExpression.ArgumentList.Arguments.First();
            var newArgumentName = GetNewMethodArgument(oldArgument);
            if (newArgumentName == null)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newArgumentName),
                    createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldArgument, newArgumentName, c, "CMS.EventLog"),
                    equivalenceKey: nameof(EventLogArgumentsCodeFixProvider)),
                diagnostic);
        }

        private ArgumentSyntax GetNewMethodArgument(ArgumentSyntax oldArgument)
        {
            string newArgumentText;
            switch (oldArgument.Expression.ToString())
            {
                case "\"I\"":
                    newArgumentText = "EventType.INFORMATION";
                    break;
                case "\"W\"":
                    newArgumentText = "EventType.WARNING";
                    break;
                case "\"E\"":
                    newArgumentText = "EventType.ERROR";
                    break;
                default:
                    return null;
            }

            return SyntaxFactory.Argument(SyntaxFactory.ParseExpression(newArgumentText));
        }
    }
}