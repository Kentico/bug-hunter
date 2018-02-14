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
    public class PageBaseTest : CodeFixVerifier<PageBaseAnalyzer, PageBaseCodeFixProvider>
    {
        private static readonly FakeFileInfo PagesFakeFileInfo = new FakeFileInfo { FileExtension = "aspx.cs" };

        private static readonly object[] CodeFixesTestSource =
        {
            new object[] { nameof(CMS.UIControls.AbstractCMSPage), "CMS.UIControls", 0 },
            new object[] { nameof(CMS.UIControls.CMSUIPage), "CMS.UIControls", 1 },
        };

        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences
                .Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference, ReferencesHelper.CMSUiControls })
                .ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [Ignore("Implementation has change and is now agnostic of file paths")]
        [TestCase(@"cs", @"")]
        [TestCase(@"cs", @": System.Web.UI.Page")]
        [TestCase(@"this.should.prevent.from.diagnostic.being.raised.cs", @"")]
        [TestCase(@"this.should.prevent.from.diagnostic.being.raised.cs", @": System.Web.UI.Page")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedFileExtension, string baseList)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass {baseList}
    {{
    }}
}}";
            var fakeFileInfo = new FakeFileInfo { FileExtension = excludedFileExtension };
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

            VerifyCSharpDiagnostic(test, PagesFakeFileInfo);
        }

        [TestCase(nameof(CMS.UIControls.AbstractCMSPage))]
        [TestCase("CMS.UIControls.AbstractCMSPage")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"using CMS.UIControls;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, PagesFakeFileInfo);
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
        public string GetName() 
        {{
            var name = nameof(SampleClass);
            return name;
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(5, 26, PagesFakeFileInfo);

            VerifyCSharpDiagnostic(test, PagesFakeFileInfo, expectedDiagnostic);
        }

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.Page
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(3, 26, PagesFakeFileInfo);

            VerifyCSharpDiagnostic(test, PagesFakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, PagesFakeFileInfo);
        }

        [Test]
        public void InputWithError_MultiplePartialClassDeclarations_SurfacesDiagnosticsForAllLocations()
        {
            var test = @"namespace SapleTestProject.CsSamples
{
    public partial class SampleClass
    {
    }

    public partial class SampleClass : System.Web.UI.Page
    {   
    }

}";
            var firstDeclarationLocation = GetDiagnosticResult("SampleClass").WithLocation(3, 26, PagesFakeFileInfo);
            var secondDeclarationLocation = GetDiagnosticResult("SampleClass").WithLocation(7, 26, PagesFakeFileInfo);

            VerifyCSharpDiagnostic(test, PagesFakeFileInfo, firstDeclarationLocation, secondDeclarationLocation);
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArguments)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.PageBase,
                Message = $"'{messageArguments[0]}' should inherit from some abstract CMSPage.",
                Severity = DiagnosticSeverity.Warning,
            };
    }
}