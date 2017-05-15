// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// Explicit string passed as first argument of <c>CMS.EventLog.EventLogProvider.LogEvent</c> method
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventLogArgumentsAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="EventLogArgumentsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.EventLogArguments;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.EventLogArguments_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.EventLogArguments_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            category: nameof(AnalyzerCategories.CmsApiGuidelines),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.EventLogArguments_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "CMS.EventLog.EventLogProvider" },
            new[] { "LogEvent" });

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

        private class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            private static readonly string[] ForbiddenEventTypeArgs = { "\"I\"", "\"W\"", "\"E\"" };

            /// <summary>
            /// Initializes a new instance of the <see cref="InnerMethodInvocationAnalyzer"/> class.
            /// </summary>
            /// <param name="config">Configuration for the analysis</param>
            public InnerMethodInvocationAnalyzer(ApiReplacementConfig config)
                : base(config, new EventLogArgumentsDiagnosticFormatter())
            {
            }

            /// <summary>
            /// Checks whether first argument provided is one of forbidden ones
            /// </summary>
            /// <remarks>
            ///  Note that analysis has to be performed on syntax since it always is a string,
            ///  but we want to achieve that not plain string is passed but rather a constant defined in <c>CMS.EventLog.EventType</c>.
            ///
            /// When a variable is passed, there is no way of finding out whether the variable was created from a plain string
            /// or an constant defined in EventType class (it is not an enum), therefore no diagnostic is provided
            /// </remarks>
            /// <param name="invocationExpression">Invocation expression to be analyzed</param>
            /// <param name="methodSymbol">Method symbol for <paramref name="invocationExpression" /></param>
            /// <returns>True if forbidden usage is detected, false otherwise</returns>
            protected override bool IsForbiddenUsage(InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            {
                if (invocationExpression.ArgumentList.Arguments.Count == 0)
                {
                    return false;
                }

                var eventTypeArgument = invocationExpression.ArgumentList.Arguments.First();
                var firstArgumentText = eventTypeArgument.Expression.ToString();

                return ForbiddenEventTypeArgs.Contains(firstArgumentText);
            }
        }
    }
}
