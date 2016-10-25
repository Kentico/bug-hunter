﻿using System;
using System.Collections.Immutable;
using BugHunter.Helpers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usage of <see cref="System.Web.HttpCookie"/> as properties of <see cref="System.Web.HttpRequest"/> or <see cref="System.Web.HttpResponse"/>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestAndResponseCookiesAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HttpRequestAndResponseCookie;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.HttpRequestAndResponseCookie_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpRequestAndResponseCookie_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CsRules,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpRequestAndResponseCookie_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(context, typeof(System.Web.HttpRequest), nameof(System.Web.HttpRequest.Cookies));
            RegisterAction(context, typeof(System.Web.HttpResponse), nameof(System.Web.HttpResponse.Cookies));
            RegisterAction(context, typeof(System.Web.HttpRequestBase), nameof(System.Web.HttpRequestBase.Cookies));
            RegisterAction(context, typeof(System.Web.HttpResponseBase), nameof(System.Web.HttpResponseBase.Cookies));
        }

        private void RegisterAction(AnalysisContext context, Type accessedType, string memberName)
        {
            var analyzer = new MemberAccessAnalysisHelper(Rule, accessedType, memberName);

            context.RegisterSyntaxNodeAction(c => analyzer.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
        }
    }
}
