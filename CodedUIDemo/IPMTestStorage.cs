using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using IPMATFramework.TestSettings;


namespace IPMATFramework
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class IPMTestStorage
    {
        public IPMTestStorage()
        {
        }

        #region SCHEDULE

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72555.csv", "T72555#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T7255.csv")]
        [TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72555_LDTE()
        {
            #region Test data preparation

            Assert.IsTrue(UIMap.IsTestContentDeployed(TestSettings.TestSettings.GetScheduleContentFiles()));

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "LDTE";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "License Days to Expiration", DaysRange = new ContentDefinitions.Schedule.Range() { From = "10", To = "25" }}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    LicenseExpiration = new ContentCustomization.LicenseExpirationType
                    {
                         DaysToExpiration = new ContentCustomization.NumericRangeType[] { new ContentCustomization.NumericRangeType { From = "10", To = "25" } }
                    }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion

            Assert.IsTrue((newsObtained.Schedule.LicenseExpiration.DaysToExpiration.Length == 1), "Unexpected number of date ranges");

            bool condition = (newsRef.Schedule.LicenseExpiration.DaysToExpiration[0].From == newsObtained.Schedule.LicenseExpiration.DaysToExpiration[0].From) &&
                (newsRef.Schedule.LicenseExpiration.DaysToExpiration[0].To == newsObtained.Schedule.LicenseExpiration.DaysToExpiration[0].To);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");            
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72558.csv", "T72558#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72558.csv"), TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72558_SDTE()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "SDTE";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "License Days to Subscription Expiration", DaysRange = new ContentDefinitions.Schedule.Range() { From = "10", To = "25" }}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    LicenseSubscription = new ContentCustomization.LicenseSubscriptionType
                    {
                         DaysToExpiration = new ContentCustomization.NumericRangeType[] { new ContentCustomization.NumericRangeType { From = "10", To = "25" } }
                    }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion
            
            Assert.IsTrue((newsObtained.Schedule.LicenseSubscription.DaysToExpiration.Length == 1), "Unexpected number of date ranges");

            bool condition = (newsRef.Schedule.LicenseSubscription.DaysToExpiration[0].From == newsObtained.Schedule.LicenseSubscription.DaysToExpiration[0].From) &&
                (newsRef.Schedule.LicenseSubscription.DaysToExpiration[0].To == newsObtained.Schedule.LicenseSubscription.DaysToExpiration[0].To);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");            
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72560.csv", "T72560#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72560.csv"), TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72560_LDAE()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "LDAE";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "License Days after Expiration", DaysRange = new ContentDefinitions.Schedule.Range() { From = "10", To = "25" }}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    LicenseExpiration = new ContentCustomization.LicenseExpirationType
                    {
                        DaysAfterExpiration = new ContentCustomization.NumericRangeType[] { new ContentCustomization.NumericRangeType { From = "10", To = "25" } }
                    }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion

            Assert.IsTrue((newsObtained.Schedule.LicenseExpiration.DaysAfterExpiration.Length == 1), "Unexpected number of date ranges");

            bool condition = (newsRef.Schedule.LicenseExpiration.DaysAfterExpiration[0].From == newsObtained.Schedule.LicenseExpiration.DaysAfterExpiration[0].From) &&
                (newsRef.Schedule.LicenseExpiration.DaysAfterExpiration[0].To == newsObtained.Schedule.LicenseExpiration.DaysAfterExpiration[0].To);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72562.csv", "T72562#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72562.csv"), TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72562_SDAE()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "SDAE";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "License Days after Subscription Expiration", DaysRange = new ContentDefinitions.Schedule.Range() { From = "10", To = "25" }}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    LicenseSubscription  = new ContentCustomization.LicenseSubscriptionType
                    {
                        DaysAfterExpiration = new ContentCustomization.NumericRangeType[] { new ContentCustomization.NumericRangeType { From = "10", To = "25" } }
                    }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion

            Assert.IsTrue((newsObtained.Schedule.LicenseSubscription.DaysAfterExpiration.Length == 1), "Unexpected number of date ranges");

            bool condition = (newsRef.Schedule.LicenseSubscription.DaysAfterExpiration[0].From == newsObtained.Schedule.LicenseSubscription.DaysAfterExpiration[0].From) &&
                (newsRef.Schedule.LicenseSubscription.DaysAfterExpiration[0].To == newsObtained.Schedule.LicenseSubscription.DaysAfterExpiration[0].To);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", 
            "|DataDirectory|\\TestData\\T72595.csv", "T72595#csv", 
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72595.csv"), TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72595_72606_72608_Include()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "Include";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "Application Status", Operator = "Include", Value = "NoLicense"},
                new ContentDefinitions.Schedule() { Name = "Application Status", Operator = "Include", Value = "TrialExpired"},
                new ContentDefinitions.Schedule() { Name = "Application Status", Operator = "Include", Value = "LicenseLimited"},
                new ContentDefinitions.Schedule() { Name = "License Legal Status", Operator = "Include", Value = "Valid"},
                new ContentDefinitions.Schedule() { Name = "License Type", Operator = "Include", Value = "Beta"}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    IncludeApplicationStatusList = new ContentCustomization.ApplicationStatusEnum[]
                      {
                          ContentCustomization.ApplicationStatusEnum.NoLicense, 
                          ContentCustomization.ApplicationStatusEnum.TrialExpired,
                          ContentCustomization.ApplicationStatusEnum.LicenseLimited
                      },
                    IncludeLicenseStatusList = new ContentCustomization.LicenseStatusEnum[] { ContentCustomization.LicenseStatusEnum.Valid },
                    IncludeLicenseTypeList = new ContentCustomization.LicenseTypeEnum[] { ContentCustomization.LicenseTypeEnum.Beta }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion

            Assert.IsTrue((newsObtained.Schedule.IncludeApplicationStatusList.Length == 3) && (newsObtained.Schedule.IncludeLicenseStatusList.Length == 1) &&
                (newsObtained.Schedule.IncludeLicenseTypeList.Length == 1), "Error! Not all schedule entries are presented in meta.xml!");

            bool condition = (newsRef.Schedule.IncludeApplicationStatusList[0] == newsObtained.Schedule.IncludeApplicationStatusList[0]) &&
                (newsRef.Schedule.IncludeApplicationStatusList[1] == newsObtained.Schedule.IncludeApplicationStatusList[1]) &&
                (newsRef.Schedule.IncludeApplicationStatusList[2] == newsObtained.Schedule.IncludeApplicationStatusList[2]) &&
                (newsRef.Schedule.IncludeLicenseStatusList[0] == newsObtained.Schedule.IncludeLicenseStatusList[0]) &&
                (newsRef.Schedule.IncludeLicenseTypeList[0] == newsObtained.Schedule.IncludeLicenseTypeList[0]);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72596.csv", "T72596#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72596.csv"), TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72596_72607_72610_Exclude()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "Exclude";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "Application Status", Operator = "Exclude", Value = "NoLicense"},
                new ContentDefinitions.Schedule() { Name = "Application Status", Operator = "Exclude", Value = "TrialExpired"},
                new ContentDefinitions.Schedule() { Name = "Application Status", Operator = "Exclude", Value = "LicenseLimited"},
                new ContentDefinitions.Schedule() { Name = "License Legal Status", Operator = "Exclude", Value = "Valid"},
                new ContentDefinitions.Schedule() { Name = "License Type", Operator = "Exclude", Value = "Beta"}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    ExcludeApplicationStatusList = new ContentCustomization.ApplicationStatusEnum[]
                    {
                          ContentCustomization.ApplicationStatusEnum.NoLicense, 
                          ContentCustomization.ApplicationStatusEnum.TrialExpired,
                          ContentCustomization.ApplicationStatusEnum.LicenseLimited
                    },
                    ExcludeLicenseStatusList = new ContentCustomization.LicenseStatusEnum[] { ContentCustomization.LicenseStatusEnum.Valid },
                    ExcludeLicenseTypeList = new ContentCustomization.LicenseTypeEnum[] { ContentCustomization.LicenseTypeEnum.Beta }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion

            Assert.IsTrue((newsObtained.Schedule.ExcludeApplicationStatusList.Length == 3) && (newsObtained.Schedule.ExcludeLicenseStatusList.Length == 1) &&
                (newsObtained.Schedule.ExcludeLicenseTypeList.Length == 1), "Error! Not all schedule entries are presented in meta.xml!");

            bool condition = (newsRef.Schedule.ExcludeApplicationStatusList[0] == newsObtained.Schedule.ExcludeApplicationStatusList[0]) &&
                (newsRef.Schedule.ExcludeApplicationStatusList[1] == newsObtained.Schedule.ExcludeApplicationStatusList[1]) &&
                (newsRef.Schedule.ExcludeApplicationStatusList[2] == newsObtained.Schedule.ExcludeApplicationStatusList[2]) &&
                (newsRef.Schedule.ExcludeLicenseStatusList[0] == newsObtained.Schedule.ExcludeLicenseStatusList[0]) &&
                (newsRef.Schedule.ExcludeLicenseTypeList[0] == newsObtained.Schedule.ExcludeLicenseTypeList[0]);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72601.csv", "T72601#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72601.csv"), TestMethod]
        [TestProperty("Area", "Schedule")]
        public void T72601_DD()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = new ContentDefinitions.News
            {
                Name = TestContext.DataRow["NewsName"].ToString(),
                Title = TestContext.DataRow["NewsTitle"].ToString(),
                Announcement = TestContext.DataRow["Announcement"].ToString(),
                Severity = TestContext.DataRow["Severity"].ToString(),
                AutoOpen = Convert.ToInt32(TestContext.DataRow["AutoOpen"].ToString()),
                IgnoreDisable = Convert.ToInt32(TestContext.DataRow["IgnoreDisable"].ToString()),
                Files = new string[TestSettings.TestSettings.GetScheduleContentFilesLength()]
            };
            news.Files = TestSettings.TestSettings.GetScheduleContentFiles();
            news.ScheduleType = "DisplayDates";
            news.ScheduleSettings = new ContentDefinitions.Schedule[]
            {
                new ContentDefinitions.Schedule() { Name = "Display Dates", DaysRange = new ContentDefinitions.Schedule.Range() { From = "5/1/2014", To = "6/1/2014" }},
                new ContentDefinitions.Schedule() { Name = "Display Dates", DaysRange = new ContentDefinitions.Schedule.Range() { From = "7/1/2014", To = "8/1/2014" }}
            };

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            UIMap.Campaign_ApproveAndPublish();
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.PubContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentCustomization.IpmContentCustomization icc = new ContentCustomization.IpmContentCustomization();
            icc.Item = new ContentCustomization.NewsType()
            {
                Schedule = new ContentCustomization.ScheduleType
                {
                    DateTimeRangeList = new ContentCustomization.DateTimeRangeType[]
                    {
                        new ContentCustomization.DateTimeRangeType() { From = new DateTime(2014, 5, 1), To = new DateTime(2014, 6, 1)},
                        new ContentCustomization.DateTimeRangeType() { From = new DateTime(2014, 7, 1), To = new DateTime(2014, 8, 1)}
                    }
                }
            };
            ContentCustomization.NewsType newsRef = (ContentCustomization.NewsType)icc.Item;
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.NewsType newsObtained = (ContentCustomization.NewsType)iccObtained.Item;

            #endregion

            Assert.IsTrue((newsObtained.Schedule.DateTimeRangeList.Length == 2), "Error! Not all schedule entries are presented in meta.xml!");

            bool condition = (newsRef.Schedule.DateTimeRangeList[0].From == newsObtained.Schedule.DateTimeRangeList[0].From) &&
                (newsRef.Schedule.DateTimeRangeList[0].To == newsObtained.Schedule.DateTimeRangeList[0].To) &&
                (newsRef.Schedule.DateTimeRangeList[1].From == newsObtained.Schedule.DateTimeRangeList[1].From) &&
                (newsRef.Schedule.DateTimeRangeList[1].To == newsObtained.Schedule.DateTimeRangeList[1].To);

            Assert.IsTrue(condition, "Error! Not all schedule values are equal");
        }

        #endregion

        #region CAMPAIGN CONTENT
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72659.csv", "T72659#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72659.csv"), TestMethod]
        [TestProperty("Area", "Content")]
        public void T72659_WP()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.WelcomePage wp = new ContentDefinitions.WelcomePage
            {
                Name = TestContext.DataRow["WPName"].ToString(),
                DisableWelcomePage = Convert.ToInt32(TestContext.DataRow["DisableWelcomePage"].ToString()),
                Files = new string[] 
                { 
                    TestContext.DataRow["File1"].ToString(), 
                    TestContext.DataRow["File2"].ToString(),
                    TestContext.DataRow["File3"].ToString(),
                    TestContext.DataRow["File4"].ToString()
                }
            };

            campaign.ContentTypes = new string[] { "WP" };
            campaign.Content = new object[] { wp };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            //UIMap.Campaign_ApproveAndPublish();
            //Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);
                        
            ContentDefinitions.WelcomePage wpExpected = (ContentDefinitions.WelcomePage)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath +  @"\meta.xml");
            ContentCustomization.WelcomePageType wpObtained = (ContentCustomization.WelcomePageType)iccObtained.Item;

            #endregion

            bool condition = (wpObtained.DisableWelcomePage.ToString() == UIMap.ConvertIntToBool(wpExpected.DisableWelcomePage));
            string errMsg = "Error! Not all values are equal! Expected: DisableWelcomePage: \"" + UIMap.ConvertIntToBool(wpExpected.DisableWelcomePage) + 
                "\"\nResult: DisableWelcomePage: \"" + wpObtained.DisableWelcomePage.ToString() + "\"";
            Assert.IsTrue(condition, errMsg);

            DirectoryInfo testDir = new DirectoryInfo(unzippedFilesPath);
            Assert.IsTrue(UIMap.CompareFilesInDirectories(testDir, TestSettings.TestSettings.ReferenceContentStoragePath + @"WP\"), "Reference and result files do NOT match!");
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72660.csv", "T72660#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72660.csv"), TestMethod]
        [TestProperty("Area", "Content")]
        public void T72660_EOLP()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.EndOfLicense eolp = new ContentDefinitions.EndOfLicense(new string[] 
            {
                TestContext.DataRow["File1"].ToString(), 
                TestContext.DataRow["File2"].ToString(), 
                TestContext.DataRow["File3"].ToString(),
                TestContext.DataRow["File4"].ToString()
            });

            campaign.ContentTypes = new string[] { "EOLP" };
            campaign.Content = new object[] { eolp };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            //UIMap.Campaign_ApproveAndPublish();
            //Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.EndOfLicense eolpExpected = (ContentDefinitions.EndOfLicense)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.EndOfLicenseType eolpObtained = (ContentCustomization.EndOfLicenseType)iccObtained.Item;

            #endregion

            bool condition = (UIMap.ConvertIntToBool(eolpExpected.DisableEOLP) == eolpObtained.DisableEndOfLicense.ToString());
            string errMsg = "Error! Not all schedule values are equal! Expected: DisableEOLP: \"" + UIMap.ConvertIntToBool(eolpExpected.DisableEOLP) +
                "\"\nExpected: DisableEOLP: \"" + eolpObtained.DisableEndOfLicense.ToString() + "\"";
            Assert.IsTrue(condition, errMsg);

            DirectoryInfo testDir = new DirectoryInfo(unzippedFilesPath);
            Assert.IsTrue(UIMap.CompareFilesInDirectories(testDir, TestSettings.TestSettings.ReferenceContentStoragePath + @"EOLP\"));
        }            

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", 
            "|DataDirectory|\\TestData\\T72661.csv", 
            "T72661#csv", DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72661.csv"), TestMethod]
        [TestProperty("Area", "Content")]
        public void T72661_RP()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.RegretPage rp = new ContentDefinitions.RegretPage
            {
                Name = TestContext.DataRow["Name"].ToString(),
                DisableRegretPage = Convert.ToInt32(TestContext.DataRow["DisableRegretPage"].ToString()),
                RegretPageURL = TestContext.DataRow["RegretPageURL"].ToString()
            };

            campaign.ContentTypes = new string[] { "RP" };
            campaign.Content = new object[] { rp };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.RegretPage rpExpected = (ContentDefinitions.RegretPage)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.RegretPageType rpObtained = (ContentCustomization.RegretPageType)iccObtained.Item;

            #endregion

            bool condition;
            if (rpExpected.DisableRegretPage == 1)
                condition = (rpObtained.DisableRegretPage.ToString() == UIMap.ConvertIntToBool(rpExpected.DisableRegretPage));
            else
                condition = (rpObtained.DisableRegretPage.ToString() == UIMap.ConvertIntToBool(rpExpected.DisableRegretPage)) &&
                    (rpObtained.ContentUri == rpExpected.RegretPageURL);
            string errMsg = "Error! Not all values are equal! Expected: DisableRegretPage: \"" + UIMap.ConvertIntToBool(rpExpected.DisableRegretPage) +
                "\"\tRegretPageURL: \"" + rpExpected.RegretPageURL + "\"\nResult: DisableRegretPage: \"" + rpObtained.DisableRegretPage.ToString() +
                "\"\tRegretPageURL: \"" + rpObtained.ContentUri + "\"";

            Assert.IsTrue(condition, errMsg);
        }

        [TestMethod]
        [TestProperty("Area", "Content")]
        public void T72664_AB()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.AppButtons ab = ContentDefinitions.AppButtons.CreateDefaultData();

            campaign.ContentTypes = new string[] { "AB" };
            campaign.Content = new object[] { ab };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.AppButtons abExpected = (ContentDefinitions.AppButtons)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.ApplicationButtonsTypeList abObtained = (ContentCustomization.ApplicationButtonsTypeList)iccObtained.Item;

            #endregion

            int i = 0;
            bool condition;
            foreach (ContentCustomization.ApplicationButtonType btnObtained in abObtained.Button)
            {
                if (abExpected.Collection[i].AppAction == "")
                    condition = (btnObtained.ButtonName == abExpected.Collection[i].ButtonName) &&
                    (btnObtained.ButtonType.ToString() == ContentDefinitions.AppButtons.ConvertButtonTypeUIToBE(abExpected.Collection[i].ButtonType));
                else condition = (btnObtained.ButtonName == abExpected.Collection[i].ButtonName) &&
                    (btnObtained.ButtonType.ToString() == ContentDefinitions.AppButtons.ConvertButtonTypeUIToBE(abExpected.Collection[i].ButtonType)) &&
                    (btnObtained.ItemElementName.ToString() == ContentDefinitions.AppButtons.ConvertAppActionUIToBE(abExpected.Collection[i].AppAction)) &&
                    (btnObtained.Item == abExpected.Collection[i].CustomURL);
                string errMsg = "Error! Not all values are equal! Expected: ButtonName: \"" + abExpected.Collection[i].ButtonName + "\"\tButtonType: \"" +
                    ContentDefinitions.AppButtons.ConvertButtonTypeUIToBE(abExpected.Collection[i].ButtonType) + "\"\tAppAction: \"" + 
                    ContentDefinitions.AppButtons.ConvertAppActionUIToBE(abExpected.Collection[i].AppAction) + "\"\tURL: \"" + abExpected.Collection[i].CustomURL + "\"\n" + 
                    "Result: ButtonName: \"" + btnObtained.ButtonName + "\"\tButtonType: \"" + btnObtained.ButtonType.ToString() +
                    "\"\tAppAction: \"" + btnObtained.ItemElementName.ToString() + "\"\tURL: \"" + btnObtained.Item + "\"";
                Assert.IsTrue(condition, errMsg);
                i++;
            } 
        }

        [TestMethod]
        [TestProperty("Area", "Content")]
        public void T72665_LN()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.LicenseNotification ln = ContentDefinitions.LicenseNotification.CreateDefaultData();
            ln.ScheduleCreationType = IPMATFramework.UIMap.ScheduleCreationType.Default;

            campaign.ContentTypes = new string[] { "LN" };
            campaign.Content = new object[] { ln };
            
            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);
            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.LicenseNotification lnExpected = (ContentDefinitions.LicenseNotification)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.LicenseNotificationType lnObtained = (ContentCustomization.LicenseNotificationType)iccObtained.Item;

            #endregion

            bool condition = (lnObtained.Severity.ToString() == lnExpected.MainPage.Severity) &&
                (lnObtained.ShowToast.Value.ToString() == UIMap.ConvertIntToBool(lnExpected.MainPage.ShowToast)) &&
                (lnObtained.ApplicationMainPage.LicenseStateName == lnExpected.MainPage.LicenseState) &&
                (lnObtained.ApplicationMainPage.ProtectionStateName == lnExpected.MainPage.ProtectionState) &&
                (lnObtained.ApplicationMainPage.ShowApplicationButton.ToString() == UIMap.ConvertIntToBool(lnExpected.MainPage.ShowAppButton));
            string errMsg = lnObtained.Severity.ToString() + "\t" + lnObtained.ShowToast.Value.ToString() + "\t" + lnObtained.ApplicationMainPage.LicenseStateName +
                "\t" + lnObtained.ApplicationMainPage.ProtectionStateName + "\t" + lnObtained.ApplicationMainPage.ShowApplicationButton.ToString();

            Assert.IsTrue(condition, errMsg);

            DirectoryInfo testDir = new DirectoryInfo(unzippedFilesPath);
            Assert.IsTrue(UIMap.CompareFilesInDirectories(testDir, TestSettings.TestSettings.ReferenceContentStoragePath + @"LN\"), "Reference and result files do NOT match!");
        }

        [TestMethod]
        [TestProperty("Area", "Content")]
        public void T72668_LNS()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.LicenseNotificationSettings lns = ContentDefinitions.LicenseNotificationSettings.CreateDefaultData();

            campaign.ContentTypes = new string[] { "LNS" };
            campaign.Content = new object[] { lns };

            lns.ScheduleType = "";
            lns.ScheduleSettings = null;

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            //UIMap.Campaign_ApproveAndPublish();
            //Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.LicenseNotificationSettings lnsExpected = (ContentDefinitions.LicenseNotificationSettings)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.LicenseNotificationSettingsType lnsObtained = (ContentCustomization.LicenseNotificationSettingsType)iccObtained.Item;

            #endregion

            bool condition = (Convert.ToInt32(lnsObtained.EnableIpmMessagesOnly) == lnsExpected.DisableNotIPMMessages);
            string errMsg = "Error! Not all values are equal! Expected: DisableIPMMessages = \"" + lnsExpected.DisableNotIPMMessages.ToString() + "\"\n";
            errMsg += "Result: DisableIPMMessages = \"" + lnsObtained.EnableIpmMessagesOnly.ToString() + "\"";
            
            Assert.IsTrue(condition, errMsg);
        }

        [TestMethod]
        [TestProperty("Area", "Content")]
        public void T72671_UCP()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.UCP ucp = ContentDefinitions.UCP.CreateDefaultData();
            
            campaign.ContentTypes = new string[] { "UCP" };
            campaign.Content = new object[] { ucp };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            //UIMap.Campaign_ApproveAndPublish();
            //Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.UCP ucpRef = (ContentDefinitions.UCP)campaign.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.UcpReadySettingsType  ucpObtained = (ContentCustomization.UcpReadySettingsType)iccObtained.Item;

            #endregion

            bool condition = (ucpObtained.UcpConnectionType.ToString() == ContentDefinitions.UCP.ConvertConnectionTypeUIToBE(ucpRef.UCPConnectionType)) 
                && (ucpObtained.UcpPortalUri == ucpRef.UCPRegistrationURL);
            string errMsg = "Error! Not all values are equal! Expected: ConnectionType = \"" + ContentDefinitions.UCP.ConvertConnectionTypeUIToBE(ucpRef.UCPConnectionType) 
                + "\", URL = \"" + ucpRef.UCPRegistrationURL + "\"\n";
            errMsg += "Result: ConnectionType = \"" + ucpObtained.UcpConnectionType.ToString() + "\", URL = \"" + ucpObtained.UcpPortalUri + "\"";

            Assert.IsTrue(condition, errMsg); 
        }

        #endregion

        #region IPM Actions

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72784.csv", "T72784#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72784.csv"), TestMethod]
        [TestProperty("Area", "IPMActions")]
        public void T72784_Buttons()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = ContentDefinitions.News.CreateCustomizedData(TestContext);

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            //UIMap.Campaign_ApproveAndPublish();
            //Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            #endregion

            DirectoryInfo testDir = new DirectoryInfo(unzippedFilesPath);
            Assert.IsTrue(UIMap.CompareFilesInDirectories(testDir, TestSettings.TestSettings.ReferenceContentStoragePath + @"IPMActionsButtons\"), "Reference and result files do NOT match!");  
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72786.csv", "T72786#csv",
            DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72786.csv"), TestMethod]
        [TestProperty("Area", "IPMActions")]
        public void T72786_Links()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.News news = ContentDefinitions.News.CreateCustomizedData(TestContext);

            campaign.ContentTypes = new string[] { "NEWS" };
            campaign.Content = new object[] { news };

            #endregion

            string campaignID = UIMap.Campaign_CreateAndVerify(TestSettings.TestSettings.TestServer, campaign);

            Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");
            //UIMap.Campaign_ApproveAndPublish();
            //Assert.IsTrue(UIMap.WaitForContentGroupState(campaignID, UIMap.ContentGroupState.Published), "Publication timeout....");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(campaignID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + campaignID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            #endregion

            DirectoryInfo testDir = new DirectoryInfo(unzippedFilesPath);
            Assert.IsTrue(UIMap.CompareFilesInDirectories(testDir, TestSettings.TestSettings.ReferenceContentStoragePath + @"IPMActionsLinks\"), "Reference and result files do NOT match!");
        }

        #endregion

        #region Other
        /// <summary>
        /// Content. LN. Schedule is required
        /// </summary>
        [TestMethod]
        public void T73113()
        {
            #region Test data preparation

            ContentDefinitions.ContentGroup campaign = ContentDefinitions.ContentGroup.CreateDefaultData();
            ContentDefinitions.LicenseNotification ln = ContentDefinitions.LicenseNotification.CreateDefaultData();
            ln.ScheduleCreationType = UIMap.ScheduleCreationType.None;
            campaign.ContentTypes = new string[] { "LN" };
            campaign.Content = new object[] { ln };

            #endregion

            UIMap.Campaign_CreateWithoutSaving(TestSettings.TestSettings.TestServer, campaign);

            UIMap.LN_AssertNoSchedule();
        }

        #endregion

        #region APPLICATION MESSAGE

        [TestMethod]
        public void T72678_GAS()
        {
            #region Test data preparation
            ContentDefinitions.ContentGroup appMsg = ContentDefinitions.ContentGroup.CreateDefaultData();
            appMsg.ContentTypes = new string[] { "GAS" };
            appMsg.Content = new object[] { ContentDefinitions.GoogleAnaliticsSettings.CreateDefaultData() };
            #endregion

            string appMsgID = UIMap.AppMsg_CreateAndVerify(TestSettings.TestSettings.TestServer, appMsg);
            Assert.IsTrue(UIMap.WaitForContentGroupState(appMsgID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(appMsgID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + appMsgID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.GoogleAnaliticsSettings gasRef = (ContentDefinitions.GoogleAnaliticsSettings)appMsg.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.GoogleAnalyticsSettingsType gasObtained = (ContentCustomization.GoogleAnalyticsSettingsType)iccObtained.Item;

            #endregion

            bool condition = ( (UIMap.ConvertIntToBool(gasRef.SendApplicationStatistics) == gasObtained.SendApplicationStatistics.ToString()) );
            string errMsg = "Error! Not all values are equal! Expected: SendApplicationStatistics: \"" + UIMap.ConvertIntToBool(gasRef.SendApplicationStatistics) + 
                "\", \nResult: \"" + gasObtained.SendApplicationStatistics.ToString();
            
            Assert.IsTrue(condition, errMsg);
        }

        [TestMethod]
        public void T72708_GPS()
        {
            #region Test data preparation
            ContentDefinitions.ContentGroup appMsg = ContentDefinitions.ContentGroup.CreateDefaultData();
            appMsg.ContentTypes = new string[] { "GPS" };
            appMsg.Content = new object[] { ContentDefinitions.GooglePlaySettings.CreateDefaultData() };
            #endregion

            string appMsgID = UIMap.AppMsg_CreateAndVerify(TestSettings.TestSettings.TestServer, appMsg);
            Assert.IsTrue(UIMap.WaitForContentGroupState(appMsgID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(appMsgID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + appMsgID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.GooglePlaySettings gpsRef = (ContentDefinitions.GooglePlaySettings)appMsg.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.GooglePlaySettingsType gpsObtained = (ContentCustomization.GooglePlaySettingsType)iccObtained.Item;

            #endregion

            bool condition = ( (gpsRef.BuyGPProductId == gpsObtained.BuyGooglePlayProductId) && (gpsRef.RenewGPProductId == gpsObtained.RenewGooglePlayProductId) );
            string errMsg = "Error! Not all values are equal! Expected: BuyGPProductId: \"" + gpsRef.BuyGPProductId + "\",\tRenewGPProductId: \"" +
                gpsRef.RenewGPProductId + "\"\nResult: BuyGPProductId: \"" + gpsObtained.BuyGooglePlayProductId + "\",\tRenewGPProductId: \"" + 
                gpsObtained.RenewGooglePlayProductId + "\"";

            Assert.IsTrue(condition, errMsg);
        }

        [TestMethod]
        public void T72710_WSS()
        {
            #region Test data preparation
            ContentDefinitions.ContentGroup appMsg = ContentDefinitions.ContentGroup.CreateDefaultData();
            appMsg.ContentTypes = new string[] { "WSS" };
            appMsg.Content = new object[] { ContentDefinitions.WebSurveySettings.CreateDefaultData() };
            #endregion

            string appMsgID = UIMap.AppMsg_CreateAndVerify(TestSettings.TestSettings.TestServer, appMsg);
            Assert.IsTrue(UIMap.WaitForContentGroupState(appMsgID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(appMsgID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + appMsgID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.WebSurveySettings wssRef = (ContentDefinitions.WebSurveySettings)appMsg.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.WebSurveySettingsType wssObtained = (ContentCustomization.WebSurveySettingsType)iccObtained.Item;

            #endregion

            bool condition = ((UIMap.ConvertIntToBool(wssRef.PerformAfterPurchaseSurvey) == wssObtained.PerformAfterPurchaseSurvey.ToString()) && 
                (wssRef.SurveyURL == wssObtained.SurveyUrl) );
            string errMsg = "Error! Not all values are equal! Expected: SendApplicationStatistics: \"" + UIMap.ConvertIntToBool(wssRef.PerformAfterPurchaseSurvey) +
                "\",\tSurveyURL: \"" + wssRef.SurveyURL + "\",\nResult: SendApplicationStatistics: \"" + wssObtained.PerformAfterPurchaseSurvey.ToString() + 
                "\", SurveyURL: \"" + wssObtained.SurveyUrl + "\"";

            Assert.IsTrue(condition, errMsg);
        }

        [TestMethod]
        public void T72717_SS()
        {
            #region Test data preparation
            ContentDefinitions.ContentGroup appMsg = ContentDefinitions.ContentGroup.CreateDefaultData();
            appMsg.ContentTypes = new string[] { "SS" };
            appMsg.Content = new object[] { ContentDefinitions.SocializationSettings.CreateDefaultData() };
            #endregion

            string appMsgID = UIMap.AppMsg_CreateAndVerify(TestSettings.TestSettings.TestServer, appMsg);
            Assert.IsTrue(UIMap.WaitForContentGroupState(appMsgID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(appMsgID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + appMsgID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.SocializationSettings ssRef = (ContentDefinitions.SocializationSettings)appMsg.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.SocializationSettingsType ssObtained = (ContentCustomization.SocializationSettingsType)iccObtained.Item;

            #endregion

            bool condition = ((UIMap.ConvertIntToBool(ssRef.EnableSocialization) == ssObtained.EnableSocialization.ToString()) &&
                (ssRef.SocializationId == ssObtained.SocializationId));
            string errMsg = "Error! Not all values are equal! Expected: EnableSocialization: \"" + UIMap.ConvertIntToBool(ssRef.EnableSocialization) +
                "\",\tSocializationId: \"" + ssRef.SocializationId + "\",\nResult: EnableSocialization: \"" + ssObtained.EnableSocialization.ToString() + 
                "\", SocializationId: \"" + ssObtained.SocializationId + "\"";

            Assert.IsTrue(condition, errMsg);
        }

        [TestMethod]
        public void T72718_BRS()
        {
            #region Test data preparation
            ContentDefinitions.ContentGroup appMsg = ContentDefinitions.ContentGroup.CreateDefaultData();
            appMsg.ContentTypes = new string[] { "BRS" };
            appMsg.Content = new object[] { ContentDefinitions.BrowserRedirectionSettings.CreateDefaultData() };
            #endregion

            string appMsgID = UIMap.AppMsg_CreateAndVerify(TestSettings.TestSettings.TestServer, appMsg);
            Assert.IsTrue(UIMap.WaitForContentGroupState(appMsgID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(appMsgID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + appMsgID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.BrowserRedirectionSettings brsRef = (ContentDefinitions.BrowserRedirectionSettings)appMsg.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.KisBrowserRedirectionSettingsType brsObtained = (ContentCustomization.KisBrowserRedirectionSettingsType)iccObtained.Item;

            #endregion

            bool condition = ((brsRef.RedirectURL == brsObtained.RedirectionServiceUri) && brsRef.AreEqual(brsObtained.BrowserUriList) );
            string errMsg = "Error! Not all values are equal! Expected: RedirectURL: \"" + brsRef.RedirectURL + "\",\tBrowserURL: \"";
            foreach (string url in brsRef.BrowserURL) errMsg += "\"" + url + "\",";
            errMsg += "\nResult: RedirectURL : \"" + brsObtained.RedirectionServiceUri + "\",\tBrowserURL: \"";
            foreach (string url in brsObtained.BrowserUriList) errMsg += "\"" + url + "\",";

            Assert.IsTrue(condition, errMsg);
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\TestData\\T72720.csv",
            "T72720#csv", DataAccessMethod.Sequential),
            DeploymentItem("\\TestData\\T72720.csv"), TestMethod]
        public void T72720_NVI()
        {
            #region Test data preparation
            ContentDefinitions.ContentGroup appMsg = ContentDefinitions.ContentGroup.CreateDefaultData();
            appMsg.ContentTypes = new string[] { "NVI" };
            ContentDefinitions.NewVersionInfo nvi = ContentDefinitions.NewVersionInfo.CreateDefaultData();
            nvi.Content = new string[] 
            {
                TestContext.DataRow["File1"].ToString(),
                TestContext.DataRow["File2"].ToString(),
                TestContext.DataRow["File3"].ToString(),
                TestContext.DataRow["File4"].ToString()
            };
            appMsg.Content = new object[] { nvi };
            #endregion

            string appMsgID = UIMap.AppMsg_CreateAndVerify(TestSettings.TestSettings.TestServer, appMsg);
            Assert.IsTrue(UIMap.WaitForContentGroupState(appMsgID, UIMap.ContentGroupState.VerificationPublished), "Verification Publication timeout or something went terribly wrong!");

            #region Verification data preparation

            string archiveFullName = TestSettings.TestSettings.VerContentStoragePath + UIMap.GetCorrespondingContentArchive(appMsgID);
            string unzippedFilesPath = TestSettings.TestSettings.UnzippedContentDefaultStoragePath + appMsgID;
            UIMap.UnzipFiles(archiveFullName, unzippedFilesPath, true);

            ContentDefinitions.NewVersionInfo nviRef = (ContentDefinitions.NewVersionInfo)appMsg.Content[0];
            ContentCustomization.IpmContentCustomization iccObtained = UIMap.DeserializeContentCustomization(unzippedFilesPath + @"\meta.xml");
            ContentCustomization.KisNewVersionInfoType nviObtained = (ContentCustomization.KisNewVersionInfoType)iccObtained.Item;

            #endregion

            bool condition = ( (nviRef.DistributionAppId == nviObtained.ApplicationId) &&
                (nviRef.DistributionAppVersion == nviObtained.ApplicationVersion) && (nviRef.DistributionDownloadURL == nviObtained.DistributiveDownloadUri) &&
                (nviRef.AboutNewProductDownloadURL == nviObtained.ContentUri) && (nviRef.DistributionType == nviObtained.DistributiveType.ToString()) &&
                (nviRef.MessageTitle == nviObtained.ShowMessageTitle) && (nviRef.MessagePeriodInDays == nviObtained.ShowMessageDaysCount) &&
                (nviRef.MessageDelayInDays == nviObtained.ShowMessageDelayInDays) );
            string errMsg = string.Format("Error! Not all values are equal! Expected: DistributionAppId: {0}, DistributionAppVersion: {1}, DistributionDownloadURL: {2}, " + 
                "AboutNewProductURL: {3}, DistributionType: {4}, MessageTitle: {5}, MessagePeriod: {6}, MessageDelay: {7},\nResult: DistributionAppId: {8}, " +
                "DistributionAppVersion: {9}, DistributionDownloadURL: {10}, AboutNewProductURL: {11}, DistributionType: {12}, MessageTitle: {13}, MessagePeriod: {14}, MessageDelay: {15}",
                new object[] { nviRef.DistributionAppId, nviRef.DistributionAppVersion, nviRef.DistributionDownloadURL, nviRef.AboutNewProductDownloadURL, nviRef.DistributionType, 
                nviRef.MessageTitle, nviRef.MessagePeriodInDays, nviRef.MessageDelayInDays, nviObtained.ApplicationId, nviObtained.ApplicationVersion, nviObtained.DistributiveDownloadUri,
                nviObtained.ContentUri, nviObtained.DistributiveType.ToString(), nviObtained.ShowMessageTitle, nviObtained.ShowMessageDaysCount, nviObtained.ShowMessageDelayInDays});

            Assert.IsTrue(condition, errMsg);

            DirectoryInfo testDir = new DirectoryInfo(unzippedFilesPath);
            Assert.IsTrue(UIMap.CompareFilesInDirectories(testDir, TestSettings.TestSettings.ReferenceContentStoragePath + @"NVI\"), "Reference and result files do NOT match!");
        }


        #endregion


        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
