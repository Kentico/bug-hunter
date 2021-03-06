﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BugHunter.Analyzers.CmsApiReplacementRules {
    using System;
    using System.Reflection;
    
    
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
    internal class CmsApiReplacementsResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CmsApiReplacementsResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BugHunter.Analyzers.CmsApiReplacementRules.CmsApiReplacementsResources", typeof(CmsApiReplacementsResources).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to Use when redirecting to external URL..
        /// </summary>
        internal static string RedirectCodeFixExternal {
            get {
                return ResourceManager.GetString("RedirectCodeFixExternal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use when redirecting to local URL..
        /// </summary>
        internal static string RedirectCodeFixLocal {
            get {
                return ResourceManager.GetString("RedirectCodeFixLocal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;System.IO&apos; namespace should not be used directly. Use equivalent method from namespace &apos;CMS.IO&apos;..
        /// </summary>
        internal static string SystemIo_Description {
            get {
                return ResourceManager.GetString("SystemIo_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; should not use &apos;System.IO&apos; directly. Use equivalent method from namespace &apos;CMS.IO&apos;..
        /// </summary>
        internal static string SystemIo_MessageFormat {
            get {
                return ResourceManager.GetString("SystemIo_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not use System.IO.
        /// </summary>
        internal static string SystemIo_Title {
            get {
                return ResourceManager.GetString("SystemIo_Title", resourceCulture);
            }
        }
    }
}
