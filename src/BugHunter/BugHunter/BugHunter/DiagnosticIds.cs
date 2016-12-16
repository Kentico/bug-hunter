﻿namespace BugHunter
{
    /// <summary>
    /// Class defining the IDs for diagnostic analyzers.
    /// </summary>
    internal static class DiagnosticIds
    {
        // CsRules
        // API Replacements
        public const string HTTP_SESSION_SESSION_ID = "BH1000";
        public const string HTTP_SESSION_ELEMENT_ACCESS_GET = "BH1001";
        public const string HTTP_SESSION_ELEMENT_ACCESS_SET = "BH1002";
        public const string HTTP_REQUEST_COOKIES = "BH1003";
        public const string HTTP_RESPONSE_COOKIES = "BH1004";
        public const string HTTP_REQUEST_USER_HOST_ADDRESS = "BH1005";
        public const string HTTP_REQUEST_URL = "BH1006";
        public const string HTTP_REQUEST_BROWSER = "BH1007";

        public const string FORMS_AUTHENTICATION_SIGN_OUT = "BH1008";
        public const string PAGE_IS_POST_BACK = "BH1009";
        public const string PAGE_IS_CALLBACK = "BH1010";
        public const string CLIENT_SCRIPT_METHODS = "BH1011";
        public const string HTTP_RESPONSE_REDIRECT = "BH1012";

        // ????
        public const string WHERE_LIKE_METHOD = "BH10X1";
        public const string EVENT_LOG_ARGUMENTS = "BH10X2";
        public const string LUCERNE_SEARCH_DOCUMENT = "BH10X3";


        // String methods
        public const string STRING_MANIPULATION_METHODS = "BH2000";
    }
}
