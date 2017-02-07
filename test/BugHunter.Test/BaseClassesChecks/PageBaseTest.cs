﻿using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.BaseClassesRules.CodeFixes;
using BugHunter.Test.Verifiers;
using CMS.UIControls;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.BaseClassesChecks
{
    [TestFixture]
    public class PageBaseTest : CodeFixVerifier<PageBaseAnalyzer, PageBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference }).ToArray();
        }

        private readonly FakeFileInfo _pagesFakeFileInfo = new FakeFileInfo() {FilePath = ProjectPaths.PAGES};

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.PAGE_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from some abstract CMSPage.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"", @"")]
        [TestCase(@"", @": System.Web.UI.Page")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @"")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @": System.Web.UI.Page")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedPath, string baseList)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass {baseList}
    {{
    }}
}}";
            var fakeFileInfo = new FakeFileInfo() {FilePath = excludedPath};
            VerifyCSharpDiagnostic(test, fakeFileInfo);
        }

        [Test]
        public void InputWithError_ClassNotExtendingAnyClass_NoDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass
    {{
    }}
}}";

            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo);
        }

        [TestCase(nameof(CMS.UIControls.AbstractCMSPage))]
        [TestCase("CMS.UIControls.AbstractCMSPage")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo);
        }

        [TestCase(nameof(System.Web.UI.Page))]
        [TestCase("System.Web.UI.Page")]
        public void InputWithError_ClassNotExtendingCMSClass_SurfacesDiagnostic(string oldUsage)
        {
            var test = $@"using System.Web.UI;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo, expectedDiagnostic.WithLocation(5, 26, ProjectPaths.PAGES + "Test0.cs"));
        }

        private static readonly object[] CodeFixesTestSource = {
            new object [] {nameof(AbstractCMSPage), "CMS.UIControls", 0},
            new object [] {nameof(CMSUIPage), "CMS.UIControls", 1},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.Page
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo, expectedDiagnostic.WithLocation(3, 26, ProjectPaths.PAGES + "Test0.cs"));

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, _pagesFakeFileInfo);
        }
    }
}