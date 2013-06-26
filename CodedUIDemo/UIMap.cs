namespace IPMATFramework
{
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using System.Windows.Input;
    using System.Windows.Forms;
    using System.CodeDom.Compiler;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    using Ionic.Zip;
    using IPMATFramework.ContentCustomization;
    using IPMATFramework.ContentInfo;

    public partial class UIMap
    {
        public enum ContentGroupState : int
        {
            //[EnumMember]
            Draft = 1,
            //[EnumMember]
            WaitingForVerification = 2,
            //[EnumMember]
            VerificationPublished = 3,
            //[EnumMember]
            Publishing = 4,
            //[EnumMember]
            Published = 5,
            //[EnumMember]
            WaitingForPausing = 6,
            //[EnumMember]
            Paused = 7,
            //[EnumMember]
            WaitingForFinishing = 8,
            //[EnumMember]
            Finished = 9,
            //[EnumMember]
            Approved = 10,
            //[EnumMember]
            WaitingForPublication = 11
        }

        public enum ScheduleCreationType : int
        {
            None = 0,
            Default = 1,
            User = 2
        }

        #region COMMON

        public void StartIPM(string homeURL)
        {
            // Go to web page 'http://ipm2.avp-test.ru/default.aspx' using new browser instance
            this.UIBlankPageWindowsInteWindow.LaunchUrl(new System.Uri(homeURL));//(new System.Uri(this.CreateCampaignPropsFiltersParams.UIBlankPageWindowsInteWindowUrl));
        }

        /// <summary>
        /// Compares unzipped files from test directory with reference files. Excluding meta.xml and [Content_Types].xml
        /// </summary>
        /// <param name="testDir">Directory where content group files were unzipped to</param>
        /// <param name="refDirPath">Directory with reference files</param>
        /// <returns></returns>
        public bool CompareFilesInDirectories(DirectoryInfo testDir, string refDirPath)
        {
            string refFilePath;
            string errMsg;
            foreach (FileInfo fi in testDir.GetFiles())
            {
                if ((fi.Name != "meta.xml") && (fi.Name != "[Content_Types].xml"))
                {
                    refFilePath = refDirPath + fi.Name;
                    errMsg = "Original file: \"" + refFilePath + "\"\nResult: \"" + fi.FullName + "\"";
                    if (!UIMap.CompareTwoFiles(refFilePath, fi.FullName)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Performs file comparison via comparing md5 hash of 4kb blocks
        /// </summary>
        /// <param name="file1">First file to compare</param>
        /// <param name="file2">Second file to compare</param>
        /// <returns>True if files are equal, otherwise - false</returns>
        public static bool CompareTwoFiles(string file1, string file2)
        {
            int f1eof;
            int f2eof;
            byte[] file1byte = new byte[4096];
            byte[] file2byte = new byte[4096];
            FileStream fs1;
            FileStream fs2;
            StringBuilder sbF1 = new StringBuilder();
            StringBuilder sbF2 = new StringBuilder();
            MD5 md5Hasher = MD5.Create();

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                f1eof = fs1.Read(file1byte, 0, 4096);
                f2eof = fs2.Read(file2byte, 0, 4096);

                foreach (Byte b in md5Hasher.ComputeHash(file1byte))
                    sbF1.Append(b.ToString("x2").ToLower());
                foreach (Byte b in md5Hasher.ComputeHash(file2byte))
                    sbF2.Append(b.ToString("x2").ToLower());

                if (sbF1.ToString() != sbF2.ToString())
                {
                    fs1.Close();
                    fs2.Close();
                    return false;
                }
                sbF1.Clear();
                sbF2.Clear();
            }
            while ((f1eof == 4096) && (f2eof == 4096));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return true; ;
        }

        /// <summary>
        /// Unzipes target archive to desired folder
        /// </summary>
        /// <param name="ExistingZipFile">Full path to archive</param>
        /// <param name="TargetDirectory">Folder the archive will be extracted to</param>
        /// <param name="overwrite">If true, files in destination folder will be overwritten</param>
        public void UnzipFiles(string ExistingZipFile, string TargetDirectory, bool overwrite)
        {
            using (ZipFile zip = ZipFile.Read(ExistingZipFile))
            {
                foreach (ZipEntry e in zip)
                {
                    if (overwrite) e.Extract(TargetDirectory, ExtractExistingFileAction.OverwriteSilently);  // overwrite == true
                    else e.Extract(TargetDirectory, ExtractExistingFileAction.DoNotOverwrite);
                }
            }
        }

        /// <summary>
        /// Gets URL of page test is currently running on
        /// </summary>
        /// <returns>URL of page test is running on</returns>
        private string GetCurrentPageURL()
        {
            return this.UIBlankPageWindowsInteWindow.Uri.ToString();
        }

        /// <summary>
        /// Extracts ContentGroupID from page URL
        /// </summary>
        /// <returns>ContentGroupID</returns>
        private string ExtractCampaignID()
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            string uri = GetCurrentPageURL();
            if ((uri.Length < 52) || (!uri.Contains("?ContentGroupId="))) Assert.Fail("Problem encountered during extracting content group ID");
            string id = uri.Substring(uri.IndexOf("?ContentGroupId=") + 16, 36);
            return id;
        }

        /// <summary>
        /// Retrieves content archive name for content group from DB
        /// </summary>
        /// <param name="campaignID">ContentGroupID</param>
        /// <returns>Content archive name</returns>
        public string GetCorrespondingContentArchive(string campaignID)
        {
            SqlConnection myConnection = new SqlConnection("server=dbt6.avp-test.ru;" +
                                       "Trusted_Connection=yes;" +
                                       "database=MarketingCampaignDatabase; " +
                                       "connection timeout=30");
            myConnection.Open();

            string cmdStr = "SELECT ContentArchiveFileName FROM dbo.Content WHERE ContentGroupId=@campaignID";
            SqlCommand cmd = new SqlCommand(cmdStr, myConnection);
            cmd.Parameters.Add("@campaignID", System.Data.SqlDbType.UniqueIdentifier);
            cmd.Parameters["@campaignID"].Value = new Guid(campaignID);

            object obj = cmd.ExecuteScalar();
            myConnection.Close();
            if (obj == null) return null;
            else return obj.ToString();
        }

        /// <summary>
        /// Waits until content group is in desired state
        /// </summary>
        /// <param name="campaignID">ContentGroupID</param>
        /// <param name="state">Desired state of content group</param>
        /// <returns>True if desired content group state is reached in given time, otherwise - false</returns>
        public bool WaitForContentGroupState(string campaignID, ContentGroupState state)
        {
            SqlConnection myConnection = new SqlConnection("server=dbt6.avp-test.ru;" +
                                       "Trusted_Connection=yes;" +
                                       "database=MarketingCampaignDatabase; " +
                                       "connection timeout=30");
            myConnection.Open();

            string cmdStr = "SELECT State FROM dbo.ContentGroup WHERE ContentGroupId=@campaignID";
            SqlCommand cmd = new SqlCommand(cmdStr, myConnection);
            cmd.Parameters.Add("@campaignID", System.Data.SqlDbType.UniqueIdentifier);
            cmd.Parameters["@campaignID"].Value = new Guid(campaignID);
            object obj = null;
            int counter = 0;

            while (true)
            {
                obj = cmd.ExecuteScalar();

                if (obj == null) { myConnection.Close(); return false; }
                else if (obj.ToString() == ((int)state).ToString()) { myConnection.Close(); return true; }
                System.Threading.Thread.Sleep(30000);
                counter++;
                if (counter > 15) { myConnection.Close(); return false; }
            }
        }

        /// <summary>
        /// Deserializes content's meta.xml
        /// </summary>
        /// <param name="xmlPath">Path to content's meta.xml</param>
        /// <returns>Parsed into class content's customization</returns>
        public IpmContentCustomization DeserializeContentCustomization(string xmlPath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(IpmContentCustomization));
            IpmContentCustomization icc;
            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                icc = (IpmContentCustomization)ser.Deserialize(reader);
                return icc;
            }
        }

        /// <summary>
        /// Deserializes "<verification|publishing>_metadata.xml"
        /// </summary>
        /// <param name="xmlPath">Path to "<verification|publishing>_metadata.xml"</param>
        /// <returns>Parsed into class content's info</returns>
        public IpmContentInfo DeserializeContentInfo(string xmlPath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(IpmContentInfo));
            IpmContentInfo ici;
            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                ici = (IpmContentInfo)ser.Deserialize(reader);
                return ici;
            }
        }

        public static string ConvertIntToBool(int arg)
        {
            if (arg == 1) return "True";
            if (arg == 0) return "False";
            else return null;
        }

        public static bool IsTestContentDeployed(string[] files)
        {
            bool isDeployed = true;
            foreach (string file in files)
            {
                isDeployed = (File.Exists(file) ? isDeployed : !isDeployed);
            }
            return isDeployed;
        }

        #endregion

        #region Content: NEWS

        private void News_AddContentToCampaign(ContentDefinitions.News news)
        {
            News_CreateNew();
            News_SetName(news.Name);
            News_SetProps(news);

            if ((news.Files != null) && (news.Files.Length > 0))
                foreach (string filePath in news.Files)
                {
                    News_UploadFile(filePath);
                }

            if (news.ScheduleSettings != null)
            {
                if (news.ScheduleType == "Include" || news.ScheduleType == "Exclude") News_AddScheduleIncludeExclude(news.ScheduleSettings);
                else if (news.ScheduleType == "DisplayDates") News_AddScheduleDisplayDates(news.ScheduleSettings);
                else if (news.ScheduleType == "LDTE" || news.ScheduleType == "LDAE" || news.ScheduleType == "LDAA" ||
                    news.ScheduleType == "SDTE" || news.ScheduleType == "SDAE") News_AddScheduleLicenseAndSubscription(news.ScheduleSettings);
            }

            News_Save();
        }

        private void News_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow3.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uINewsHyperlink = this.UIEditCampaignWindowsIWindow3.UIEditCampaignDocument.UINewsHyperlink;
            #endregion

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(48, 23));

            // Click 'News' link
            Mouse.Click(uINewsHyperlink, new Point(46, 12));
            Playback.Wait(3000);
        }

        private void News_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow4.UIDlgFrame836915570042Frame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow4.UIDlgFrame836915570042Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'NEWS' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;// this.OLD_AddContentNewsParams.UITxtContentTitleEditText;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(82, 10));
        }

        private void News_SetProps(ContentDefinitions.News news)
        {
            #region Variable Declarations
            HtmlEdit uITitleTextBoxEdit = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UITitleTextBoxEdit;
            HtmlTextArea uIAnnouncementTextBoxEdit = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UIAnnouncementTextBoxEdit;
            HtmlComboBox uISeverityDropDownComboBox = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UISeverityDropDownComboBox;
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UICtl00PlaceHolderMainCheckBox;
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox1 = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UICtl00PlaceHolderMainCheckBox1;
            #endregion

            // Type 'newsTitle' in 'TitleTextBox' text box
            uITitleTextBoxEdit.Text = news.Title;//this.OLD_AddContentNewsParams.UITitleTextBoxEditText;

            // Type 'newsAnnouncement' in 'AnnouncementTextBox' text box
            uIAnnouncementTextBoxEdit.Text = news.Announcement;//this.OLD_AddContentNewsParams.UIAnnouncementTextBoxEditText;

            // Select 'Alarm' in 'SeverityDropDown' combo box
            uISeverityDropDownComboBox.SelectedItem = news.Severity;//this.OLD_AddContentNewsParams.UISeverityDropDownComboBoxSelectedItem;

            // Select 'ctl00$PlaceHolderMain$ctl02$ctl07$AutoOpenInApplic...' check box
            if (news.AutoOpen == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;
            //uICtl00PlaceHolderMainCheckBox.Checked = this.OLD_AddContentNewsParams.UICtl00PlaceHolderMainCheckBoxChecked;

            // Select 'ctl00$PlaceHolderMain$ctl02$ctl07$IgnoreDisableSho...' check box
            if (news.IgnoreDisable == 1) uICtl00PlaceHolderMainCheckBox1.Checked = true;
            else uICtl00PlaceHolderMainCheckBox1.Checked = false;
            //uICtl00PlaceHolderMainCheckBox1.Checked = this.OLD_AddContentNewsParams.UICtl00PlaceHolderMainCheckBox1Checked;
        }

        private void News_UploadFile(string fullFilePath)
        {
            #region Variable Declarations
            HtmlFileInput uICtl00PlaceHolderMainFileInput = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UICtl00PlaceHolderMainFileInput;
            HtmlInputButton uIUploadfileButton = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UIUploadfileButton;
            #endregion

            // Type 'C:\Users\ext_zvezdin.KL\Downloads\CONTENT\News\news.html' in 'ctl00$PlaceHolderMain$ctl02$ctl07$DataEditor$FileT...' file input
            uICtl00PlaceHolderMainFileInput.FileName = fullFilePath;//this.OLD_AddContentNewsParams.UICtl00PlaceHolderMainFileInputFileName;

            // Click 'Upload file' button
            Mouse.Click(uIUploadfileButton, new Point(45, 11));
        }

        private void News_AddScheduleIncludeExclude(ContentDefinitions.Schedule[] schedule)
        {
            #region Variable Declarations
            HtmlHyperlink uIScheduleHyperlink = this.UIEditNewsWindowsInterWindow.UIEditNewsDocument.UICtl00_PlaceHolderMaiPane.UIScheduleHyperlink;
            HtmlComboBox uITypeselectComboBox = this.UIEditNewsWindowsInterWindow.UIEditNewsDocument.UITypeselectComboBox;
            HtmlHyperlink uIAddHyperlink = this.UIEditNewsWindowsInterWindow.UIEditNewsDocument.UIAddHyperlink;
            HtmlComboBox uIItemComboBox = new HtmlComboBox(this.UIEditNewsWindowsInterWindow.UIEditNewsDocument);//.UISchedulegridCustom);
            HtmlComboBox uIValueComboBox = new HtmlComboBox(this.UIEditNewsWindowsInterWindow.UIEditNewsDocument);

            #endregion

            // Click 'Schedule' link
            Mouse.Click(uIScheduleHyperlink, new Point(57, 15));

            int counter = 1;
            foreach (ContentDefinitions.Schedule sch in schedule)
            {
                #region Set fields' properties
                #region Operator
                uIItemComboBox.SearchProperties[HtmlComboBox.PropertyNames.Id] = null;
                uIItemComboBox.SearchProperties[HtmlComboBox.PropertyNames.Name] = null;
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.Size] = "0";
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.LabeledBy] = null;
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.Title] = null;
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.ItemCount] = "2";
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.Class] = "ipm-operator-type";
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.ControlDefinition] = "class=ipm-operator-type";
                uIItemComboBox.FilterProperties[HtmlComboBox.PropertyNames.TagInstance] = (counter * 2).ToString();
                uIItemComboBox.WindowTitles.Add("Edit News");
                #endregion

                #region Value
                switch (sch.Name)
                {
                    case "Application Status":
                        uIValueComboBox.SearchProperties[HtmlComboBox.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_Schedule_ApplicationStatusList";
                        uIValueComboBox.SearchProperties[HtmlComboBox.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$Schedule$ApplicationStatusList";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Size] = "0";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.LabeledBy] = null;
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Title] = null;
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.ItemCount] = "17";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Class] = "ipm-application-status";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.ControlDefinition] = "name=ctl00$PlaceHolderMain$ctl02$ctl07$S";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.TagInstance] = (counter * 2 + 1).ToString();
                        uIValueComboBox.WindowTitles.Add("Edit News");
                        break;
                    case "License Legal Status":
                        uIValueComboBox.SearchProperties[HtmlComboBox.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_Schedule_LicenseStatusList";
                        uIValueComboBox.SearchProperties[HtmlComboBox.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$Schedule$LicenseStatusList";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Size] = "0";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.LabeledBy] = null;
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Title] = null;
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.ItemCount] = "3";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Class] = "ipm-license-status";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.ControlDefinition] = "name=ctl00$PlaceHolderMain$ctl02$ctl07$S";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.TagInstance] = (counter * 2 + 1).ToString();
                        uIValueComboBox.WindowTitles.Add("Edit News");
                        break;
                    case "License Type":
                        uIValueComboBox.SearchProperties[HtmlComboBox.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_Schedule_LicenseTypeList";
                        uIValueComboBox.SearchProperties[HtmlComboBox.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$Schedule$LicenseTypeList";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Size] = "0";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.LabeledBy] = null;
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Title] = null;
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.ItemCount] = "8";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.Class] = "ipm-license-type";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.ControlDefinition] = "name=ctl00$PlaceHolderMain$ctl02$ctl07$S";
                        uIValueComboBox.FilterProperties[HtmlComboBox.PropertyNames.TagInstance] = (counter * 2 + 1).ToString();
                        uIValueComboBox.WindowTitles.Add("Edit News");
                        break;
                    default:
                        break;
                }
                #endregion
                #endregion

                // Select 'Application Status' in 'type-select' combo box
                uITypeselectComboBox.SelectedItem = sch.Name;

                // Click 'Add' link
                Mouse.Click(uIAddHyperlink, new Point(10, 11));

                // Select 'Include' in combo box
                uIItemComboBox.SelectedItem = sch.Operator;

                // Select 'NoLicense' in 'ApplicationStatusList' combo box
                uIValueComboBox.SelectedItem = sch.Value;

                counter++;
                Playback.Wait(500);
            }
        }

        private void News_AddScheduleDisplayDates(ContentDefinitions.Schedule[] schedule)
        {
            #region Variable Declarations
            HtmlHyperlink uIScheduleHyperlink = this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument.UICtl00_PlaceHolderMaiPane.UIScheduleHyperlink;
            HtmlComboBox uITypeselectComboBox = this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument.UITypeselectComboBox;
            HtmlHyperlink uIAddHyperlink = this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument.UIAddHyperlink;

            HtmlEdit uIFrom = new HtmlEdit(this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument);
            HtmlEdit uITo = new HtmlEdit(this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument);
            HtmlEdit uIDynamicIdSearch = new HtmlEdit(this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument);

            HtmlDiv uIFromToPane = this.UIEditNewsWindowsInterWindow1.UIEditNewsDocument.UISchedulegridCustom.UIFromToPane;
            #endregion

            // Click 'Schedule' link
            Mouse.Click(uIScheduleHyperlink, new Point(55, 14));

            uIDynamicIdSearch.SearchProperties.Add("Id", "dp", PropertyExpressionOperator.Contains);
            UITestControlCollection tcc;

            int counter = 0;
            foreach (ContentDefinitions.Schedule sch in schedule)
            {
                // Select 'Display Dates' in 'type-select' combo box
                uITypeselectComboBox.SelectedItem = sch.Name;

                // Click 'Add' link
                Mouse.Click(uIAddHyperlink, new Point(14, 9));

                #region Set fields' properties
                tcc = uIDynamicIdSearch.FindMatchingControls();

                uIFrom.SearchProperties[HtmlEdit.PropertyNames.Id] = tcc[counter++].FriendlyName;
                uIFrom.SearchProperties[HtmlEdit.PropertyNames.Name] = null;
                uIFrom.SearchProperties[HtmlEdit.PropertyNames.LabeledBy] = null;
                uIFrom.SearchProperties[HtmlEdit.PropertyNames.Type] = "SINGLELINE";
                uIFrom.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                uIFrom.FilterProperties[HtmlEdit.PropertyNames.Class] = "ipm-date-picker from hasDatepicker";
                uIFrom.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "class=\"ipm-date-picker from hasDatepicke";
                uIFrom.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "40";
                uIFrom.WindowTitles.Add("Edit News");


                uITo.SearchProperties[HtmlEdit.PropertyNames.Id] = tcc[counter++].FriendlyName;
                uITo.SearchProperties[HtmlEdit.PropertyNames.Name] = null;
                uITo.SearchProperties[HtmlEdit.PropertyNames.LabeledBy] = null;
                uITo.SearchProperties[HtmlEdit.PropertyNames.Type] = "SINGLELINE";
                uITo.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                uITo.FilterProperties[HtmlEdit.PropertyNames.Class] = "ipm-date-picker to hasDatepicker";
                uITo.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "class=\"ipm-date-picker to hasDatepicker\"";
                uITo.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "41";
                uITo.WindowTitles.Add("Edit News");
                #endregion

                //tcc = uIFrom.FindMatchingControls();

                uIFrom.Text = sch.DaysRange.From.ToString();
                uITo.Text = sch.DaysRange.To.ToString();

                // Click 'From: To:' pane
                Mouse.Click(uIFromToPane, new Point(276, 5));
            }

        }

        private void News_AddScheduleLicenseAndSubscription(ContentDefinitions.Schedule[] schedule)
        {
            #region Variable Declarations
            HtmlHyperlink uIScheduleHyperlink = this.UIEditNewsWindowsInterWindow2.UIEditNewsDocument.UICtl00_PlaceHolderMaiPane.UIScheduleHyperlink;
            HtmlComboBox uITypeselectComboBox = this.UIEditNewsWindowsInterWindow2.UIEditNewsDocument.UITypeselectComboBox;
            HtmlHyperlink uIAddHyperlink = this.UIEditNewsWindowsInterWindow2.UIEditNewsDocument.UIAddHyperlink;
            HtmlHyperlink uIAddRangeHyperlink = this.UIEditNewsWindowsInterWindow2.UIEditNewsDocument.UISchedulegridCustom.UIAddRangeHyperlink;
            HtmlEdit uIItemEditFrom = this.UIEditNewsWindowsInterWindow2.UIEditNewsDocument.UISchedulegridCustom.UIItemEdit;
            HtmlEdit uIItemEditTo = this.UIEditNewsWindowsInterWindow2.UIEditNewsDocument.UISchedulegridCustom.UIItemEdit1;
            #endregion

            Assert.IsFalse((schedule.Length == 0), "No data for schedule provided");
            Assert.IsTrue((schedule.Length == 1), "Multiple schedule entries of this type are not allowed! Type: " + schedule[0].Name);

            // Click 'Schedule' link
            Mouse.Click(uIScheduleHyperlink, new Point(58, 15));

            // Select 'License Days to Expiration' in 'type-select' combo box
            uITypeselectComboBox.SelectedItem = schedule[0].Name;

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink, new Point(11, 7));

            // Click 'Add Range' link
            Mouse.Click(uIAddRangeHyperlink, new Point(36, 9));

            // Type '10' in text box
            uIItemEditFrom.Text = schedule[0].DaysRange.From;

            // Type '21' in text box
            uIItemEditTo.Text = schedule[0].DaysRange.To;
        }

        private void News_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditCampaignWindowsIWindow5.UIEditNewsDocument.UISaveButton;
            HtmlInputButton uISaveButton1 = this.UIEditCampaignWindowsIWindow3.UIEditCampaignDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(39, 10));
            Playback.Wait(5000);
            // Click 'Save' button
            //Mouse.Click(uISaveButton1, new Point(39, 17));
        }

        #endregion

        #region Content: WelcomePage

        public void WP_AddContentToCampaign(ContentDefinitions.WelcomePage wp)
        {
            WP_CreateNew();
            WP_SetName(wp.Name);
            WP_SetProps(wp);

            if ((wp.Files != null) && (wp.Files.Length > 0))
                foreach (string filePath in wp.Files)
                {
                    News_UploadFile(filePath);
                }

            WP_Save();
        }

        private void WP_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow9.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uIWelcomePageHyperlink = this.UIEditCampaignWindowsIWindow9.UIEditCampaignDocument.UIWelcomePageHyperlink;
            #endregion

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(53, 27));

            // Click 'Welcome Page' link
            Mouse.Click(uIWelcomePageHyperlink, new Point(57, 11));
            Playback.Wait(3000);
        }

        private void WP_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow10.UIDlgFrame9cb4425e1123Frame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow10.UIDlgFrame9cb4425e1123Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'contentWP' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(50, 9));
        }

        private void WP_SetProps(ContentDefinitions.WelcomePage wp)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditCampaignWindowsIWindow11.UIEditWelcomePageDocument.UICtl00PlaceHolderMainCheckBox;
            #endregion

            // Select 'ctl00$PlaceHolderMain$ctl02$ctl07$DisableCheckBox' check box
            if (wp.DisableWelcomePage == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else if (wp.DisableWelcomePage == 0) uICtl00PlaceHolderMainCheckBox.Checked = false;
        }

        private void WP_UploadFile(string fullFilePath)
        {
            #region Variable Declarations
            HtmlFileInput uICtl00PlaceHolderMainFileInput = this.UIEditCampaignWindowsIWindow11.UIEditWelcomePageDocument.UICtl00PlaceHolderMainFileInput;
            HtmlInputButton uIUploadfileButton = this.UIEditCampaignWindowsIWindow11.UIEditWelcomePageDocument.UIUploadfileButton;
            #endregion

            // Type 'C:\Users\ext_zvezdin.KL\Downloads\CONTENT\WelcomePage\iso_bcg.jpg' in 'ctl00$PlaceHolderMain$ctl02$ctl07$DataEditor$FileT...' file input
            uICtl00PlaceHolderMainFileInput.FileName = fullFilePath;

            // Click 'Upload file' button
            Mouse.Click(uIUploadfileButton, new Point(54, 12));
        }

        private void WP_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditCampaignWindowsIWindow11.UIEditWelcomePageDocument.UISaveButton;
            HtmlInputButton uISaveButton1 = this.UIEditCampaignWindowsIWindow9.UIEditCampaignDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(38, 9));

            // Click 'Save' button
            //Mouse.Click(uISaveButton1, new Point(34, 15));
        }

        #endregion

        #region Content: EndOfLicense Page
        public void EOLP_AddContentToCampaign(ContentDefinitions.EndOfLicense eolp)
        {
            EOLP_CreateNew();
            EOLP_SetName(eolp.Name);
            EOLP_SetProps(eolp);

            if ((eolp.Files != null) && (eolp.Files.Length > 0))
                foreach (string filePath in eolp.Files)
                {
                    EOLP_UploadFile(filePath);
                }

            EOLP_Save();
        }

        private void EOLP_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow12.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uIEndofLicensePageHyperlink = this.UIEditCampaignWindowsIWindow12.UIEditCampaignDocument.UIEndofLicensePageHyperlink;
            #endregion

            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(47, 24));

            // Click 'End of License Page' link
            Mouse.Click(uIEndofLicensePageHyperlink, new Point(40, 15));
        }

        private void EOLP_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow13.UIDlgFrame81b836db5112Frame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow13.UIDlgFrame81b836db5112Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'EOLcontent' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(40, 8));
        }

        private void EOLP_SetProps(ContentDefinitions.EndOfLicense eolp)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditCampaignWindowsIWindow14.UIEditEndOfLicensePageDocument.UICtl00PlaceHolderMainCheckBox;
            #endregion

            // Select 'ctl00$PlaceHolderMain$ctl02$ctl07$IsDisabledCheckb...' check box
            if (eolp.DisableEOLP == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else if (eolp.DisableEOLP == 0) uICtl00PlaceHolderMainCheckBox.Checked = false;
        }

        private void EOLP_UploadFile(string fullFilePath)
        {
            #region Variable Declarations
            HtmlFileInput uICtl00PlaceHolderMainFileInput = this.UIEditCampaignWindowsIWindow14.UIEditEndOfLicensePageDocument.UICtl00PlaceHolderMainFileInput;
            HtmlInputButton uIUploadfileButton = this.UIEditCampaignWindowsIWindow14.UIEditEndOfLicensePageDocument.UIUploadfileButton;
            #endregion

            // Type 'C:\Users\ext_zvezdin.KL\Downloads\CONTENT\EndOfLicense\eol.html' in 'ctl00$PlaceHolderMain$ctl02$ctl07$DataEditor$FileT...' file input
            uICtl00PlaceHolderMainFileInput.FileName = fullFilePath;

            // Click 'Upload file' button
            Mouse.Click(uIUploadfileButton, new Point(89, 18));
        }

        private void EOLP_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditCampaignWindowsIWindow14.UIEditEndOfLicensePageDocument.UISaveButton;
            HtmlInputButton uISaveButton1 = this.UIEditCampaignWindowsIWindow12.UIEditCampaignDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(50, 23));

            // Click 'Save' button
            //Mouse.Click(uISaveButton1, new Point(22, 14));
        }
        #endregion

        #region Content: Regret page
        private void RP_AddContentToCampaign(ContentDefinitions.RegretPage rp)
        {
            RP_CreateNew();
            RP_SetName(rp.Name);
            RP_SetProps(rp);
            RP_Save();
        }

        private void RP_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow29.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uIRegretPageHyperlink = this.UIEditCampaignWindowsIWindow29.UIEditCampaignDocument.UIRegretPageHyperlink;
            #endregion

            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(53, 23));

            // Click 'Regret Page' link
            Mouse.Click(uIRegretPageHyperlink, new Point(45, 13));
            Playback.Wait(5000);
        }

        private void RP_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow30.UIDlgFramec78af8101b1dFrame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow30.UIDlgFramec78af8101b1dFrame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'Name' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(72, 13));
        }

        private void RP_SetProps(ContentDefinitions.RegretPage rp)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditCampaignWindowsIWindow31.UIEditRegretPageDocument.UICtl00PlaceHolderMainCheckBox;
            HtmlEdit uIRegretPageUrlEdit = this.UIEditCampaignWindowsIWindow31.UIEditRegretPageDocument.UIRegretPageUrlEdit;
            #endregion

            // Clear 'ctl00$PlaceHolderMain$ctl02$ctl07$DisableRegretPag...' check box
            if (rp.DisableRegretPage == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;

            // Type 'http://regret.page.url.com' in 'RegretPageUrl' text box
            uIRegretPageUrlEdit.Text = rp.RegretPageURL;
        }

        private void RP_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditCampaignWindowsIWindow31.UIEditRegretPageDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(33, 12));
            Playback.Wait(5000);
        }


        #endregion

        #region Content: ApplicationButtons

        private void AB_AddContentToCampaign(ContentDefinitions.AppButtons appBut)
        {
            AB_CreateNew();
            AB_SetName(appBut.Name);
            AB_SetProps(appBut);
            AB_Save();
        }

        private void AB_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow18.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uIApplicationButtonsHyperlink = this.UIEditCampaignWindowsIWindow18.UIEditCampaignDocument.UIApplicationButtonsHyperlink;
            #endregion

            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(52, 21));

            // Click 'Application Buttons' link
            Mouse.Click(uIApplicationButtonsHyperlink, new Point(51, 9));
        }

        private void AB_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow19.UIDlgFrame408a5981f827Frame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow19.UIDlgFrame408a5981f827Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'appButtons' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(27, 15));
        }

        private void AB_SetProps(ContentDefinitions.AppButtons appBut)
        {
            #region Variable Declarations
            HtmlHyperlink uIAddButtonHyperlink = this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument.UIAddButtonHyperlink;
            HtmlComboBox butType = new HtmlComboBox(this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument);//this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument.UIButtonTypeList0ComboBox;
            HtmlEdit butName = new HtmlEdit(this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument);//this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument.UIButtonNameTextBox0Edit;
            HtmlComboBox butAction = new HtmlComboBox(this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument);//this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument.UIButtonActionList0ComboBox;
            HtmlEdit butURL = new HtmlEdit(this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument);//this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument.UIButtonUriTextBox0Edit;
            int count = 0;
            #endregion

            foreach (ContentDefinitions.AppButtons.SingleButton button in appBut.Collection)
            {
                // Click 'Add Button' link
                Mouse.Click(uIAddButtonHyperlink, new Point(60, 18));
                Playback.Wait(3000);

                butType.SearchProperties[HtmlComboBox.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_ButtonTypeList" + count.ToString();
                butType.SearchProperties[HtmlComboBox.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$ButtonTypeList" + count.ToString();
                butType.SelectedItem = button.ButtonType;

                butName.SearchProperties[HtmlEdit.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_ButtonNameTextBox" + count.ToString();
                butName.SearchProperties[HtmlEdit.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$ButtonNameTextBox" + count.ToString();
                butName.Text = button.ButtonName;

                butAction.SearchProperties[HtmlComboBox.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_ButtonActionList" + count.ToString();
                butAction.SearchProperties[HtmlComboBox.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$ButtonActionList" + count.ToString();
                if (button.AppAction != "") butAction.SelectedItem = button.AppAction;

                butURL.SearchProperties[HtmlEdit.PropertyNames.Id] = "ctl00_PlaceHolderMain_ctl02_ctl07_ButtonUriTextBox" + count.ToString();
                butURL.SearchProperties[HtmlEdit.PropertyNames.Name] = "ctl00$PlaceHolderMain$ctl02$ctl07$ButtonUriTextBox" + count.ToString();
                if (button.CustomURL != "") butURL.Text = button.CustomURL;

                count++;
            }
        }

        private void AB_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditCampaignWindowsIWindow20.UIEditApplicationButtoDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(40, 11));
            Playback.Wait(5000);
        }

        #endregion

        #region Content: License Notification

        public void LN_AddContentToCampaign(ContentDefinitions.LicenseNotification ln)
        {
            LN_CreateNew();
            LN_SetName(ln.Name);
            LN_SetMainPagePane(ln);
            LN_SetBalloonPane(ln);
            LN_SetLicenseManagerPane(ln);
            LN_SetMessageBoxPane(ln);
            LN_SetProblemListPane(ln);

            if (ln.ScheduleCreationType == ScheduleCreationType.Default) LN_SetDefaultSchedule();
            else if (ln.ScheduleCreationType == ScheduleCreationType.User) LN_SetUserSchedule();

            LN_Save();
        }

        public void LN_AddContentToVerifyRequiredFields(ContentDefinitions.LicenseNotification ln)
        {
            LN_CreateNew();
            LN_SetName(ln.Name);
            LN_SetSeverityOnMainPagePane(ln.MainPage.Severity);
            LNS_Save();
        }

        public void LN_AssertNoSchedule()
        {
            #region Variable Declarations
            HtmlSpan uIPleasedefinetheschedPane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument2.UICtl00_MSO_ContentDivPane.UIPleasedefinetheschedPane;
            #endregion

            // Verify that the 'InnerText' property of 'Please define the schedule for this cont' pane equals 'Please define the schedule for this content'
            Assert.AreEqual("Please define the schedule for this content", uIPleasedefinetheschedPane.InnerText, "Required error message was not shown to user");
        }

        private void LN_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow21.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uILicenseNotificationHyperlink = this.UIEditCampaignWindowsIWindow21.UIEditCampaignDocument.UILicenseNotificationHyperlink;
            #endregion

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(47, 22));

            // Click 'License Notification' link
            Mouse.Click(uILicenseNotificationHyperlink, new Point(82, 11));
            Playback.Wait(3000);
        }

        private void LN_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow22.UIDlgFrameec8e071b9143Frame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow22.UIDlgFrameec8e071b9143Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'LN' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(20, 9));
        }

        private void LN_SetMainPagePane(ContentDefinitions.LicenseNotification ln)
        {
            #region Variable Declarations
            HtmlComboBox uISeverityDropDownComboBox = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UISeverityDropDownComboBox;
            HtmlSpan uIShowMainPagePane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane.UIShowMainPagePane;
            HtmlEdit uIProtectionStateTextBEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIProtectionStateTextBEdit;
            HtmlEdit uILicenseStateTextBoxEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseStateTextBoxEdit;
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainCheckBox;
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox1 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainCheckBox1;
            #endregion

            // Select 'Warning' in 'SeverityDropDown' combo box
            uISeverityDropDownComboBox.SelectedItem = ln.MainPage.Severity;

            // Click 'Show Main Page' pane
            Mouse.Click(uIShowMainPagePane, new Point(103, 18));

            // Type 'protectionState__LOW' in 'ProtectionStateTextBox' text box
            uIProtectionStateTextBEdit.Text = ln.MainPage.ProtectionState;

            // Type 'licenseState__LOW' in 'LicenseStateTextBox' text box
            uILicenseStateTextBoxEdit.Text = ln.MainPage.LicenseState;

            // Select 'ctl00$PlaceHolderMain$ctl02$ctl07$ShowApplicationB...' check box
            if (ln.MainPage.ShowAppButton == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;

            // Select 'ctl00$PlaceHolderMain$ctl02$ctl07$ShowToastCheckBo...' check box
            if (ln.MainPage.ShowToast == 1) uICtl00PlaceHolderMainCheckBox1.Checked = true;
            else uICtl00PlaceHolderMainCheckBox1.Checked = false;
        }

        private void LN_SetSeverityOnMainPagePane(string severity)
        {
            #region Variable Declarations
            HtmlComboBox uISeverityDropDownComboBox = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UISeverityDropDownComboBox;
            #endregion

            // Select 'Warning' in 'SeverityDropDown' combo box
            uISeverityDropDownComboBox.SelectedItem = severity;
        }

        private void LN_SetBalloonPane(ContentDefinitions.LicenseNotification ln)
        {
            #region Variable Declarations
            HtmlHyperlink uIBalloonHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane1.UIBalloonHyperlink;
            HtmlSpan uIShowBalloonPane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane2.UIShowBalloonPane;
            HtmlTextArea uIBalloonDescriptionTeEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIBalloonDescriptionTeEdit;
            #endregion

            // Click 'Balloon' link
            Mouse.Click(uIBalloonHyperlink, new Point(38, 13));

            // Click 'Show Balloon' pane
            Mouse.Click(uIShowBalloonPane, new Point(89, 15));

            // Type 'BALOOOOOOOOOOOOOOOOOOOOOON content' in 'BalloonDescriptionTextBox' text box
            uIBalloonDescriptionTeEdit.Text = ln.Balloon.Content;
        }

        private void LN_SetLicenseManagerPane(ContentDefinitions.LicenseNotification ln)
        {
            #region Variable Declarations
            HtmlHyperlink uILicenseManagerHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane1.UILicenseManagerHyperlink;
            HtmlSpan uIShowLicenseManagerPane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane3.UIShowLicenseManagerPane;
            HtmlEdit uILicenseManagerTitleTEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseManagerTitleTEdit;
            HtmlTextArea uILicenseManagerDescriEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseManagerDescriEdit;
            HtmlHyperlink uIAddButtonHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIAddButtonHyperlink;
            HtmlEdit uIButtonSearchBase = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane4.UIItemEdit;
            HtmlEdit uIButtonName = new HtmlEdit(this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument);//.UICtl00_PlaceHolderMaiPane4);//.UIItemEdit;
            HtmlCheckBox uIButtonDisplayCart = new HtmlCheckBox(this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument);//.UICtl00_PlaceHolderMaiPane4);//.UIItemCheckBox;
            HtmlComboBox uIButtonApplicationAction = /*new HtmlComboBox(*/this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIApplicationActionLisComboBox;
            HtmlEdit uIButtonURL = new HtmlEdit(this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument);//.UICtl00_PlaceHolderMaiPane4);//.UIItemEdit1;
            HtmlHyperlink uIAddButtonHyperlink1 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIAddButtonHyperlink1;
            HtmlEdit uIItemEdit2 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane5.UIItemEdit;
            HtmlComboBox uIApplicationActionLisComboBox1 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIApplicationActionLisComboBox1;
            HtmlSpan uIShowBackgroundPane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicensemanagerpanelPane.UIShowBackgroundPane;
            HtmlFileInput uICtl00PlaceHolderMainFileInput = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainFileInput;
            HtmlInputButton uIUploadButton = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIUploadButton;
            HtmlEdit uILicenseManagerLeftMaEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseManagerLeftMaEdit;
            HtmlEdit uILicenseManagerTopMarEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseManagerTopMarEdit;
            HtmlEdit uILicenseManagerRightMEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseManagerRightMEdit;
            HtmlEdit uILicenseManagerBottomEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UILicenseManagerBottomEdit;
            #endregion

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.TopLevelWindow;

            // Click 'License Manager' link
            Mouse.Click(uILicenseManagerHyperlink, new Point(59, 11));

            // Click 'Show License Manager' pane
            Mouse.Click(uIShowLicenseManagerPane, new Point(147, 15));

            // Type 'LicenseManagerTitle' in 'LicenseManagerTitleTextBox' text box
            uILicenseManagerTitleTEdit.Text = ln.LicenseManager.Title;

            // Type 'LicenseManagerDescription' in 'LicenseManagerDescriptionTextBox' text box
            uILicenseManagerDescriEdit.Text = ln.LicenseManager.Description;

            //int i = 1;
            int j = 0;
            // TODO: Keyboard workaround should be substituted with operations on controls
            if (ln.LicenseManager.Buttons.Length > 0)
                foreach (ContentDefinitions.LicenseNotification.LicenseManagerPane.Button button in ln.LicenseManager.Buttons)
                {
                    // Click 'Add Button' link
                    Mouse.Click(uIAddButtonHyperlink, new Point(78, 12));
                    Keyboard.SendKeys(button.Name);
                    Keyboard.SendKeys("{Tab}");
                    Keyboard.SendKeys("{Tab}");

                    if (button.DisplayShoppingCart == 1) Keyboard.SendKeys("{Space}");
                    //Keyboard.SendKeys("{Tab}");
                    //Keyboard.SendKeys("{Tab}");

                    uIButtonApplicationAction.FilterProperties[HtmlComboBox.PropertyNames.TagInstance] = (uIButtonApplicationAction.TagInstance + j).ToString();
                    uIButtonApplicationAction.SelectedItem = button.AppAction;
                    uIButtonApplicationAction.SetFocus();

                    //uIButtonURL = new HtmlEdit(this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane4);//.UIItemEdit1;

                    if (button.URL != null || button.URL != "")
                    {
                        Keyboard.SendKeys("{Tab}");
                        Keyboard.SendKeys("{Tab}");
                        Keyboard.SendKeys(button.URL);
                        //uIButtonURL.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = (uIButtonURL.TagInstance + j * 4).ToString();
                        //uIButtonURL.Text = button.URL;
                    }
                    j++;
                }

            // Click 'Show Background' pane
            Mouse.Click(uIShowBackgroundPane, new Point(127, 16));

            // Type 'C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\iso_bcg.jpg' in 'ctl00$PlaceHolderMain$ctl02$ctl07$BackgroundImageU...' file input
            uICtl00PlaceHolderMainFileInput.FileName = ln.LicenseManager.BackgroundSettings.ImagePath;

            // Click 'Upload' button
            Mouse.Click(uIUploadButton, new Point(34, 13));

            // Type '2' in 'LicenseManagerLeftMarginTextBox' text box
            uILicenseManagerLeftMaEdit.Text = ln.LicenseManager.BackgroundSettings.LeftMargin;

            // Type '2' in 'LicenseManagerTopMarginTextBox' text box
            uILicenseManagerTopMarEdit.Text = ln.LicenseManager.BackgroundSettings.TopMargin;

            // Type '2' in 'LicenseManagerRightMarginTextBox' text box
            uILicenseManagerRightMEdit.Text = ln.LicenseManager.BackgroundSettings.RightMargin;

            // Type '2' in 'LicenseManagerBottomMarginTextBox' text box
            uILicenseManagerBottomEdit.Text = ln.LicenseManager.BackgroundSettings.BottomMargin;
        }

        private void LN_SetMessageBoxPane(ContentDefinitions.LicenseNotification ln)
        {
            #region Variable Declarations
            HtmlHyperlink uIMessageBoxHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane1.UIMessageBoxHyperlink;
            HtmlSpan uIShowMessageBoxPane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane11.UIShowMessageBoxPane;
            HtmlEdit uIWindowHeightTextBoxEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIWindowHeightTextBoxEdit;
            HtmlEdit uIWindowWidthTextBoxEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIWindowWidthTextBoxEdit;
            HtmlFileInput uICtl00PlaceHolderMainFileInput1 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainFileInput1;
            HtmlInputButton uIUploadfileButton = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIUploadfileButton;
            HtmlFileInput uICtl00PlaceHolderMainFileInput2 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainFileInput2;
            HtmlInputButton uIUploadfileButton1 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIUploadfileButton1;
            HtmlFileInput uICtl00PlaceHolderMainFileInput3 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainFileInput3;
            HtmlInputButton uIUploadfileButton2 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIUploadfileButton2;
            HtmlFileInput uICtl00PlaceHolderMainFileInput4 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainFileInput4;
            HtmlInputButton uIUploadfileButton3 = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIUploadfileButton3;
            #endregion

            // Click 'Message Box' link
            Mouse.Click(uIMessageBoxHyperlink, new Point(59, 16));

            // Click 'Show Message Box' pane
            Mouse.Click(uIShowMessageBoxPane, new Point(114, 17));

            // Type '500' in 'WindowHeightTextBox' text box
            uIWindowHeightTextBoxEdit.Text = ln.MessageBox.WindowHeight;

            // Type '500' in 'WindowWidthTextBox' text box
            uIWindowWidthTextBoxEdit.Text = ln.MessageBox.WindowWidth;

            if (ln.MessageBox.Files.Length > 0)
                foreach (string filePath in ln.MessageBox.Files)
                {
                    // Type 'C:\Users\ext_zvezdin.KL\Downloads\CONTENT\LicenseNotification\ln.html' in 'ctl00$PlaceHolderMain$ctl02$ctl07$ctl46$FileToUplo...' file input
                    uICtl00PlaceHolderMainFileInput1.FileName = filePath;

                    // Click 'Upload file' button
                    Mouse.Click(uIUploadfileButton, new Point(59, 14));
                }
        }

        private void LN_SetProblemListPane(ContentDefinitions.LicenseNotification ln)
        {
            #region Variable Declarations
            HtmlHyperlink uIProblemListHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane1.UIProblemListHyperlink;
            HtmlSpan uIShowProblemListPane = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane12.UIShowProblemListPane;
            HtmlEdit uIProblemListTitleTextEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIProblemListTitleTextEdit;
            HtmlEdit uIProblemListDescriptiEdit = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UIProblemListDescriptiEdit;
            #endregion

            // Click 'Problem List' link
            Mouse.Click(uIProblemListHyperlink, new Point(72, 16));

            // Click 'Show Problem List' pane
            Mouse.Click(uIShowProblemListPane, new Point(123, 12));

            // Type 'problemListTitle' in 'ProblemListTitleTextBox' text box
            uIProblemListTitleTextEdit.Text = ln.ProblemList.Title;

            // Type 'problemListDescription' in 'ProblemListDescriptionTextBox' text box
            uIProblemListDescriptiEdit.Text = ln.ProblemList.Description;
        }

        private void LN_SetDefaultSchedule()
        {
            #region Variable Declarations
            HtmlHyperlink uIScheduleHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument1.UICtl00_PlaceHolderMaiPane.UIScheduleHyperlink;
            HtmlComboBox uITypeselectComboBox = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument1.UITypeselectComboBox;
            HtmlHyperlink uIAddHyperlink = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument1.UIAddHyperlink;
            HtmlComboBox uIItemComboBox = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument1.UISchedulegridCustom.UIItemComboBox;
            HtmlComboBox uIApplicationStatusLisComboBox = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument1.UIApplicationStatusLisComboBox;
            #endregion

            // Click 'Schedule' link
            Mouse.Click(uIScheduleHyperlink, new Point(48, 15));

            // Select 'Application Status' in 'type-select' combo box
            uITypeselectComboBox.SelectedItem = "Application Status";

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink, new Point(6, 7));

            // Select 'Include' in combo box
            uIItemComboBox.SelectedItem = "Include";

            // Select 'TrialExpired' in 'ApplicationStatusList' combo box
            uIApplicationStatusLisComboBox.SelectedItem = "TrialExpired";
        }

        //ToDo: Create
        private void LN_SetUserSchedule() { }

        private void LN_Save()
        {
            #region
            HtmlInputButton uISaveButton = this.UIEditLicenseNotificatWindow.UIEditLicenseNotificatDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(23, 13));
        }
        #endregion

        #region Content: License Notification Settings

        public void LNS_AddContentToCampaign(ContentDefinitions.LicenseNotificationSettings lns)
        {
            LNS_CreateNew();
            LNS_SetName(lns.Name);
            LNS_SetProps(lns);

            //ToDo: Change
            if (lns.ScheduleSettings != null)
            {
                /*if (lns.ScheduleType == "Include" || lns.ScheduleType == "Exclude") News_AddScheduleIncludeExclude(lns.ScheduleSettings);
                else if (lns.ScheduleType == "DisplayDates") News_AddScheduleDisplayDates(lns.ScheduleSettings);
                else if (lns.ScheduleType == "LDTE" || lns.ScheduleType == "LDAE" || lns.ScheduleType == "LDAA" ||
                    lns.ScheduleType == "SDTE" || lns.ScheduleType == "SDAE") News_AddScheduleLicenseAndSubscription(lns.ScheduleSettings);*/
            }
            else LNS_SetDefaultSchedule();

            LNS_Save();
        }

        private void LNS_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAddContentHyperlink = this.UIEditCampaignWindowsIWindow23.UIEditCampaignDocument.UIAddContentPane.UIAddContentHyperlink;
            HtmlHyperlink uILicenseNotificationSHyperlink = this.UIEditCampaignWindowsIWindow23.UIEditCampaignDocument.UILicenseNotificationSHyperlink;
            #endregion

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            // Click 'Add Content' link
            Mouse.Click(uIAddContentHyperlink, new Point(45, 20));

            // Click 'License Notification Settings' link
            Mouse.Click(uILicenseNotificationSHyperlink, new Point(92, 9));
            Playback.Wait(3000);
        }

        private void LNS_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow24.UIDlgFrameff26e13fcc5fFrame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow24.UIDlgFrameff26e13fcc5fFrame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'LicenseNoteficationSettings' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(25, 7));
        }

        private void LNS_SetProps(ContentDefinitions.LicenseNotificationSettings lns)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditCampaignWindowsIWindow25.UIEditLicenseNotificatDocument.UICtl00PlaceHolderMainCheckBox;
            #endregion

            if (lns.DisableNotIPMMessages == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;
        }

        private void LNS_SetDefaultSchedule()
        {
            #region Variable Declarations
            HtmlHyperlink uIScheduleHyperlink = this.UIEditLicenseNotificatWindow2.UIEditLicenseNotificatDocument.UICtl00_PlaceHolderMaiPane.UIScheduleHyperlink;
            HtmlComboBox uITypeselectComboBox = this.UIEditLicenseNotificatWindow2.UIEditLicenseNotificatDocument.UITypeselectComboBox;
            HtmlHyperlink uIAddHyperlink = this.UIEditLicenseNotificatWindow2.UIEditLicenseNotificatDocument.UIAddHyperlink;
            HtmlComboBox uIItemComboBox = this.UIEditLicenseNotificatWindow2.UIEditLicenseNotificatDocument.UISchedulegridCustom.UIItemComboBox;
            HtmlComboBox uIApplicationStatusLisComboBox = this.UIEditLicenseNotificatWindow2.UIEditLicenseNotificatDocument.UIApplicationStatusLisComboBox;
            #endregion

            // Click 'Schedule' link
            Mouse.Click(uIScheduleHyperlink, new Point(58, 17));

            // Select 'Application Status' in 'type-select' combo box
            uITypeselectComboBox.SelectedItem = "Application Status";

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink, new Point(14, 12));

            // Select 'Include' in combo box
            uIItemComboBox.SelectedItem = "Include";

            // Select 'TrialExpired' in 'ApplicationStatusList' combo box
            uIApplicationStatusLisComboBox.SelectedItem = "TrialExpired";
        }

        private void LNS_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditLicenseNotificatWindow1.UIEditLicenseNotificatDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(33, 17));
        }

        #endregion

        #region Content: UCP
        private void UCP_AddContentToCampaign(ContentDefinitions.UCP ucp)
        {
            UCP_CreateNew();
            UCP_SetName(ucp.Name);
            UCP_SetProps(ucp);
            UCP_Save();
        }

        private void UCP_CreateNew()
        {
            #region Variable Declarations
            HtmlImage uIAddContentImage = this.UIEditCampaignWindowsIWindow32.UIEditCampaignDocument.UIAddContentPane.UIAddContentImage;
            HtmlHyperlink uIUcpReadySettingsHyperlink = this.UIEditCampaignWindowsIWindow32.UIEditCampaignDocument.UIUcpReadySettingsHyperlink;
            #endregion

            // Click 'Add Content' image
            Mouse.Click(uIAddContentImage, new Point(51, 25));

            // Click 'Ucp Ready Settings' link
            Mouse.Click(uIUcpReadySettingsHyperlink, new Point(66, 11));
        }

        private void UCP_SetName(string name)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentTitleEdit = this.UIEditCampaignWindowsIWindow33.UIDlgFramec28ed9f5a0e0Frame.UIHttpipm2avptestru_laDocument.UITxtContentTitleEdit;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow33.UIDlgFramec28ed9f5a0e0Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Type 'UCP SETTINGS' in 'txtContentTitle' text box
            uITxtContentTitleEdit.Text = name;

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(69, 18));
        }

        private void UCP_SetProps(ContentDefinitions.UCP ucp)
        {
            #region Variable Declarations
            HtmlComboBox uIConnectionTypeDropDoComboBox = this.UIEditCampaignWindowsIWindow34.UIEditUcpReadySettingsDocument.UIConnectionTypeDropDoComboBox;
            HtmlEdit uIUcpPortalUriTextBoxEdit = this.UIEditCampaignWindowsIWindow34.UIEditUcpReadySettingsDocument.UIUcpPortalUriTextBoxEdit;
            #endregion

            // Select 'Registration Always' in 'ConnectionTypeDropDown' combo box
            uIConnectionTypeDropDoComboBox.SelectedItem = ucp.UCPConnectionType;

            // Type 'http://internet.com' in 'UcpPortalUriTextBox' text box
            uIUcpPortalUriTextBoxEdit.Text = ucp.UCPRegistrationURL;
        }

        private void UCP_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditCampaignWindowsIWindow34.UIEditUcpReadySettingsDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(29, 20));
            Playback.Wait(3000);
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
        }
        #endregion

        #region CAMPAIGN

        /// <summary>
        /// Creates and sends to verification content group with desired settings
        /// </summary>
        /// <param name="testServerURL">URL of IPM frontend</param>
        /// <param name="campaign">Content group</param>
        /// <returns>ContentGroupId of created content group</returns>
        public string Campaign_CreateAndVerify(string testServerURL, ContentDefinitions.ContentGroup campaign)
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            StartIPM(testServerURL);
            Campaign_CreateNew();
            Campaign_SetProps(campaign);
            Campaign_SetFilters();
            Campaign_AddContent(campaign);
            Campaign_Save();
            Campaign_Verify();
            Playback.Wait(3000);
            return ExtractCampaignID();
        }

        public void Campaign_CreateWithoutSaving(string testServerURL, ContentDefinitions.ContentGroup campaign)
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            StartIPM(testServerURL);
            Campaign_CreateNew();
            Campaign_SetProps(campaign);
            Campaign_SetFilters();
            Campaign_AddContent(campaign);
            Playback.Wait(3000);
        }

        public void Campaign_ApproveAndPublish()
        {
            #region Variable Declarations
            HtmlDiv uIS4mainareaPane = this.UIEditApplicationButtoWindow.UIViewCampaignDocument1.UIS4mainareaPane;
            HtmlImage uIApproveImage = this.UIEditApplicationButtoWindow.UIViewCampaignDocument1.UIApprovePane.UIApproveImage;
            HtmlHyperlink uIApprovewithpublishinHyperlink = this.UIEditApplicationButtoWindow.UIViewCampaignDocument1.UIApprovewithpublishinHyperlink;
            HtmlInputButton uIOKButton = this.UIEditApplicationButtoWindow.UIViewCampaignDocument1.UIOKButton;
            #endregion

            // Click 's4-mainarea' pane
            Mouse.Click(uIS4mainareaPane, new Point(248, 408));

            // Type 'Control + {F5}' in 's4-mainarea' pane
            Keyboard.SendKeys(uIS4mainareaPane, "{F5}", ModifierKeys.Control);
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;

            // Click 'Approve' image
            Mouse.Click(uIApproveImage, new Point(49, 26));

            // Click 'Approve with publishing' link
            Mouse.Click(uIApprovewithpublishinHyperlink, new Point(65, 15));

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(23, 17));
        }

        private void Campaign_AddContent(ContentDefinitions.ContentGroup campaign)
        {
            if (campaign.ContentTypes.Length == campaign.Content.Length)
            {
                int count = 0;
                foreach (string type in campaign.ContentTypes)
                {
                    switch (type)
                    {
                        case "NEWS":
                            News_AddContentToCampaign((ContentDefinitions.News)campaign.Content[count++]);
                            break;

                        case "WP":
                            WP_AddContentToCampaign((ContentDefinitions.WelcomePage)campaign.Content[count++]);
                            break;

                        case "EOLP":
                            EOLP_AddContentToCampaign((ContentDefinitions.EndOfLicense)campaign.Content[count++]);
                            break;

                        case "RP":
                            RP_AddContentToCampaign((ContentDefinitions.RegretPage)campaign.Content[count++]);
                            break;

                        case "LN":
                            LN_AddContentToCampaign((ContentDefinitions.LicenseNotification)campaign.Content[count++]);
                            break;

                        case "LNS":
                            LNS_AddContentToCampaign((ContentDefinitions.LicenseNotificationSettings)campaign.Content[count++]);
                            break;

                        case "UCP":
                            UCP_AddContentToCampaign((ContentDefinitions.UCP)campaign.Content[count++]);
                            break;

                        case "AB":
                            AB_AddContentToCampaign((ContentDefinitions.AppButtons)campaign.Content[count++]);
                            break;

                        default:
                            count++;
                            break;

                    }
                }
            }
        }

        private void Campaign_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uIAllCampaignsHyperlink = this.UIBlankPageWindowsInteWindow2.UIHomeIPMDocument.UIZz16_V4QuickLaunchMePane.UIAllCampaignsHyperlink;
            HtmlHyperlink uINewHyperlink = this.UIBlankPageWindowsInteWindow3.UIViewCampaignsDocument.UINewHyperlink;
            #endregion

            // Click 'All Campaigns' link
            Mouse.Click(uIAllCampaignsHyperlink, new Point(61, 17));

            // Click 'New' link
            Mouse.Click(uINewHyperlink, new Point(25, 47));
            Playback.Wait(5000);
        }

        private void Campaign_SetProps(ContentDefinitions.ContentGroup campaign)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentGroupNameEdit = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UITxtContentGroupNameEdit;
            HtmlTextArea uITxtContentGroupCommeEdit = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UITxtContentGroupCommeEdit;
            HtmlEdit uITxtExpirationDateEdit = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UITxtExpirationDateEdit;
            HtmlDiv uICtl00_PlaceHolderMaiPane = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UICtl00_PlaceHolderMaiPane;
            HtmlEdit uITxtExpirationPeriodEdit = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UITxtExpirationPeriodEdit;
            HtmlEdit uIPublicationDateTextBEdit = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UIPublicationDateTextBEdit;
            HtmlInputButton uINextButton = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UINextButton;
            HtmlDiv uIExpirationPerioddaysPane = this.UIBlankPageWindowsInteWindow4.UIEditCampaignDocument.UICtl00_PlaceHolderMaiPane1.UIExpirationPerioddaysPane;
            #endregion

            // Type 'testCampaign_0000' in 'txtContentGroupName' text box
            if (campaign.Name != "")
            {
                uITxtContentGroupNameEdit.Text = campaign.Name;//this.CreateCampaignPropsFiltersParams.UITxtContentGroupNameEditText;
            }

            // Type 'testComment' in 'txtContentGroupComment' text box
            if (campaign.Comment != "")
            {
                uITxtContentGroupCommeEdit.Text = campaign.Comment;//this.CreateCampaignPropsFiltersParams.UITxtContentGroupCommeEditText;
            }

            // Type '10/30/2013' in 'txtExpirationDate' text box
            if (campaign.ExpDate != "")
            {
                uITxtExpirationDateEdit.Text = campaign.ExpDate;//this.CreateCampaignPropsFiltersParams.UITxtExpirationDateEditText;
                // Click 'Expiration Period days Incorrect' pane
                Mouse.Click(uIExpirationPerioddaysPane, new Point(484, 20));
            }

            // Type '123' in 'txtExpirationPeriod' text box
            if (campaign.ExpPeriod != "")
            {
                uITxtExpirationPeriodEdit.Text = campaign.ExpPeriod;//this.CreateCampaignPropsFiltersParams.UITxtExpirationPeriodEditText;
            }

            // Type '4/10/2013' in 'PublicationDateTextBox' text box
            if (campaign.PublicationDate != "")
            {
                uIPublicationDateTextBEdit.Text = campaign.PublicationDate;//this.CreateCampaignPropsFiltersParams.UIPublicationDateTextBEditText;
            }

            // Click 'Next' button
            Mouse.Click(uINextButton, new Point(27, 15));
            Playback.Wait(5000);
        }

        private void Campaign_SetFilters()
        {
            #region Variable Declarations
            HtmlImage uISearchImage = this.UIEditCampaignWindowsIWindow.UIEditCampaignDocument1.UIFilterRow.UISearchImage;
            HtmlHyperlink uIAddHyperlink = this.UIEditCampaignWindowsIWindow1.UIDlgFramee414703debabFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink;
            HtmlHyperlink uIAddHyperlink1 = this.UIEditCampaignWindowsIWindow1.UIDlgFramee414703debabFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink1;
            HtmlInputButton uIOKButton = this.UIEditCampaignWindowsIWindow1.UIDlgFramee414703debabFrame.UIHttpipm2avptestru_laDocument.UIOKButton;
            HtmlComboBox uITypeselectComboBox = this.UIEditCampaignWindowsIWindow.UIEditCampaignDocument1.UITypeselectComboBox;
            HtmlHyperlink uIAddHyperlink2 = this.UIEditCampaignWindowsIWindow.UIEditCampaignDocument1.UIAddHyperlink;
            HtmlImage uISearchImage1 = this.UIEditCampaignWindowsIWindow.UIEditCampaignDocument1.UIFilterRow1.UISearchImage;
            HtmlHyperlink uIAddHyperlink3 = this.UIEditCampaignWindowsIWindow2.UIDlgFrame6af24095e8faFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink;
            HtmlHyperlink uIAddHyperlink11 = this.UIEditCampaignWindowsIWindow2.UIDlgFrame6af24095e8faFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink1;
            HtmlHyperlink uIAddHyperlink21 = this.UIEditCampaignWindowsIWindow2.UIDlgFrame6af24095e8faFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink2;
            HtmlHyperlink uIAddHyperlink31 = this.UIEditCampaignWindowsIWindow2.UIDlgFrame6af24095e8faFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink3;
            HtmlInputButton uIOKButton1 = this.UIEditCampaignWindowsIWindow2.UIDlgFrame6af24095e8faFrame.UIHttpipm2avptestru_laDocument.UIOKButton;
            HtmlInputButton uINextButton1 = this.UIEditCampaignWindowsIWindow.UIEditCampaignDocument1.UINextButton;
            #endregion

            // Click 'search' image
            Mouse.Click(uISearchImage, new Point(36, 9));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink, new Point(16, 10));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink1, new Point(12, 9));

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(123, 17));

            // Select 'Application Localization' in 'type-select' combo box
            uITypeselectComboBox.SelectedItem = "Application Localization";//this.CreateCampaignPropsFiltersParams.UITypeselectComboBoxSelectedItem;

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink2, new Point(9, 13));

            // Click 'search' image
            Mouse.Click(uISearchImage1, new Point(35, 9));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink3, new Point(16, 11));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink11, new Point(16, 6));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink21, new Point(15, 11));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink31, new Point(15, 10));

            // Click 'OK' button
            Mouse.Click(uIOKButton1, new Point(117, 14));

            // Click 'Next' button
            Mouse.Click(uINextButton1, new Point(26, 18));
            Playback.Wait(5000);
        }

        private void Campaign_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton1 = this.UIEditCampaignWindowsIWindow3.UIEditCampaignDocument.UISaveButton;
            HtmlHyperlink uISaveHyperlink = this.UIEditCampaignWindowsIWindow35.UIEditCampaignDocument.UISaveHyperlink;
            #endregion

            // Click 'Save' link
            Mouse.Click(uISaveHyperlink, new Point(20, 38));
            // OLD
            //Mouse.Click(uISaveButton1, new Point(39, 17));
        }

        private void Campaign_Verify()
        {
            #region Variable Declarations
            HtmlHyperlink uIVerifyHyperlink = this.UIEditApplicationButtoWindow.UIViewCampaignDocument.UIVerifyHyperlink;
            HtmlInputButton uIOKButton = this.UIEditApplicationButtoWindow.UIViewCampaignDocument.UIOKButton;
            #endregion

            // Click 'Verify' link
            Mouse.Click(uIVerifyHyperlink, new Point(19, 43));

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(27, 13));
        }

        #endregion

        #region APPLICATION MESSAGE

        public string AppMsg_CreateAndVerify(string testServerURL, ContentDefinitions.ContentGroup appMsg)
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            StartIPM(testServerURL);
            AppMsg_NavigateToAllAppMessages();

            int i = 0;
            if (appMsg.ContentTypes.Length == appMsg.Content.Length)
                foreach (object content in appMsg.Content)
                {
                    AppMsg_AddContent(appMsg.ContentTypes[i]);
                    AppMsg_SetProps(appMsg);
                    AppMsg_SetFilters();
                    AppMsg_SetContentValues(appMsg.ContentTypes[i], appMsg.Content[i]);
                    i++;
                }
            //AppMsg_Save();
            AppMsg_Verify();
            Playback.Wait(3000);
            return ExtractCampaignID();
        }

        private void AppMsg_NavigateToAllAppMessages()
        {
            #region Variable Declarations
            HtmlHyperlink uIAllApplicationMessagHyperlink = this.UIBlankPageWindowsInteWindow2.UIHomeIPMDocument.UIZz16_V4QuickLaunchMePane.UIAllApplicationMessagHyperlink;
            #endregion

            // Click 'All Application Messages' link
            Mouse.Click(uIAllApplicationMessagHyperlink, new Point(67, 26));
        }

        private void AppMsg_AddContent(string appMsgType)
        {
            switch (appMsgType)
            {
                case "GAS":
                    GAS_CreateNew();
                    break;

                case "GPS":
                    GPS_CreateNew();
                    break;

                case "WSS":
                    WSS_CreateNew();
                    break;

                case "SS":
                    SS_CreateNew();
                    break;

                case "BRS":
                    BRS_CreateNew();
                    break;

                case "NVI":
                    NVI_CreateNew();
                    break;

                default:
                    break;
            }
        }

        private void AppMsg_SetProps(ContentDefinitions.ContentGroup appMsg)
        {
            #region Variable Declarations
            HtmlEdit uITxtContentGroupNameEdit = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UITxtContentGroupNameEdit;
            HtmlTextArea uITxtContentGroupCommeEdit = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UITxtContentGroupCommeEdit;
            HtmlEdit uITxtExpirationDateEdit = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UITxtExpirationDateEdit;
            HtmlEdit uITxtExpirationPeriodEdit = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UITxtExpirationPeriodEdit;
            HtmlEdit uIPublicationDateTextBEdit = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UIPublicationDateTextBEdit;
            HtmlInputButton uINextButton = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UINextButton;
            #endregion

            // Type 'googleAS' in 'txtContentGroupName' text box
            uITxtContentGroupNameEdit.Text = appMsg.Name;

            // Type 'gas_comment' in 'txtContentGroupComment' text box
            if (appMsg.Comment != "" && appMsg.Comment != null)
                uITxtContentGroupCommeEdit.Text = appMsg.Comment;

            // Type '' in 'txtExpirationDate' text box
            if (appMsg.ExpDate != "" && appMsg.ExpDate != null)
                uITxtExpirationDateEdit.Text = appMsg.ExpDate;

            // Type '123' in 'txtExpirationPeriod' text box
            if (appMsg.ExpPeriod != "" && appMsg.ExpPeriod != null)
                uITxtExpirationPeriodEdit.Text = appMsg.ExpPeriod;

            // Click 'PublicationDateTextBox' text box
            if (appMsg.PublicationDate != "" && appMsg.PublicationDate != null)
                uIPublicationDateTextBEdit.Text = appMsg.PublicationDate;

            // Click 'Next' button
            Mouse.Click(uINextButton, new Point(45, 19));
        }

        private void AppMsg_SetFilters()
        {
            #region Variable Declarations
            HtmlImage uISearchImage = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UIFilterRow.UISearchImage;
            HtmlHyperlink uIAddHyperlink = this.UIBlankPageWindowsInteWindow7.UIDlgFrame556c0ed543f5Frame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink;
            HtmlHyperlink uIAddHyperlink1 = this.UIBlankPageWindowsInteWindow7.UIDlgFrame556c0ed543f5Frame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink1;
            HtmlInputButton uIOKButton = this.UIBlankPageWindowsInteWindow7.UIDlgFrame556c0ed543f5Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            HtmlDiv uISelectApplicationIdPane = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UISelectApplicationIdPane;
            HtmlImage uISearchImage1 = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UIFilterRow1.UISearchImage;
            HtmlHyperlink uIAddHyperlink2 = this.UIBlankPageWindowsInteWindow8.UIDlgFrame102bbbd42f66Frame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink;
            HtmlHyperlink uIAddHyperlink11 = this.UIBlankPageWindowsInteWindow8.UIDlgFrame102bbbd42f66Frame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink1;
            HtmlHyperlink uIAddHyperlink21 = this.UIBlankPageWindowsInteWindow8.UIDlgFrame102bbbd42f66Frame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink2;
            HtmlHyperlink uIAddHyperlink3 = this.UIBlankPageWindowsInteWindow8.UIDlgFrame102bbbd42f66Frame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UIAddHyperlink3;
            HtmlInputButton uIOKButton1 = this.UIBlankPageWindowsInteWindow8.UIDlgFrame102bbbd42f66Frame.UIHttpipm2avptestru_laDocument.UIOKButton;
            HtmlDiv uISelectLocalizationPane = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UISelectLocalizationPane;
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UICtl00PlaceHolderMainCheckBox;
            HtmlInputButton uINextButton = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UINextButton;
            HtmlInputButton uISaveButton = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'search' image
            Mouse.Click(uISearchImage, new Point(29, 5));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink, new Point(8, 10));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink1, new Point(15, 5));

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(112, 11));

            // Click 'search' image
            Mouse.Click(uISearchImage1, new Point(40, 4));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink2, new Point(17, 9));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink11, new Point(19, 10));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink21, new Point(19, 8));

            // Click 'Add' link
            Mouse.Click(uIAddHyperlink3, new Point(18, 8));

            // Click 'OK' button
            Mouse.Click(uIOKButton1, new Point(120, 7));

            // Click 'Next' button
            Mouse.Click(uINextButton, new Point(35, 17));
        }

        private void AppMsg_SetContentValues(string appMsgType, object content)
        {
            switch (appMsgType)
            {
                case "GAS":
                    GAS_AddContentToAppMsg((ContentDefinitions.GoogleAnaliticsSettings)content);
                    break;

                case "GPS":
                    GPS_AddContentToAppMsg((ContentDefinitions.GooglePlaySettings)content);
                    break;

                case "WSS":
                    WSS_AddContentToAppMsg((ContentDefinitions.WebSurveySettings)content);
                    break;

                case "SS":
                    SS_AddContentToAppMsg((ContentDefinitions.SocializationSettings)content);
                    break;

                case "BRS":
                    BRS_AddContentToAppMsg((ContentDefinitions.BrowserRedirectionSettings)content);
                    break;

                case "NVI":
                    NVI_AddContentToCampaign((ContentDefinitions.NewVersionInfo)content);
                    break;

                default:
                    break;
            }
        }

        private void AppMsg_Verify()
        {
            #region Variable Declarations
            HtmlImage uIVerifyImage = this.UIEditApplicationMessaWindow1.UIViewApplicationMessaDocument.UIVerifyHyperlink.UIVerifyImage;
            HtmlInputButton uIOKButton = this.UIEditApplicationMessaWindow1.UIViewApplicationMessaDocument.UIOKButton;
            #endregion

            // Click 'Verify' image
            Mouse.Click(uIVerifyImage, new Point(17, 29));

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(31, 13));
        }

        #endregion

        #region GoogleAnaliticsSettings

        public void GAS_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uINewHyperlink1 = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UINewPane.UINewHyperlink1;
            HtmlHyperlink uIGoogleAnalyticsSettiHyperlink = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UIGoogleAnalyticsSettiHyperlink;
            #endregion

            // Click 'New' link
            Mouse.Click(uINewHyperlink1, new Point(28, 14));

            // Click 'Google Analytics Settings' link
            Mouse.Click(uIGoogleAnalyticsSettiHyperlink, new Point(43, 14));
        }

        public void GAS_AddContentToAppMsg(ContentDefinitions.GoogleAnaliticsSettings gas)
        {
            GAS_SetProps(gas);
            GAS_Save();
        }

        private void GAS_SetProps(ContentDefinitions.GoogleAnaliticsSettings gas)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UICtl00PlaceHolderMainCheckBox;
            #endregion


            // Select 'ctl00$PlaceHolderMain$ctl01$ctl05$SendApplicationS...' check box
            if (gas.SendApplicationStatistics == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;
        }

        private void GAS_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIBlankPageWindowsInteWindow6.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(41, 18));
        }

        #endregion

        #region Google Play Settings

        public void GPS_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uINewHyperlink1 = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UINewPane.UINewHyperlink1;
            HtmlHyperlink uIGooglePlaySettingsHyperlink = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UIGooglePlaySettingsHyperlink;
            #endregion

            // Click 'New' link
            Mouse.Click(uINewHyperlink1, new Point(27, 16));

            // Click 'Google Play Settings' link
            Mouse.Click(uIGooglePlaySettingsHyperlink, new Point(51, 12));
        }

        public void GPS_AddContentToAppMsg(ContentDefinitions.GooglePlaySettings gps)
        {
            GPS_SetProps(gps);
            GPS_Save();
        }

        private void GPS_SetProps(ContentDefinitions.GooglePlaySettings gps)
        {
            #region Variable Declarations
            HtmlEdit uIBuyGooglePlayProductEdit = this.UIViewApplicationMessaWindow1.UIEditApplicationMessaDocument.UIBuyGooglePlayProductEdit;
            HtmlEdit uIRenewGooglePlayProduEdit = this.UIViewApplicationMessaWindow1.UIEditApplicationMessaDocument.UIRenewGooglePlayProduEdit;
            #endregion

            // Type '12345' in 'BuyGooglePlayProductId' text box
            if (gps.BuyGPProductId != "" && gps.BuyGPProductId != null)
                uIBuyGooglePlayProductEdit.Text = gps.BuyGPProductId;

            // Type '67890' in 'RenewGooglePlayProductId' text box
            if (gps.RenewGPProductId != "" && gps.RenewGPProductId != null)
                uIRenewGooglePlayProduEdit.Text = gps.RenewGPProductId;
        }

        private void GPS_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIViewApplicationMessaWindow1.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(32, 16));
        }

        #endregion

        #region WebSurveySettings

        public void WSS_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uINewHyperlink = this.UIViewApplicationMessaWindow16.UIViewApplicationMessaDocument.UINewPane.UINewHyperlink;
            HtmlHyperlink uIWebSurveySettingsHyperlink = this.UIViewApplicationMessaWindow16.UIViewApplicationMessaDocument.UIWebSurveySettingsHyperlink;
            #endregion

            // Click 'New' link
            Mouse.Click(uINewHyperlink, new Point(28, 21));

            // Click 'Web Survey Settings' link
            Mouse.Click(uIWebSurveySettingsHyperlink, new Point(59, 16));
        }

        public void WSS_AddContentToAppMsg(ContentDefinitions.WebSurveySettings wss)
        {
            WSS_SetProps(wss);
            WSS_Save();
        }

        private void WSS_SetProps(ContentDefinitions.WebSurveySettings wss)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIEditApplicationMessaWindow.UIEditApplicationMessaDocument.UICtl00PlaceHolderMainCheckBox;
            HtmlEdit uISurveyUrlEdit = this.UIEditApplicationMessaWindow.UIEditApplicationMessaDocument.UISurveyUrlEdit;
            #endregion

            // Select 'ctl00$PlaceHolderMain$ctl01$ctl05$PerformAfterPurc...' check box
            if (wss.PerformAfterPurchaseSurvey == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;

            // Type 'http://survey.com' in 'SurveyUrl' text box
            if (wss.SurveyURL != "" && wss.SurveyURL != null)
                uISurveyUrlEdit.Text = wss.SurveyURL;
        }

        private void WSS_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIEditApplicationMessaWindow.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(39, 12));
        }

        #endregion

        #region Socialization Settings

        public void SS_CreateNew()
        {
            #region Variable Declarations
            HtmlImage uINewImage = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UINewPane.UINewImage;
            HtmlHyperlink uISocializationSettingHyperlink = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UISocializationSettingHyperlink;
            #endregion

            // Click 'New' image
            Mouse.Click(uINewImage, new Point(48, 26));

            // Click 'Socialization Settings' link
            Mouse.Click(uISocializationSettingHyperlink, new Point(57, 16));
        }

        public void SS_AddContentToAppMsg(ContentDefinitions.SocializationSettings ss)
        {
            SS_SetProps(ss);
            SS_Save();
        }

        private void SS_SetProps(ContentDefinitions.SocializationSettings ss)
        {
            #region Variable Declarations
            HtmlCheckBox uICtl00PlaceHolderMainCheckBox = this.UIViewApplicationMessaWindow7.UIEditApplicationMessaDocument.UICtl00PlaceHolderMainCheckBox;
            HtmlEdit uISocializationIdEdit = this.UIViewApplicationMessaWindow7.UIEditApplicationMessaDocument.UISocializationIdEdit;
            #endregion

            // Select 'ctl00$PlaceHolderMain$ctl01$ctl05$EnableSocializat...' check box
            if (ss.EnableSocialization == 1) uICtl00PlaceHolderMainCheckBox.Checked = true;
            else uICtl00PlaceHolderMainCheckBox.Checked = false;

            // Type '6F9619FF-8B86-D011-B42D-00CF4FC964FF' in 'SocializationId' text box
            if (ss.SocializationId != "" && ss.SocializationId != null)
                uISocializationIdEdit.Text = ss.SocializationId;
        }

        private void SS_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIViewApplicationMessaWindow7.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(37, 17));
        }

        #endregion

        #region Browser Redirection Settings

        public void BRS_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uINewHyperlink1 = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UINewPane.UINewHyperlink1;
            HtmlHyperlink uIBrowserRedirectionSeHyperlink = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UIBrowserRedirectionSeHyperlink;
            #endregion

            // Click 'New' link
            Mouse.Click(uINewHyperlink1, new Point(23, 16));

            // Click 'Browser Redirection Settings' link
            Mouse.Click(uIBrowserRedirectionSeHyperlink, new Point(51, 9));
        }

        public void BRS_AddContentToAppMsg(ContentDefinitions.BrowserRedirectionSettings brs)
        {
            BRS_SetProps(brs);
            BRS_Save();
        }

        private void BRS_SetProps(ContentDefinitions.BrowserRedirectionSettings brs)
        {
            #region Variable Declarations
            HtmlTextArea uIRedirectionServiceUrEdit = this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument.UIRedirectionServiceUrEdit;
            HtmlHyperlink uIAddValueHyperlink = this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument.UIAddValueHyperlink;
            HtmlEdit uIBrowserurlEdit = this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument.UIBrowserurlEdit;
            //HtmlEdit uIBrowserurlEdit1 = this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument.UIBrowserurlEdit1;
            //HtmlEdit uIBrowserurlEdit2 = this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument.UIBrowserurlEdit2;

            HtmlEdit browserURL = new HtmlEdit(this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument);
            browserURL.SearchProperties[HtmlEdit.PropertyNames.Id] = "browser-url";
            browserURL.SearchProperties[HtmlEdit.PropertyNames.Name] = null;
            browserURL.SearchProperties[HtmlEdit.PropertyNames.LabeledBy] = null;
            browserURL.SearchProperties[HtmlEdit.PropertyNames.Type] = "SINGLELINE";
            browserURL.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
            browserURL.FilterProperties[HtmlEdit.PropertyNames.Class] = null;
            browserURL.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "id=browser-url type=text size=50 maxLeng";
            browserURL.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "48";
            #endregion

            // Type 'http://redirector?helloworld.aspx?a=12&b=13' in 'RedirectionServiceUrl' text box
            if (brs.RedirectURL != "" && brs.RedirectURL != null)
                uIRedirectionServiceUrEdit.Text = brs.RedirectURL;

            int i = 1;
            foreach (string url in brs.BrowserURL)
            {
                // Click 'Add Value' link
                Mouse.Click(uIAddValueHyperlink, new Point(10, 8));

                browserURL.Find();
                // Type 'http://kaspersky.com' in 'browser-url' text box
                //uIBrowserurlEdit.Text = url;
                browserURL.Text = url;
                browserURL.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = (browserURL.TagInstance + i).ToString();
                i++;
            }

        }

        private void BRS_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIViewApplicationMessaWindow10.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(32, 23));
        }

        #endregion

        #region New Version Info

        public void NVI_CreateNew()
        {
            #region Variable Declarations
            HtmlHyperlink uINewHyperlink1 = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UINewPane.UINewHyperlink1;
            HtmlHyperlink uINewVersionInfoHyperlink = this.UIViewApplicationMessaWindow.UIViewApplicationMessaDocument.UINewVersionInfoHyperlink;
            #endregion

            // Click 'New' link
            Mouse.Click(uINewHyperlink1, new Point(26, 6));

            // Click 'New Version Info' link
            Mouse.Click(uINewVersionInfoHyperlink, new Point(51, 11));
        }

        public void NVI_AddContentToCampaign(ContentDefinitions.NewVersionInfo nvi)
        {
            NVI_SelectDistributionAppId();
            NVI_SetProps(nvi);
            NVI_Save();
        }

        private void NVI_SelectDistributionAppId()
        {
            #region Variable Declarations
            HtmlImage uISearchImage = this.UIEditApplicationMessaWindow2.UIEditApplicationMessaDocument.UIItemHyperlink.UISearchImage;
            HtmlHyperlink uISelectHyperlink = this.UIEditApplicationMessaWindow2.UIEditApplicationMessaDocument.UIDlgFrame0d608567200eFrame.UIHttpipm2avptestru_laDocument.UISearchresultsPane.UISelectHyperlink;
            HtmlInputButton uIOKButton = this.UIEditApplicationMessaWindow2.UIEditApplicationMessaDocument.UIDlgFrame0d608567200eFrame.UIHttpipm2avptestru_laDocument.UIOKButton;
            #endregion

            // Click 'search' image
            Mouse.Click(uISearchImage, new Point(25, 7));

            // Click 'Select' link
            Mouse.Click(uISelectHyperlink, new Point(24, 11));

            // Click 'OK' button
            Mouse.Click(uIOKButton, new Point(111, 12));
        }

        private void NVI_SetProps(ContentDefinitions.NewVersionInfo nvi)
        {
            #region Variable Declarations
            HtmlEdit uIApplicationVersionEdit = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIApplicationVersionEdit;
            HtmlEdit uIDistributiveDownloadEdit = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIDistributiveDownloadEdit;
            HtmlEdit uIAboutNewProductUriEdit = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIAboutNewProductUriEdit;
            HtmlComboBox uIDistributiveTypeComboBox = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIDistributiveTypeComboBox;
            HtmlEdit uIShowMessageTitleEdit = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIShowMessageTitleEdit;
            HtmlEdit uIShowMessageDaysCountEdit = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIShowMessageDaysCountEdit;
            HtmlEdit uIShowMessageDelayInDaEdit = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIShowMessageDelayInDaEdit;
            HtmlFileInput uICtl00PlaceHolderMainFileInput = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UICtl00PlaceHolderMainFileInput;
            HtmlInputButton uIUploadfileButton = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UIUploadfileButton;
            #endregion

            // Type '1.0.0.2354' in 'ApplicationVersion' text box
            if (nvi.DistributionAppVersion != "" && nvi.DistributionAppVersion != null)
                uIApplicationVersionEdit.Text = nvi.DistributionAppVersion;

            // Type 'http://distribution.download.url.com' in 'DistributiveDownloadUri' text box
            if (nvi.DistributionDownloadURL != "" && nvi.DistributionDownloadURL != null)
                uIDistributiveDownloadEdit.Text = nvi.DistributionDownloadURL;

            // Type 'http://about.new.product.com' in 'AboutNewProductUri' text box
            if (nvi.AboutNewProductDownloadURL != "" && nvi.AboutNewProductDownloadURL != null)
                uIAboutNewProductUriEdit.Text = nvi.AboutNewProductDownloadURL;

            // Select 'Webpack' in 'DistributiveType' combo box
            if (nvi.DistributionType != "" && nvi.DistributionType != null)
                uIDistributiveTypeComboBox.SelectedItem = nvi.DistributionType;

            // Type 'messageTitle' in 'ShowMessageTitle' text box
            if (nvi.MessageTitle != "" && nvi.MessageTitle != null)
                uIShowMessageTitleEdit.Text = nvi.MessageTitle;

            // Type '15' in 'ShowMessageDaysCount' text box
            if (nvi.MessagePeriodInDays != "" && nvi.MessagePeriodInDays != null)
                uIShowMessageDaysCountEdit.Text = nvi.MessagePeriodInDays;

            // Type '1' in 'ShowMessageDelayInDays' text box
            if (nvi.MessageDelayInDays != "" && nvi.MessageDelayInDays != null)
                uIShowMessageDelayInDaEdit.Text = nvi.MessageDelayInDays;

            foreach (string filePath in nvi.Content)
            {
                uICtl00PlaceHolderMainFileInput.FileName = filePath;
                // Click 'Upload file' button
                Mouse.Click(uIUploadfileButton, new Point(55, 15));
            }
        }

        private void NVI_Save()
        {
            #region Variable Declarations
            HtmlInputButton uISaveButton = this.UIViewApplicationMessaWindow.UIEditApplicationMessaDocument.UISaveButton;
            #endregion

            // Click 'Save' button
            Mouse.Click(uISaveButton, new Point(31, 14));
        }

        #endregion

    }
    
}
