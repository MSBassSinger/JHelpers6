using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;

namespace Jeff.Jones.JHelpers6
{


	/// <summary>
	/// Enumerated constant to reflect what type of length is used
	/// 
	/// </summary>
	public enum DistanceUnitsOfMeasureEnum
	{

		/// <summary>
		/// Value used as default, or unassigned.
		/// </summary>
		Unassigned = 0,

		/// <summary>
		/// Use miles.
		/// </summary>
		Miles = 1,

		/// <summary>
		/// Use kilometers.
		/// </summary>
		Kilometers = 2,

		/// <summary>
		/// Use feet.
		/// </summary>
		Feet = 3,

		/// <summary>
		/// Use meters.
		/// </summary>
		Meters = 4
	}

	/// <summary>
	/// A struct for two point geo data.
	/// </summary>
	public class AddressGeoData
	{
		/// <summary>
		/// Latitude for the first geo point.
		/// </summary>
		public Double Latitude1;

		/// <summary>
		/// Longitude for the first geo point.
		/// </summary>
		public Double Longitude1;

		/// <summary>
		/// Altitude for the first geo point.  If you do not have this value, use 0.
		/// </summary>
		public Double Altitude1;

		/// <summary>
		/// Latitude for the second geo point.
		/// </summary>
		public Double Latitude2;

		/// <summary>
		/// Longitude for the second geo point.
		/// </summary>
		public Double Longitude2;

		/// <summary>
		/// Altitude for the second geo point.  If you do not have this value, use 0.
		/// </summary>
		public Double Altitude2;

		/// <summary>
		/// 
		/// </summary>
		public Double LinearDistance;

		/// <summary>
		/// 
		/// </summary>
		public DistanceUnitsOfMeasureEnum UnitsOfMeasure;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pLatitude1"></param>
		/// <param name="pLongitude1"></param>
		/// <param name="pAltitude1"></param>
		/// <param name="pLatitude2"></param>
		/// <param name="pLongitude2"></param>
		/// <param name="pAltitude2"></param>
		/// <param name="pUnitsOfMeasure"></param>
		public AddressGeoData(Double pLatitude1,
								Double pLongitude1,
								Double pAltitude1,
								Double pLatitude2,
								Double pLongitude2,
								Double pAltitude2,
								DistanceUnitsOfMeasureEnum pUnitsOfMeasure)
		{
			this.Latitude1 = pLatitude1;
			this.Longitude1 = pLongitude1;
			this.Altitude1 = pAltitude1;
			this.Latitude2 = pLatitude2;
			this.Longitude2 = pLongitude2;
			this.Altitude2 = pAltitude2;
			this.UnitsOfMeasure = pUnitsOfMeasure;
			this.LinearDistance = CommonHelpers.GetLinearDistance(pLatitude1,
																pLongitude1,
																pAltitude1,
																pLatitude2,
																pLongitude2,
																pAltitude2,
																pUnitsOfMeasure);
		}

		/// <summary>
		/// Set the units of measure used for distance.
		/// </summary>
		/// <param name="lngUnitsOfMeasure"></param>
		public void SetUnitsOfMeasure(DistanceUnitsOfMeasureEnum lngUnitsOfMeasure)
		{
			this.UnitsOfMeasure = lngUnitsOfMeasure;
		}

		/// <summary>
		/// Sets the linear distance between points, as calculated outside this class.
		/// </summary>
		/// <param name="dDistance"></param>
		public void SetLinearDistance(Double dDistance)
		{
			this.LinearDistance = dDistance;
		}
	} // END public class AddressGeoData


	/// <summary>
	/// A static class that provides numerous helper methods.
	/// </summary>
	public static class CommonHelpers
	{
		private static PerformanceCounter m_CPUCounter = null;
		private static PerformanceCounter m_RAMCounter = null;

		private const double EARTHFLATNESS = 0.003352810681183637418;  // From GRS-80

		private const double AVG_EARTH_RADIUS_IN_MILES = 6378.1363;

		private const double AVG_EARTH_RADIUS_IN_FEET = 6378.1363 * 5280;

		private const double AVG_EARTH_RADIUS_IN_KM = 3963.1902;

		private const double AVG_EARTH_RADIUS_IN_METERS = 3963.1902 * 1000;

		private const Int32 FEET_PER_MILE = 5280;

		private const Int32 METERS_PER_KM = 1000;

		private const Double RADIAN_CONVERSION_CONST = Math.PI / 180.0d;

		private const Double DEGREE_CONVERSION_CONST = 180.0d / Math.PI;

		private const Double CELSIUS_CONVERSION_CONST = 0.9 / 0.5;

		private const Double FAHRENHEIT_CONVERSION_CONST = 0.5 / 0.9;


