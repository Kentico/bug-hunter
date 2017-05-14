// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpResponseRedirectTest : CodeFixVerifier<HttpResponseRedirectAnalyzer, HttpResponseRedirectCodeFixProvider>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"new System.Web.HttpResponse(null)", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponse(null)", "LocalRedirect", 1)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "LocalRedirect", 1)]
        public void InputWithIncident_SipleMemberAccess_SurfacesDiagnostic(string instance, string codeFix, int codeFixNumber)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{ 
            var r = {instance};
            r.Redirect(""url"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult(@"r.Redirect(""url"")").WithLocation(9, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{ 
            var r = {instance};
            URLHelper.{codeFix}(""url"");
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [TestCase(@"new System.Web.HttpResponse(null)", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponse(null)", "LocalRedirect", 1)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "LocalRedirect", 1)]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string instance, string codeFix, int codeFixNumber)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            {instance}.Redirect(""url"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{instance}.Redirect(""url"")").WithLocation(8, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            URLHelper.{codeFix}(""url"");
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [TestCase(@"new System.Web.HttpResponse(null)")]
        [TestCase(@"new System.Web.HttpResponse(null)")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))")]
        public void InputWithIncident_ConditionalAccess_SurfacesDiagnostic_NoCodeFix(string instance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{ 
            var r = {instance};
            r?.Redirect(""url"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult(@".Redirect(""url"")").WithLocation(9, 15);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            VerifyCSharpFix(test, test);
        }

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HttpResponseRedirect,
                Message = string.Format(MessagesConstants.MessageNoSuggestion, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}