namespace IPMATFramework.TestSettings
{
    using System.IO;

    public static class TestSettings
    {
        /// <summary>
        /// Server tests will be run on
        /// </summary>
         public static string TestServer = @"http://ipm2.avp-test.ru/default.aspx";

        /// <summary>
        /// Path to verification published content
        /// </summary>
        public static string VerContentStoragePath = @"\\ss-services.avp-test.ru\c$\ipm\verification\verification_content\";

        /// <summary>
        /// Path to published content
        /// </summary>
        public static string PubContentStoragePath = @"\\ss-services.avp-test.ru\c$\ipm\publishing\publishing_content\";

        /// <summary>
        /// Path to verification_metadata.xml
        /// </summary>
        public static string VerMetadataXMLPath = @"\\ss-services.avp-test.ru\c$\ipm\verification\verification_metadata.xml";

        /// <summary>
        /// Path to publishing_metadata.xml
        /// </summary>
        public static string PubMetadataXMLPath = @"\\ss-services.avp-test.ru\c$\ipm\publishing\publishing_metadata.xml";

        /// <summary>
        /// Path to folder where all reference files are stored
        /// </summary>
        public static string ReferenceContentStoragePath = @"ReferenceStorage\";//@"C:\Content\ReferenceStorage\";

        /// <summary>
        /// Folder path content group files to be unzipped to
        /// </summary>
        public static string UnzippedContentDefaultStoragePath = @"Unzipped\";//@"C:\Content\Test\";

        /// <summary>
        /// Relative paths to files used for tests with Schedule trait
        /// </summary>
        private static string[] ScheduleContentFiles = new string[]
        {
            @"Schedule\news.html", 
            @"Schedule\iso_logo.png",
            @"Schedule\second.html",
            @"Schedule\iso_bcg.jpg"
        };

        public static int GetScheduleContentFilesLength()
        {
            return ScheduleContentFiles.Length;
        }

        /// <summary>
        /// Array of absolute paths to files used for tests with Schedule trait
        /// </summary>
        /// <returns></returns>
        public static string[] GetScheduleContentFiles()
        {
            string[] files = new string[ScheduleContentFiles.Length];
            for (int i = 0; i < ScheduleContentFiles.Length; i++)
            {
                files[i] = Path.GetFullPath(ScheduleContentFiles[i]);
            }
            return files;
        }
    }
}