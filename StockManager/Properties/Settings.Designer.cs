//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StockManager.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>.eps</string>\r\n  <string>.ai</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AllowedFileExtensions {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AllowedFileExtensions"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Icons")]
        public string IconDirectoryName {
            get {
                return ((string)(this["IconDirectoryName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Backgrounds")]
        public string BackgroundDirectoryName {
            get {
                return ((string)(this["BackgroundDirectoryName"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection NameTemplates {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["NameTemplates"]));
            }
            set {
                this["NameTemplates"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Sets")]
        public string SetsDirectory {
            get {
                return ((string)(this["SetsDirectory"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int DocumentSize {
            get {
                return ((int)(this["DocumentSize"]));
            }
            set {
                this["DocumentSize"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>icon</string>
  <string>vector</string>
  <string>illustration</string>
  <string>set</string>
  <string>pictogram</string>
  <string>symbol</string>
  <string>sign</string>
  <string>design</string>
  <string>art</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection RequiredKeywords {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RequiredKeywords"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"For templates there are several patterrns:

theme - Theme name (capitalized)
themeLowerCase - Theme name (lower case)
keywords - comma separated keywords
keywordCount - number of keywords in composition
icons - comma separated icon names (in database not file names)
iconCount - number of icons in composition
keywordsRandom:n - comma separated random keywords - n is for number
iconsRandom:n/IconsRandom:n - comma separated random icon names - n is for number

All patterns must be enclosed in square brackets. Example:
Icon set about [themeLowerCase] with keywords: [keywordsRandom:6]")]
        public string NameTemplatesInfo {
            get {
                return ((string)(this["NameTemplatesInfo"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        public int JpegMaxKeywords {
            get {
                return ((int)(this["JpegMaxKeywords"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RestartIllustrator {
            get {
                return ((bool)(this["RestartIllustrator"]));
            }
            set {
                this["RestartIllustrator"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int AfterEachNSets {
            get {
                return ((int)(this["AfterEachNSets"]));
            }
            set {
                this["AfterEachNSets"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string GeneratorKeywords {
            get {
                return ((string)(this["GeneratorKeywords"]));
            }
            set {
                this["GeneratorKeywords"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Adobe\\Adobe Illustrator CC 2017\\Support Files\\Contents\\Windows\\I" +
            "llustrator.exe")]
        public string IllustratorPath {
            get {
                return ((string)(this["IllustratorPath"]));
            }
            set {
                this["IllustratorPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120000")]
        public int WaitForIllustrator {
            get {
                return ((int)(this["WaitForIllustrator"]));
            }
            set {
                this["WaitForIllustrator"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("500")]
        public int WaitForFileInterval {
            get {
                return ((int)(this["WaitForFileInterval"]));
            }
            set {
                this["WaitForFileInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int WaitForFileTriesNumber {
            get {
                return ((int)(this["WaitForFileTriesNumber"]));
            }
            set {
                this["WaitForFileTriesNumber"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3000")]
        public int WaitForIllustratorSavesFile {
            get {
                return ((int)(this["WaitForIllustratorSavesFile"]));
            }
            set {
                this["WaitForIllustratorSavesFile"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Scripts")]
        public string ScriptsDirectory {
            get {
                return ((string)(this["ScriptsDirectory"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int JpegSize {
            get {
                return ((int)(this["JpegSize"]));
            }
            set {
                this["JpegSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int MinimumCombinationsWithoutCheck {
            get {
                return ((int)(this["MinimumCombinationsWithoutCheck"]));
            }
            set {
                this["MinimumCombinationsWithoutCheck"] = value;
            }
        }
    }
}
