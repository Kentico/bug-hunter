﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Helpers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClientScriptMethodsCodeFixProvider)), Shared]
    public class ClientScriptMethodsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ClientScriptMethodsAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocation = await editor.GetDiagnosedInvocation();
            if (invocation == null)
            {
                return;
            }

            // TODO check if derives from System.Web.UI.control
            // if not, no codefix should be provided
            var usingNamespace = "CMS.Base.Web.UI";
            var newInvocationExpression = GetNewInvocatioExpression(invocation);
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newInvocationExpression),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocationExpression, usingNamespace),
                    equivalenceKey: nameof(ClientScriptMethodsCodeFixProvider)),
                diagnostic);
        }

        private ArgumentListSyntax GetNewArgumentList(InvocationExpressionSyntax oldInvocation)
        {
            var oldArgumentList = oldInvocation.ArgumentList.Arguments;

            var thisArgument = SyntaxFactory.ParseExpression("this");
            var newArgumentIdentifier = SyntaxFactory.Argument(thisArgument);
            var newArguments = SyntaxFactory.SeparatedList(new[] { newArgumentIdentifier }.Concat(oldArgumentList));
            var newArgumentList = SyntaxFactory.ArgumentList(newArguments);

            return newArgumentList;
        }

        private InvocationExpressionSyntax GetNewInvocatioExpression(InvocationExpressionSyntax oldInvocation)
        {
            // Methods in script helper are named same as methods in ClientScriptManager
            var oldMethodName = MethodInvocationHelper.GetMethodName(oldInvocation);
            var newInvocationExpression = SyntaxFactory.ParseExpression($"ScriptHelper.{oldMethodName}()") as InvocationExpressionSyntax;
            var newArgumentList = GetNewArgumentList(oldInvocation);
            
            return newInvocationExpression?.WithArgumentList(newArgumentList); ;
        }
    }
}