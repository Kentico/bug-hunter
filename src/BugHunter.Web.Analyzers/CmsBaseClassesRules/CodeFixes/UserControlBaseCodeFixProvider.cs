﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using BugHunter.Core.Models;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UserControlBaseCodeFixProvider)), Shared]
    public class UserControlBaseCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UserControlBaseAnalyzer.DIAGNOSTIC_ID);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        private static readonly ClassAndItsNamespace[] SuggestedBaseClasses =
        {
            new ClassAndItsNamespace { ClassNamespace = "CMS.UIControls", ClassName = "CMSUserControl"},
            new ClassAndItsNamespace { ClassNamespace = "CMS.Base.Web.UI", ClassName = "AbstractUserControl"},
        };

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var baseTypeCodeFixHelper = new ClassDeclarationCodeFixHelper(context);

            var diagnostic = baseTypeCodeFixHelper.GetFirstDiagnostic(FixableDiagnosticIds.ToArray());
            var classDeclaration = await baseTypeCodeFixHelper.GetDiagnosedClassDeclarationSyntax(diagnostic);
            if (classDeclaration == null)
            {
                return;
            }

            foreach (var classAndItsNamespace in SuggestedBaseClasses)
            {
                var newClassDeclaration = classDeclaration.WithBaseClass(classAndItsNamespace.ClassName);
                context.RegisterCodeFix(
                   CodeAction.Create(
                       title: CodeFixMessagesProvider.GetInheritFromMessage(classAndItsNamespace.ClassName),
                       createChangedDocument: c => baseTypeCodeFixHelper.ReplaceExpressionWith(classDeclaration, newClassDeclaration, c, classAndItsNamespace.ClassNamespace),
                       equivalenceKey: nameof(UserControlBaseCodeFixProvider) + classAndItsNamespace.ClassName),
                   diagnostic);
            }
        }
    }
}