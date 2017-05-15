// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Constants;
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
    /// <summary>
    /// Makes the class diagnosed by <see cref="WebPartBaseAnalyzer"/> inherit from <c>CMS.UIControls.CMSAbstractUIWebpart</c> if it is located in UIWebParts folder;
    /// otherwise it will inherit one of CMS Abstract WebParts
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WebPartBaseCodeFixProvider)), Shared]
    public class WebPartBaseCodeFixProvider : CodeFixProvider
    {
        private static readonly ClassAndItsNamespace[] UiWebPartBaseClasses =
        {
            new ClassAndItsNamespace { ClassNamespace = "CMS.UIControls", ClassName = "CMSAbstractUIWebpart" },
        };

        private static readonly ClassAndItsNamespace[] WebPartBaseClasses =
        {
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractWebPart" },
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractEditableWebPart" },
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractLayoutWebPart" },
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractWizardWebPart" },
            new ClassAndItsNamespace { ClassNamespace = "CMS.Ecommerce.Web.UI", ClassName = "CMSCheckoutWebPart" },
        };

        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(WebPartBaseAnalyzer.WebPartDiagnosticId, WebPartBaseAnalyzer.UIWebPartDiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

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

            var suggestions = diagnostic.Id == DiagnosticIds.UIWebPartBase
                ? UiWebPartBaseClasses
                : WebPartBaseClasses;

            foreach (var classAndItsNamespace in suggestions)
            {
                var newClassDeclaration = classDeclaration.WithBaseClass(classAndItsNamespace.ClassName);
                context.RegisterCodeFix(
                   CodeAction.Create(
                       title: CodeFixMessagesProvider.GetInheritFromMessage(classAndItsNamespace.ClassName),
                       createChangedDocument: c => baseTypeCodeFixHelper.ReplaceExpressionWith(classDeclaration, newClassDeclaration, c, classAndItsNamespace.ClassNamespace),
                       equivalenceKey: nameof(WebPartBaseCodeFixProvider) + classAndItsNamespace.ClassName),
                   diagnostic);
            }
        }
    }
}