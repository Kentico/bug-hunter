﻿using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <c>CMS.Helpers.ValidationHelper</c> and their access to <c>GetDouble</c>, <c>GetDateTime</c> methods
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidationHelperGetAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="ValidationHelperGetAnalyzer"/>
        /// </summary>
        public const string DIAGNOSTIC_ID = DiagnosticIds.VALIDATION_HELPER_GET;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                category: nameof(AnalyzerCategories.CmsApiGuidelines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ValidationHelperGet_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(DIAGNOSTIC_ID));

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);
        
        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            var analyzer = new InnerMethodInvocationAnalyzer();
            context.RegisterSyntaxNodeAction(analyzer.Run, SyntaxKind.InvocationExpression);
        }

        private sealed class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            new[] { "CMS.Helpers.ValidationHelper" },
            new[] { "GetDouble", "GetDecimal", "GetDate", "GetDateTime" });

            /// <summary>
            /// Constructor initializing config and diagnostic formatter of <see cref="MethodInvocationAnalyzer"/> base class
            /// </summary>
            public InnerMethodInvocationAnalyzer() : base(config, new MethodInvocationOnlyNoArgsDiagnosticFormatter())
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
