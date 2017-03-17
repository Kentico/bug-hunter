﻿using System.Collections.Generic;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    public static class StringComparisonOptions
    {
        internal static IEnumerable<string> GetAll()
        {
            yield return "StringComparison.Ordinal";
            yield return "StringComparison.OrdinalIgnoreCase";
            yield return "StringComparison.CurrentCulture";
            yield return "StringComparison.CurrentCultureIgnoreCase";
            yield return "StringComparison.InvariantCulture";
            yield return "StringComparison.InvariantCultureIgnoreCase";
        }

        internal static IEnumerable<string> GetCaseInsensitive()
        {
            yield return "StringComparison.OrdinalIgnoreCase";
            yield return "StringComparison.CurrentCultureIgnoreCase";
            yield return "StringComparison.InvariantCultureIgnoreCase";
        }

        internal static IEnumerable<string> GetCaseSensitive()
        {
            yield return "StringComparison.Ordinal";
            yield return "StringComparison.CurrentCulture";
            yield return "StringComparison.InvariantCulture";
        }
    }
}