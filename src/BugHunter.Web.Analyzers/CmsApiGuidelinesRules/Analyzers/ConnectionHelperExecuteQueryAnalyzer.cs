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
    /// Method <c>CMS.DataEngine.ConnectionHelper.ExecuteQuery</c> is invoked from presentation layer
    /// (one of <c>.aspx.cs</c>, <c>.ascx.cs</c>, <c>.ashx.cs</c> or <c>.mester.cs</c>)
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConnectionHelperExecuteQueryAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ConnectionHelperExecuteQueryAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.ConnectionHelperExecuteQuery;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.CmsApiGuidelines),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "CMS.DataEngine.ConnectionHelper" },
            new[] { "ExecuteQuery" });

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
                : base(config, new MethodInvocationDiagnosticFormatter())
            {
            }

            /// <inheritdoc />
            protected override bool IsOnForbiddenPath(string filePath)
            {
                return filePath.EndsWith(FileExtensions.Pages)
                    || filePath.EndsWith(FileExtensions.Controls)
                    || filePath.EndsWith(FileExtensions.Handlers)
                    || filePath.EndsWith(FileExtensions.MasterPage);
            }
        }
    }
}