		/// <summary>
		/// Returns error messages from the parent exception and any 
		/// exceptions down the stack, and optionally, the data collection.
		/// 
		/// </summary>
		/// <param name="ex2Examine">The exception to examine.</param>
		/// <param name="getDataCollection">True if the data collection items are to be included; False if not.</param>
		/// <param name="getStackTrace">True if the stack trace is to be included; False if not.</param>
		/// <returns>A string with the error messages</returns>
		public static String GetFullExceptionMessage(this Exception ex2Examine,
													 Boolean getDataCollection,
													 Boolean getStackTrace)
		{

			String retValue = "";
			String message = "";
			String data = "";
			String stackTrace = "";

			try
			{

				if (((ex2Examine != null)))
				{

					if ((getStackTrace))
					{
						// The stack trace is most complete at the top-level
						// exception.  If we are to include it, we grab it here.
						if (((ex2Examine.StackTrace != null)))
						{
							stackTrace = "; Stack Trace=[" + ex2Examine.StackTrace + "].";
						}

					}

					Exception nextException = ex2Examine;

					message = "";

					// We need to loop through all child exceptions to get all the messages.
					// For example, an exception caught when using a SqlClient may not
					// show a message that explains the problem.  There may be 1, 2, or even 3 
					// inner exceptions stacked up. The deepest will likely have the cause
					// of the failure in its message.  So it is a good practice to capture
					// all the messages, pulled from each instance.
					while (nextException != null)
					{

						data = "";

						message += nextException.Message ?? "NULL";


						if (nextException.Source != null)
						{
							message += "; Source=[" + nextException.Source + "]";

						}

						// The Exception provides a Data collection of name-value
						// pairs.  This provides a means, at each method level from 
						// initiation up through the stack, to capture the runtime data
						// which helps diagnose the problem.
						if (getDataCollection)
						{
							if (nextException.Data != null)
							{
								if (nextException.Data.Count > 0)
								{
									foreach (DictionaryEntry item in nextException.Data)
									{
										data += "{" + item.Key.ToString() + "}={" + item.Value.ToString() + "}|";
									}

									data = data.Substring(0, data.Length - 1);
								}

							}

						}

						if (getDataCollection)
						{
							if ((data.Length > 0))
							{
								message = message + "; Data=[" + data + "]";
							}
							else
							{
								message += "; Data=[None]";
							}
						}

						if (nextException.InnerException == null)
						{
							break;
						}
						else
						{
							nextException = nextException.InnerException;
						}

						message += "::";

					}

					if ((stackTrace.Length > 0))
					{
						message = message.Trim();

						if (message.EndsWith(";"))
						{
							message += " " + stackTrace;
						}
						else
						{
							message += "; " + stackTrace;
						}
					}

				}

				retValue = message.Trim();

				if (retValue.EndsWith("::"))
				{
					retValue = retValue.Substring(0, retValue.Length - 2);
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("getDataCollection", getDataCollection.ToString());

				exUnhandled.Data.AddCheck("getStackTrace", getStackTrace.ToString());

				throw;

			}

			return retValue;

		}  // END public static String GetFullExceptionMessage( ... )

		/// <summary>
		/// String extension to allow string comparison type on the string to check.
		/// </summary>
		/// <param name="source">Original string (passed by default)</param>
		/// <param name="toCheck">The string you want to find in the source</param>
		/// <param name="strComp">How the strings are compared</param>
		/// <returns>True if it the toCheck string is in the source string,, false if not.</returns>
		public static Boolean Contains(this String source, String toCheck, StringComparison strComp)
		{

			Boolean retVal = false;

			try
			{

				if (toCheck != null)
				{
					if (toCheck.Length > 0)
					{
						retVal = (source?.IndexOf(toCheck, strComp) >= 0);
					}
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("source", source ?? "NULL");
				exUnhandled.Data.AddCheck("toCheck", toCheck ?? "NULL");
				exUnhandled.Data.AddCheck("strComp", strComp.ToString());

				throw;

			}

			return retVal;

		}  // END public static Boolean Contains(this String source, String toCheck, StringComparison strComp)

		/// <summary>
		/// Gets the count of how many times a give string occurs within another string.
		/// </summary>
		/// <param name="source">The string to be searched.</param>
		/// <param name="toCheck">The string to search for.</param>
		/// <param name="ignoreCase">True to ignore case differences, false if case-sensitive.</param>
		/// <returns>Count of 0 to n occurrences</returns>
		public static Int32 ContainsHowMany(this String source, String toCheck, Boolean ignoreCase = false)
		{

			Int32 retVal = 0;

			try
			{

				if (toCheck != null)
				{
					if (toCheck.Length > 0)
					{
						if (ignoreCase)
						{
							retVal = Regex.Matches(source, toCheck, RegexOptions.IgnoreCase).Count;
						}
						else
						{
							retVal = Regex.Matches(source, toCheck).Count;
						}
					}
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("source", source ?? "NULL");
				exUnhandled.Data.AddCheck("toCheck", toCheck ?? "NULL");

				throw;

			}

			return retVal;

		}  // END public static Int32 ContainsHowMany(this String source, String toCheck, Boolean ignoreCase = false)


		/// <summary>
		/// String extension to convert a string, assumed to be in a format that can be converted, to a Boolean value.
		/// Recognizes as true (case insensitive) true, on, yes, up, ok, good, 1, -1
		/// Recognizes as false (case insensitive): false, off, no, down, not ok, bad, 0
		/// If the conversion fails, false is returned.  Check the isBoolean out value to see if the 
		/// conversion detected a boolean value.  If it is false, the value was not converted.
		/// </summary>
		/// <param name="valueToExamine">The string value to be converted, that also host this method.</param>
		/// <param name="isBoolean">Out parameter that is True if it can be converted to Boolean, and False if not.</param>
		/// <returns>Returns the Boolean value, and whether the conversion was successful by out parameter isBoolean.</returns>
		public static Boolean ConvertToBoolean(this String valueToExamine, out Boolean isBoolean)
		{


			Boolean retVal = false;

			isBoolean = false;

			try
			{

				if (String.IsNullOrWhiteSpace(valueToExamine))
				{
					retVal = false;
					isBoolean = false;
				}
				else
				{
					if (Boolean.TryParse(valueToExamine, out retVal))
					{
						isBoolean = true;
					}
					else
					{

						retVal = BoolConvert(valueToExamine, out isBoolean);
					}
				}
			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("valueToExamine", valueToExamine ?? "NULL");
				throw;

			}

			return retVal;

		}  // END public static Boolean ConvertToBoolean(this String valueToExamine, out Boolean isBoolean)


		/// <summary>
		/// Tests a string, assumed to be in a format that can be converted, to a Boolean value.
		/// Recognizes as true (case insensitive) true, on, yes, up, ok, good, start, 1, -1
		/// Recognizes as false (case insensitive): false, off, no, down, not ok, bad, stop, 0
		/// If the conversion fails, False is returned.  Otherwise, True is returned.
		/// </summary>
		/// <param name="valueToExamine">The string value to be converted, that also host this method.</param>
		/// <returns>Returns true if it can be converted, false if not.</returns>
		public static Boolean IsBoolean(this String valueToExamine)
		{

			Boolean retVal = false;

			try
			{

				if (String.IsNullOrWhiteSpace(valueToExamine))
				{
					retVal = false;
				}
				else
				{
					Boolean convertedValue = false;

					if (Boolean.TryParse(valueToExamine, out convertedValue))
					{
						retVal = true;
					}
					else
					{

						Boolean isBoolean = false;
						convertedValue = BoolConvert(valueToExamine, out isBoolean);

						if (isBoolean)
						{
							retVal = true;
						}
						else
						{
							retVal = false;
						}

					}
				}  // END else of [if ((String.IsNullOrWhiteSpace(valueToExamine)))]

			}  // END try
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("valueToExamine", valueToExamine ?? "NULL");
				throw;

			}

			return retVal;

		}  // END public static Boolean IsBoolean(this String valueToExamine)

		/// <summary>
		/// Converts a string to a Boolean value.  This function is only used internally.
		/// </summary>
		/// <param name="valueToExamine">The string to examine.</param>
		/// <param name="didConvert">True if it did convert, false if it could not be converted.</param>
		/// <returns>The converted Boolean value, or if conversion was not possible, false. Check the didConvert value.</returns>
		private static Boolean BoolConvert(String valueToExamine, out Boolean didConvert)
		{

			Boolean retVal = false;

			didConvert = false;

			valueToExamine = valueToExamine.ToLower();

			switch (valueToExamine)
			{

				case "on":
				case "yes":
				case "up":
				case "ok":
				case "good":
				case "start":
				case "1":
				case "-1":
					retVal = true;
					didConvert = true;
					break;
				case "off":
				case "no":
				case "down":
				case "not ok":
				case "bad":
				case "stop":
				case "0":
					retVal = false;
					didConvert = true;
					break;
				default:
					retVal = false;
					break;
			}

			return retVal;

		}  // END private static Boolean BoolConvert(String valueToExamine, out Boolean didConvert)

		/// <summary>
		/// This string extension process checks for all characters being digits.  
		/// Conversion functions to test numbers may translate letters as Hex values.
		/// </summary>
		/// <param name="testString">String to be examined.</param>
		/// <param name="includePeriod">True if the one and only period is treated as being a number, so that decimal number strings can be handled properly.</param>
		/// <returns>True if the string is only digits, False if not.</returns>
		public static Boolean IsOnlyDigits(this String testString, Boolean includePeriod = false)
		{

			Boolean retValue = true;
			Int32 periodCount = 0;

			try
			{

				if (String.IsNullOrWhiteSpace(testString))
				{
					retValue = false;
				}
				else
				{

					if ((testString.Length == 0))
					{
						retValue = false;
					}
					else
					{

						// Loop through the string, checking each character.
						for (Int32 index = 0; index <= testString.Length - 1; index++)
						{
							if (includePeriod)
							{
								if (testString[index] == '.')
								{
									periodCount += 1;
								}

								if ((!char.IsDigit(testString[index])) && (testString[index] != '.'))
								{
									retValue = false;
									break;
								}
							}
							else
							{
								if (!char.IsDigit(testString[index]))
								{
									retValue = false;
									break;
								}
							}

						}

						if (periodCount > 1)
						{
							retValue = false;
						}

					}
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("testString", testString ?? "NULL");

				throw;

			}

			return retValue;

		}  // END public static Boolean IsOnlyDigits(this String testString)


		/// <summary>
		/// This string extension process gets all digits in a string
		/// and ignores any non-digits.  If includePeriod is true, then 
		/// the first period in the string will be included in the results.
		/// </summary>
		/// <param name="testString">String to be examined.</param>
		/// <param name="includePeriod">True if the one and only period is treated as being a number, so that decimal number strings can be handled properly.</param>
		/// <returns>String of digits.</returns>
		public static String GetOnlyDigits(this String testString, Boolean includePeriod = false)
		{

			String retValue = "";

			Int32 periodCount = 0;

			try
			{
				// remove white spaces 
				testString ??= "";

				testString = testString.Trim();

				if ((testString.Length == 0))
				{
					retValue = "";
				}
				else
				{
					// Loop through the string, checking each character.
					for (Int32 index = 0; index <= testString.Length - 1; index++)
					{
						if (includePeriod)
						{
							if (testString[index] == '.')
							{
								if (periodCount == 0)
								{
									retValue += ".";
								}

								periodCount += 1;
							}
							else
							{
								if ((char.IsDigit(testString[index]) == true))
								{
									retValue += testString[index];
								}
							}
						}  // END if (includePeriod)
						else
						{
							if (char.IsDigit(testString[index]) == true)
							{
								retValue += testString[index];
							}
						}  // END else of [if (includePeriod)]

					}  // END for (Int32 index = 0; index <= testString.Length - 1; index++)

				}  // END else of [if ((testString.Length == 0))]

			}  // END try
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("testString", testString ?? "NULL");

				throw;

			}

			return retValue;

		}  // END public static String GetOnlyDigits(this String testString, Boolean includePeriod = false)

		/// <summary>
		/// This string extension process gets all letters in a string 
		/// and ignores any non-letters, unless spaces are requested.
		/// </summary>
		/// <param name="testString">String to be examined.</param>
		/// <param name="includeSpace">True if spaces are to be included in the return value.</param>
		/// <returns>String of digits.</returns>
		public static String GetOnlyLetters(this String testString, Boolean includeSpace = false)
		{

			String retValue = "";

			try
			{
				// remove white spaces 
				testString ??= "";

				testString = testString.Trim();

				if ((testString.Length == 0))
				{
					retValue = "";
				}
				else
				{
					// Loop through the string, checking each character.
					for (Int32 index = 0; index <= testString.Length - 1; index++)
					{
						if (includeSpace)
						{
							if ((char.IsLetter(testString[index])) || (testString[index] == ' '))
							{
								retValue += testString[index];
							}
						}
						else
						{
							if (char.IsLetter(testString[index]))
							{
								retValue += testString[index];
							}
						}

					}  // END for (Int32 index = 0; index <= testString.Length - 1; index++)

				}  // END else of [if ((testString.Length == 0))]

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("testString", testString ?? "NULL");

				throw;

			}

			return retValue;

		}  // END public static String GetOnlyLetters(this String testString, Boolean includeSpace = false)


		/// <summary>
		/// This process gets all letters and digits in a string, 
		/// and ignores all else, with the exception of periods and spaces
		/// when includePeriodAndSpace is true.
		/// </summary>
		/// <param name="testString">String to be examined.</param>
		/// <param name="includePeriodAndSpace">True to include all periods and spaces as if they were letters.</param>
		/// <returns>String of characters.</returns>
		public static String GetOnlyLettersAndDigits(this String testString,
													 Boolean includePeriodAndSpace = false)
		{

			String retValue = "";

			try
			{
				// remove white spaces 
				testString ??= "";

				testString = testString.Trim();

				if ((testString.Length == 0))
				{
					retValue = "";
				}
				else
				{
					// Loop through the string, checking each character.
					for (Int32 index = 0; index <= testString.Length - 1; index++)
					{

						if (includePeriodAndSpace)
						{
							if (char.IsLetterOrDigit(testString[index]) ||
								(testString[index] == ' ') ||
								(testString[index] == '.'))
							{
								retValue += testString[index];
							}
						}
						else
						{
							if (char.IsLetterOrDigit(testString[index]))
							{
								retValue += testString[index];
							}
						}

					}

				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("testString", testString ?? "NULL");

				throw;

			}

			return retValue;

		}  // END public static String GetOnlyLettersAndDigits(this String testString)


		/// <summary>
		/// This process checks for all characters being letters.  If includeSpace 
		/// is true, then spaces are accepted as if they were letters.
		/// </summary>
		/// <param name="testString">String to be examined.</param>
		/// <param name="includeSpace">True if a space should be considered a letter, false if not.</param>
		/// <returns>True if the string is only letters, False if not.</returns>
		public static Boolean IsOnlyLetters(this String testString, Boolean includeSpace = false)
		{

			Boolean retValue = true;

			try
			{
				if (String.IsNullOrWhiteSpace(testString))
				{
					retValue = false;
				}
				else
				{

					if ((testString.Length == 0))
					{
						retValue = false;
					}
					else
					{

						// Loop through the string, checking each character.
						for (Int32 index = 0; index <= testString.Length - 1; index++)
						{
							if (includeSpace)
							{
								if ((!char.IsLetter(testString[index])) && (testString[index] != ' '))
								{
									retValue = false;
									break;
								}
							}
							else
							{
								if (!char.IsLetter(testString[index]))
								{
									retValue = false;
									break;
								}
							}

						}

					}
				}
			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("testString", testString ?? "NULL");

				throw;

			}

			return retValue;

		}  // END public static Boolean IsOnlyLetters(this String testString)


		/// <summary>
		/// This extension method checks for all characters being only letters and numbers.
		/// </summary>
		/// <param name="testString">String to be examined.</param>
		/// <param name="includePeriodAndSpace">True if spaces and all periods are to be counted in the return value.</param>
		/// <returns>True if the string is only letters, False if not.</returns>
		public static Boolean IsOnlyLettersAndOrDigits(this String testString,
													   Boolean includePeriodAndSpace = false)
		{

			Boolean retValue = true;

			try
			{
				if (String.IsNullOrWhiteSpace(testString))
				{
					retValue = false;
				}
				else
				{
					if (testString.Length == 0)
					{
						retValue = false;
					}
					else
					{
						// Loop through the string, checking each character.
						for (Int32 index = 0; index <= testString.Length - 1; index++)
						{
							if (includePeriodAndSpace)
							{

								if (!char.IsLetterOrDigit(testString[index]) &&
									(testString[index] != '.') &&
									(testString[index] != ' '))
								{
									retValue = false;
									break;
								}
							}
							else
							{
								if (!char.IsLetterOrDigit(testString[index]))
								{
									retValue = false;
									break;
								}
							}

						}

					}
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("testString", testString ?? "NULL");

				throw;

			}

			return retValue;

		}  // END public static Boolean IsOnlyLettersAndOrDigits(this String testString, Boolean includePeriodAndSpace = false)

		/// <summary>
		/// This extension method for the String object checks the string to see if it is a valid email format.
		/// It does not check whether it is a valid, working email.
		/// </summary>
		/// <param name="email">The email address to test</param>
		/// <returns>True if formatted correctly, False if not.</returns>
		public static Boolean IsEmailFormat(this String email)
		{

			Boolean retValue = false;

			System.Net.Mail.MailAddress mailAddress = null;

			try
			{
				if (((email != null)))
				{
					if ((email.ContainsHowMany("@") == 1))
					{
						if (email.LastIndexOf(".") > email.LastIndexOf("@"))
						{
							if (!email.Contains(' '))
							{
								mailAddress = new System.Net.Mail.MailAddress(email);
								retValue = true;
							}
							else
							{
								retValue = false;
							}
						}
						else
						{
							retValue = false;
						}
					}
					else
					{
						retValue = false;
					}
				}
				else
				{
					email = "";
					retValue = true;
				}

			}
			catch (FormatException exFormat)
			{
				// I hate using exceptions as a part of normal flow, but this object
				// does not have a TryParse() option
				retValue = false;

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("email", email ?? "NULL");

				throw;

			}
			finally
			{
				mailAddress = null;
			}

			return retValue;

		}  // END public static Boolean IsEmailFormat(this String email)


		/// <summary>
		/// This string extension converts string to date, or returns the default value.
		/// </summary>
		/// <param name="dateString">A string that can be converted to a DateTime variable</param>
		/// <param name="dateDefault">The default value if dateString cannot be converted</param>
		/// <returns>The DateTime value if the string converts, the default value if not</returns>
		public static DateTime GetDateTime(this String dateString, DateTime dateDefault)
		{
			DateTime retVal = dateDefault;

			dateString ??= "";

			try
			{

				if (dateString.Length > 0)
				{

					if (!DateTime.TryParse(dateString, out retVal))
					{
						retVal = dateDefault;
					}
				}
			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("dateString", dateString ?? "NULL");

				throw;

			}

			return retVal;

		}  // END public static DateTime GetDateTime(this String dateString, DateTime dateDefault)

		/// <summary>
		/// This string extension converts string to decimal value, or returns the default value.
		/// </summary>
		/// <param name="numberString">A string that can be converted to a Decimal variable</param>
		/// <param name="decimalDefault">The default value if numberString cannot be converted</param>
		/// <returns>The Decimal value if the string converts, the default value if not</returns>
		public static Decimal GetDecimal(this String numberString, Decimal decimalDefault)
		{
			Decimal retVal = decimalDefault;

			numberString ??= "";

			try
			{

				if (numberString.Length > 0)
				{

					if (!Decimal.TryParse(numberString, out retVal))
					{
						retVal = decimalDefault;
					}
				}
			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("numberString", numberString ?? "NULL");

				throw;

			}

			return retVal;

		}  // END public static Decimal GetDecimal(this String numberString, Decimal decimalDefault)


		/// <summary>
		/// This string extension converts string to Int32 value, or returns the default value.
		/// </summary>
		/// <param name="numberString">A string that can be converted to an Int32 variable</param>
		/// <param name="integerDefault">The default value if numberString cannot be converted</param>
		/// <returns>The Int32 value if the string converts, the default value if not</returns>
		public static Int32 GetInt32(this String numberString, Int32 integerDefault)
		{
			Int32 retVal = integerDefault;

			numberString ??= "";

			try
			{

				if (numberString.Length > 0)
				{

					if (!Int32.TryParse(numberString, out retVal))
					{
						retVal = integerDefault;
					}
				}
			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("numberString", numberString ?? "NULL");

				throw;

			}

			return retVal;

		}  // END public static Int32 GetInt32(this String numberString, Int32 integerDefault)

		/// <summary>
		/// This string extension converts string to Int64 value, or returns the default value.
		/// </summary>
		/// <param name="numberString">A string that can be converted to a Int64 variable</param>
		/// <param name="integerDefault">The default value if numberString cannot be converted</param>
		/// <returns>The Int64 value if the string converts, the default value if not</returns>
		public static Int64 GetInt64(this String numberString, Int64 integerDefault)
		{
			Int64 retVal = integerDefault;

			numberString ??= "";

			try
			{

				if (numberString.Length > 0)
				{

					if (!Int64.TryParse(numberString, out retVal))
					{
						retVal = integerDefault;
					}
				}
			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("numberString", numberString ?? "NULL");

				throw;

			}

			return retVal;

		}  // END public static Int64 GetInt64(this String numberString, Int64 integerDefault)

		/// <summary>
		/// Gets a default value for a type, if one exists.
		/// </summary>
		/// <param name="t">The type for which a default is being sought.</param>
		/// <returns>New value or default value.</returns>
		public static Object GetDefaultValue(this Type t)
		{
			if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
			{
				return Activator.CreateInstance(t);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// This value can be applied to the value for a constant.
		/// 
		/// RowDelimiter is the same non-printable ASCII character used
		/// in teletypes and other devices to indicate a new row, 
		/// and not likely to be seen in string data.
		/// </summary>
		public static String RowDelimiter
		{
			get
			{
				return ((Char)29).ToString();
			}
		}

		/// <summary>
		/// This value can be applied to the value for a constant.
		/// 
		/// ColumnDelimiter is the same non-printable ASCII character used
		/// in teletypes and other devices to indicate a new column, 
		/// and not likely to be seen in string data.
		/// </summary>
		public static String ColumnDelimiter
		{
			get
			{
				return ((Char)28).ToString();
			}
		}

		/// <summary>
		/// This value can be applied to the value for a constant.
		/// 
		/// TableDelimiter is the same non-printable ASCII character used
		/// in teletypes and other devices to indicate a new table of data, 
		/// and not likely to be seen in string data.
		/// </summary>
		public static String TableDelimiter
		{
			get
			{
				return ((Char)30).ToString();
			}
		}

		/// <summary>
		/// Gets the full computer name that DNS will recognize in any domain
		/// </summary>
		public static String FullComputerName
		{
			get
			{
				String FQComputerName = "";

				try
				{

					String DomainNameAppendage = IPGlobalProperties.GetIPGlobalProperties().DomainName;
					String DNSHostName = Dns.GetHostName();
					if (!DNSHostName.Contains(DomainNameAppendage))
					{
						FQComputerName = DNSHostName + "." + DomainNameAppendage;
					}
					else
					{
						FQComputerName = DNSHostName;
					}

				} // END try
				catch
				{

					FQComputerName = System.Environment.MachineName;
				}


				return FQComputerName;
			}
		}

		/// <summary>
		/// Gets the DNS host entry table name for a given computer name.
		/// If pComputerName is not provided, then it returns the name of the host computer.
		/// </summary>
		/// <param name="pComputerName">Pass in any computer name.  If left blank or null, the current computer name will be used.</param>
		/// <returns></returns>
		public static String GetDNSName(String pComputerName = "")
		{


			String DNSName = "";

			if (String.IsNullOrWhiteSpace(pComputerName))
			{
				pComputerName = Environment.MachineName;
			}

			try
			{

				DNSName = Dns.GetHostEntry(pComputerName).HostName;
			}
			catch
			{
				DNSName = pComputerName;
			}

			return DNSName;

		}  // END public static String GetDNSName(String pComputerName = "")

		/// <summary>
		/// Summary:
		///     Gets or sets the fully qualified path of the current working directory.
		///     For services, the current directory via normal means shows the Windows System32 
		///     directory because the service runs under an exe located there.  This property
		///     accounts for that by using one method call for running in the IDE, and another for 
		///     running compiled.
		///
		/// Returns:
		///     A String containing a directory path.
		///
		/// Exceptions:
		///   System.UnauthorizedAccessException      [Directory.GetCurrentDirectory()]
		///   System.NotSupportedException            [Directory.GetCurrentDirectory()]
		///   System.ArgumentException                [Path.GetDirectoryName()]
		///   System.IO.PathTooLongException          [Path.GetDirectoryName()]
		///   </summary>
		public static String CurDir
		{
			get
			{
				String strCurDir = "";

				// When using the IDE
				if (Debugger.IsAttached)
				{
					strCurDir = Directory.GetCurrentDirectory();

				}

				// When running compiled in case running under a service
				else
				{
					strCurDir = System.IO.Path.GetDirectoryName(Environment.CurrentDirectory);

				}

				return strCurDir;
			}

			set
			{
				System.Environment.CurrentDirectory = value;
			}
		}

		/// <summary>
		/// IDictionary extension method that is an enhanced Add to check to see if a key exists, and if so, 
		/// adds the key with an ordinal appended to the key name to prevent overwrite.
		/// This is useful with the Exception.Data IDictionary collection, among other
		/// IDictionary implementations.
		/// </summary>
		/// <param name="dct">The IDictionaryimplementation</param>
		/// <param name="dataKey">The string key for the name-value pair.</param>
		/// <param name="dataValue">The value for the name-value pair.  Accepts any data type, which is resolved to the type at runtime.</param>
		public static void AddCheck(this IDictionary dct, String dataKey, dynamic dataValue)
		{

			if (dct != null)
			{
				if (dct.Contains(dataKey))
				{
					for (int i = 1; i < 101; i++)
					{
						String newKey = dataKey + "-" + i.ToString();

						if (!dct.Contains(newKey))
						{
							if (dataValue == null)
							{
								dct.Add(newKey, "NULL");
							}
							else
							{
								dct.Add(newKey, dataValue);
							}

							break;
						}
					}
				}
				else
				{
					dct.Add(dataKey, dataValue);
				}
			}
		}


		/// <summary>
		/// This method will return true if this project, or any project that
		/// calls this component as compiled code, is running under the IDE
		/// </summary>
		/// <returns>This method returns false if no IDE is being used.</returns>
		public static Boolean AmIRunningInTheIDE
		{
			get
			{
				return Debugger.IsAttached;
			}
		}

		/// <summary>
		/// Checks to see if the computer is in a domain.
		/// </summary>
		/// <returns>True if the host computer is in a domain, false if not.</returns>
		public static Boolean IsInDomain()
		{
			Boolean retVal = false;

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					Domain ThisDomain = Domain.GetComputerDomain();
					retVal = true;
				}
			}
			catch
			{
				retVal = false;
			}

			return retVal;
		}

		/// <summary>
		/// Converts feet to meters.
		/// </summary>
		/// <param name="feet">Number of feet to be converted.</param>
		/// <returns>Length in meters.</returns>
		public static Decimal ConvertFeetToMeters(Decimal feet)
		{
			return (Decimal)(feet * 0.3048m);
		}

		/// <summary>
		/// Converts meters to feet.
		/// </summary>
		/// <param name="meters">Number of meters to be converted.</param>
		/// <returns>Length in feet.</returns>
		public static Decimal ConvertMetersToFeet(Decimal meters)
		{
			return (Decimal)(meters * 3.2808399m);
		}

		/// <summary>
		/// Converts miles to kilometers.
		/// </summary>
		/// <param name="miles">Number of miles to be converted.</param>
		/// <returns>Length in kilometers.</returns>
		public static Decimal ConvertMilesToKilometers(Decimal miles)
		{
			return (Decimal)(miles * 1.609344m);
		}

		/// <summary>
		/// Converts kilometers to miles.
		/// </summary>
		/// <param name="kilometers">Number of kilometers to convert.</param>
		/// <returns>Length in miles.</returns>
		public static Decimal ConvertKilometersToMiles(Decimal kilometers)
		{
			return (Decimal)(kilometers * 0.62137119m);
		}

		/// <summary>
		/// Converts gallons to liters.
		/// </summary>
		/// <param name="gallons">Volume in liters.</param>
		/// <returns></returns>
		public static Decimal ConvertGallonsToLiters(Decimal gallons)
		{
			return (Decimal)(gallons * 3.78541178m);
		}

		/// <summary>
		/// Converts liters to gallons.
		/// </summary>
		/// <param name="liters">Number of liters to convert.</param>
		/// <returns>Volume in gallons.</returns>
		public static Decimal ConvertLitersToGallons(Decimal liters)
		{
			return (Decimal)(liters * 0.26417205m);
		}


		/// <summary>
		/// Returns the Domain which the computer is joined to.  Note: if user is logged in as local account the domain of computer is still returned.
		/// </summary>
		/// <returns>A String with the domain name if it's joined.  String.Empty if it isn't.</returns>
		public static String GetComputerDomainName()
		{
			String retVal = "";

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					retVal = Domain.GetComputerDomain().Name;
				}
			}
			catch
			{
				retVal = Environment.UserDomainName;

			}

			return retVal;
		}


		/// <summary>
		/// Returns the full domain name instead of the alias.
		/// </summary>
		/// <returns>String with the full domain name.</returns>
		public static String GetFullComputerDomainName()
		{
			String retVal = "";

			try
			{
				try
				{

					if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					{
						retVal = Forest.GetCurrentForest().RootDomain.Name;
					}

				}
				catch
				{
					// Set to a value sure to try an alternate method below.
					retVal = Environment.UserDomainName;

				}  // END catch (Exception exRootDomain)

				if (retVal.Equals(Environment.UserDomainName, StringComparison.CurrentCultureIgnoreCase))
				{

					try
					{
						if (Environment.OSVersion.Platform == PlatformID.Win32NT)
						{
							retVal = Forest.GetCurrentForest().Name;
						}

					}
					catch
					{
						// Set to a value sure to try an alternate method below.
						retVal = Environment.UserDomainName;

					}

					if (retVal.Equals(Environment.UserDomainName, StringComparison.CurrentCultureIgnoreCase))
					{

						try
						{
							retVal = IPGlobalProperties.GetIPGlobalProperties().DomainName;

							if (retVal.Length == 0)
							{
								retVal = Environment.UserDomainName;
							}

						}  // END try
						catch
						{
							// Set to a value sure to try an alternate method below.
							retVal = Environment.UserDomainName;

						}  // END catch (Exception exRootDomain)

					}  // END if (ThisComputersDomain.Equals(Environment.UserDomainName, StringComparison.CurrentCultureIgnoreCase))

				}  // END if (ThisComputersDomain.Equals(Environment.UserDomainName, StringComparison.CurrentCultureIgnoreCase))

			}  // try
			catch
			{
				// Handle exception here if desired.
				retVal = GetComputerDomainName();
			}

			return retVal;
		}

		/// <summary>
		/// True if the datetime supplied falls within the period of Daylight Savings.
		/// </summary>
		/// <param name="dtmToTest">The date to test for DST.</param>
		/// <returns></returns>
		public static Boolean IsDaylightSavingsTime(DateTime dtmToTest)
		{
			return TimeZoneInfo.Local.IsDaylightSavingTime(dtmToTest);
		}

		/// <summary>
		/// Checks if the current time is DST.
		/// </summary>
		/// <returns>True if it is currently daylight savings time.</returns>
		public static Boolean IsDaylightSavingsTime()
		{
			return TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
		}
		/// <summary>
		/// Name of the current time zone for daylight savings.
		/// </summary>
		/// <returns>String with the current time zone's DST name.</returns>
		public static String CurrentTimeZoneDaylightName
		{
			get
			{
				return TimeZoneInfo.Local.DaylightName;
			}
		}

		/// <summary>
		/// Current time zone name.
		/// 
		/// Exceptions:
		///   System.ArgumentNullException - An attempt was made to set this property to null.
		/// </summary>
		/// <returns>A string with the current time zone name, DST or standard time.</returns>
		public static String CurrentTimeZoneName
		{
			get
			{
				return TimeZoneInfo.Local.StandardName;
			}
		}

		/// <summary>
		/// Same functionality as the VB6 ASC function - give it a character, get back the ASCII 
		/// decimal number.  If you provide more than one character, only the first one is used.
		/// </summary>
		/// <param name="strChar"></param>
		/// <returns>ASCII value of the single character.</returns>
		public static Int32 Asc(String strChar)
		{
			if (strChar == null)
			{
				return 0;
			}
			else
			{
				Char s = strChar[0];

				return (Int32)s;
			}
		}

		/// <summary>
		/// Same as the VB6 function.  Converts a 32 bit integer to a String hex value.
		/// </summary>
		/// <param name="lngValue"></param>
		/// <returns>String hex value.</returns>
		public static String Hex(Int32 lngValue)
		{
			String retVal = lngValue.ToString("X");
			return retVal;

		}



		/// <summary>
		/// Gets the current % processor time from the performance counter.
		/// </summary>
		/// <returns>% Processor time</returns>
		public static Int32 GetCurrentCPUUsage()
		{

			Int32 retVal = 0;

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{

					if (m_CPUCounter == null)
					{
						m_CPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
					}

					try
					{
						float CPUUSageRaw = m_CPUCounter.NextValue();

						if (CPUUSageRaw == 0.0f)
						{
							Thread.Sleep(100);
							CPUUSageRaw = m_CPUCounter.NextValue();

							if (CPUUSageRaw == 0.0f)
							{
								Thread.Sleep(500);
								CPUUSageRaw = m_CPUCounter.NextValue();
							}
						}

						retVal = Convert.ToInt32(Math.Round(CPUUSageRaw, 0));
					}
					catch
					{
						retVal = -1;
					}
				}
			}  // END try
			catch
			{
				retVal = -1;
			}

			return retVal;

		}

		/// <summary>
		/// Returns available RAM MBs from the performance counter.
		/// </summary>
		/// <returns>Returns available RAM MBs</returns>
		public static Int32 AvailableRAMinMB()
		{

			Int32 retVal = 0;

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					if (m_RAMCounter == null)
					{
						m_RAMCounter = new PerformanceCounter("Memory", "Available MBytes", true);
					}

					try
					{
						float curValue = m_RAMCounter.NextValue();

						if (curValue == 0.0f)
						{
							curValue = m_RAMCounter.NextValue();
						}

						retVal = Convert.ToInt32(Math.Round(m_RAMCounter.NextValue(), 0));
					}
					catch
					{
						retVal = -1;
					}
				}
			}  // END try
			catch
			{
				retVal = -1;
			}

