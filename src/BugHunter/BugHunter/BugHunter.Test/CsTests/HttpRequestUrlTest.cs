﻿using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpRequestUrlTest : CodeFixVerifier<HttpRequestUrlAnalyzer, HttpRequestUrlCodeFixProvider>
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
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var url = request.Url;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_URL,
                Message = @"'request.Url' should not be used. Use 'RequestContext.Url' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 23) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var url = RequestContext.URL;
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
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var url = {requestInstance}.Url;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_URL,
                Message = $@"'{requestInstance}.Url' should not be used. Use 'RequestContext.Url' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 23) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var url = RequestContext.URL;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}