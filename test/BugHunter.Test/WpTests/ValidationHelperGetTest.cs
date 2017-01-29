﻿using System;
using BugHunter.Test.Verifiers;
using BugHunter.WpRules.Analyzers;
using BugHunter.WpRules.CodeFixes;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.WpTests
{
    [TestFixture]
    public class ValidationHelperGetTest : CodeFixVerifier<ValidationHelperGetAnalyzer, ValidationHelperGetCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return new[] {ReferencesHelper.CMSHelpersReference};
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.VALIDATION_HELPER_GET,
                Message = string.Format("Do not use {0}(). Use Get method with 'System' instead to ensure specific culture representation.", messageArgumentStrings),
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"GetDouble(""0"", 0)", @"GetDoubleSystem(""0"", 0)")]
        [TestCase(@"GetDouble(""0"", 0, CultureInfo.CurrentUICulture)", @"GetDoubleSystem(""0"", 0)")]
        [TestCase(@"GetDate(""0"", DateTime.MaxValue)", @"GetDateSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDate(""0"", DateTime.MaxValue, CultureInfo.CurrentUICulture)", @"GetDateSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDateTime(""0"", DateTime.MaxValue)", @"GetDateTimeSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDateTime(""0"", DateTime.MaxValue, ""en-us"")", @"GetDateTimeSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDateTime(""0"", DateTime.MaxValue, CultureInfo.CurrentUICulture)", @"GetDateTimeSystem(""0"", DateTime.MaxValue)")]
        public void InputWithError_SimpleMemberAccess_SurfacesDiagnostic(string oldUsage, string codeFix)
        {
            var test = $@"using System;
using System.Globalization;
using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            ValidationHelper.{oldUsage}.ToString();
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(oldUsage.Substring(0, oldUsage.IndexOf("(", StringComparison.Ordinal))).WithLocation(11, 30);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;
using System.Globalization;
using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            ValidationHelper.{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, null, true);
        }
    }
}