			return retVal;

		}

		/// <summary>
		/// Summary:
		///     Pings the specified server.
		///
		/// Parameters:
		///   hostNameOrAddress:
		///     String. The URL, computer name, or IP number of the server to ping. Required.
		///
		///   timeout:
		///     System.Int32. Time threshold in milliseconds for contacting the destination.
		///     Default is 500. Required.
		///
		/// Returns:
		///     PingReply instance indicating whether or not the operation was successful.
		/// </summary>
		/// <param name="strHostName"></param>
		/// <param name="lngTimeout"></param>
		/// <returns>System.Net.NetworkInformation.PingReply object with information about the ping.</returns>
		public static PingReply Ping(String strHostName, Int32 lngTimeout)
		{


			DateTime dtmMethodStart = DateTime.Now;

			PingReply retVal = null;

			Ping oPing = null;

			try
			{
				oPing = new Ping();

				retVal = oPing.Send(strHostName, lngTimeout);

			} // END try

			catch 
			{

				retVal = null;

			}  // END catch 

			finally
			{

				if (oPing != null)
				{
					oPing.Dispose();

					oPing = null;
				}

			}  // END finally

			return retVal;
		}

		/// <summary>
		/// This function uses the Haversine formula to calculate linear distance between two sets of
		/// latitude and longitude, with an adjustment for the earth's radius based on the latitude.
		/// Haversine is used instead of Vincenty’s formula to keep the computation simpler and less
		/// processor intensive.
		/// This function takes a List of address geo data instances and processes them, updating the 
		/// individual class instances in the List.
		/// </summary>
		/// <param name="objAddressList"></param>
		public static void GetLinearDistances(ref List<AddressGeoData> objAddressList)
		{

			double dblDistance = 0.0d;

			foreach (AddressGeoData objGeoData in objAddressList)
			{

				dblDistance = GetLinearDistance(objGeoData.Latitude1,
												objGeoData.Longitude1,
												objGeoData.Altitude1,
												objGeoData.Latitude2,
												objGeoData.Longitude2,
												objGeoData.Altitude2,
												objGeoData.UnitsOfMeasure);

				objGeoData.SetLinearDistance(dblDistance);

			}  // END foreach (AddressGeoData objGeoData in objAddressList)

		}  // END public static void GetLinearDistances(ref List<AddressGeoData> objAddressList, Boolean UseMiles)

