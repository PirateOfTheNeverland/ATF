using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IPMATFramework
{
    public class ContentDefinitions
    {
        public class ContentGroup
        {
            public ContentGroup() { }

            public string Name { get; set; }
            public string Comment { get; set; }
            public string ExpDate { get; set; }
            public string ExpPeriod { get; set; }
            public string PublicationDate { get; set; }
            public string[] ContentTypes { get; set; }
            public object[] Content { get; set; }

            public static ContentGroup CreateDefaultData()
            {
                ContentGroup campaign = new ContentGroup
                {
                    Name = DateTime.Now.ToString(),
                    Comment = "ContentGroupComment",
                    ExpDate = "",
                    ExpPeriod = "123",
                    PublicationDate = ""
                };
                return campaign;
            }
        }

        public class News
        {
            public News() { }

            public string Name { get; set; }
            public string Title { get; set; }
            public string Announcement { get; set; }
            public string Severity { get; set; }
            public int AutoOpen { get; set; }
            public int IgnoreDisable { get; set; }
            public string[] Files { get; set; }
            public string ScheduleType { get; set; }
            public Schedule[] ScheduleSettings { get; set; }

            public static News CreateDefaultData()
            {
                News content = new News
                {
                    Name = "NEWS",
                    Title = "NewsTitle",
                    Announcement = "NewsAnnouncement",
                    Severity = "Alarm",
                    AutoOpen = 1,
                    IgnoreDisable = 1,
                    ScheduleSettings = null
                };
                return content;
            }
            public static News CreateCustomizedData(TestContext tc)
            {
                News news = new News
                {
                    Name = tc.DataRow["Name"].ToString(),
                    Title = tc.DataRow["Title"].ToString(),
                    Announcement = tc.DataRow["Announcement"].ToString(),
                    Severity = tc.DataRow["Severity"].ToString(),
                    AutoOpen = Convert.ToInt32(tc.DataRow["AutoOpen"].ToString()),
                    IgnoreDisable = Convert.ToInt32(tc.DataRow["IgnoreDisable"].ToString()),
                    Files = new string[]
                    {
                        tc.DataRow["File1"].ToString(), 
                        tc.DataRow["File2"].ToString(), 
                        tc.DataRow["File3"].ToString(), 
                        tc.DataRow["File4"].ToString()
                    }
                };

                return news;
            }
        }

        public class WelcomePage
        {
            public WelcomePage() { }

            public string Name { get; set; }
            public int DisableWelcomePage { get; set; }
            public string[] Files { get; set; }

            public static WelcomePage CreateDefaultData()
            {
                WelcomePage wp = new WelcomePage
                {
                    Name = "WELCOME_PAGE",
                    DisableWelcomePage = 0
                };
                return wp;
            }
        }

        public class EndOfLicense
        {
            public EndOfLicense() { }
            public EndOfLicense(string[] files)
            {
                if (files != null && files.Length > 0)
                {
                    Name = "END_OF_LICENSE_PAGE";
                    DisableEOLP = 0;
                    Files = new string[files.Length];
                    int i = 0;
                    foreach (string fileName in files) Files[i++] = fileName;
                }
            }

            public string Name { get; set; }
            public int DisableEOLP { get; set; }
            public string[] Files { get; set; }

            public static EndOfLicense CreateDefaultData()
            {
                EndOfLicense eolp = new EndOfLicense
                {
                    Name = "END_OF_LICENSE_PAGE",
                    DisableEOLP = 0
                };
                return eolp;
            }
        }

        public class RegretPage
        {
            public RegretPage() { }

            public string Name { get; set; }
            public int DisableRegretPage { get; set; }
            public string RegretPageURL { get; set; }

            public static RegretPage CreateDefaultData()
            {
                RegretPage rp = new RegretPage
                {
                    Name = "REGRET PAGE",
                    DisableRegretPage = 0,
                    RegretPageURL = "http://regret.page.url.com"
                };
                return rp;
            }
        }

        public class AppButtons
        {
            public AppButtons() { }

            public string Name { get; set; }
            public SingleButton[] Collection { get; set; }

            public static AppButtons CreateDefaultData()
            {
                AppButtons appBut = new AppButtons
                {
                    Name = "APPLICATION BUTTONS",
                    Collection = new SingleButton[]
                    {
                        new SingleButton { ButtonType = "Buy", ButtonName = "First button", AppAction = "Custom URL", CustomURL = "http://custom.url.com"},
                        new SingleButton { ButtonType = "Renew", ButtonName = "Second button", AppAction = "Product Store URL", CustomURL = "http://product.store.url.com"},
                        new SingleButton { ButtonType = "Upgrade", ButtonName = "Third button", AppAction = "Custom URL", CustomURL = "http://another.custom.url.com"},
                        new SingleButton { ButtonType = "Manage License", ButtonName = "Fourth button", AppAction = "", CustomURL = ""},
                        new SingleButton { ButtonType = "Manage Subscription", ButtonName = "Fifth button", AppAction = "", CustomURL = ""},
                        new SingleButton { ButtonType = "Subscription Provider", ButtonName = "Sixth button", AppAction = "", CustomURL = ""}
                    }
                };
                return appBut;
            }

            public class SingleButton
            {
                public SingleButton() { }

                public string ButtonType { get; set; }
                public string ButtonName { get; set; }
                public string AppAction { get; set; }
                public string CustomURL { get; set; }
            }

            public static string ConvertAppActionUIToBE(string uiAppAction)
            {
                if (AppActionMappingUIToBE.ContainsKey(uiAppAction)) return AppActionMappingUIToBE[uiAppAction];
                else return "";
            }

            public static string ConvertButtonTypeUIToBE(string uiButtonType)
            {
                if (ButtonTypeMappingUIToBE.ContainsKey(uiButtonType)) return ButtonTypeMappingUIToBE[uiButtonType];
                else return "";
            }

            private static Dictionary<string, string> AppActionMappingUIToBE = new Dictionary<string, string>()
            {
                {"Custom URL", "CustomUri"}, {"Product Store URL", "ProductStoreUri"}
            };

            private static Dictionary<string, string> ButtonTypeMappingUIToBE = new Dictionary<string, string>()
            {
                {"Buy", "Buy"}, {"Renew", "Renew"}, {"Upgrade", "Upgrade"},
                {"Manage License", "ManageLicense"}, {"Manage Subscription", "ManageSubscription"}, {"Subscription Provider", "SubscriptionProvider"}
            };
        }

        public class LicenseNotification
        {
            public LicenseNotification() { }

            public string Name { get; set; }
            public MainPagePane MainPage { get; set; }
            public BalloonPane Balloon { get; set; }
            public LicenseManagerPane LicenseManager { get; set; }
            public MessageBoxPane MessageBox { get; set; }
            public ProblemListPane ProblemList { get; set; }
            public UIMap.ScheduleCreationType ScheduleCreationType { get; set; }
            public string ScheduleType { get; set;}
            public Schedule[] ScheduleSettings { get; set; }

            public static LicenseNotification CreateDefaultData()
            {
                LicenseNotification ln = new LicenseNotification
                {
                    Name = "LICENSE_NOTIFICATION",
                    MainPage = new MainPagePane { Severity = "Alarm", ProtectionState = "Protection_state_is_missing", LicenseState = "License_state_is_missing", ShowAppButton = 1, ShowToast = 1 },
                    Balloon = new BalloonPane { Content = "This_is_the_content_of_the_balloon" },
                    LicenseManager = new LicenseManagerPane
                    {
                        Title = "LicenseManagerPaneTitle",
                        Description = "LicenseManagerDescription",
                        BackgroundSettings = new LicenseManagerPane.Background { ImagePath = @"C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\iso_bcg.jpeg", BottomMargin = "2", LeftMargin = "2", RightMargin = "2", TopMargin = "2" },
                        Buttons = new LicenseManagerPane.Button[] 
                        {
                            new LicenseManagerPane.Button { Name = "FirstButton", DisplayShoppingCart = 1, AppAction = "Open URL in Browser", URL = "http://blablabla.com"},
                            new LicenseManagerPane.Button { Name = "SecondButton", DisplayShoppingCart = 0, AppAction = "Buy", URL = ""}
                        }
                    },
                    MessageBox = new MessageBoxPane 
                    { 
                        WindowHeight = "200", 
                        WindowWidth = "200", 
                        Files = new string[] 
                        {
                            @"C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\iso_bcg.jpg",
                            @"C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\iso_logo.png",
                            @"C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\ln.html",
                            @"C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\second.html"
                        } 
                    },
                    ProblemList = new ProblemListPane { Title = "ProblemListTitle", Description = "ProblemListDescription" },
                    ScheduleType = "",
                    //ScheduleSettings = new Schedule {  }
                };
                return ln;
            }

            public class MainPagePane
            {
                public MainPagePane() { }
                public string Severity { get; set; }
                public string ProtectionState { get; set; }
                public string LicenseState { get; set; }
                public int ShowAppButton { get; set; }
                public int ShowToast { get; set; }
            }

            public class BalloonPane
            {
                public BalloonPane() { }
                public string Content { get; set; }
            }

            public class LicenseManagerPane
            {
                public LicenseManagerPane() { }
                public string Title { get; set; }
                public string Description { get; set; }
                public Button[] Buttons { get; set; }
                public Background BackgroundSettings { get; set; }

                public class Button
                {
                    public Button() { }
                    public string Name { get; set; }
                    public int DisplayShoppingCart { get; set; }
                    public string AppAction { get; set; }
                    public string URL { get; set; }
                }
                public class Background
                {
                    public Background() { }
                    public string ImagePath { get; set; }
                    public string LeftMargin { get; set; }
                    public string TopMargin { get; set; }
                    public string RightMargin { get; set; }
                    public string BottomMargin { get; set; }
                }
            }

            public class MessageBoxPane
            {
                public MessageBoxPane() { }
                public string[] Files { get; set; }
                public string WindowHeight { get; set; }
                public string WindowWidth { get; set; }
            }

            public class ProblemListPane
            {
                public ProblemListPane() { }
                public string Title { get; set; }
                public string Description { get; set; }
            }
        }

        public class LicenseNotificationSettings
        {
            public LicenseNotificationSettings() { }

            public string Name { get; set;}
            public int DisableNotIPMMessages { get; set;}
            public string ScheduleType { get; set; }
            public Schedule[] ScheduleSettings { get; set;}

            public static LicenseNotificationSettings CreateDefaultData()
            {
                LicenseNotificationSettings lns = new LicenseNotificationSettings
                {
                    Name = "LICENSE_NOTIFICATION_SETTINGS",
                    DisableNotIPMMessages = 1
                };
                return lns;
            }
        }

        public class UCP
        {
            public UCP() { }

            public string Name { get; set; }
            public string UCPConnectionType { get; set; } // Disable/Registration on Demand/Registration Always
            public string UCPRegistrationURL { get; set; }
            

            public static UCP CreateDefaultData()
            {
                UCP ucp = new UCP
                {
                    Name = "UCP_SETTINGS",
                    UCPConnectionType = "Registration Always",
                    UCPRegistrationURL = "http://ucp.registration.url.com"
                };
                return ucp;
            }

            public static string ConvertConnectionTypeUIToBE(string uiConnectionType)
            {
                if (ConnectionTypeMappingUIToBE.ContainsKey(uiConnectionType)) return ConnectionTypeMappingUIToBE[uiConnectionType];
                else return "";
            }

            private static Dictionary<string, string> ConnectionTypeMappingUIToBE = new Dictionary<string, string>()
            {
                {"Disable", "Disable"}, {"Registration on Demand", "RegistrationOnDemand"}, {"Registration Always", "RegistrationAlways"}
            };
        }

        public class GoogleAnaliticsSettings
        {
            public GoogleAnaliticsSettings() { }

            public int SendApplicationStatistics { get; set; }

            public static GoogleAnaliticsSettings CreateDefaultData()
            {
                GoogleAnaliticsSettings gas = new GoogleAnaliticsSettings()
                {
                    SendApplicationStatistics = 1
                };
                return gas;
            }

        }

        public class GooglePlaySettings
        {
            public GooglePlaySettings() { }

            public string BuyGPProductId { get; set; }
            public string RenewGPProductId { get; set; }

            public static GooglePlaySettings CreateDefaultData()
            {
                GooglePlaySettings gps = new GooglePlaySettings()
                {
                    BuyGPProductId = "kl9999xxix",
                    RenewGPProductId = "kl000aaa666"
                };
                return gps;
            }
        }

        public class WebSurveySettings
        {
            public WebSurveySettings() { }

            public int PerformAfterPurchaseSurvey { get; set; }
            public string SurveyURL { get; set; }

            public static WebSurveySettings CreateDefaultData()
            {
                WebSurveySettings wss = new WebSurveySettings
                {
                    PerformAfterPurchaseSurvey = 1,
                    SurveyURL = "http://survey.com"
                };
                return wss;
            }
        }

        public class SocializationSettings
        {
            public SocializationSettings() { }

            public int EnableSocialization { get; set; }
            public string SocializationId { get; set; }

            public static SocializationSettings CreateDefaultData()
            {
                SocializationSettings ss = new SocializationSettings()
                {
                    EnableSocialization = 1,
                    SocializationId = "6F9619FF-8B86-D011-B42D-00CF4FC964FF"
                };
                return ss;
            }
        }

        public class BrowserRedirectionSettings
        {
            public BrowserRedirectionSettings() { }

            public string RedirectURL { get; set; }
            public string[] BrowserURL { get; set; }

            public static BrowserRedirectionSettings CreateDefaultData()
            {
                BrowserRedirectionSettings brs = new BrowserRedirectionSettings()
                {
                    RedirectURL = "http://redirect.kaspersky.com/sales?DEFAULT_URL=<TargetUrl>",
                    BrowserURL = new string[] { "*google*", "http://ya????.ru" }
                };
                return brs;
            }

            public bool AreEqual(string[] urlList)
            {
                if (this.BrowserURL.Length != urlList.Length) return false;
                for (int i = 0; i < urlList.Length; i++)
                {
                    if (this.BrowserURL[i] != urlList[i]) return false;
                }

                return true;
            }
        }

        public class NewVersionInfo
        {
            public NewVersionInfo() { }

            public string DistributionAppId { get; set; }
            public string DistributionAppVersion { get; set; }
            public string DistributionDownloadURL { get; set; }
            public string AboutNewProductDownloadURL { get; set; }
            public string DistributionType { get; set; } // Setup/Webpack
            public string MessageTitle { get; set; }
            public string MessagePeriodInDays { get; set; }
            public string MessageDelayInDays { get; set; }
            public string[] Content { get; set; }

            public static NewVersionInfo CreateDefaultData()
            {
                NewVersionInfo nvi = new NewVersionInfo()
                {
                    DistributionAppId = "1437",
                    DistributionAppVersion = "1.0.1.2345",
                    DistributionDownloadURL = "http://distribution.download.url.com",
                    AboutNewProductDownloadURL = "http://about.new.version.url.com",
                    DistributionType = "Webpack",
                    MessageTitle = "MessageTitle",
                    MessagePeriodInDays = "100",
                    MessageDelayInDays = "2"
                };
                return nvi;
            }
        }
        
        public class Schedule
        {
            public Schedule() { }

            public string Name { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
            public Range DaysRange { get; set; }

            public class Range
            {
                public Range() { }

                public string From;
                public string To;
            }
        }
    }
}
