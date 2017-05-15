// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers;
using BugHunter.Core.Extensions;
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
    /// Replaces invocation of methods diagnosed by <see cref="WhereLikeMethodAnalyzer"/> by one of <c>WhereContains</c>,
    /// <c>WhereStartsWith</c>, <c>WhereEndswith</c> or their negated variants
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhereLikeMethodCodeFixProvider)), Shared]
    public class WhereLikeMethodCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// Enumeration containing Method names of possible fixes
        /// </summary>
        internal enum PossibleFixes
        {
            /// <summary>
            /// Option for 'WhereContains' method
            /// </summary>
            WhereContains,

            /// <summary>
            /// Option for 'WhereStartsWith' method
            /// </summary>
            WhereStartsWith,

            /// <summary>
            /// Option for 'WhereEndsWith' method
            /// </summary>
            WhereEndsWith
        }

        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(WhereLikeMethodAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var codeFixHelper = new MemberInvocationCodeFixHelper(context);
            var invocationExpression = await codeFixHelper.GetDiagnosedInvocation();
            SimpleNameSyntax methodNameNode;
            if (invocationExpression == null || !invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return;
            }

            var containsMethodName = GetNewMethodName(PossibleFixes.WhereContains, methodNameNode);
            var startsWithMethodName = GetNewMethodName(PossibleFixes.WhereStartsWith, methodNameNode);
            var endsWithMethodName = GetNewMethodName(PossibleFixes.WhereEndsWith, methodNameNode);

           RegisterCodeFixVariant(context, codeFixHelper, methodNameNode, containsMethodName);
           RegisterCodeFixVariant(context, codeFixHelper, methodNameNode, startsWithMethodName);
           RegisterCodeFixVariant(context, codeFixHelper, methodNameNode, endsWithMethodName);
        }

        private void RegisterCodeFixVariant(CodeFixContext context, CodeFixHelper codeFixHelper, SimpleNameSyntax oldMethodName, SimpleNameSyntax newMethodName)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newMethodName),
                    createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldMethodName, newMethodName, c),
                    equivalenceKey: newMethodName.ToString()),
                codeFixHelper.GetFirstDiagnostic());
        }

        private SimpleNameSyntax GetNewMethodName(PossibleFixes forFix, SimpleNameSyntax currentMethodName)
            => SyntaxFactory.IdentifierName(GetNewMethodNameText(forFix, currentMethodName));

        private string GetNewMethodNameText(PossibleFixes forFix, SimpleNameSyntax currentMethodName)
        {
            var isCurrentCallNegated = currentMethodName.Identifier.ToString().Contains("Not");

            switch (forFix)
            {
                case PossibleFixes.WhereContains:
                    return isCurrentCallNegated ? "WhereNotContains" : "WhereContains";
                case PossibleFixes.WhereStartsWith:
                    return isCurrentCallNegated ? "WhereNotStartsWith" : "WhereStartsWith";
                case PossibleFixes.WhereEndsWith:
                    return isCurrentCallNegated ? "WhereNotEndsWith" : "WhereEndsWith";
                default:
                    throw new ArgumentOutOfRangeException(nameof(forFix));
            }
        }
    }
}