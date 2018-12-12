using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Standard.Common.Utilities;

//using DotNet.Utilities;

namespace DotNet.Core.Demo.Utilities
{
    public static class CommonUtil
    {
        /// <summary>
        /// 获取某年某月的天数
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static int GetDays(int year, int month)
        {
            var days = 0;
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                days = 31;
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                days = 30;
            }
            else if (month == 2)
            {
                days = 28;
                if ((year % 400 == 0 && year % 3200 != 0)
                    || (year % 4 == 0 && year % 100 != 0)
                    || (year % 3200 == 0 && year % 172800 == 0))
                {
                    //闰年
                    days = 29;
                }
            }
            return days;
        }

        /// <summary>
        /// 获取类静态属性的Description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this object value)
        {
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
            {
                if (propertyInfo.PropertyType == value.GetType() && propertyInfo.GetValue(null, null).Equals(value))
                {
                    var dna = (DescriptionAttribute)propertyInfo.GetCustomAttribute(typeof(DescriptionAttribute));
                    if (dna != null && !string.IsNullOrEmpty(dna.Description))
                        return dna.Description;
                    return "";
                }
            }
            return "";
        }

        public static bool IsPhoneNumber(this string value)
        {
            return Regex.IsMatch(value, @"^(0|86|17951)?1[0-9]{10}$");
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool HasValue<T>(this T[] value)
        {
            return value != null && value.Length > 0;
        }

        public static bool HasBetween<T>(this T[] value)
        {
            return value != null && value.Length == 2;
        }

        /// <summary>
        /// datas区间是否在一个月内
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static bool IsFewMonth(this IDictionary<string, object> ps)
        {
            return IsFewMonth(ps, "dates");
        }

        /// <summary>
        /// key字段区间是否在一个月内
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsFewMonth(this IDictionary<string, object> ps, string key)
        {
            return IsFewMonth(ps, 1, key);
        }

        /// <summary>
        /// key字段区间是否在months月内
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="months"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsFewMonth(this IDictionary<string, object> ps, int months, string key)
        {
            var isdates = false;
            try
            {
                if (ps != null && ps.ContainsKey(key))
                {
                    var dates = ps[key].ToString().ToObject<string[]>();
                    if (dates.Length == 2)
                    {
                        DateTime date1, date2;
                        if (DateTime.TryParse(dates[0], out date1) && DateTime.TryParse(dates[1], out date2))
                        {
                            if (date1.AddMonths(months) <= date2)
                            {
                                isdates = true;
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                LogUtil.WriteLog("IsFewMonth", er);
            }
            return isdates;
        }

        public static bool ValueTransfer(this IDictionary<string, object> ps, string fromKey, string toKey)
        {
            if(ps != null && ps.ContainsKey(fromKey))
            {
                if(ps.ContainsKey(toKey))
                {
                    ps[toKey] = ps[fromKey];
                }
                else
                {
                    ps.Add(toKey, ps[fromKey]);
                }
                ps.Remove(fromKey);
                return true;
            }
            return false;
        }

        public static bool ValueTransfer(this IDictionary<string, string> ps, string fromKey, string toKey)
        {
            if (ps != null && ps.ContainsKey(fromKey))
            {
                if (ps.ContainsKey(toKey))
                {
                    ps[toKey] = ps[fromKey];
                }
                else
                {
                    ps.Add(toKey, ps[fromKey]);
                }
                ps.Remove(fromKey);
                return true;
            }
            return false;
        }

        public static IDictionary<string, object> SetValue(this IDictionary<string, object> ps, string key, object value)
        {
            if (ps != null)
            {
                if (ps.ContainsKey(key))
                {
                    ps[key] = value;
                }
                else
                {
                    ps.Add(key, value);
                }
                return ps;
            }
            return null;
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> ps, string key, out T value)
        {
            value = default(T);
            if (ps == null) return false;
            if (!ps.ContainsKey(key)) return false;
            try
            {
                value = (T)Convert.ChangeType(ps[key], typeof(T));
                return true;
            }
            catch (Exception)
            { }
            return false;
        }

        /*private static readonly HttpClient httpClient = new HttpClient
        {
            DefaultRequestHeaders = { { "User-Agent", "XFB/1.0.0" } },
            MaxResponseContentBufferSize = 256000,
            Timeout = new TimeSpan(0, 0, 2, 0)
        };*/

        public static async Task<string> HttpPostAsync(this Uri requestUri, object data)
        {
            /*var response = await httpClient.PostAsync(requestUri, new StringContent(data.ToJsonString(), Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();*/
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler) { DefaultRequestHeaders = { { "User-Agent", "XFB/1.0.0" } } })
            {
                http.MaxResponseContentBufferSize = 256000;
                http.Timeout = new TimeSpan(0, 0, 2, 0);
                var response = await http.PostAsync(requestUri, new StringContent(data.ToJsonString(), Encoding.UTF8, "application/json"));
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> HttpPostAsync(this Uri requestUri, IDictionary<string, string> headers,
            object data)
        {
            var handler = new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip};
            using (var http = new HttpClient(handler) {DefaultRequestHeaders = {{"User-Agent", "XFB/1.0.0"}}})
            {
                //http.MaxResponseContentBufferSize = 256000;
                //http.Timeout = new TimeSpan(0, 0, 2, 0);
                foreach (var key in headers.Keys)
                {
                    http.DefaultRequestHeaders.Remove(key);
                    http.DefaultRequestHeaders.Add(key, headers[key]);
                }
                var response = await http.PostAsync(requestUri, new StringContent(data.ToJsonString(), Encoding.UTF8, "application/json"));
                //LogUtil.WriteLog("HttpPostAsync Response", response.StatusCode.ToString());
                return await response.Content.ReadAsStringAsync();
            }
        }

        public static string HttpPost(this Uri requestUri, IDictionary<string, string> headers, object data)
        {
            var handler = new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip};
            using (var http = new HttpClient(handler) {DefaultRequestHeaders = {{"User-Agent", "XFB/1.0.0"}}})
            {
                //http.MaxResponseContentBufferSize = 256000;
                //http.Timeout = new TimeSpan(0, 0, 2, 0);
                foreach (var key in headers.Keys)
                {
                    http.DefaultRequestHeaders.Remove(key);
                    http.DefaultRequestHeaders.Add(key, headers[key]);
                }
                var response = http.PostAsync(requestUri, new StringContent(data.ToJsonString(), Encoding.UTF8, "application/json"));
                //LogUtil.WriteLog("http response", response.Result.StatusCode.ToString());
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        public static string HttpPost(this Uri requestUri, object data)
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler) { DefaultRequestHeaders = { { "User-Agent", "XFB/1.0.0" } } })
            {
                var response = http.PostAsync(requestUri, new StringContent(data.ToJsonString(), Encoding.UTF8, "application/json"));
                //LogUtil.WriteLog("http response", response.Result.StatusCode.ToString());
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        private const string PassWordKey = "__jetech";

        /// <summary>
        ///     加密帐号口令
        /// </summary>
        /// <param name="wordString"></param>
        /// <returns></returns>
        public static string DesEncryptPassWord(string wordString)
        {
            return DesEncrypt(wordString, PassWordKey);
        }

        /// <summary>
        ///     解密帐号口令
        /// </summary>
        /// <param name="encryptWordString"></param>
        /// <returns></returns>
        public static string DecryptPassWord(string encryptWordString)
        {
            return DesDecrypt(encryptWordString, PassWordKey);
        }

        #region 加密过程

        /// <summary>
        ///     DEC 加密过程
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <param name="encKey"></param>
        /// <returns></returns>
        public static string DesEncrypt(string strToEncrypt, string encKey)
        {
            var des = new DESCryptoServiceProvider(); //把字符串放到byte数组中

            var inputByteArray = Encoding.Default.GetBytes(strToEncrypt);

            des.Key = Encoding.ASCII.GetBytes(encKey); //建立加密对象的密钥和偏移量
            des.IV = Encoding.ASCII.GetBytes(encKey); //原文使用ASCIIEncoding.ASCII方法的GetBytes方法
            var ms = new MemoryStream(); //使得输入密码必须输入英文文本
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region 解密过程

        /// <summary>
        ///     DEC 解密过程
        /// </summary>
        /// <param name="strToDecrypt"></param>
        /// <param name="encKey"></param>
        /// <returns></returns>
        public static string DesDecrypt(string strToDecrypt, string encKey)
        {
            var ms = new MemoryStream();
            try
            {
                var des = new DESCryptoServiceProvider();

                var inputByteArray = new byte[strToDecrypt.Length / 2];
                for (var x = 0; x < strToDecrypt.Length / 2; x++)
                {
                    var i = Convert.ToInt32(strToDecrypt.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }

                des.Key = Encoding.ASCII.GetBytes(encKey); //建立加密对象的密钥和偏移量，此值重要，不能修改
                des.IV = Encoding.ASCII.GetBytes(encKey);

                var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
            }
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion
    }
}
