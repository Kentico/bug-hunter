﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BugHunter.Analyzers.CmsBaseClassesRules {
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
    internal class CmsBaseClassesResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CmsBaseClassesResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BugHunter.Analyzers.CmsBaseClassesRules.CmsBaseClassesResources", typeof(CmsBaseClassesResources).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to Add attribute: {0}..
        /// </summary>
        internal static string ModuleRegistration_CodeFix {
            get {
                return ResourceManager.GetString("ModuleRegistration_CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Modules and ModuleEntries must be registered in the same file where they are declared. Add assembly attribute [assembly: RegisterModule(typeof(&lt;ClassName&gt;))] to the file..
        /// </summary>
        internal static string ModuleRegistration_Description {
            get {
                return ResourceManager.GetString("ModuleRegistration_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module or ModuleEntry &apos;{0}&apos; is not registered in the same file where it is declared. Add assembly attribute [assembly: RegisterModule(typeof({0}))] to the file..
        /// </summary>
        internal static string ModuleRegistration_MessageFormat {
            get {
                return ResourceManager.GetString("ModuleRegistration_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module or ModuleEntry must be registered..
        /// </summary>
        internal static string ModuleRegistration_Title {
            get {
                return ResourceManager.GetString("ModuleRegistration_Title", resourceCulture);
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
    }
}