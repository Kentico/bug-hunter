// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// One of methods <c>GetDouble</c>, <c>GetDecimal</c>, <c>GetDateTime</c> or <c>GetDate</c> of <c>CMS.Helpers.ValidationHelper</c> class
    /// is invoked from a WebPart file
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidationHelperGetAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ValidationHelperGetAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.ValidationHelperGet;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.CmsApiGuidelines),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "CMS.Helpers.ValidationHelper" },
            new[] { "GetDouble", "GetDecimal", "GetDate", "GetDateTime" });

        private static readonly ISyntaxNodeAnalyzer Analyzer = new InnerMethodInvocationAnalyzer(Config);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyzer.Run, SyntaxKind.InvocationExpression);
        }

        private sealed class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InnerMethodInvocationAnalyzer"/> class.
            /// </summary>
            /// <param name="config">Configuration for the analysis</param>
            public InnerMethodInvocationAnalyzer(ApiReplacementConfig config)
                : base(config, new MethodInvocationOnlyNoArgsDiagnosticFormatter())
            {
            }

            /// <inheritdoc />
            protected override bool IsOnForbiddenPath(string filePath)
            {
                return SolutionFolders.FileIsInWebPartsFolder(filePath);
            }
        }
    }
}
