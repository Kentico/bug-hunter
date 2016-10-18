﻿namespace BugHunter
{
    /// <summary>
    /// Class defining the IDs for diagnostic analyzers.
    /// </summary>
    internal static class DiagnosticIds
    {
        // CsRules
        public const string WhereLikeMethod = "BH1000";
        public const string EventLogArguments = "BH1001";
        public const string RequestUserHostAddress = "BH1002";
        public const string HttpSessionAccess = "BH1003";
        public const string HttpSessionId = "BH1004";
    }
}
