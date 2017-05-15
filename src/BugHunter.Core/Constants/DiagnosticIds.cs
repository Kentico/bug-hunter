// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BugHunter.Core.Constants
{
    /// <summary>
    /// Class defining the IDs for all diagnostic analyzers.
    /// </summary>
    /// <remarks>
    /// No not move to separate files/projects in order to ensure no duplicate identifiers are assigned
    /// </remarks>
#pragma warning disable SA1124 // Do not use regions
    public static class DiagnosticIds
    {
        #region AbstractionOverImplementation category

        /// <summary>
        /// The ID for LuceneSearchDocument diagnostics
        /// </summary>
        public const string LuceneSearchDocument = "BH0000";
        #endregion

        #region CmsApiReplacements category

        /// <summary>
        /// The ID for HttpSessionSessionId diagnostics
        /// </summary>
        public const string HttpSessionSessionId = "BH1000";

        /// <summary>
        /// The ID for HttpSessionElementAccessGet diagnostics
        /// </summary>
        public const string HttpSessionElementAccessGet = "BH1001";

        /// <summary>
        /// The ID for HttpSessionElementAccessSet diagnostics
        /// </summary>
        public const string HttpSessionElementAccessSet = "BH1002";

        /// <summary>
        /// The ID for HttpRequestCookies diagnostics
        /// </summary>
        public const string HttpRequestCookies = "BH1003";

        /// <summary>
        /// The ID for HttpResponseCookies diagnostics
        /// </summary>
        public const string HttpResponseCookies = "BH1004";

        /// <summary>
        /// The ID for HttpRequestUserHostAddress diagnostics
        /// </summary>
        public const string HttpRequestUserHostAddress = "BH1005";

        /// <summary>
        /// The ID for HttpRequestUrl diagnostics
        /// </summary>
        public const string HttpRequestUrl = "BH1006";

        /// <summary>
        /// The ID for HttpRequestBrowser diagnostics
        /// </summary>
        public const string HttpRequestBrowser = "BH1007";

        /// <summary>
        /// The ID for HttpResponseRedirect diagnostics
        /// </summary>
        public const string HttpResponseRedirect = "BH1008";

        /// <summary>
        /// The ID for HttpRequestQueryString diagnostics
        /// </summary>
        public const string HttpRequestQueryString = "BH1009";

        /// <summary>
        /// The ID for PageIsCallback diagnostics
        /// </summary>
        public const string PageIsCallback = "BH1010";

        /// <summary>
        /// The ID for PageIsPostBack diagnostics
        /// </summary>
        public const string PageIsPostBack = "BH1011";

        /// <summary>
        /// The ID for FormsAuthenticationSignOut diagnostics
        /// </summary>
        public const string FormsAuthenticationSignOut = "BH1012";

        /// <summary>
        /// The ID for ClientScriptMethods diagnostics
        /// </summary>
        public const string ClientScriptMethods = "BH1013";

        /// <summary>
        /// The ID for SystemIO diagnostics
        /// </summary>
        public const string SystemIO = "BH1014";
        #endregion

        #region CmsApiGuidelines category

        /// <summary>
        /// The ID for WhereLikeMethod diagnostics
        /// </summary>
        public const string WhereLikeMethod = "BH2000";

        /// <summary>
        /// The ID for EventLogArguments diagnostics
        /// </summary>
        public const string EventLogArguments = "BH2001";

        /// <summary>
        /// The ID for ValidationHelperGet diagnostics
        /// </summary>
        public const string ValidationHelperGet = "BH2500";

        /// <summary>
        /// The ID for ConnectionHelperExecuteQuery diagnostics
        /// </summary>
        public const string ConnectionHelperExecuteQuery = "BH2501";
        #endregion

        #region CmsBaseClasses category

        /// <summary>
        /// The ID for ModuleRegistration diagnostics
        /// </summary>
        public const string ModuleRegistration = "BH3000";

        /// <summary>
        /// The ID for WebPartBase diagnostics
        /// </summary>
        public const string WebPartBase = "BH3500";

        /// <summary>
        /// The ID for UIWebPartBase diagnostics
        /// </summary>
        public const string UIWebPartBase = "BH3501";

        /// <summary>
        /// The ID for PageBase diagnostics
        /// </summary>
        public const string PageBase = "BH3502";

        /// <summary>
        /// The ID for UserControlBase diagnostics
        /// </summary>
        public const string UserControlBase = "BH3503";
        #endregion

        #region StringAndCulture category

        /// <summary>
        /// The ID for StringManipulationMethods diagnostics
        /// </summary>
        public const string StringManipulationMethods = "BH4000";

        /// <summary>
        /// The ID for StringEqualsMethod diagnostics
        /// </summary>
        public const string StringEqualsMethod = "BH4001";

        /// <summary>
        /// The ID for StringCompareToMethod diagnostics
        /// </summary>
        public const string StringCompareToMethod = "BH4002";

        /// <summary>
        /// The ID for StringStartsEndsWithMethods diagnostics
        /// </summary>
        public const string StringStartsEndsWithMethods = "BH4003";

        /// <summary>
        /// The ID for StringIndexOfMethods diagnostics
        /// </summary>
        public const string StringIndexOfMethods = "BH4004";

        /// <summary>
        /// The ID for StringCompareStaticMethod diagnostics
        /// </summary>
        public const string StringCompareStaticMethod = "BH4005";
        #endregion
    }
#pragma warning restore SA1124 // Do not use regions
}
