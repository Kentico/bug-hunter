﻿using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.StringMethodsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringStartsEndsWithTest : CodeFixVerifier<StringStartAndEndsWithMethodsAnalyzer, StringComparisonMethodsWithModifierCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return null;
        }

        static readonly object[] TestSource =
        {
            new object[] { @"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.CurrentCulture)", 0 },
            new object[] { @"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.CurrentCultureIgnoreCase)", 1 },
            new object[] { @"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCulture)", 2 },
            new object[] { @"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)", 3 },
            new object[] { @"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.CurrentCulture)", 0 },
            new object[] { @"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.CurrentCultureIgnoreCase)", 1 },
            new object[] { @"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCulture)", 2 },
            new object[] { @"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)", 3 },
        };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"StartsWith(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"", false, CultureInfo.CurrentCulture)")]
        public void AllowedOverloadCalled_NoDiagnostic(string methodUsed)
        {
            var test = $@"using System;
using System.Globalization;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{methodUsed};
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {   
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
            };
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{codeFix};
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_PrecedingMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.Substring(0).{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 48) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.Substring(0).{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }
    }
}