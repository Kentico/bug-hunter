﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.AbstractionOverImplementation.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.AbstractionOverImplementation.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LuceneSearchDocumentCodeFixProvider)), Shared]
    public class LuceneSearchDocumentCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(LuceneSearchDocumentAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            
            var editor = new CodeFixHelper(context);
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnosedNode = root.FindNode(diagnosticSpan).FirstAncestorOrSelf<QualifiedNameSyntax>() ??
                                (NameSyntax) root.FindNode(diagnosticSpan).FirstAncestorOrSelf<IdentifierNameSyntax>();

            if (diagnosedNode == null)
            {
                return;
            }

            const string usingNamespace = "CMS.DataEngine";
            var newIdentifierNameNode = SyntaxFactory.ParseExpression("ISearchDocument");
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newIdentifierNameNode),
                    createChangedDocument: c => editor.ReplaceExpressionWith(diagnosedNode, newIdentifierNameNode, usingNamespace),
                    equivalenceKey: nameof(LuceneSearchDocumentCodeFixProvider)),
                diagnostic);
        }
    }
}