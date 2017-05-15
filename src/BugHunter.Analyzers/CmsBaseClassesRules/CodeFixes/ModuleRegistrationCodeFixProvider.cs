// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsBaseClassesRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsBaseClassesRules.CodeFixes
{
    /// <summary>
    /// Registers Module or ModuleEntry diagnosed by <see cref="ModuleRegistrationAnalyzer"/> by adding an assembly attribute <c>CMS.RegisterModule</c>
    /// to the file, where the diagnosed class is declared
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModuleRegistrationCodeFixProvider)), Shared]
    public class ModuleRegistrationCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ModuleRegistrationAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var classDeclarationCodeFixHelper = new ClassDeclarationCodeFixHelper(context);

            var diagnostic = classDeclarationCodeFixHelper.GetFirstDiagnostic(FixableDiagnosticIds.ToArray());
            var classDeclaration = await classDeclarationCodeFixHelper.GetDiagnosedClassDeclarationSyntax(diagnostic);
            if (classDeclaration == null)
            {
                return;
            }

            var className = classDeclaration.Identifier.ToString();

            var containingNamespaceNode = classDeclaration
                .AncestorsAndSelf()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            var containingNamespaceName = containingNamespaceNode?.Name.ToString();

            var attributeToBeAdded = GetAttributeListToBeAdded(containingNamespaceName, className);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(CmsBaseClassesResources.ModuleRegistration_CodeFix, className),
                    createChangedDocument: c => classDeclarationCodeFixHelper.ApplyRootModification(oldRoot => oldRoot.AddAttributeLists(attributeToBeAdded), c, "CMS"),
                    equivalenceKey: nameof(ModuleRegistrationCodeFixProvider)),
                diagnostic);
        }

        private static AttributeListSyntax GetAttributeListToBeAdded(string containingNamespaceName, string className)
        {
            var attributeName = SyntaxFactory.ParseName("RegisterModule");
            var attributeArgumentListSyntax = SyntaxFactory.ParseAttributeArgumentList($"(typeof({containingNamespaceName}.{className}))");
            var separatedSyntaxList = default(SeparatedSyntaxList<AttributeSyntax>).Add(SyntaxFactory.Attribute(attributeName, attributeArgumentListSyntax));
            var attributeTargetSpecifierSyntax = SyntaxFactory.AttributeTargetSpecifier(SyntaxFactory.Identifier("assembly"));
            var attributeToBeAdded = SyntaxFactory.AttributeList(attributeTargetSpecifierSyntax, separatedSyntaxList);

            return attributeToBeAdded;
        }
    }
}