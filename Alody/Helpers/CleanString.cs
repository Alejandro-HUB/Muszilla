using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Alody.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;
using Alody.Helpers;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace Alody.Helpers
{
    public class CleanString
    {
        //by MSDN http://msdn.microsoft.com/en-us/library/844skk0h(v=vs.71).aspx
        public static string UseRegex(string strIn)
        {
            // Replace invalid characters with empty strings.
            return Regex.Replace(strIn, @"[^\w\.@-]", "");
        }

        // by Paolo Tedesco
        public static String UseStringBuilder(string strIn)
        {
            const string removeChars = " ?&^$#@!()+-,:;<>’\'-_*";
            // specify capacity of StringBuilder to avoid resizing
            StringBuilder sb = new StringBuilder(strIn.Length);
            foreach (char x in strIn.Where(c => !removeChars.Contains(c)))
            {
                sb.Append(x);
            }
            return sb.ToString();
        }

        // by Paolo Tedesco, but using a HashSet
        public static String UseStringBuilderWithHashSet(string strIn)
        {
            var hashSet = new HashSet<char>("?&^$#@!()+-,:;<>’\'-_*");
            // specify capacity of StringBuilder to avoid resizing
            StringBuilder sb = new StringBuilder(strIn.Length);
            foreach (char x in strIn.Where(c => !hashSet.Contains(c)))
            {
                sb.Append(x);
            }
            return sb.ToString();
        }

        public static string EscapeForeignCharacters(string s)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in s)
            {
                int i = (int)c;
                if (i < 32 || i > 126)
                {
                    sb.AppendFormat("", i);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        // by SteveDog
        public static string UseStringBuilderWithHashSet2(string dirtyString)
        {
            HashSet<char> removeChars = new HashSet<char>(" ?&^$#@!()+-,:;<>’\'-_*");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
                if (removeChars.Contains(c))
                    result.Append(c);
            return result.ToString();
        }

        // original by patel.milanb
        public static string UseReplace(string dirtyString)
        {
            string removeChars = " ?&^$#@!()+-,:;<>’\'-_*";
            string result = dirtyString;

            foreach (char c in removeChars)
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            return result;
        }

        // by L.B
        public static string UseWhere(string dirtyString)
        {
            return new String(dirtyString.Where(Char.IsLetterOrDigit).ToArray());
        }

        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 127;

            return input.Any(c => c > MaxAnsiCode);
        }

        public static bool IsValidFilename(string fileName)
        {
            return Path.GetInvalidFileNameChars().All(invalidChar => fileName.Contains(invalidChar) != true);
        }
    }
}
