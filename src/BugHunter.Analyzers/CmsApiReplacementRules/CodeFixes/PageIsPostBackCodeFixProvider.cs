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
    /// <summary>
    /// Replaces diagnosed syntax node from <see cref="PageIsPostBackAnalyzer"/> with a call to <c>CMS.Helpers.RequestHelper.IsPostBack()</c>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PageIsPostBackCodeFixProvider)), Shared]
    public class PageIsPostBackCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(PageIsPostBackAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            if (!diagnostic.IsMarkedAsSimpleMemberAccess())
            {
                // no codefix for conditional accesses
                return;
            }

            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetDiagnosedMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var newMemberAccess = SyntaxFactory.ParseExpression("RequestHelper.IsPostBack()");

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, c, usingNamespace),
                    equivalenceKey: nameof(PageIsPostBackCodeFixProvider)),
                diagnostic);
        }
    }
}