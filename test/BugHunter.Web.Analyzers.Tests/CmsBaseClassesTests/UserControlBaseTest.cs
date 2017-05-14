// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.CodeFixes;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Web.Analyzers.Tests.CmsBaseClassesTests
{
    [TestFixture]
    public class UserControlBaseTest : CodeFixVerifier<UserControlBaseAnalyzer, UserControlBaseCodeFixProvider>
    {
        private static readonly FakeFileInfo UserControlFakeFileInfo = new FakeFileInfo { FileExtension = "ascx.cs" };

        private static readonly object[] CodeFixesTestSource =
        {
            new object[] { nameof(CMS.UIControls.CMSUserControl), "CMS.UIControls", 0 },
            new object[] { nameof(CMS.Base.Web.UI.AbstractUserControl), "CMS.Base.Web.UI", 1 },
        };

        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences
                .Union(ReferencesHelper.GetReferencesFor(typeof(CMS.UIControls.CMSUserControl)))
                .Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference }).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [Ignore("Implementation has change and is now agnostic of file paths")]
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
            VerifyCSharpDiagnostic(test, new FakeFileInfo { FileExtension = excludedFileExtension });
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

            VerifyCSharpDiagnostic(test, UserControlFakeFileInfo);
        }

        [TestCase(nameof(CMS.Base.Web.UI.AbstractUserControl))]
        [TestCase("CMS.Base.Web.UI.AbstractUserControl")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"using CMS.Base.Web.UI;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, UserControlFakeFileInfo);
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
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(5, 26, UserControlFakeFileInfo);

            VerifyCSharpDiagnostic(test, UserControlFakeFileInfo, expectedDiagnostic);
        }

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.UserControl
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(3, 26, UserControlFakeFileInfo);

            VerifyCSharpDiagnostic(test, UserControlFakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, UserControlFakeFileInfo);
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArguments)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.UserControlBase,
                Message = $"'{messageArguments[0]}' should inherit from some abstract CMSUserControl.",
                Severity = DiagnosticSeverity.Warning,
            };
    }
}