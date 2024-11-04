using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary
{
    public static class EnumExtensions
    {
        public static bool TryParse(Type enumType, string value, bool ignoreCase, out Enum result)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException("Not an enum type.", nameof(enumType));

            MethodInfo baseMethod = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == nameof(Enum.TryParse))
                .First(m => m.GetParameters().Length == 3);

            MethodInfo realMethod = baseMethod.MakeGenericMethod(enumType);
            object[] parameters = new object[] { value, ignoreCase, null };
            bool parsed = (bool)realMethod.Invoke(null, parameters);
            result = parsed ? (Enum)parameters[2] : default;
            return parsed;
        }

        public static bool TryParse(Type enumType, string value, out Enum result)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException("Not an enum type.", nameof(enumType));

            MethodInfo baseMethod = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == nameof(Enum.TryParse))
                .First(m => m.GetParameters().Length == 2);

            MethodInfo realMethod = baseMethod.MakeGenericMethod(enumType);
            object[] parameters = new object[] { value, null };
            bool parsed = (bool)realMethod.Invoke(null, parameters);
            result = parsed ? (Enum)parameters[1] : default;
            return parsed;
        }
    }

    public static class Helpers
    {
        public static string GetNextSequenceNumber(string currentSequenceNumber)
        {
            int numericFieldLength;
            long? enumerator = null;

            int? numericIndex = GetNumericIndex(currentSequenceNumber, out numericFieldLength);
            if (numericIndex == null)
                return string.Empty;
            else
            {
                enumerator = Int64.Parse(currentSequenceNumber.Substring(numericIndex.Value, currentSequenceNumber.Length - numericIndex.Value));
                string valueToFillStringOnly = currentSequenceNumber.Substring(0, currentSequenceNumber.ToString().Length - numericFieldLength);

                enumerator += 1;
                return AppendStringWithEnumerator(valueToFillStringOnly, (long)enumerator, numericFieldLength);
            }
        }

        /// <summary>
        /// Separate parts of string to alphabets and enumerated numbers starting from the end
        /// </summary>
        /// <param name="stringToExtractNumbers">the entire numbering string</param>
        /// <param name="numericFieldLength">length calculated from the end of the string indicating where enumeration happens</param>
        /// <returns></returns>
        public static int? GetNumericIndex(string stringToExtractNumbers, out int numericFieldLength)
        {
            //string pattern = @"\d+$";
            //Regex rgx = new Regex(pattern);
            //var matches = rgx.Match(stringToExtractNumbers);
            //if (matches.Value == string.Empty)
            //    return null;

            numericFieldLength = 0;
            if (stringToExtractNumbers == null || stringToExtractNumbers == string.Empty)
                return null;

            var stack = new Stack<char>();
            int? returnValue = null;
            bool isLeadingZeros = false;
            for (var i = stringToExtractNumbers.Length - 1; i >= 0; i--)
            {
                char extractChar = stringToExtractNumbers[i];
                if (!char.IsNumber(extractChar))
                    return returnValue;

                numericFieldLength += 1;
                if (extractChar == '0' && isLeadingZeros)
                    continue;
                else
                {
                    //any zeros from here onwards are classified as leading zeros
                    isLeadingZeros = true;
                    returnValue = i;
                }
            }

            return returnValue;
        }


        /// <summary>
        /// Extract parts of the enumerated string and append it back with the same amount of length allocated to the string from the right
        /// </summary>
        /// <param name="stringPortionOnly">string portion to append</param>
        /// <param name="enumerator">current numeric value to append to string</param>
        /// <param name="numericFieldLength">portion of string allocated for enumeration from the right</param>
        /// <returns></returns>
        public static string AppendStringWithEnumerator(string stringPortionOnly, long enumerator, int numericFieldLength)
        {
            string enumeratorString = enumerator.ToString();
            int numberOfLeadingZeros = numericFieldLength - enumeratorString.Length;
            for (int i = 0; i < numberOfLeadingZeros; i++)
            {
                stringPortionOnly += "0";
            }
            return stringPortionOnly += enumeratorString;
        }
    }
}
