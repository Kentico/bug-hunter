﻿using System;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if Web Part file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebPartBaseAnalyzer : BaseClassDeclarationSyntaxAnalyzer
    {
        public const string WEB_PART_DIAGNOSTIC_ID = DiagnosticIds.WEB_PART_BASE;
        public const string UI_WEB_PART_DIAGNOSTIC_ID = DiagnosticIds.UI_WEB_PART_BASE;

        private static readonly DiagnosticDescriptor WebPartRule = BaseClassesInheritanceRuleBuilder.GetRule(WEB_PART_DIAGNOSTIC_ID, "Web Part", "some abstract CMS WebPart");
        private static readonly DiagnosticDescriptor UiWebPartRule = BaseClassesInheritanceRuleBuilder.GetRule(UI_WEB_PART_DIAGNOSTIC_ID, "UI Web Part", "some abstract CMS UI WebPart");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WebPartRule, UiWebPartRule);

        private static readonly string[] UiWebPartBases =
        {
            "CMS.UIControls.CMSAbstractUIWebpart"
        };

        private static readonly string[] WebPartBases =
        {
            "CMS.PortalEngine.Web.UI.CMSAbstractWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart",
            "CMS.Ecommerce.Web.UI.CMSCheckoutWebPart"
        };


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(symbolAnalysisContext =>
            {
                var namedTypeSymbol = symbolAnalysisContext.Symbol as INamedTypeSymbol;
                if (namedTypeSymbol == null || namedTypeSymbol.IsAbstract || namedTypeSymbol.IsNested())
                {
                    return;
                }

                // Symbol is defined in multiple locations if it is a partial class -- all locations need to be checked
                var symbolFilePaths = namedTypeSymbol.Locations.Select(location => location?.SourceTree?.FilePath).ToArray();
                var isInWebPartsFolder = symbolFilePaths.Any(FileIsInWebPartsFolder);
                if (!isInWebPartsFolder)
                {
                    return;
                }

                var isUiWebPart = symbolFilePaths.Any(IsUIWebPart);
                var allowedBaseTypeNames = isUiWebPart ? UiWebPartBases : WebPartBases;
                var ruleToBeUsed = isUiWebPart ? UiWebPartRule : WebPartRule;

                var baseTypeSymbol = namedTypeSymbol.BaseType;
                var doesNotExtendAnything = baseTypeSymbol?.SpecialType == SpecialType.System_Object;
                var inheritsFromOneOfRequiredTypes = allowedBaseTypeNames.Any(allowedTypeName => allowedTypeName.Equals(baseTypeSymbol?.ToString()));
                if (doesNotExtendAnything || inheritsFromOneOfRequiredTypes)
                {
                    return;
                }

                // TODO how to use all locations
                var diagnostic = Diagnostic.Create(ruleToBeUsed, namedTypeSymbol.Locations.FirstOrDefault(), namedTypeSymbol.Name);
                symbolAnalysisContext.ReportDiagnostic(diagnostic);
            }, SymbolKind.NamedType);

            //context.RegisterCompilationStartAction(compilationContext =>
            //{
            //    // leave here for performance optimization
            //    var uiWebPartBaseTypes = UiWebPartBases.Select(compilationContext.Compilation.GetTypeByMetadataName).ToArray();
            //    var webPartBaseTypes = WebPartBases.Select(compilationContext.Compilation.GetTypeByMetadataName).ToArray();

            //    if (uiWebPartBaseTypes.Union(webPartBaseTypes).Any(baseType => baseType == null))
            //    {
            //        return;
            //    }

            //    compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
            //    {
            //        var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
            //        if (!FileIsInWebPartsFolder(filePath))
            //        {
            //            return;
            //        }

            //        var isUIWebPart = IsUIWebPart(filePath);
            //        var enforcedWebPatBaseTypes = isUIWebPart ? uiWebPartBaseTypes : webPartBaseTypes;
            //        var ruleToBeUsed = isUIWebPart ? UiWebPartRule : WebPartRule;

            //        var publicInstantiableClassDeclarations = GetAllClassDeclarations(syntaxTreeAnalysisContext)
            //            .Where(classDeclarationSyntax
            //                => classDeclarationSyntax.IsPublic()
            //                && !classDeclarationSyntax.IsAbstract())
            //            .ToArray();

            //        if (!publicInstantiableClassDeclarations.Any())
            //        {
            //            return;
            //        }

            //        var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);
                    
            //        foreach (var classDeclaration in publicInstantiableClassDeclarations)
            //        {
            //            var baseTypeTypeSymbol = GetBaseTypeSymbol(classDeclaration, semanticModel);
            //            if (baseTypeTypeSymbol == null)
            //            {
            //                continue;
            //            }

            //            // if class inherits from one of enforced base types, skip
            //            if (enforcedWebPatBaseTypes.Any(enforcedWebPartBase => baseTypeTypeSymbol.Equals(enforcedWebPartBase)))
            //            {
            //                continue;
            //            }

            //            var diagnostic = CreateDiagnostic(syntaxTreeAnalysisContext, classDeclaration, ruleToBeUsed);

            //            syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
            //        }
            //    });
            //});
        }
        
        private static bool FileIsInWebPartsFolder(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) &&
                   !filePath.Contains("_files\\") &&
                   (filePath.Contains(SolutionFolders.UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) || filePath.Contains(SolutionFolders.WEB_PARTS, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Checks if the given file is UI web part.
        /// </summary>
        /// <param name="path">Path to web part file.</param>
        private static bool IsUIWebPart(string path)
        {
            return !string.IsNullOrEmpty(path) && path.Contains(SolutionFolders.UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase);
        }
    }
}
