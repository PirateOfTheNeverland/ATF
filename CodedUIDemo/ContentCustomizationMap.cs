namespace IPMATFramework.ContentCustomization
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Runtime.Serialization;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    [System.Runtime.Serialization.DataContractAttribute(Name = "IpmContentCustomization")]
    public partial class IpmContentCustomization
    {

        [System.Xml.Serialization.XmlElementAttribute("ApplicationButtonList", typeof(ApplicationButtonsTypeList), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("EndOfLicense", typeof(EndOfLicenseType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("GoogleAnalyticsSettings", typeof(GoogleAnalyticsSettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("GooglePlaySettings", typeof(GooglePlaySettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("KisBrowserRedirectionSettings", typeof(KisBrowserRedirectionSettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("KisNewVersionInfo", typeof(KisNewVersionInfoType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("LicenseNotification", typeof(LicenseNotificationType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("LicenseNotificationSettings", typeof(LicenseNotificationSettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("News", typeof(NewsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("RegretPage", typeof(RegretPageType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("SocializationSettings", typeof(SocializationSettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("UcpReadySettings", typeof(UcpReadySettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("WebSurveySettings", typeof(WebSurveySettingsType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("WelcomePage", typeof(WelcomePageType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Item { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ApplicationButtonsTypeList")]
    public partial class ApplicationButtonsTypeList
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("Button", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ApplicationButtonType[] Button { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ApplicationButtonType")]
    public partial class ApplicationButtonType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ButtonName { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ApplicationButtonEnum ButtonType { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("CustomUri", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Xml.Serialization.XmlElementAttribute("ProductStoreUri", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Item { get; set; }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ItemChoiceType ItemElementName { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum ApplicationButtonEnum
    {

        /// <remarks/>
        Buy,

        /// <remarks/>
        Renew,

        /// <remarks/>
        Upgrade,

        /// <remarks/>
        ManageLicense,

        /// <remarks/>
        ManageSubscription,

        /// <remarks/>
        SubscriptionProvider,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemChoiceType
    {

        /// <remarks/>
        CustomUri,

        /// <remarks/>
        ProductStoreUri,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "KisBrowserRedirectionSettingsType")]
    public partial class KisBrowserRedirectionSettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("BrowserUri", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] BrowserUriList { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RedirectionServiceUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "KisNewVersionInfoType")]
    public partial class KisNewVersionInfoType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContentUri { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ApplicationId { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ApplicationVersion { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ShowMessageTitle { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DistributiveDownloadUri { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public DistributiveTypeEnum DistributiveType { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ShowMessageDaysCount { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ShowMessageDelayInDays { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum DistributiveTypeEnum
    {

        /// <remarks/>
        Setup,

        /// <remarks/>
        Webpack,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "SocializationSettingsType")]
    public partial class SocializationSettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SocializationId { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableSocialization { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "GooglePlaySettingsType")]
    public partial class GooglePlaySettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BuyGooglePlayProductId { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RenewGooglePlayProductId { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "WebSurveySettingsType")]
    public partial class WebSurveySettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PerformAfterPurchaseSurvey { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SurveyUrl { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "GoogleAnalyticsSettingsType")]
    public partial class GoogleAnalyticsSettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool SendApplicationStatistics { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "UcpReadySettingsType")]
    public partial class UcpReadySettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public UcpConnectionTypeEnum UcpConnectionType { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UcpPortalUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum UcpConnectionTypeEnum
    {

        /// <remarks/>
        Disable,

        /// <remarks/>
        RegistrationOnDemand,

        /// <remarks/>
        RegistrationAlways,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ApplicationMainPageType")]
    public partial class ApplicationMainPageType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProtectionStateName { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LicenseStateName { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowApplicationButton { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ProblemListType")]
    public partial class ProblemListType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LicenseProblemDescription { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseManagerButtonType")]
    public partial class LicenseManagerButtonType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ButtonName { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DisplayShoppingCart { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ApplicationUriEnum ApplicationAction { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CustomUri { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProductStoreUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum ApplicationUriEnum
    {

        /// <remarks/>
        OpenCustomUri,

        /// <remarks/>
        Buy,

        /// <remarks/>
        UpgradeToKIS,

        /// <remarks/>
        OpenCustomProductStore,

        /// <remarks/>
        LicenseManager,

        /// <remarks/>
        CloudProtection,

        /// <remarks/>
        SettingsProtection,

        /// <remarks/>
        SettingsScan,

        /// <remarks/>
        SettingsUpdate,

        /// <remarks/>
        SettingsAdvanced,

        /// <remarks/>
        SettingsKSN,

        /// <remarks/>
        SettingsNotifications,

        /// <remarks/>
        ReportThreats,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseManagerTypeBackgroundType")]
    public partial class LicenseManagerTypeBackgroundType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BackgroundImageUri { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RightMargin { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LeftMargin { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TopMargin { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BottomMargin { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseManagerType")]
    public partial class LicenseManagerType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseManagerTypeBackgroundType Background { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Button", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseManagerButtonType[] Buttons { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "BalloonType")]
    public partial class BalloonType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MessageBoxType")]
    public partial class MessageBoxType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WindowHeigth { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WindowWidth { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContentUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseNotificationType")]
    public partial class LicenseNotificationType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public SeverityEnum Severity { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ScheduleType Schedule { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MessageBoxType MessageBox { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public BalloonType Balloon { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseManagerType LicenseManager { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ProblemListType ProblemList { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ApplicationMainPageType ApplicationMainPage { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<bool> ShowToast { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ShowToastSpecified
        {
            get
            {
                return ShowToast.HasValue;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum SeverityEnum
    {

        /// <remarks/>
        Alarm,

        /// <remarks/>
        Warning,

        /// <remarks/>
        Information,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ScheduleType")]
    public partial class ScheduleType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseExpirationType LicenseExpiration { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseSubscriptionType LicenseSubscription { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LicenseStatus", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseStatusEnum[] IncludeLicenseStatusList { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LicenseStatus", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseStatusEnum[] ExcludeLicenseStatusList { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LicenseType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseTypeEnum[] IncludeLicenseTypeList { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LicenseType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public LicenseTypeEnum[] ExcludeLicenseTypeList { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("ApplicationStatus", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ApplicationStatusEnum[] IncludeApplicationStatusList { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("ApplicationStatus", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ApplicationStatusEnum[] ExcludeApplicationStatusList { get; set; }
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("DateTimeRange", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public DateTimeRangeType[] DateTimeRangeList { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ProviderParamsType ProviderParamList { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseExpirationType")]
    public partial class LicenseExpirationType
    {

        [System.Xml.Serialization.XmlElementAttribute("DaysToExpiration", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NumericRangeType[] DaysToExpiration { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("DaysAfterExpiration", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NumericRangeType[] DaysAfterExpiration { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("DaysAfterActivation", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NumericRangeType[] DaysAfterActivation { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "NumericRangeType")]
    public partial class NumericRangeType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string From { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string To { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseSubscriptionType")]
    public partial class LicenseSubscriptionType
    {

        [System.Xml.Serialization.XmlElementAttribute("DaysToExpiration", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NumericRangeType[] DaysToExpiration { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("DaysAfterExpiration", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NumericRangeType[] DaysAfterExpiration { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum LicenseStatusEnum
    {

        /// <remarks/>
        Blocked,

        /// <remarks/>
        Limited,

        /// <remarks/>
        Valid,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum LicenseTypeEnum
    {

        /// <remarks/>
        None,

        /// <remarks/>
        Commercial,

        /// <remarks/>
        Beta,

        /// <remarks/>
        Trial,

        /// <remarks/>
        Test,

        /// <remarks/>
        Oem,

        /// <remarks/>
        Subscription,

        /// <remarks/>
        SubscriptionLimit,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    public enum ApplicationStatusEnum
    {

        /// <remarks/>
        LicenseBlocked,

        /// <remarks/>
        LicenseInvalid,

        /// <remarks/>
        LicenseLimited,

        /// <remarks/>
        TrialExpired,

        /// <remarks/>
        LicenseExpired,

        /// <remarks/>
        SubscriptionSuspended,

        /// <remarks/>
        NoLicense,

        /// <remarks/>
        SubscriptionInGracePeriod,

        /// <remarks/>
        ProductNotProtected,

        /// <remarks/>
        BasesOutOfDate,

        /// <remarks/>
        ThreatsSuspiciousUntreated,

        /// <remarks/>
        ThreatsRiskwareUntreated,

        /// <remarks/>
        LicenseIsTrial,

        /// <remarks/>
        ProductNotAutoRun,

        /// <remarks/>
        UpdateNotAuto,

        /// <remarks/>
        ProtectionSafeMode,

        /// <remarks/>
        ProductFunctionalityDisabled,
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "DateTimeRangeType")]
    public partial class DateTimeRangeType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime From { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime To { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ProviderParamsType")]
    public partial class ProviderParamsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ProviderParamType ProviderParam { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ProviderParamType")]
    public partial class ProviderParamType
    {

        [System.Xml.Serialization.XmlElementAttribute("StringParam", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ProviderStringParamType[] StringParam { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("IntegerParam", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ProviderIntegerParamType[] IntegerParam { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("DateParam", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ProviderDateParamType[] DateParam { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ProviderStringParamType")]
    public partial class ProviderStringParamType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ParamId { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Value { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ProviderIntegerParamType")]
    public partial class ProviderIntegerParamType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ParamId { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public NumericRangeType Value { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ProviderDateParamType")]
    public partial class ProviderDateParamType
    {

        [System.Xml.Serialization.XmlElementAttribute("CurrentDateValue", typeof(bool), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("ExactDateValue", typeof(System.DateTime), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("ExactDateValueRange", typeof(DateTimeRangeType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("ParamId", typeof(string), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "integer")]
        [System.Xml.Serialization.XmlElementAttribute("ParamIdRange", typeof(NumericRangeType), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object[] Items { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "NewsType")]
    public partial class NewsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContentUri { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public SeverityEnum Severity { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Announcement { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ScheduleType Schedule { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime PublicationDate { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutoOpenInApplication { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IgnoreDisableShowSetting { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "RegretPageType")]
    public partial class RegretPageType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DisableRegretPage { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContentUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "WelcomePageType")]
    public partial class WelcomePageType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DisableWelcomePage { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContentUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "EndOfLicenseType")]
    public partial class EndOfLicenseType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DisableEndOfLicense { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "anyURI")]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ContentUri { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LicenseNotificationSettingsType")]
    public partial class LicenseNotificationSettingsType
    {

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ScheduleType Schedule { get; set; }
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EnableIpmMessagesOnly { get; set; }
    }
}