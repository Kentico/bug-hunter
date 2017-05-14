// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers;
using BugHunter.Analyzers.CmsApiGuidelinesRules.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiGuidelines
{
    [TestFixture]
    public class WhereLikeMethodTest : CodeFixVerifier<WhereLikeMethodAnalyzer, WhereLikeMethodCodeFixProvider>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences;

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithWhereLike_SimpleMemberAccess_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{oldMethodCall}(""columnName"", ""value"");
            whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")").WithLocation(9, 45);

            VerifyCSharpDiagnostic(
                test,
                expectedDiagnostic,
                expectedDiagnostic.WithLocation(10, 46));

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{newMethodCall}(""columnName"", ""value"");
            whereCondition = whereCondition?.{newMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithWhereLike_FollowUpMemberAccess_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{oldMethodCall}(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName1"", ""value"")").WithLocation(9, 45);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{newMethodCall}(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithWhereLike_PrecedingMemberAccess_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.Or().{oldMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")").WithLocation(9, 50);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.Or().{newMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithFullyQualifiedWhereLike_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition().{oldMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")").WithLocation(8, 70);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition().{newMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike")]
        [TestCase("WhereNotLike")]
        public void InputWithWhereLike_ConditionalAccesses_SurfacesAllDiagnostics(string methodCall)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{methodCall}(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");
            whereCondition = whereCondition?.{methodCall}(""columnName2"", ""value"").WhereTrue(""this is gonna be tricky"");
            whereCondition = whereCondition.{methodCall}(""columnName3"", ""value"")?.WhereTrue(""this is gonna be tricky"");
            whereCondition = whereCondition?.{methodCall}(""columnName4"", ""value"")?.WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";

            VerifyCSharpDiagnostic(
                test,
                CreateDiagnosticResult($@"{methodCall}(""columnName1"", ""value"")").WithLocation(9, 45),
                CreateDiagnosticResult($@"{methodCall}(""columnName2"", ""value"")").WithLocation(10, 46),
                CreateDiagnosticResult($@"{methodCall}(""columnName3"", ""value"")").WithLocation(11, 45),
                CreateDiagnosticResult($@"{methodCall}(""columnName4"", ""value"")").WithLocation(12, 46));
        }

        [TestCase(
            @"whereCondition = whereCondition.WhereNotLike(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");",
                  @"whereCondition = whereCondition.WhereNotContains(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");")]
        [TestCase(
            @"whereCondition = whereCondition?.WhereNotLike(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");",
                  @"whereCondition = whereCondition?.WhereNotContains(""columnName1"", ""value"").WhereTrue(""this is gonna be tricky"");")]
        [TestCase(
            @"whereCondition = whereCondition.WhereNotLike(""columnName1"", ""value"")?.WhereTrue(""this is gonna be tricky"");",
                  @"whereCondition = whereCondition.WhereNotContains(""columnName1"", ""value"")?.WhereTrue(""this is gonna be tricky"");")]
        [TestCase(
            @"whereCondition = whereCondition?.WhereNotLike(""columnName1"", ""value"")?.WhereTrue(""this is gonna be tricky"");",
                  @"whereCondition = whereCondition?.WhereNotContains(""columnName1"", ""value"")?.WhereTrue(""this is gonna be tricky"");")]
        public void InputWithWhereLike_ConditionalAccesses_AppliesFirstCodeFixInAllCases(string before, string after)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            {before}
        }}
    }}
}}";

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            {after}
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, 0);
        }

        [Test]
        public void InputWithPossibleFalsePositive_NoDiagnostic()
        {
            var test = @"
namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        public void WhereLike() {
            // do nothing
        }

        public void FalsePositiveForWhereLike()
        {
            WhereLike();
            this.WhereLike();
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        private static DiagnosticResult CreateDiagnosticResult(string usage)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.WhereLikeMethod,
                Message = string.Format(MessagesConstants.MessageNoSuggestion, usage),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}