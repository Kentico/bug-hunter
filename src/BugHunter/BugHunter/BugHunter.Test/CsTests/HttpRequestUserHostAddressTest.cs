﻿using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Kentico.Google.Apis.Util;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpRequestUserHostAddressTest : CodeFixVerifier<HttpRequestUserHostAddressAnalyzer, HttpRequestUserHostAddressCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var address = request.UserHostAddress;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_USER_HOST_ADDRESS,
                Message = MessagesConstants.MESSAGE.FormatString("request.UserHostAddress", "RequestContext.UserHostAddress"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var address = RequestContext.UserHostAddress;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var address = {requestInstance}.UserHostAddress;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_USER_HOST_ADDRESS,
                Message = MessagesConstants.MESSAGE.FormatString($"{requestInstance}.UserHostAddress", "RequestContext.UserHostAddress"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var address = RequestContext.UserHostAddress;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}