		/// <summary>
		/// This function uses the Haversine formula to calculate linear distance between two sets of
		/// latitude and longitude, with an adjustment for the earth's radius based on the latitude.
		/// Haversine is used instead of Vincenty’s formula to keep the computation simpler and less
		/// processor intensive.
		/// 
		/// This overload allows the caller to specify what units of measure is desired for the return value.
		/// </summary>
		/// <param name="Latitude1"></param>
		/// <param name="Longitude1"></param>
		/// <param name="Altitude1"></param>
		/// <param name="Latitude2"></param>
		/// <param name="Longitude2"></param>
		/// <param name="Altitude2"></param>
		/// <param name="lngUnitsOfMeasure"></param>
		/// <returns>Linear distance between points in the units of measure chosen.</returns>
		public static Double GetLinearDistance(double Latitude1,
											   double Longitude1,
											   double Altitude1,
											   double Latitude2,
											   double Longitude2,
											   double Altitude2,
											   DistanceUnitsOfMeasureEnum lngUnitsOfMeasure)
		{

			double dblDistance = Double.MinValue;

			try
			{
				/*
				  This function uses the Haversine formula to calculate linear distance between two sets of
				  latitude and longitude, with an adjustment for the earth's radius based on the latitude.
				  Haversine is used instead of Vincenty’s formula to keep the computation simpler and less
				  processor intensive.
				*/
				double AvgEarthRadius = Double.MinValue;
				double AltitdueDelta = Double.MinValue;

				double dblLatitude1InRadians = Latitude1 * RADIAN_CONVERSION_CONST;
				double dblLongitude1InRadians = Longitude1 * RADIAN_CONVERSION_CONST;
				double dblLatitude2InRadians = Latitude2 * RADIAN_CONVERSION_CONST;
				double dblLongitude2InRadians = Longitude2 * RADIAN_CONVERSION_CONST;

				double dblLongitudeDelta = dblLongitude2InRadians - dblLongitude1InRadians;
				double dblLatitudeDelta = dblLatitude2InRadians - dblLatitude1InRadians;


				switch (lngUnitsOfMeasure)
				{
					case DistanceUnitsOfMeasureEnum.Feet:
						AvgEarthRadius = AVG_EARTH_RADIUS_IN_FEET;
						AltitdueDelta = Math.Abs(Altitude1 - Altitude2);
						break;
					case DistanceUnitsOfMeasureEnum.Miles:
						AvgEarthRadius = AVG_EARTH_RADIUS_IN_MILES;
						AltitdueDelta = Math.Abs(Altitude1 - Altitude2) / FEET_PER_MILE;
						break;
					case DistanceUnitsOfMeasureEnum.Meters:
						AvgEarthRadius = AVG_EARTH_RADIUS_IN_METERS;
						AltitdueDelta = Math.Abs(Altitude1 - Altitude2);
						break;
					case DistanceUnitsOfMeasureEnum.Kilometers:
						AvgEarthRadius = AVG_EARTH_RADIUS_IN_KM;
						AltitdueDelta = Math.Abs(Altitude1 - Altitude2) / METERS_PER_KM;
						break;
					default:
						AvgEarthRadius = AVG_EARTH_RADIUS_IN_MILES;
						AltitdueDelta = Math.Abs(Altitude1 - Altitude2) / FEET_PER_MILE;
						break;
				}

				// Square the latitude difference for the radius adjustment formula
				Double dblAvgLatitudeSquared = Math.Pow(Math.Sin(dblLatitudeDelta), 2);

				// Calculate the localized earth's radius at sea level.  RM data does not 
				// contain elevation data yet, so we assume sea level.
				Double dblLocalizedEarthRadius = 1 - (EARTHFLATNESS * dblAvgLatitudeSquared * AvgEarthRadius);


				// Intermediate result #1.
				double dblIntermediateValue = Math.Pow(Math.Sin(dblLatitudeDelta / 2.0), 2.0) +
						   Math.Cos(dblLatitude1InRadians) * Math.Cos(dblLatitude2InRadians) *
						   Math.Pow(Math.Sin(dblLongitudeDelta / 2.0), 2.0);

				// Intermediate result #2 (great circle distance in Radians).
				double dblDistanceInRadians = 2.0 * Math.Atan2(Math.Sqrt(dblIntermediateValue), Math.Sqrt(1.0 - dblIntermediateValue));

				// Radian length corrected for localized earth radius
				Double dblDistanceInRadiansCorrected = dblLocalizedEarthRadius * dblDistanceInRadians;

				// Radians to distance conversion.  See UnitsOfMeasureEnum for the types supported.
				dblDistance = AvgEarthRadius * dblDistanceInRadiansCorrected;

				// Recalculate the value based on a difference between altitudes.
				// Simple use of the Pythagorean theorem
				// Altitude has very little effect on long distances, but if calculating
				// closer coordinates, such as points along a route where altitude changes,
				// this gives a much more accurate estimate of the linear distance travelled, 
				// as opposed to the assumed "parallel to earth's surface" distance.
				// For example, if the route between bus stops involves hills, the coordinates measured
				// at the crest and foot of each hill with the correct altitude will yield a 
				// noticeably more accurate value that will match the odometer reading much closer.
				// For example, the GPS_POSITION struct in Windows Mobile includes both the
				// altitude above sea level and with respect to the WGS84 ellipsoid.
				if (AltitdueDelta > 0.0d)
				{
					dblDistance = Math.Sqrt(Math.Pow(AltitdueDelta, 2) + Math.Pow(dblDistance, 2));
				}
			}  // END try

			catch
			{
				dblDistance = -1;
			}

			return dblDistance;
		}



