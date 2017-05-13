using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestBrowserAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="HttpRequestBrowserAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.HttpRequestBrowser;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "Request.Browser", "BrowserHelper.GetBrowser()");

        private static readonly ApiReplacementConfig ApiReplacementConfig = new ApiReplacementConfig(
            Rule,
            new[] { "System.Web.HttpBrowserCapabilities", "System.Web.HttpBrowserCapabilitiesBase" },
            new[] { "Browser" });

        private static readonly ApiReplacementForMemberAnalyzer ApiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(ApiReplacementConfig);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            ApiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
