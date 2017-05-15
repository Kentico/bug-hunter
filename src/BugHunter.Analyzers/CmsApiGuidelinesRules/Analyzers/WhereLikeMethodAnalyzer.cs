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

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// Method <c>WhereLike</c> or <c>WhereNotLike</c> of <c>CMS.DataEngine.WhereConditionBase{TInfo}</c> is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereLikeMethodAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="WhereLikeMethodAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.WhereLikeMethod;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.WhereLikeMethod_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.WhereLikeMethod_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.AbstractionOverImplementation),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.WhereLikeMethod_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "CMS.DataEngine.WhereConditionBase`1" },
            new[] { "WhereLike", "WhereNotLike" });

        private static readonly ISyntaxNodeAnalyzer Analyzer = new MethodInvocationAnalyzer(Config, new MethodInvocationOnlyDiagnosticFormatter());

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
    }
}