		/// <summary>
		/// This function uses the Haversine formula to calculate linear distance between two sets of
		/// latitude and longitude, with an adjustment for the earth's radius based on the latitude.
		/// Haversine is used instead of Vincenty’s formula to keep the computation simpler and less
		/// processor intensive.
		/// 
		/// This overload allows the user to choose between miles and kilometers (UseMiles param)
		/// </summary>
		/// <param name="Latitude1"></param>
		/// <param name="Longitude1"></param>
		/// <param name="Altitude1"></param>
		/// <param name="Latitude2"></param>
		/// <param name="Longitude2"></param>
		/// <param name="Altitude2"></param>
		/// <param name="UseMiles"></param>
		/// <returns>Linear distance in miles or kilometers.</returns>
		public static Double GetLinearDistance(Double Latitude1,
											   Double Longitude1,
											   Double Altitude1,
											   Double Latitude2,
											   Double Longitude2,
											   Double Altitude2,
											   Boolean UseMiles)
		{

			double dblReturnValue = 0.0d;

			if (UseMiles)
			{

				dblReturnValue = GetLinearDistance(Latitude1, Longitude1, Altitude1, Latitude2, Longitude2, Altitude2, DistanceUnitsOfMeasureEnum.Miles);
			}
			else
			{
				dblReturnValue = GetLinearDistance(Latitude1, Longitude1, Altitude1, Latitude2, Longitude2, Altitude2, DistanceUnitsOfMeasureEnum.Kilometers);
			}


			return dblReturnValue;

		}

		/// <summary>
		/// Eliminates the user having to use altitude values.
		/// </summary>
		/// <param name="Latitude1">Latitude for the first geo point.</param>
		/// <param name="Longitude1">Longitude for the first geo point.</param>
		/// <param name="Latitude2">Latitude for the second geo point.</param>
		/// <param name="Longitude2">Longitude for the second geo point,</param>
		/// <param name="UnitsOfMeasure">What units the return value should represent.</param>
		/// <returns>Linear distance in the specified units of measure.</returns>
		public static Double GetLinearDistance(Double Latitude1,
											   Double Longitude1,
											   Double Latitude2,
											   Double Longitude2,
											   DistanceUnitsOfMeasureEnum UnitsOfMeasure)
		{

			double dblReturnValue = 0.0d;

			dblReturnValue = GetLinearDistance(Latitude1, Longitude1, 0, Latitude2, Longitude2, 0, UnitsOfMeasure);

			return dblReturnValue;

		}

		/// <summary>
		/// Converts degrees to radians
		/// </summary>
		/// <param name="degrees">The angular degrees to be converted.</param>
		/// <returns>Returns the value in radians.</returns>
		public static Double DegreesToRadians(Double degrees)
		{
			return degrees * (RADIAN_CONVERSION_CONST);
		}

		/// <summary>
		/// Converts radians to degrees
		/// </summary>
		/// <param name="radians">The radians to be converted.</param>
		/// <returns>Returns the value in angular degrees.</returns>
		public static Double RadianToDegree(Double radians)
		{
			return radians * (DEGREE_CONVERSION_CONST);
		}

