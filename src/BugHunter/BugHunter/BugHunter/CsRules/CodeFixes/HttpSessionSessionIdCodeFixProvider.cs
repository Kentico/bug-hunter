﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.CsRules.Analyzers;
using BugHunter.Helpers.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpSessionSessionIdCodeFixProvider)), Shared]
    public class HttpSessionSessionIdCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpSessionSessionIdAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetInnerMostMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var codeFixTitle = new LocalizableResourceString(nameof(CsResources.ApiReplacements_CodeFix), CsResources.ResourceManager, typeof(CsResources)).ToString();
            var usingNamespace = typeof(CMS.Helpers.SessionHelper).Namespace;
            var newMemberAccess = SyntaxFactory.ParseExpression("SessionHelper.GetSessionID()");
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(codeFixTitle, newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, usingNamespace),
                    equivalenceKey: "SessionHelper.GetSessionID"),
                diagnostic);
        }
    }
}