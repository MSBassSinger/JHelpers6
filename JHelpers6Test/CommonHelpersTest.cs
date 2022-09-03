using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jeff.Jones.JHelpers6;
using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace JHelpers6Test
{
    [TestClass]
    public class CommonHelpersTest
    {

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AddCheckTest()
        {
            ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException("Param1 is too large.");
            ex.Data.AddCheck("Param1", DateTime.UtcNow);
            ex.Data.AddCheck("Param1", "X35");
            ex.Data.AddCheck("Param1", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            Assert.IsTrue(ex.Data.Contains("Param1-1"));
            Assert.IsTrue(ex.Data.Contains("Param1-2"));

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AmIRunningInTheIDETest()
        {

            Boolean retVal = CommonHelpers.AmIRunningInTheIDE;

            Assert.IsFalse(retVal, "Failed to detect running in the IDE.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AscTest()
        {

            Int32 retVal = CommonHelpers.Asc("A");

            Assert.IsTrue(retVal == 65, "Failed to get the ASCII value of [A].");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AssemblyCompanyTest()
        {

            String retVal = CommonHelpers.AssemblyCompany;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the AssemblyCompany value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AssemblyCopyrightTest()
        {

            String retVal = CommonHelpers.AssemblyCopyright;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the AssemblyCopyright value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AssemblyDescriptionTest()
        {

            String retVal = CommonHelpers.AssemblyDescription;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the AssemblyDescription value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AssemblyProductTest()
        {

            String retVal = CommonHelpers.AssemblyProduct;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the AssemblyProduct value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AssemblyTitleTest()
        {

            String retVal = CommonHelpers.AssemblyTitle;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the AssemblyTitle value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AssemblyVersionTest()
        {

            String retVal = CommonHelpers.AssemblyVersion;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the AssemblyVersion value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void AvailableRAMinMBTest()
        {

            Int32 retVal = CommonHelpers.AvailableRAMinMB();

            Assert.IsTrue(retVal > 1000, "Failed to get the AvailableRAMinMB value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void StringToBase64Test()
        {

            String retVal = CommonHelpers.StringToBase64("abc");

            Assert.IsTrue(retVal == "YWJj", "Failed to get the Base64 value from a String value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Base64ToStringTest()
        {

            String retVal = CommonHelpers.Base64ToString("YWJj");

            Assert.IsTrue(retVal == "abc", "Failed to get the Base64 value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void CelciusToFahrenheitTest()
        {

            Double retVal = CommonHelpers.CelsiusToFahrenheit(0.0d);

            Assert.IsTrue(retVal == 32.0d, "Failed to get the CelciusToFahrenheit value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void CurDirTest()
        {

            String retVal = CommonHelpers.CurDir;

            Assert.IsTrue(retVal.Length > 0, "Failed to get the CurDir value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ContainsHowManyTest()
        {
            String test = "A little peanut brittle.";
            Int32 retVal = test.ContainsHowMany("tt", true);
            Assert.IsTrue(retVal == 2, "Failed the ignore case parameter.");
            Int32 retVal2 = test.ContainsHowMany("TT", false);
            Assert.IsTrue(retVal2 == 0, "Failed the case-sensitive parameter.");
        }

 

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ConvertFeetToMetersTest()
        {
            Decimal test = 0.9144M;
            Decimal retVal = CommonHelpers.ConvertFeetToMeters(3);
            Assert.IsTrue(retVal == test, "Failed to convert feet to meters.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ConvertGallonsToLitersTest()
        {
            Decimal test = 37.85411780M;
            Decimal retVal = CommonHelpers.ConvertGallonsToLiters(10);
            Assert.IsTrue(retVal == test, "Failed to convert gallons to liters.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ConvertKilometersToMilesTest()
        {
            Decimal test = 6.21371190M;
            Decimal retVal = CommonHelpers.ConvertKilometersToMiles(10);
            Assert.IsTrue(retVal == test, "Failed to convert kilometers to miles.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ConvertLitersToGallonsTest()
        {
            Decimal test = 2.64172050M;
            Decimal retVal = CommonHelpers.ConvertLitersToGallons(10);
            Assert.IsTrue(retVal == test, "Failed to convert liters to gallons.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ConvertMetersToFeetTest()
        {
            Decimal test = 32.8083990M;
            Decimal retVal = CommonHelpers.ConvertMetersToFeet(10);
            Assert.IsTrue(retVal == test, "Failed to convert meters to feet.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ConvertMilesToKilometersTest()
        {
            Decimal test = 16.093440M;
            Decimal retVal = CommonHelpers.ConvertMilesToKilometers(10);
            Assert.IsTrue(retVal == test, "Failed to convert miles to kilometers.");
        }





        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void CurrentTimeZoneDaylightNameTest()
        {

            String retVal = CommonHelpers.CurrentTimeZoneDaylightName;

            Assert.IsTrue(retVal == "Eastern Daylight Time", "Failed to get the CurrentTimeZoneDaylightName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void CurrentTimeZoneNameTest()
        {

            String retVal = CommonHelpers.CurrentTimeZoneName;

            Assert.IsTrue(retVal == "Eastern Standard Time", "Failed to get the CurrentTimeZoneName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void DegreesToRadiansTest()
        {

            Double retVal = CommonHelpers.DegreesToRadians(90d);

            Assert.IsTrue(retVal == 1.5707963267948966d, "Failed to convert degrees to radians.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void FahrenheitToCelsiusTest()
        {

            Double retVal = CommonHelpers.FahrenheitToCelsius(32.0d);

            Assert.IsTrue(retVal == 0.0d, "Failed to get the FahrenheitToCelsius value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void FullComputerNameTest()
        {

            String retVal = CommonHelpers.FullComputerName;

            Assert.IsTrue(retVal.Contains(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase), "Failed to get the FullComputerName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetComputerDomainNameTest()
        {

            String retVal = CommonHelpers.GetComputerDomainName();

            Assert.IsTrue(retVal.Length > 2, "Failed to get the GetComputerDomainName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetCurrentCPUUsageTest()
        {

            Int32 retVal = CommonHelpers.GetCurrentCPUUsage();

            Assert.IsTrue(retVal > 0, "Failed to get the GetCurrentCPUUsage value.");

            retVal = CommonHelpers.GetCurrentCPUUsage();

            Assert.IsTrue(retVal > 0, "Failed to get the GetCurrentCPUUsage value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetDNSNameTest()
        {

            String retVal = CommonHelpers.GetDNSName();

            Assert.IsTrue(retVal.Contains(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase), "Failed to get the GetDNSName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetDrivesTest()
        {

            List<System.IO.DriveInfo> retVal = CommonHelpers.GetDrives();

            Assert.IsTrue(retVal.Count > 0, "Failed to get the list of computer drives.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetFullComputerDomainNameTest()
        {

            String retVal = CommonHelpers.GetFullComputerDomainName();

            Assert.IsTrue(retVal == "JJONES-DEV", "Failed to get the GetFullComputerDomainName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetFullDateTimeStampForFileNameTest()
        {

            String retVal = CommonHelpers.GetFullDateTimeStampForFileName(DateTime.Now);

            Assert.IsTrue(retVal.Length > 0, "Failed to get the GetFullDateTimeStampForFileName value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetFullExceptionMessageTest()
        {
            Exception innerException = new Exception("Inner exception message");

            innerException.Data.Add("Inner Data 1", DateTime.Now);
            innerException.Data.Add("Inner Data 2", "This is now");

            Exception ex2Examine = new Exception("Topmost exception message", innerException);

            ex2Examine.Data.Add("Outer Data 1", 1234);
            ex2Examine.Data.Add("Outer Data 2", "Simple PIN number");

            try
            {
                throw (ex2Examine);
            }
            catch (Exception exUnhandled)
            {
                String retVal = CommonHelpers.GetFullExceptionMessage(exUnhandled, true, true);

                Assert.IsTrue(retVal.Length > 0, "Failed to get the GetFullExceptionMessage value.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetExceptionInfoTest()
        {
            Exception innerException = new Exception("Inner exception message");

            innerException.Data.Add("Inner Data 1", Convert.ToDateTime(@"4/17/2020 8:26:09 PM"));
            innerException.Data.Add("Inner Data 2", "This is now");

            Exception ex2Examine = new Exception("Topmost exception message", innerException);

            ex2Examine.Data.Add("Outer Data 1", 1234);
            ex2Examine.Data.Add("Outer Data 2", "Simple PIN number");

            String logMessage = "";
            String exceptionData = "";
            String stackTraceDescrs = "";
            String previousModule = "";
            String previousMethod = "";
            Int32 previousLineNumber = 0;
            Int32 threadID = 0;

            try
            {
                throw (ex2Examine);
            }
            catch (Exception exUnhandled)
            {
                Boolean retVal = exUnhandled.GetExceptionInfo(out logMessage, out exceptionData, out stackTraceDescrs, out previousModule, out previousMethod, out previousLineNumber, out threadID);
                Assert.IsTrue(logMessage == "Exception=[Topmost exception message]; Source=[JHelpers6Test]::Exception=[Inner exception message]", "Failed to match messages.");
                Assert.IsTrue(exceptionData == "Exception Data=[{Outer Data 1}={1234}|{Outer Data 2}={Simple PIN number}]::Exception Data=[{Inner Data 1}={4/17/2020 8:26:09 PM}|{Inner Data 2}={This is now}]", "Failed to match the Data collections.");
                Assert.IsTrue(stackTraceDescrs == @"[at JHelpers6Test.CommonHelpersTest.GetExceptionInfoTest() in C:\Projects\JHelpers6\JHelpers6Test\CommonHelpersTest.cs:line 465]", "Failed to gt stack trace descriptions");
                Assert.IsTrue(previousModule == "CommonHelpersTest", "Failed to match previous module name.");
                Assert.IsTrue(previousMethod == "Void GetExceptionInfoTest()", "Failed to match previous method name.");
                Assert.IsTrue(previousLineNumber == 465, "Failed to match previous line number.");
                Assert.IsTrue(threadID > 0, "Failed to get valid ThreadID.");
                Assert.IsTrue(retVal, "Failed to get the GetExceptionInfo value.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetIntBitValueTest()
        {

            Boolean retVal = CommonHelpers.GetIntBitValue(7, 2);

            Assert.IsTrue(retVal, "Failed to get the GetIntBitValue value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetLinearDistanceTest()
        {
            Double lat1 = 33.92527;
            Double long1 = -84.84157;
            Double alt1 = 1043;
            Double lat2 = 28.49258;
            Double long2 = -82.47222;
            Double alt2 = 10;

            Double retVal = CommonHelpers.GetLinearDistance(lat1, long1, lat2, long2, DistanceUnitsOfMeasureEnum.Miles);

            Assert.IsTrue(retVal > 0, "Failed to get the GetLinearDistance value without altitude.");

            retVal = CommonHelpers.GetLinearDistance(lat1, long1, alt1, lat2, long2, alt2, DistanceUnitsOfMeasureEnum.Miles);

            Assert.IsTrue(retVal > 0, "Failed to get the GetLinearDistance value with altitude.");

            retVal = CommonHelpers.GetLinearDistance(lat1, long1, alt1, lat2, long2, alt2, false);

            Assert.IsTrue(retVal > 0, "Failed to get the GetLinearDistance value with altitude and not miles.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetLinearDistancesTest()
        {
            Double lat1 = 33.92527;
            Double long1 = -84.84157;
            Double alt1 = 1043;
            Double lat2 = 34.0754;
            Double long2 = -84.2941;
            Double alt2 = 1132;
            Double lat3 = 28.49258;
            Double long3 = -82.47222;
            Double alt3 = 10;

            List<AddressGeoData> list = new List<AddressGeoData>();
            list.Add(new AddressGeoData(lat1, long1, alt1, lat3, long3, alt3, DistanceUnitsOfMeasureEnum.Miles));
            list.Add(new AddressGeoData(lat2, long2, alt2, lat3, long3, alt3, DistanceUnitsOfMeasureEnum.Miles));


            CommonHelpers.GetLinearDistances(ref list);

            Assert.IsTrue(list[0].LinearDistance > 0, "Failed to get the GetLinearDistances value.");
            Assert.IsTrue(list[1].LinearDistance > 0, "Failed to get the GetLinearDistances value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetLocalPrintersTest()
        {

            List<ManagementObject> retVal = CommonHelpers.GetLocalPrinters();

            Assert.IsTrue(retVal != null, "Failed to get the list of local printers.");
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetMinPasswordLengthTest()
        {

            Int32 retVal = CommonHelpers.GetMinPasswordLength();

            if (CommonHelpers.IsInDomain())
            {
                Assert.IsTrue(retVal >= 0, "Failed to get the GetMinPasswordLength value.");
            }
            else
            {
                Assert.IsTrue(retVal == -1, "Failed to get the GetMinPasswordLength value.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetNetworkPrintersTest()
        {

            List<ManagementObject> retVal = CommonHelpers.GetNetworkPrinters();

            if (CommonHelpers.IsInDomain())
            {
                Assert.IsTrue(retVal?.Count >= 0, "Failed to get the GetNetworkPrinters value.");
            }
            else
            {
                Assert.IsTrue(retVal?.Count == 0, "Failed to get the GetNetworkPrinters value.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetStackInfo()
        {

            String retVal = CommonHelpers.GetStackInfo();

            Assert.IsTrue(retVal.Length > 0, "Failed to get the GetStackInfo value.");

            Exception exTest = new Exception("Test");

            try
            {
                throw exTest;
            }
            catch (Exception exUnhandled)
            {
                retVal = CommonHelpers.GetStackInfo(exUnhandled);

                Assert.IsTrue(retVal.Length > 0, "Failed to get the GetStackInfo value.");

            }
        }

        /// <summary>
        /// Tests adding up the total free drive space 
        /// TODO: Change the drive letter if needed.
        /// </summary>
        [TestMethod]
        [DataRow("C")]
        public void GetTotalHDDFreeSpaceTest(String drive)
        {

            Int32 retVal = CommonHelpers.GetTotalHDDFreeSpace(drive);

            Assert.IsTrue(retVal > 0, $"Failed to get the GetTotalHDDFreeSpace value from [{drive}].");
        }

        /// <summary>
        /// Tests getting the total size of the specified drive.
        /// TODO: Change the drive letter if needed.
        /// </summary>
        [TestMethod]
        [DataRow("C:")]
        public void GetTotalHDDSizeTest(String drive)
        {

            Int32 retVal = CommonHelpers.GetTotalHDDSize(drive);

            Assert.IsTrue(retVal > 0, "Failed to get the GetTotalHDDSize value.");
        }



        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void HexTest()
        {

            String retVal = CommonHelpers.Hex(1024);

            Assert.IsTrue(retVal == "400", "Failed to get the Hex value.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void IsDaylightSavingsTimeTest()
        {

            Boolean retVal = CommonHelpers.IsDaylightSavingsTime(new DateTime(2019, 6, 12));

            Assert.IsTrue(retVal, "Failed to get the IsDaylightSavingsTime value with 6/12/2019.");

            retVal = CommonHelpers.IsDaylightSavingsTime(new DateTime(2019, 12, 12));

            Assert.IsFalse(retVal, "Failed to get the IsDaylightSavingsTime value with 12/12/2019.");

            retVal = CommonHelpers.IsDaylightSavingsTime();

            Boolean test = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);

            Assert.IsTrue(retVal == test, "Failed to get the IsDaylightSavingsTime value for today.");

        }

        /// <summary>
        /// Tests checking a file to see if it is a text file.
        /// TODO: Change the fileName in DataRow[()] to match where the project doe is located on your machine.
        /// </summary>
        [TestMethod]
        [DataRow(@"C:\Projects\JHelpers6\JHelpers6\License.txt")]
        public void IsFileTextTest(String fileName)
        {
            Encoding encode = Encoding.ASCII;

            Boolean retVal = CommonHelpers.IsFileText(out encode, fileName, 80);

            Assert.IsTrue(retVal, "Failed to get the IsFileText value.");

        }

        /// <summary>
        /// Tests to see if the computer is in a domain.
        /// TODO: Change the expected result in DataRow[()] to match whether your computer is in a domain or not.
        /// </summary>
        [TestMethod]
        [DataRow(false)]
        public void IsInDomainTest(Boolean expectedResult)
        {
            Boolean retVal = CommonHelpers.IsInDomain();

            Assert.IsTrue(retVal == expectedResult, "Failed to get the IsInDomain value.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void IsIPAddressTest()
        {
            IPAddress ip = null;

            Boolean retVal = CommonHelpers.IsIPAddress("192.168.0.12", out ip);

            Assert.IsTrue(retVal, "Failed to get the IsIPAddress value.");

            ip = null;

            retVal = CommonHelpers.IsIPAddress("fe80::725a:9eff:fee1:9534%27", out ip);

            Assert.IsTrue(retVal, "Failed to get the IsIPAddress value.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void PingTest()
        {
 
            PingReply retVal = CommonHelpers.Ping("www.bing.com", 1000);

            Assert.IsTrue(retVal.Status == IPStatus.Success, "Failed to get the Ping value.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void RadianToDegreeTest()
        {
            Double coordinate = 1.0d;

            Double retVal = CommonHelpers.RadianToDegree(coordinate);

            Assert.IsTrue(retVal == 57.295779513082323d, "Failed to get the RadianToDegree value.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void SetIntBitValueTest()
        {
            Int32 value = 16;

            CommonHelpers.SetIntBitValue(ref value, 1, true);

            Assert.IsTrue(value == 17, "Failed to get the SetIntBitValue value.");

        }

        /// <summary>
        /// Tests the writing of a message to a formatted log file.
        /// TODO: Change the path and file name to what you want to use.
        /// </summary>
        [TestMethod]
        [DataRow(@"C:\Projects\JHelpers6\JHelpers6\bin\Debug\net6.0\Output.txt")]
        public void WriteToLogTest(String fileName)
        {

            String mainMessage = "Initial message for a write to file";
            String secondMessage = "Secondary message in the file write";
            Boolean retVal = CommonHelpers.WriteToLog(fileName, mainMessage, secondMessage);

            Assert.IsTrue(retVal, "Failed to get the WriteToLog value.");

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ContextMgrTest()
        {
            Boolean retVal = false;

            ContextMgr.Instance.ContextValues.Add("Computer Name", Environment.MachineName);

            ContextMgr.Instance.ContextValues.Add("Startup Time", DateTime.Now);

			IPAddress[] ips = Dns.GetHostAddresses("JJONES-DEV");
            ContextMgr.Instance.ContextValues.Add("IP Addresses", ips);

			dynamic machineName = "";
            retVal = ContextMgr.Instance.ContextValues.TryGetValue("Computer Name", out machineName);
            Assert.IsTrue(retVal, "Failed to obtain computer name from the ContextMgr.");

            dynamic startupTime = "";
            retVal = ContextMgr.Instance.ContextValues.TryGetValue("Startup Time", out startupTime);
            Assert.IsTrue(retVal, "Failed to obtain computer name from the ContextMgr.");

            dynamic hostAddresses = null;
            retVal = ContextMgr.Instance.ContextValues.TryGetValue("IP Addresses", out hostAddresses);
            Assert.IsTrue(retVal, "Failed to obtain computer name from the ContextMgr.");

            ContextMgr.Instance.Dispose();

        }

        [TestMethod]
        public void ContainsTest()
        {
            Boolean result = false;

            String testString = "My dog has fleas.";

            result = testString.Contains("G h", StringComparison.CurrentCultureIgnoreCase);

            Assert.IsTrue(result, "Failed case insensitive Contains method.");

            testString = "My dawg has fleas.";

            result = testString.Contains("G h", StringComparison.CurrentCulture);

            Assert.IsFalse(result, "Failed case sensitive Contains method.");

            String test = "A little brown fox.";
            Boolean retVal = test.Contains("Little", StringComparison.CurrentCultureIgnoreCase);
            Assert.IsTrue(retVal, "Failed the ignore case parameter.");
            Boolean retVal2 = test.Contains("Little", StringComparison.CurrentCulture);
            Assert.IsFalse(retVal2, "Failed the case-sensitive parameter.");


        }  // ENDF public void ContainsTestMethod()


        [TestMethod]
        public void ConvertToBooleanTest()
        {
            Boolean result = false;
            Boolean isBoolean = false;

            String testString = "on";

            result = testString.ConvertToBoolean(out isBoolean);

            Assert.IsTrue(result, "Result failed ConvertToBoolean method with 'on'.");
            Assert.IsTrue(isBoolean, "Flag failed ConvertToBoolean method 'on'.");

            testString = "butter";
            result = testString.ConvertToBoolean(out isBoolean);

            Assert.IsFalse(result, "Result failed ConvertToBoolean method with 'butter'.");
            Assert.IsFalse(isBoolean, "Flag failed ConvertToBoolean method with 'butter'.");

        }  // ENDIF public void ConvertToBooleanTestMethod()


        [TestMethod]
        public void GetDateTimeTestMethod()
        {
            DateTime result = DateTime.MinValue;
            DateTime defaultValue = DateTime.MinValue;

            String testString = "11/18/1954";

            result = testString.GetDateTime(defaultValue);

            Assert.IsTrue(result != defaultValue, $"Result failed GetDateTime method with '{testString}'.");

            testString = "02/29/2001";

            result = testString.GetDateTime(defaultValue);

            Assert.IsTrue(result == defaultValue, $"Result failed to return default value in GetDateTime method with '{testString}'.");


        }  // ENDIF public void GetDateTimeTestMethod()


        [TestMethod]
        public void GetDecimalTestMethod()
        {
            Decimal result = 0m;
            Decimal defaultValue = Decimal.MinValue;

            String testString = "113.435";

            result = testString.GetDecimal(defaultValue);

            Assert.IsTrue(result != defaultValue, $"Result failed GetDecimal method with '{testString}'.");

            testString = "$113.435";

            result = testString.GetDecimal(defaultValue);

            Assert.IsTrue(result == defaultValue, $"Result failed to return default value in GetDecimal method with '{testString}'.");

        }  // ENDIF public void GetDecimalTestMethod()

        [TestMethod]
        public void GetFullExceptionMessageTestMethod()
        {
            Exception exTest1 = null;

            String result1 = "";
            String result2 = "";
            String result3 = "";
            String result4 = "";

            try
            {
                exTest1 = new InvalidOperationException("test exception");
                exTest1.Data.Add("Item1", 14);
                exTest1.Data.Add("Item2", DateTime.Now);
                exTest1.Data.Add("Throw on line", 115);
                throw exTest1;
            }
            catch (Exception exUnhandled)
            {
                result1 = exUnhandled.GetFullExceptionMessage(false, false);
                Assert.IsTrue(!result1.Contains("Data=[") && !result1.Contains("Stack Trace=["), "Failed excluding data and stack");

                result2 = exUnhandled.GetFullExceptionMessage(true, false);
                Assert.IsTrue(result2.Contains("Data=[") && !result2.Contains("Stack Trace=["), "Failed excluding stack");

                result3 = exUnhandled.GetFullExceptionMessage(false, true);
                Assert.IsTrue(result3.Contains("Stack Trace=[") && !result3.Contains("Data=["), "Failed excluding data");

                result4 = exUnhandled.GetFullExceptionMessage(true, true);
                Assert.IsTrue(result4.Contains("Data=[") && result4.Contains("Stack Trace=["), "Failed including data and stack");

            }

        }  // ENDIF public void GetFullExceptionMessageTestMethod()

        [TestMethod]
        public void GetInt32Test()
        {
            Int32 result = 0;
            Int32 defaultValue = Int32.MinValue;

            String testString = "113";

            result = testString.GetInt32(defaultValue);

            Assert.IsTrue(result != defaultValue, $"Result failed GetInt32 method with '{testString}'.");

            testString = "#113";

            result = testString.GetInt32(defaultValue);

            Assert.IsTrue(result == defaultValue, $"Result failed to return default value in GetInt32 method with '{testString}'.");

        }  // ENDIF public void GetInt32TestMethod()

        [TestMethod]
        public void GetInt64Test()
        {
            Int64 result = 0;
            Int64 defaultValue = Int64.MinValue;

            String testString = (Int64.MaxValue - 10000).ToString();

            result = testString.GetInt64(defaultValue);

            Assert.IsTrue(result != defaultValue, $"Result failed GetInt64 method with '{testString}'.");

            testString = "#" + (Int64.MaxValue - 10000).ToString();

            result = testString.GetInt64(defaultValue);

            Assert.IsTrue(result == defaultValue, $"Result failed to return default value in GetInt64 method with '{testString}'.");


        }  // ENDIF public void GetInt64TestMethod()

        [TestMethod]
        public void GetOnlyDigitsTestd()
        {
            String result = "";

            String testString = "Two dogs 4 you; 183 mutts in total.";

            result = testString.GetOnlyDigits(false);

            Assert.IsTrue(result == "4183", $"Result failed GetOnlyDigits method with '{testString}'.");

            testString = "#413.1.2";

            result = testString.GetOnlyDigits(true);

            Assert.IsTrue(result == "413.12", $"Result failed to return correct value in GetOnlyDigits method with '{testString}'.");

        }  // ENDIF public void GetOnlyDigitsTestMethod()

        [TestMethod]
        public void GetOnlyLettersTest()
        {
            String result = "";

            String testString = "Two dogs 4 you; 183 mutts in total.";

            result = testString.GetOnlyLetters();

            Assert.IsTrue(result == "Twodogsyoumuttsintotal", $"Result failed GetOnlyLetters method with '{testString}'.");

            testString = "1234";

            result = testString.GetOnlyLetters();

            Assert.IsTrue(result == "", $"Result failed to return empty value in GetOnlyLetters method with '{testString}'.");

        }  // ENDIF public void GetOnlyLettersTestMethod()

        [TestMethod]
        public void GetOnlyLettersAndDigitsTest()
        {
            String result = "";

            String testString = "Two dogs 4 you; 183 mutts in total.";

            result = testString.GetOnlyLettersAndDigits();

            Assert.IsTrue(result == "Twodogs4you183muttsintotal", $"Result failed GetOnlyLettersAndDigits method with '{testString}'.");

            testString = "!@#$%^&*(){}[]:\";'<>,.??|\\ ";

            result = testString.GetOnlyLettersAndDigits();

            Assert.IsTrue(result == "", $"Result failed to return empty value in GetOnlyLettersAndDigits method with '{testString}'.");

        }  // ENDIF public void GetOnlyLettersTestMethod()

        [TestMethod]
        public void IsBooleanTest()
        {
            Boolean result = false;

            String testString = "On";

            result = testString.IsBoolean();

            Assert.IsTrue(result, $"Result failed IsBoolean method with '{testString}'.");

            testString = "Booger";

            result = testString.IsBoolean();

            Assert.IsTrue(!result, $"Result failed to return empty value in IsBoolean method with '{testString}'.");

        }  // ENDIF public void IsBooleanTestMethod()

        [TestMethod]
        public void IsEmailFormatTest()
        {
            Boolean result = false;

            String testString = "Some.Body@somemail.com";

            result = testString.IsEmailFormat();

            Assert.IsTrue(result, $"Result failed IsEmailFormat method with '{testString}'.");

            testString = "#Booger";

            result = testString.IsEmailFormat();

            Assert.IsTrue(!result, $"Result failed to return false value in IsEmailFormat method with '{testString}'.");

        }  // ENDIF public void IsEmailFormatTestMethod()


        [TestMethod]
        public void IsOnlyDigitsTest()
        {
            Boolean result = false;

            String testString = "12346";

            result = testString.IsOnlyDigits();

            Assert.IsTrue(result, $"Result failed IsOnlyDigits method with '{testString}'.");

            testString = "2321.12";

            result = testString.IsOnlyDigits();

            Assert.IsTrue(!result, $"Result failed to return false value in IsOnlyDigits method with '{testString}'.");

            result = testString.IsOnlyDigits(true);

            Assert.IsTrue(result, $"Result failed IsOnlyDigits method including period with '{testString}'.");

            testString = "2.321.12";

            result = testString.IsOnlyDigits(true);

            Assert.IsTrue(!result, $"Result failed IsOnlyDigits method including multiple periods with '{testString}'.");

        }  // ENDIF public void IsOnlyDigitsTestMethod()

        [TestMethod]
        public void IsOnlyLettersTest()
        {
            Boolean result = false;

            String testString = "EveryGoodBoyDoesFine";

            result = testString.IsOnlyLetters();

            Assert.IsTrue(result, $"Result failed IsOnlyLetters method with '{testString}'.");

            testString = "#232 1";

            result = testString.IsOnlyLetters();

            Assert.IsTrue(!result, $"Result failed to return false value in IsOnlyLetters method with '{testString}'.");

            testString = "AbcD E";

            result = testString.IsOnlyLetters(true);

            Assert.IsTrue(result, $"Result failed to return value in IsOnlyLetters method including spaces with '{testString}'.");

        }  // ENDIF public void IsOnlyLettersTestMethod()


        [TestMethod]
        public void IsOnlyLettersAndOrDigitsTest()
        {
            Boolean result = false;

            String testString = "12346abc";

            result = testString.IsOnlyLettersAndOrDigits();

            Assert.IsTrue(result, $"Result failed IsOnlyLettersAndOrDigits method with '{testString}'.");

            testString = "2321.12 abcd";

            result = testString.IsOnlyLettersAndOrDigits();

            Assert.IsTrue(!result, $"Result failed to return false value in IsOnlyLettersAndOrDigits method with '{testString}'.");

            result = testString.IsOnlyLettersAndOrDigits(true);

            Assert.IsTrue(result, $"Result failed IsOnlyLettersAndOrDigits method including period and space with '{testString}'.");

            testString = "2.321.12 abc d fg";

            result = testString.IsOnlyLettersAndOrDigits(true);

            Assert.IsTrue(result, $"Result failed IsOnlyLettersAndOrDigits method including multiple periods with '{testString}'.");


        }  // END public void IsOnlyLettersAndOrDigitsTestMethod()

        [TestMethod]
        public void GetDefaultValueTest()
        {
            String testString = "";

            String defaultValueString = (String)testString.GetType().GetDefaultValue();

            Assert.IsTrue(defaultValueString == null, "Default value for string failed.");

            Object testObject = new object();

            Object defaultObject = (Object)testObject.GetType().GetDefaultValue();

            Assert.IsTrue(defaultObject == null, "Default value for object failed.");

            Int32 testInt32 = 141;

            Int32 defaultInt32 = (Int32)testInt32.GetType().GetDefaultValue();

            Assert.IsTrue(defaultInt32 == 0, "Default value for Int32 failed.");

            DistanceUnitsOfMeasureEnum testSchemaTypesEnum = DistanceUnitsOfMeasureEnum.Kilometers;

            DistanceUnitsOfMeasureEnum defaultSchemaTypesEnum = (DistanceUnitsOfMeasureEnum)testSchemaTypesEnum.GetType().GetDefaultValue();

            Assert.IsTrue(defaultSchemaTypesEnum == DistanceUnitsOfMeasureEnum.Unassigned, "Default value for DistanceUnitsOfMeasureEnum failed.");

        }


        /// <summary>
        /// Tests to see if s string is a valid email address.  Similar to the [IsEmailFormat] String extension.
        /// TODO:  You may wish to change the email address strings, or add more.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="expectedResult"></param>
        [TestMethod]
        [DataRow("Some.one@somewhere.com", true)]
        [DataRow("Some.one @ somewhere.com", false)]
		[DataRow("Some.one@somewhere", false)]
		public void IsEmailValidTest(String emailAddress, Boolean expectedResult)
        {

            Boolean result = CommonHelpers.IsEmailValid(emailAddress);

            Assert.IsTrue(result == expectedResult, $"Received [{result}], but expected [{expectedResult}].");

        }

        /// <summary>
        /// Tests getting the email address user portion.
        /// TODO:  You may wish to change the email address strings, or add more.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="expectedUser"></param>
        /// <param name="expectedResult"></param>
        [TestMethod]
        [DataRow("Someone@somewhere.com", "Someone", true)]
        [DataRow("Someone @ somewhere.com", "", false)]
		[DataRow("Someone@somewhere", "", false)]
		public void GetEmailUserTest(String emailAddress, String expectedUser, Boolean expectedResult)
        {

            Boolean result = false;

            String userName = CommonHelpers.ExtractEmailUser(emailAddress, out result);

            Assert.IsTrue(result == expectedResult, $"Received [{result}], but expected [{expectedResult}].");
            Assert.IsTrue(userName == expectedUser, $"Received [{userName}], but expected [{expectedUser}].");
        }

        /// <summary>
        /// Tests getting the email address domain (or hsot) portion.
        /// TODO:  You may wish to change the email address strings, or add more.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="expectedDomain"></param>
        /// <param name="expectedResult"></param>
        [TestMethod]
        [DataRow("Someone@somewhere.com", "somewhere.com", true)]
        [DataRow("Someone @ somewhere.com", "", false)]
		[DataRow("Someone@somewhere", "", false)]
		public void GetEmailDomainTest(String emailAddress, String expectedDomain, Boolean expectedResult)
        {

            Boolean result = false;

            String domainName = CommonHelpers.ExtractEmailDomain(emailAddress, out result);

            Assert.IsTrue(result == expectedResult, $"Received [{result}], but expected [{expectedResult}].");
            Assert.IsTrue(domainName == expectedDomain, $"Received [{domainName}], but expected [{expectedDomain}].");
        }


    }
}