		/// <summary>
		/// Converts Celsius to Fahrenheit
		/// </summary>
		/// <param name="DegC">The temperature in Celsius to be converted.</param>
		/// <returns>The temperature in Fahrenheit</returns>
		public static Double CelsiusToFahrenheit(Double DegC)
		{
			return (CELSIUS_CONVERSION_CONST * DegC) + 32;
		}

		/// <summary>
		/// Converts Fahrenheit to Celsius
		/// </summary>
		/// <param name="DegF">The temperature in Fahrenheit to be converted.</param>
		/// <returns>The temperature in Celsius</returns>
		public static Double FahrenheitToCelsius(Double DegF)
		{
			return FAHRENHEIT_CONVERSION_CONST * (DegF - 32);
		}

		/// <summary>
		/// Convert String to Base64
		/// 
		/// Exceptions:
		///   System.ArgumentNullException - String2Convert  or byte array created from it is null.
		///   System.Text.EncoderFallbackException - 
		///        A fallback occurred (see Understanding Encodings for complete explanation)-and-
		///        System.Text.Encoding.EncoderFallback is set to System.Text.EncoderExceptionFallback.
		/// 
		/// </summary>
		/// <param name="String2Convert">A string to be converted to Base64</param>
		/// <returns>String with Base64 value.</returns>
		public static String StringToBase64(String String2Convert)
		{
			byte[] ByteString = System.Text.Encoding.UTF8.GetBytes(String2Convert);
			String ByteString64 = Convert.ToBase64String(ByteString);
			return ByteString64;
		}

		/// <summary>
		/// Convert Base64String to String
		/// 
		/// Exceptions:
		///   System.ArgumentNullException - ByteString64, or the byte array made from it, is null.
		///   System.FormatException - 
		///        The length of ByteString64, ignoring white-space characters, is not zero or a multiple
		///        of 4. -or-The format of ByteString64 is invalid. s contains a non-base-64 character,
		///        more than two padding characters, or a non-white space-character among the
		///        padding characters.
		///   System.ArgumentException - The byte array contains invalid Unicode code points.
		///   System.Text.DecoderFallbackException - 
		///        A fallback occurred (see Understanding Encodings for complete explanation)-and-
		///        System.Text.Encoding.DecoderFallback is set to System.Text.DecoderExceptionFallback.
		/// 
		/// </summary>
		/// <param name="ByteString64">A Base64 string to be decoded.</param>
		/// <returns>String with converted value.</returns>
		public static String Base64ToString(String ByteString64)
		{
			byte[] ByteString = Convert.FromBase64String(ByteString64);
			return (System.Text.Encoding.UTF8.GetString(ByteString));
		}

		/// <summary>
		/// Returns a list of System.IO.DriveInfo objects about the drives on the host computer.
		/// </summary>
		/// <returns>List of System.IO.DriveInfo objects</returns>
		public static List<DriveInfo> GetDrives()
		{
			DriveInfo[] results = DriveInfo.GetDrives();
			return new List<DriveInfo>(results);
		}

