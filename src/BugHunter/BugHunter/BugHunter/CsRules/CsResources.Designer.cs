﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BugHunter.CsRules {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CsResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CsResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BugHunter.CsRules.CsResources", typeof(CsResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replace with &apos;{0}&apos;..
        /// </summary>
        internal static string ApiReplacements_CodeFix {
            get {
                return ResourceManager.GetString("ApiReplacements_CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} should not be used. Use {1} instead..
        /// </summary>
        internal static string ApiReplacements_Description {
            get {
                return ResourceManager.GetString("ApiReplacements_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1} is accessed directly from {0}..
        /// </summary>
        internal static string ApiReplacements_MessageFormat {
            get {
                return ResourceManager.GetString("ApiReplacements_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} should not be used. Use {1} instead..
        /// </summary>
        internal static string ApiReplacements_Title {
            get {
                return ResourceManager.GetString("ApiReplacements_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogEvent called with event type &apos;{0}&apos;, use &apos;EventType.{1}&apos; instead..
        /// </summary>
        internal static string EventLogArguments_Description {
            get {
                return ResourceManager.GetString("EventLogArguments_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogEvent called with event type &apos;{0}&apos;..
        /// </summary>
        internal static string EventLogArguments_MessageFormat {
            get {
                return ResourceManager.GetString("EventLogArguments_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogEvent should not be called with hardcoded event type..
        /// </summary>
        internal static string EventLogArguments_Title {
            get {
                return ResourceManager.GetString("EventLogArguments_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;HttpCookie&apos; should not be used accessed directly. Use &apos;CookieHelper.ResponseCookies&apos; or &apos;CookieHelper.RequestCookies&apos; instead..
        /// </summary>
        internal static string HttpRequestAndResponseCookie_Description {
            get {
                return ResourceManager.GetString("HttpRequestAndResponseCookie_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}.{1}&apos; should not be used. Use &apos;CookieHelper.ResponseCookies&apos; or &apos;CookieHelper.RequestCookies&apos; instead..
        /// </summary>
        internal static string HttpRequestAndResponseCookie_MessageFormat {
            get {
                return ResourceManager.GetString("HttpRequestAndResponseCookie_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;HttpCookie&apos; should not be used accessed directly. Use &apos;CookieHelper.ResponseCookies&apos; or &apos;CookieHelper.RequestCookies&apos; instead..
        /// </summary>
        internal static string HttpRequestAndResponseCookie_Title {
            get {
                return ResourceManager.GetString("HttpRequestAndResponseCookie_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Request.Browser&apos; should not be used. Use &apos;BrowserHelper.GetBrowser()&apos; instead..
        /// </summary>
        internal static string HttpRequestBrowser_Description {
            get {
                return ResourceManager.GetString("HttpRequestBrowser_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}.{1}&apos; should not be used. Use &apos;BrowserHelper.GetBrowser()&apos; instead..
        /// </summary>
        internal static string HttpRequestBrowser_MessageFormat {
            get {
                return ResourceManager.GetString("HttpRequestBrowser_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Request.Browser&apos; should not be used..
        /// </summary>
        internal static string HttpRequestBrowser_Title {
            get {
                return ResourceManager.GetString("HttpRequestBrowser_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Request.Url&apos; should not be used. Use &apos;RequestContext.Url&apos; instead..
        /// </summary>
        internal static string HttpRequestUrl_Description {
            get {
                return ResourceManager.GetString("HttpRequestUrl_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}.{1}&apos; should not be used. Use &apos;RequestContext.Url&apos; instead..
        /// </summary>
        internal static string HttpRequestUrl_MessageFormat {
            get {
                return ResourceManager.GetString("HttpRequestUrl_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Request.Url&apos; should not be used..
        /// </summary>
        internal static string HttpRequestUrl_Title {
            get {
                return ResourceManager.GetString("HttpRequestUrl_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Request.UserHostAddress&apos; property is used. Use &apos;RequestContext.UserHostAddress&apos; instead..
        /// </summary>
        internal static string HttpRequestUserHostAddress_Description {
            get {
                return ResourceManager.GetString("HttpRequestUserHostAddress_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}.{1}&apos; should not be used. Use &apos;RequestContext.UserHostAddress&apos; instead..
        /// </summary>
        internal static string HttpRequestUserHostAddress_MessageFormat {
            get {
                return ResourceManager.GetString("HttpRequestUserHostAddress_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Request.UserHostAddress&apos; should not be used..
        /// </summary>
        internal static string HttpRequestUserHostAddress_Title {
            get {
                return ResourceManager.GetString("HttpRequestUserHostAddress_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Response.Redirect()&apos; should not be used. Use &apos;TODO&apos; instead..
        /// </summary>
        internal static string HttpResponseRedirect_Description {
            get {
                return ResourceManager.GetString("HttpResponseRedirect_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}.{1}&apos; should not be used. Use &apos;TODO&apos; instead..
        /// </summary>
        internal static string HttpResponseRedirect_MessageFormat {
            get {
                return ResourceManager.GetString("HttpResponseRedirect_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Response.Redirect()&apos; should not be used..
        /// </summary>
        internal static string HttpResponseRedirect_Title {
            get {
                return ResourceManager.GetString("HttpResponseRedirect_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Session[]&apos; should not be used. Use &apos;SessionHelper.GetValue()&apos; or &apos;SessionHelper.SetValue()&apos;  instead..
        /// </summary>
        internal static string HttpSessionElementAccess_Description {
            get {
                return ResourceManager.GetString("HttpSessionElementAccess_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; should not be used. Use &apos;SessionHelper.GetValue()&apos; or &apos;SessionHelper.SetValue()&apos; instead..
        /// </summary>
        internal static string HttpSessionElementAccess_MessageFormat {
            get {
                return ResourceManager.GetString("HttpSessionElementAccess_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Session[]&apos; should not be used. Use &apos;SessionHelper.GetValue()&apos; or &apos;SessionHelper.SetValue()&apos;  instead..
        /// </summary>
        internal static string HttpSessionElementAccess_Title {
            get {
                return ResourceManager.GetString("HttpSessionElementAccess_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Session.SessionID&apos; should not be used. Use &apos;SessionHelper.GetSessionID()&apos; instead..
        /// </summary>
        internal static string HttpSessionSessionId_Description {
            get {
                return ResourceManager.GetString("HttpSessionSessionId_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}.{1}&apos; should not be used. Use &apos;SessionHelper.GetSessionID()&apos; instead..
        /// </summary>
        internal static string HttpSessionSessionId_MessageFormat {
            get {
                return ResourceManager.GetString("HttpSessionSessionId_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;Session.SessionID&apos; should not be used. Use &apos;SessionHelper.GetSessionID()&apos; instead..
        /// </summary>
        internal static string HttpSessionSessionId_Title {
            get {
                return ResourceManager.GetString("HttpSessionSessionId_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using one of &apos;WhereStartsWith()&apos;, &apos;WhereEndsWith()&apos; or &apos;WhereContains()&apos; methods instead..
        /// </summary>
        internal static string WhereLikeMethod_Description {
            get {
                return ResourceManager.GetString("WhereLikeMethod_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method &apos;{1}&apos; should not be used..
        /// </summary>
        internal static string WhereLikeMethod_MessageFormat {
            get {
                return ResourceManager.GetString("WhereLikeMethod_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method &apos;WhereLike()&apos; or &apos;WhereNotLike()&apos; should not be used used..
        /// </summary>
        internal static string WhereLikeMethod_Title {
            get {
                return ResourceManager.GetString("WhereLikeMethod_Title", resourceCulture);
            }
        }
    }
}
