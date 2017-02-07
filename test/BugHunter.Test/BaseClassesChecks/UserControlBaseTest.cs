﻿using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.BaseClassesRules.CodeFixes;
using BugHunter.Test.Verifiers;
using CMS.Base.Web.UI;
using CMS.UIControls;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.BaseClassesChecks
{
    [TestFixture]
    public class UserControlBaseTest : CodeFixVerifier<UserControlBaseAnalyzer, UserControlBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference }).ToArray();
        }

        private readonly FakeFileInfo _userControlFakeFileInfo = new FakeFileInfo { FileExtension= "ascx.cs" };
        
        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.USER_CONTROL_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from some abstract CMSControl.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"cs", @"")]
        [TestCase(@"cs", @": System.Web.UI.UserControl")]
        [TestCase(@"this.should.prevent.from.diagnostic.being.raised.cs", @"")]
        [TestCase(@"this.should.prevent.from.diagnostic.being.raised.cs", @": System.Web.UI.UserControl")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedFileExtension, string baseList)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass {baseList}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, new FakeFileInfo {FileExtension = excludedFileExtension});
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

            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo);
        }

        [TestCase(nameof(CMS.Base.Web.UI.AbstractUserControl))]
        [TestCase("CMS.Base.Web.UI.AbstractUserControl")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo);
        }

        [TestCase(nameof(System.Web.UI.UserControl))]
        [TestCase("System.Web.UI.UserControl")]
        public void InputWithError_ClassNotExtendingCMSClass_SurfacesDiagnostic(string oldUsage)
        {
            var test = $@"using System.Web.UI;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(5, 26, _userControlFakeFileInfo);

            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo, expectedDiagnostic);
        }

        private static readonly object[] CodeFixesTestSource = {
            new object [] {nameof(CMSUserControl), "CMS.UIControls", 0},
            new object [] {nameof(AbstractUserControl), "CMS.Base.Web.UI", 1},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.UserControl
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(3, 26, _userControlFakeFileInfo);

            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, _userControlFakeFileInfo);
        }
    }
}