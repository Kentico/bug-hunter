// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers
{
    /// <summary>
    /// A non-abstract class located in a WebPar or UIWebPart folder does not inherit from one of required base classes
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebPartBaseAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="WebPartBaseAnalyzer"/> for normal web parts
        /// </summary>
        public const string WebPartDiagnosticId = DiagnosticIds.WebPartBase;

        /// <summary>
        /// The ID for diagnostics raised by <see cref="WebPartBaseAnalyzer"/> for UI web parts
        /// </summary>
        public const string UIWebPartDiagnosticId = DiagnosticIds.UIWebPartBase;

        private static readonly DiagnosticDescriptor WebPartRule = BaseClassesInheritanceRulesProvider.GetRule(WebPartDiagnosticId, "Web Part", "some abstract CMS WebPart");

        private static readonly DiagnosticDescriptor UiWebPartRule = BaseClassesInheritanceRulesProvider.GetRule(UIWebPartDiagnosticId, "UI Web Part", "some abstract CMS UI WebPart");

        private static readonly ISymbolDiagnosticFormatter<INamedTypeSymbol> DiagnosticFormatter = new NamedTypeSymbolDiagnosticFormatter();

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

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(WebPartRule, UiWebPartRule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
        }

        private static void Analyze(SymbolAnalysisContext symbolAnalysisContext)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolAnalysisContext.Symbol;
            if (namedTypeSymbol == null ||
                namedTypeSymbol.IsAbstract ||
                namedTypeSymbol.IsNested())
            {
                return;
            }

            // Symbol is defined in multiple locations if it is a partial class -- all locations need to be checked
            var symbolFilePaths = namedTypeSymbol.Locations.Select(location => location?.SourceTree?.FilePath).ToArray();
            var isInWebPartsFolder = symbolFilePaths.Any(SolutionFolders.FileIsInWebPartsFolder);
            if (!isInWebPartsFolder)
            {
                return;
            }

            var isUiWebPart = symbolFilePaths.Any(SolutionFolders.IsUIWebPart);
            var allowedBaseTypeNames = isUiWebPart ? UiWebPartBases : WebPartBases;
            var ruleToBeUsed = isUiWebPart ? UiWebPartRule : WebPartRule;

            var extendsOnlyObject = namedTypeSymbol.ExtendsOnlyObject();
            var inheritsFromOneOfRequiredTypes = InheritsFromOneOfRequiredTypes(allowedBaseTypeNames, namedTypeSymbol);
            if (extendsOnlyObject || inheritsFromOneOfRequiredTypes)
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(ruleToBeUsed, namedTypeSymbol);
            symbolAnalysisContext.ReportDiagnostic(diagnostic);
        }

        private static bool InheritsFromOneOfRequiredTypes(string[] allowedBaseTypeNames, INamedTypeSymbol namedTypeSymbol)
        {
            return allowedBaseTypeNames
                .Any(allowedTypeName => allowedTypeName.Equals(namedTypeSymbol.BaseType?.ToString()));
        }
    }
}