		/// <summary>
		/// Gets a list of System.Management.ManagementObject objects with info about network printers.
		/// </summary>
		/// <returns>List of System.Management.ManagementObject objects</returns>
		public static List<ManagementObject> GetNetworkPrinters()
		{

			List<ManagementObject> retVal = null;
			ObjectQuery objPrinterQuery = null;
			ManagementObjectSearcher objPrinterSearcher = null;
			ManagementObjectCollection objQueryCollection = null;

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					// This is the list to be returned, which will only be populated with 
					// printers that are network printers
					retVal = new List<ManagementObject>();

					// Use the ObjectQuery to get the list of configured printers via WMI
					objPrinterQuery = new ObjectQuery("SELECT * FROM Win32_Printer");

					objPrinterSearcher = new ManagementObjectSearcher(objPrinterQuery);

					// Fetch the list of all printers
					objQueryCollection = objPrinterSearcher.Get();

					// Iterate through the list and look for network printers
					foreach (ManagementObject objPrinter in objQueryCollection)
					{
						if ((bool)objPrinter.Properties["Network"].Value)
						{
							// This is a network printer, so add it to the list.
							retVal.Add(objPrinter);
						}
					}
				}
			}
			catch
			{
				if (retVal == null)
				{
					retVal = new List<ManagementObject>();
				}
			}
			finally
			{
				// Cleanup
				objPrinterQuery = null;

				if (objPrinterSearcher != null)
				{
					objPrinterSearcher.Dispose();
					objPrinterSearcher = null;
				}

				if (objQueryCollection != null)
				{
					objQueryCollection.Dispose();
					objQueryCollection = null;
				}
			}

			return retVal;
		}  // END public static List<ManagementObject> GetNetworkPrinters()

		/// <summary>
		/// Gets a list of network printers using one ManagementObject instance per printer
		/// in a List object.
		/// </summary>
		/// <returns>List of System.Management.ManagementObject objects</returns>
		public static List<ManagementObject> GetLocalPrinters()
		{

			List<ManagementObject> retVal = null;
			ObjectQuery objPrinterQuery = null;
			ManagementObjectSearcher objPrinterSearcher = null;
			ManagementObjectCollection objQueryCollection = null;

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					// This is the list to be returned, which will only be populated with 
					// printers that are network printers
					retVal = new List<ManagementObject>();

					// Use the ObjectQuery to get the list of configured printers via WMI
					objPrinterQuery = new ObjectQuery("SELECT * FROM Win32_Printer");

					objPrinterSearcher = new ManagementObjectSearcher(objPrinterQuery);

					// Fetch the list of all printers
					objQueryCollection = objPrinterSearcher.Get();

					// Iterate through the list and look for local printers
					foreach (ManagementObject objPrinter in objQueryCollection)
					{
						if ((bool)objPrinter.Properties["Local"].Value)
						{
							// This is a local printer, so add it to the list.
							retVal.Add(objPrinter);
						}

					}
				}
			}
			catch
			{
				if (retVal == null)
				{
					retVal = new List<ManagementObject>();
				}
			}
			finally
			{
				// Cleanup
				objPrinterQuery = null;

				if (objPrinterSearcher != null)
				{
					objPrinterSearcher.Dispose();
					objPrinterSearcher = null;
				}

				if (objQueryCollection != null)
				{
					objQueryCollection.Dispose();
					objQueryCollection = null;
				}
			}

			return retVal;
		}  // END public static List<ManagementObject> GetLocalPrinters()

		/// <summary>
		/// Helper method to set bit value of an Int on/off.
		/// </summary>
		/// <param name="intValue">The integer holding the bit which should be set on/off</param> 
		/// <param name="bitPlaceValue">The bit place value to set on/off. Ex. 32, 64, 0x10, 0x200, etc.</param>
		/// <param name="setBitOn">'True' will set the bit on; 'False' will turn it off.</param>
		public static void SetIntBitValue(ref Int32 intValue, Int32 bitPlaceValue, Boolean setBitOn)
		{
			if (setBitOn)
			{
				intValue |= bitPlaceValue;
			}
			else
			{
				intValue &= ~bitPlaceValue;
			}
		}

		/// <summary>
		/// Helper method to get the state of a bit value of an Int.
		/// </summary>
		/// <param name="intValue">The integer holding the bit which should be checked if it's on/off</param> 
		/// <param name="bitPlaceValue">The bit place value to check if it's on/off. Ex. 32, 64, 0x10, 0x200, etc.</param>
		/// <returns>'True' if the bit on; 'False' if it is off.</returns>
		public static Boolean GetIntBitValue(Int32 intValue, Int32 bitPlaceValue)
		{
			return (intValue & bitPlaceValue) == bitPlaceValue;
		}

		/// <summary>
		/// This gets the module, function, line number, and column number info in a String 
		/// This is useful when logging and creating exceptions to define exactly where something occurred.
		/// </summary>
		/// <returns>A string with stack information, module name, method name, line number, and column number.</returns>
		public static String GetStackInfo()
		{
			String retVal = "";

			try
			{
				StackTrace objTrace = new(true);
				StackFrame objFrame = objTrace.GetFrame(1);
				MethodBase objMethod = objFrame.GetMethod();
				MemberInfo objMember = objMethod.DeclaringType;

				retVal = objMethod.DeclaringType.FullName + "::" +
							objMethod.Name.Replace(".ctor", objMember.Name) + "() @ " +
							"Line [" + objFrame.GetFileLineNumber() + "], Column [" + objFrame.GetFileColumnNumber() + "]";

			}  // END try

			catch
			{
				retVal = "";
			}

			return retVal;

		}

		/// <summary>
		/// This gets the module, function, line number, and column number info in a String based on an exception.
		/// This is useful when logging and creating exceptions to define exactly where something occurred.
		/// </summary>
		/// <returns>A string with stack information, module name, method name, line number, and column number.</returns>
		public static String GetStackInfo(Exception ex)
		{
			String retVal = "";

			try
			{
				System.Diagnostics.StackTrace objTrace = new(ex, true);
				System.Diagnostics.StackFrame objFrame = objTrace.GetFrame(0);
				System.Reflection.MethodBase objMethod = objFrame.GetMethod();
				System.Reflection.MemberInfo objMember = objMethod.DeclaringType;

				retVal = objMethod.DeclaringType.FullName + "::" +
							objMethod.Name.Replace(".ctor", objMember.Name) + "() @ " +
							"Line [" + objFrame.GetFileLineNumber() + "], Column [" + objFrame.GetFileColumnNumber() + "]";

			}  // END try

			catch
			{
				retVal = "";
			}

			return retVal;

		}  // END public static String GetStackInfo(Exception ex)

		/// <summary>
		/// This returns a String with a consistent datetime format for a filename.
		/// </summary>
		/// <param name="dtmDate">Date to use for the date-time stamp.</param>
		/// <returns>String suitable for use in a file name.</returns>
		public static String GetFullDateTimeStampForFileName(DateTime dtmDate)
		{

			String retVal = "";

			retVal = dtmDate.ToString("o").Replace(":", ".");

			return retVal;

		}  // END private static String GetFullDateTimeStampForFileName(DateTime dtmDate)

		/// <summary> 
		/// Detect if a file is text and detects the encoding. 
		/// </summary> 
		/// <param name="lngEncoding">The detected encoding, as an out parameter.</param> 
		/// <param name="strFileName">The file name.</param> 
		/// <param name="lngNumCharactersToRead">The number in the file to use for testing. There is a minimum of 80 characters although if the file is smaller than the specified size, all characters will be used.</param> 
		/// <returns>true if the file is text.</returns> 
		public static Boolean IsFileText(out Encoding lngEncoding, String strFileName, Int32 lngNumCharactersToRead)
		{
			Boolean retValue = true;

			lngEncoding = Encoding.Default;

			if (lngNumCharactersToRead < 80)
			{
				lngNumCharactersToRead = 80;
			}

			try
			{

				using (var objFileStream = File.OpenRead(strFileName))
				{
					var varRawData = new byte[lngNumCharactersToRead];
					var varText = new char[lngNumCharactersToRead];

					// Read raw bytes 
					var varRawLength = objFileStream.Read(varRawData, 0, varRawData.Length);
					objFileStream.Seek(0, SeekOrigin.Begin);

					// Detect encoding correctly (from Rick Strahl's blog) 
					// http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader 
					if (varRawData[0] == 0xef && varRawData[1] == 0xbb && varRawData[2] == 0xbf)
					{
						lngEncoding = Encoding.UTF8;
					}
					else if (varRawData[0] == 0xfe && varRawData[1] == 0xff)
					{
						lngEncoding = Encoding.Unicode;
					}
					else if (varRawData[0] == 0 && varRawData[1] == 0 && varRawData[2] == 0xfe && varRawData[3] == 0xff)
					{
						lngEncoding = Encoding.UTF32;
					}
					else if (varRawData[0] == 0x2b && varRawData[1] == 0x2f && varRawData[2] == 0x76)
					{
						lngEncoding = Encoding.UTF7;
					}
					else
					{
						lngEncoding = Encoding.Default;
					}

					// Read text and detect the encoding 
					using (var objStreamReader = new StreamReader(objFileStream))
					{
						objStreamReader.Read(varText, 0, varText.Length);
					}

					using (var varMemoryStream = new MemoryStream())
					{
						using (var varStreamWriter = new StreamWriter(varMemoryStream, lngEncoding))
						{
							// Write the text to a buffer 
							varStreamWriter.Write(varText);
							varStreamWriter.Flush();

							// Get the buffer from the memory stream for comparison 
							var varMemoryBuffer = varMemoryStream.GetBuffer();

							// Compare only bytes read 
							for (var i = 0; i < varRawLength && retValue; i++)
							{
								retValue = varRawData[i] == varMemoryBuffer[i];
							}
						}  // END using (var varStreamWriter = new StreamWriter(varMemoryStream, lngEncoding))

					}  // END using (var varMemoryStream = new MemoryStream())

				}  // END using (var objFileStream = File.OpenRead(strFileName))

			} // END try

			catch
			{
				retValue = false;
			}  // END catch (Exception exUnhandled)

			return retValue;

		}

		/// <summary>
		/// Checks the specified drive for free disk space.
		/// </summary>
		/// <param name="pDriveName">The drive name.</param>
		/// <returns>Returns MB of free disk space</returns>
		public static Int32 GetTotalHDDFreeSpace(String pDriveName)
		{
			DriveInfo[] knownDrives = DriveInfo.GetDrives();

			Int32 retVal = 0;

			if (pDriveName.Length == 1)
			{
				pDriveName += @":\";
			}

			if (pDriveName.Length == 2)
			{
				pDriveName += @"\";
			}

			for (int i = 0; i < knownDrives.Length; i++)
			{

				try
				{
					if (knownDrives[i].IsReady && knownDrives[i].Name == pDriveName)
					{
						Int64 BytesLeft = knownDrives[i].TotalFreeSpace;

						try
						{
							retVal = Convert.ToInt32(BytesLeft / 1048576);
						}
						catch
						{
							retVal = 0;
						}
					}
				}  // END try

				catch
				{
					retVal = -1;
				}  // END catch (Exception exUnhandled)

			}
			return retVal;
		}

		/// <summary>
		/// Gets the total disk size of the specified drive.
		/// </summary>
		/// <param name="pDriveName">The drive name.</param>
		/// <returns>Returns MB of total space</returns>
		public static Int32 GetTotalHDDSize(String pDriveName)
		{
			Int32 retVal = -1;

			if (pDriveName.Length == 1)
			{
				pDriveName += @":\";
			}

			if (pDriveName.Length == 2)
			{
				pDriveName += @"\";
			}

			foreach (DriveInfo DriveObject in DriveInfo.GetDrives())
			{
				if (DriveObject.IsReady && DriveObject.Name.Equals(pDriveName, StringComparison.CurrentCultureIgnoreCase))
				{
					Int64 BytesTotal = DriveObject.TotalSize;

					retVal = Convert.ToInt32(BytesTotal / 1048576);

					break;
				}
			}

			return retVal;
		}  // END public static Int32 GetTotalHDDSize(String pDriveName)

		/// <summary>
		/// Returns the minimum password length from a domain, if one exists.
		/// If no domain exists, -1 is returned.
		/// </summary>
		/// <returns>-1 if no domain exists, or the minimum password length for the domain.</returns>
		public static Int32 GetMinPasswordLength()
		{
			Int32 minPwdLength = -1;

			try
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					Domain AgentCurrentDomain = null;

					try
					{
						AgentCurrentDomain = Domain.GetCurrentDomain();
					}
					catch
					{
						AgentCurrentDomain = null;

					}  // END catch (Exception exUnhandled)

					if (AgentCurrentDomain != null)
					{
						using (DirectoryEntry DirectoryEntry4Domain = AgentCurrentDomain.GetDirectoryEntry())
						{
							DirectorySearcher DirSrchr = new(DirectoryEntry4Domain, "(objectClass=*)", null, SearchScope.Base);

							SearchResult SrchRslt = DirSrchr.FindOne();

							minPwdLength = default;

							if (SrchRslt.Properties.Contains("minPwdLength"))
							{
								minPwdLength = (Int32)SrchRslt.Properties["minPwdLength"][0];
							}

						}
					}
				}
			} // END try

			catch
			{
				minPwdLength = -1;
			}  // END catch (Exception exUnhandled)

			return minPwdLength;

		}  // END public static Int32 GetMinPasswordLength()


		/// <summary>
		/// Checks to see if a string is an IPv4 or IPv6 address, and returns an IPAddress object as an out parameter.
		/// </summary>
		/// <param name="pValue">The string representation of the IP address.</param>
		/// <param name="pIPValue">Out parameter as a System.Net.IPAddress object. Null if not a valid IP address.</param>
		/// <returns>True if a valid IP address, false if not.</returns>
		public static Boolean IsIPAddress(String pValue, out IPAddress pIPValue)
		{

			DateTime dtmMethodStart = DateTime.Now;

			Boolean retVal = false;

			pIPValue = null;

			try
			{

				if (pValue == null)
				{
					retVal = false;
				}
				else
				{
					retVal = IPAddress.TryParse(pValue, out pIPValue);
				}  // END else [if (pValue == null)]

			} // END try

			catch
			{
				retVal = false;

			}  // END catch 

			return retVal;

		}  // END public static Boolean IsIPAddress(String strValue)

		/// <summary>
		/// Checks the .NET assembly for a title.
		/// </summary>
		/// <returns>Returns the title of the .NET assembly.</returns>
		public static String AssemblyTitle
		{
			get
			{
				String retVal = "";

				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						retVal = titleAttribute.Title;
					}
				}
				else
				{
					retVal = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
				}

				return retVal;
			}
		}

		/// <summary>
		/// Checks the .NET assembly for the version.
		/// </summary>
		/// <returns>Returns a string with the assembly version.</returns>
		public static String AssemblyVersion
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Version.ToString();
			}
		}

		/// <summary>
		/// Checks the .NET assembly for the description.
		/// </summary>
		/// <returns>Returns a string with the .NET assembly description.</returns>
		public static String AssemblyDescription
		{
			get
			{
				String retVal = "";
				
				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

				if (attributes.Length > 0)
				{
					retVal = ((AssemblyDescriptionAttribute)attributes[0]).Description;
				}
				else
				{
					retVal = Assembly.GetEntryAssembly().GetName().Name;
				}

				return retVal;
			}
		}

		/// <summary>
		/// Checks the .NET assembly for the product description.
		/// </summary>
		/// <return>Returns a string with the .NET asssembly product description.</return>
		public static String AssemblyProduct
		{
			get
			{
				String retVal = "";

				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

				if (attributes.Length > 0)
				{
					retVal = ((AssemblyProductAttribute)attributes[0]).Product;
				}

				return retVal;
			}
		}

		/// <summary>
		/// Checks the .NET assembly for the copyright description.
		/// </summary>
		/// <return>Returns a string with the .NET asssembly copyright description.</return>
		public static String AssemblyCopyright
		{
			get
			{
				String retVal = "";

				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

				if (attributes.Length > 0)
				{
					retVal = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
				}

				return retVal;
			}
		}

		/// <summary>
		/// Checks the .NET assembly for the company name.
		/// </summary>
		/// <return>Returns a string with the .NET asssembly company name.</return>
		public static String AssemblyCompany
		{
			get
			{
				String retVal = "";

				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

				if (attributes.Length > 0)
				{
					retVal = ((AssemblyCompanyAttribute)attributes[0]).Company;
				}

				return retVal;
			}
		}

		/// <summary>
		/// Takes an exception gets the stack of messages, stack trace, etc.
		/// </summary>
		/// <param name="pExceptionToUse">Exception instance to be examined.</param>
		/// <param name="LogMessage">Out string with the exception message(s), including inner exceptions.</param>
		/// <param name="ExceptionData">Out string with the exception's Data collection, including inner exceptions.</param>
		/// <param name="StackTraceDescrs">Out string with the outer exception's stack trace descriptions.</param>
		/// <param name="PreviousModule">Out string with the module name where the outer exception was thrown.</param>
		/// <param name="PreviousMethod">Out string with the method name where the outer exception was thrown.</param>
		/// <param name="PreviousLineNumber">Out Int32 with the line number where the outer exception was thrown</param>
		/// <param name="lngThreadID">Out Int32 with the .NET thread ID when the outer exception was thrown.</param>
		/// <returns>True if the information could be gathered, false otherwise.</returns>
		public static Boolean GetExceptionInfo(this Exception pExceptionToUse,
										out String LogMessage,
										out String ExceptionData,
										out String StackTraceDescrs,
										out String PreviousModule,
										out String PreviousMethod,
										out Int32 PreviousLineNumber,
										out Int32 lngThreadID)
		{
			Boolean retVal = false;

			LogMessage = "";
			ExceptionData = "";
			StackTraceDescrs = "";
			PreviousModule = "";
			PreviousMethod = "";
			PreviousLineNumber = 0;
			lngThreadID = 0;

			StackTrace PreviousStackTrace = null;

			StackFrame frame = null;

			try
			{

				lngThreadID = Environment.CurrentManagedThreadId;

				if (pExceptionToUse == null)
				{
					LogMessage = "Exception was null.  No message information can be obtained.";

					StackTraceDescrs = "Exception was null.  No stack information can be obtained.";

					ExceptionData = "Exception was null.  No data collection information can be obtained.";

				}
				else
				{
					LogMessage = GetExceptionMessages(pExceptionToUse);

					StackTraceDescrs = GetExceptionStackTrace(pExceptionToUse);

					ExceptionData = GetExceptionData(pExceptionToUse);

				}

				PreviousStackTrace = new StackTrace(true);

				if (PreviousStackTrace.FrameCount >= 2)
				{
					frame = PreviousStackTrace.GetFrame(1);
				}
				else
				{
					frame = PreviousStackTrace.GetFrame(0);
				}

				PreviousLineNumber = frame.GetFileLineNumber();

				PreviousModule = frame.GetMethod().ReflectedType.Name;

				// Get the name of the method in that module from where this exception was raised.
				PreviousMethod = frame.GetMethod().ToString();

				// If the method name is ".ctor", that is shorthand for a Constructor method.
				if (PreviousMethod == ".ctor")
				{
					PreviousMethod = "Default Constructor";
				}
				else if (PreviousMethod.Contains("Void .ctor"))
				{
					PreviousMethod = PreviousMethod.Replace("Void .ctor", "");
				}
				else
				{
					// No change needed
				}

				LogMessage = LogMessage.Replace(Environment.NewLine, " | ");

				StackTraceDescrs = StackTraceDescrs.Replace(Environment.NewLine, " | ");

				retVal = true;


			}  // END try
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("pExceptionToUse", pExceptionToUse == null ? "NULL" : pExceptionToUse.GetType().Name);
				exUnhandled.Data.AddCheck("LogMessage", LogMessage ?? "NULL");
				exUnhandled.Data.AddCheck("ExceptionData", ExceptionData ?? "NULL");
				exUnhandled.Data.AddCheck("StackTraceDescrs", StackTraceDescrs ?? "NULL");
				exUnhandled.Data.AddCheck("PreviousModule", PreviousModule ?? "NULL");
				exUnhandled.Data.AddCheck("PreviousMethod", PreviousMethod ?? "NULL");
				exUnhandled.Data.AddCheck("PreviousLineNumber", PreviousLineNumber.ToString());
				exUnhandled.Data.AddCheck("lngThreadID", lngThreadID.ToString());

				throw;
			}
			finally
			{
				PreviousStackTrace = null;

				frame = null;


			}


			return retVal;

		}  // END public static Boolean GetExceptionInfo(...)

		/// <summary>
		/// Method to get all the exception messages, generally when the outer exception has inner exceptions.
		/// </summary>
		/// <param name="ex2Examine">Outer parameter to examine.</param>
		/// <returns>String with the error messages.</returns>
		private static String GetExceptionMessages(Exception ex2Examine)
		{

			String retValue = "";
			String message = "";

			try
			{

				if (((ex2Examine != null)))
				{

					Exception nextException = ex2Examine;

					message = "";

					// We need to loop through all child exceptions to get all the messages.
					// For example, an exception caught when using a SqlClient may not
					// show a message that explains the problem.  There may be 1, 2, or even 3 
					// inner exceptions stacked up. The deepest will likely have the cause
					// of the failure in its message.  So it is a good practice to capture
					// all the messages, pulled from each instance.
					while (nextException != null)
					{

						message += nextException.GetType().Name + "=[" + (nextException.Message ?? "NULL") + "]";


						if (nextException.Source != null)
						{
							message += "; Source=[" + nextException.Source + "]";

						}

						if (nextException.InnerException == null)
						{
							break;
						}
						else
						{
							nextException = nextException.InnerException;
						}

						message += "::";

					}

				}

				retValue = message.Trim();

				if (retValue.EndsWith("::"))
				{
					retValue = retValue.Substring(0, retValue.Length - 2);
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("ex2Examine", ex2Examine == null ? "NULL" : ex2Examine.GetType().Name);
				throw;
			}

			return retValue;

		}  // END public static String GetExceptionMessages( ... )

		/// <summary>
		/// Gets the stack trace as a string.
		/// </summary>
		/// <param name="ex2Examine">Exception to examine.</param>
		/// <returns>Stack trace string without carriage return-line feeds.</returns>
		private static String GetExceptionStackTrace(Exception ex2Examine)
		{

			String retValue = "[NULL]"; ;

			try
			{

				if (((ex2Examine != null)))
				{
					// The stack trace is most complete at the top-level
					// exception.  If we are to include it, we grab it here.
					if (((ex2Examine.StackTrace != null)))
					{
						String traceString = ex2Examine.StackTrace.Trim().Replace(Environment.NewLine + "   ", "::");
						traceString = traceString.Replace(Environment.NewLine, "::");

						retValue = "[" + traceString + "]";
					}
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("ex2Examine", ex2Examine == null ? "NULL" : ex2Examine.GetType().Name);

				throw;

			}

			return retValue;

		}  // END public static String GetExceptionStackTrace(Exception ex2Examine)

		/// <summary>
		/// Iterates through all the exceptions (outer and inner) for the name-value pairs in each Exception's Data collection.
		/// </summary>
		/// <param name="ex2Examine">Outer exception to check.</param>
		/// <returns></returns>
		private static String GetExceptionData(Exception ex2Examine)
		{

			String retVal = "";
			String data = "";

			try
			{

				if (((ex2Examine != null)))
				{

					Exception nextException = ex2Examine;

					// We need to loop through all child exceptions to get all the Data collection 
					// name-value pairs. For example, an exception caught when using a SqlClient may not
					// show data that helps explains the problem.  There may be 1, 2, or even 3 
					// inner exceptions stacked up. The deepest will likely have data (if it has any) related 
					// to the failure in its data collection.  So it is a good practice to capture
					// all the data collection, pulled from each exception/inner exception.
					while (nextException != null)
					{

						data = "";

						// The Exception provides a Data collection of name-value
						// pairs.  This provides a means, at each method level from 
						// initiation up through the stack, to capture the runtime data
						// which helps diagnose the problem.
						if (nextException.Data.Count > 0)
						{
							foreach (DictionaryEntry item in nextException.Data)
							{
								data += "{" + item.Key.ToString() + "}={" + item.Value.ToString() + "}|";
							}

							data = data.Substring(0, data.Length - 1);
						}

						if ((data.Length > 0))
						{
							retVal += nextException.GetType().Name + " Data=[" + data + "]";
						}
						else
						{
							retVal += nextException.GetType().Name + " Data=[None]";
						}

						if (nextException.InnerException == null)
						{
							break;
						}
						else
						{
							nextException = nextException.InnerException;
						}

						retVal += "::";

					}

					retVal = retVal.Trim();

				}

				if (retVal.EndsWith("::"))
				{
					retVal = retVal.Substring(0, retVal.Length - 2);
				}

			}
			catch (Exception exUnhandled)
			{
				exUnhandled.Data.AddCheck("ex2Examine", ex2Examine == null ? "NULL" : ex2Examine.GetType().Name);

				throw;
			}

			return retVal;

		}  // END public static String GetExceptionData( ... )




		/// <summary>
		/// Method that performs a log write where the file is tab delimited and has a column header line.
		/// </summary>
		/// <param name="logFileName">Fully qualified file name.</param>
		/// <param name="mainMessage">Primary message to write.</param>
		/// <param name="secondMessage">Secondary message to write.</param>
		/// <returns></returns>
		public static Boolean WriteToLog(String logFileName, String mainMessage, String secondMessage)
		{
			Boolean retVal = false;

			StackTrace st = new(true);

			StackFrame frame = null;

			if (!File.Exists(logFileName))
			{
				WriteHeaderToLog(logFileName);
			}

			if (st.FrameCount >= 2)
			{
				frame = st.GetFrame(1);
			}
			else
			{
				frame = st.GetFrame(0);
			}

			MethodBase callingMethod = frame.GetMethod();
			String callingFilePath = frame.GetFileName();
			String callingFileName = Path.GetFileName(callingFilePath);
			Int32 callingFileLineNumber = frame.GetFileLineNumber();

			using (StreamWriter logfile = File.AppendText(logFileName))
			{
				String logDateTime = "";

				logDateTime = DateTime.Now.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

				logfile.WriteLine($"{logDateTime}\t{mainMessage}\t{secondMessage}\t{callingFileName}\t{callingMethod}\t{callingFileLineNumber.ToString()}");
				logfile.Flush();
				logfile.Close();

				retVal = true;
			}

			return retVal;

		}

		/// <summary>
		/// Creates the header line, the first line in the log.
		/// </summary>
		/// <returns>True if successful, false otherwise.</returns>
		private static Boolean WriteHeaderToLog(String logFileName)
		{
			Boolean retVal = false;

			using (StreamWriter logfile = File.AppendText(logFileName))
			{
				String logDateTime = "";

				logDateTime = "Date Time";

				logfile.WriteLine($"{logDateTime}\tMessage\tAddtl Info\tModule\tMethod\tLine No.");
				logfile.Flush();
				logfile.Close();

				retVal = true;
			}

			return retVal;

		}


	}  // END public static class CommonHelpers

}  // END namespace Jeff.Jones.JHelpers6
