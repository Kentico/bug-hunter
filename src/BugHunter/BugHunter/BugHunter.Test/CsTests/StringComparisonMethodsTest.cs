﻿using BugHunter.CsRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class StringComparisonMethodsTest : CodeFixVerifier<StringComparisonMethodsAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return null;
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Equals(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"CompareTo(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"CompareTo(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"StartsWith(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"IndexOf(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"", false, CultureInfo.CurrentCulture)")]
        public void AllowedOverloadCalled_NoDiagnostic(string methodUsed)
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

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"CompareTo(""a"")", @"CompareTo(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"IndexOf(""a"")", @"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"")", @"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
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
                Id = DiagnosticIds.STRING_COMPARISON_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
            };
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{codeFix};
        }}
    }}
}}";
            //VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"CompareTo(""a"")", @"CompareTo(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"IndexOf(""a"")", @"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"")", @"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
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
                Id = DiagnosticIds.STRING_COMPARISON_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{codeFix}.ToString();
        }}
    }}
}}";
            //VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"CompareTo(""a"")", @"CompareTo(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"IndexOf(""a"")", @"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"")", @"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
       public void InputWithIncident_PrecedingMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
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
                Id = DiagnosticIds.STRING_COMPARISON_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 48) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.Substring(0).{codeFix}.ToString();
        }}
    }}
}}";
            //VerifyCSharpFix(test, expectedFix);
        }
    }
}