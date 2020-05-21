using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace StephenZeng.Prototypes.DisasterRecovery.Common
{
    public static class Extensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static IEnumerable<T> GetAttributes<T>(this Enum enumerationValue) where T : Attribute
        {
            return (T[])enumerationValue?.GetType().GetField(enumerationValue.ToString())
                .GetCustomAttributes(typeof(T), false);
        }

        public static string GetDescription(this Enum enumerationValue)
        {
            if (enumerationValue == null)
                return string.Empty;

            var attributes = enumerationValue.GetAttributes<DescriptionAttribute>().ToArray();
            return attributes.Length > 0 ? attributes[0].Description : enumerationValue.ToString();
        }

        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }

        public static string ToMd5Hash(this string input)
        {
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (var d in data)
                {
                    sb.Append(d.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static T DeserializeFromXml<T>(this Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(stream);
        }

        public static string SerializeToXml<T>(this T t)
        {
            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var xw = XmlWriter.Create(sb))
            {
                xw.WriteProcessingInstruction("xml", "version='1.0'");
                xmlSerializer.Serialize(xw, t);
                return sb.ToString();
            }
        }

        public static string ToMaskedMobileNumber(this string mobile)
        {
            if (mobile == null)
                return mobile;

            return string.Concat(
                    "".PadLeft(7, '*'),
                    mobile.Substring(mobile.Length >= 3 ? mobile.Length - 3 : 0)
                );
        }

        public static string ToMaskedEmail(this string email)
        {
            if (email == null)
                return string.Empty;

            var split = email.Split('@');
            if (split.Length < 2)
                return email;

            string s;
            if (split[0].Length <= 2)
                s = "";
            else
                s = $"{split[0][0]}{split[0][1]}";

            return string.Concat(
                    s.PadRight(10, '*'),
                    "@",
                    split[1]
                );
        }
    }
}
