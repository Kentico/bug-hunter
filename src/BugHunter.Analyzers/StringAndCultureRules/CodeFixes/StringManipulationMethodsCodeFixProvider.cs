﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringManipulationMethodsCodeFixProvider)), Shared]
    public class StringManipulationMethodsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringManipulationMethodsAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

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
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation1),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation1),
                    equivalenceKey: $"{nameof(StringManipulationMethodsCodeFixProvider)}-InvariantCulture"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation2),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation2, "System.Globalization"),
                    equivalenceKey: $"{nameof(StringManipulationMethodsCodeFixProvider)}-CurrentCulture"),
                diagnostic);
        }

        private static ExpressionSyntax GetNewInvocationWithCultureInfoParameter(InvocationExpressionSyntax invocation)
        {
            return SyntaxFactory.ParseExpression($"{invocation.Expression}(CultureInfo.CurrentCulture)");
        }

        private static ExpressionSyntax GetNewInvocationWithInvariantMethod(InvocationExpressionSyntax invocation)
        {
            return SyntaxFactory.ParseExpression($"{invocation.Expression}Invariant()");
        }
    }
}