﻿using BugHunter.CsRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class ConnectionHelperExecuteQueryTest : CodeFixVerifier<ConnectionHelperExecuteQueryAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences;
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.CONNECTION_HELPER_EXECUTE_QUERY,
                Message = $"'{messageArgumentStrings[0]}' should not be called directly from this file. Move logic to codebehind.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedPath)
        {
            var test = @"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass
    {{
        public void SampleMethod()
        {{
            CMS.DataEngine.ConnectionHelper.ExecuteQuery(null);
        }}
    }}
}}";
            var fakeFileInfo = new FakeFileInfo {FileLoaction = excludedPath};
            VerifyCSharpDiagnostic(test, fakeFileInfo);
        }


        [TestCase("aspx.cs")]
        [TestCase("ascx.cs")]
        [TestCase("ashx.cs")]
        [TestCase("master.cs")]
        public void InputWithError_ExecuteQueryCalledFromUi_SurfacesDiagnostic(string fileExtension)
        {
            var test = $@"using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass
    {{
        public void SampleMethod() {{
            ConnectionHelper.ExecuteQuery(null);
            CMS.DataEngine.ConnectionHelper.ExecuteQuery(null);
        }}
    }}
}}";
            var fakeFileInfo = new FakeFileInfo {FileExtension = fileExtension};
            var expectedDiagnostic1 = GetDiagnosticResult("ConnectionHelper.ExecuteQuery()").WithLocation(8, 13, fakeFileInfo);
            var expectedDiagnostic2 = GetDiagnosticResult("CMS.DataEngine.ConnectionHelper.ExecuteQuery()").WithLocation(9, 13, fakeFileInfo);

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic1, expectedDiagnostic2);
        }
    }
}