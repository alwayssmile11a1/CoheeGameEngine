﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cohee.Editor.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class GeneralRes {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal GeneralRes() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cohee.Editor.Properties.GeneralRes", typeof(GeneralRes).Assembly);
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
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;UserData&gt;
        ///  &lt;EditorApp&gt;
        ///    &lt;Backups&gt;true&lt;/Backups&gt;
        ///    &lt;Autosaves&gt;ThirtyMinutes&lt;/Autosaves&gt;
        ///    &lt;FirstSession&gt;true&lt;/FirstSession&gt;
        ///  &lt;/EditorApp&gt;
        ///  &lt;Plugins&gt;
        ///    &lt;Plugin id=&quot;CamView&quot;&gt;
        ///      &lt;CamView id=&quot;0&quot;&gt;
        ///        &lt;Perspective&gt;Parallax&lt;/Perspective&gt;
        ///        &lt;FocusDist&gt;500&lt;/FocusDist&gt;
        ///        &lt;BackgroundColor&gt;
        ///          &lt;R&gt;64&lt;/R&gt;
        ///          &lt;G&gt;64&lt;/G&gt;
        ///          &lt;B&gt;64&lt;/B&gt;
        ///          &lt;A&gt;0&lt;/A&gt;
        ///        &lt;/BackgroundColor&gt;
        ///        &lt;SnapToGridSize&gt;
        ///          &lt;X&gt;0&lt;/X&gt;
        ///           [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DefaultEditorUserData {
            get {
                return ResourceManager.GetString("DefaultEditorUserData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon IconWorkingFolder {
            get {
                object obj = ResourceManager.GetObject("IconWorkingFolder", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
