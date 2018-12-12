using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace DotNet.Demo.Utilities
{
    /*public sealed class CustomerJsonValueProviderFactory : ValueProviderFactory
    {
        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
        {
            IDictionary<string, object> d = value as IDictionary<string, object>;
            if (d != null)
            {
                foreach (KeyValuePair<string, object> entry in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
                }
                return;
            }

            IList l = value as IList;
            if (l != null)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                }
                return;
            }

            // primitive
            backingStore.Add(prefix, value);
        }

        private static object GetDeserializedObject(ControllerContext controllerContext)
        {
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // not JSON request
                return null;
            }

            StreamReader reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            string bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText))
            {
                // no JSON data
                return null;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer{MaxJsonLength = Int32.MaxValue};
            object jsonData = serializer.DeserializeObject(bodyText);
            return jsonData;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            object jsonData = GetDeserializedObject(controllerContext);
            if (jsonData == null)
            {
                return null;
            }

            Dictionary<string, object> backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            EntryLimitedDictionary backingStoreWrapper = new EntryLimitedDictionary(backingStore);
            AddToBackingStore(backingStoreWrapper, String.Empty, jsonData);
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }

        private class EntryLimitedDictionary
        {
            private static int _maximumDepth = GetMaximumDepth();
            private readonly IDictionary<string, object> _innerDictionary;
            private int _itemCount = 0;

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                _innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                if (++_itemCount > _maximumDepth)
                {
                    throw new InvalidOperationException();
                }

                _innerDictionary.Add(key, value);
            }

            private static int GetMaximumDepth()
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                {
                    string[] valueArray = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                    if (HasValue(valueArray))
                    {
                        int result;
                        if (Int32.TryParse(valueArray[0], out result))
                        {
                            return result;
                        }
                    }
                }

                return 1000; // Fallback default
            }
        }
    }*/

    public class CustomerJsonValueProviderFactory : ValueProviderFactory
    {
        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, JToken value)
        {
            if (value.Type == JTokenType.Null)
            {
                return;
            }
            var d = value as JObject;
            if (d != null)
            {
                foreach (var v in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, v.Key), v.Value);
                }
                return;
            }
            var l = value as JArray;
            if (l != null)
            {
                if (l.Count == 0)
                {
                    //解决空数组被解析成null的问题
                    backingStore.Add(prefix, new object[0]);
                }
                else
                {
                    for (var i = 0; i < l.Count; i++)
                    {
                        AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                    }
                    /*var i = 0;
                    foreach (var v in l)
                    {
                        AddToBackingStore(backingStore, MakeArrayKey(prefix, i), v);
                        i++;
                    }*/
                }
                return;
            }
            backingStore.Add(prefix, value.ToString());
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null) throw new ArgumentNullException(nameof(controllerContext));
            if (!controllerContext.HttpContext.Request.ContentType.
                StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            var bodyText = reader.ReadToEnd();
            if (string.IsNullOrEmpty(bodyText))
            {
                // no JSON data
                return null;
            }
            var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var backingStoreWrapper = new EntryLimitedDictionary(backingStore);
            AddToBackingStore(backingStoreWrapper, string.Empty,
                bodyText.StartsWith("[")
                    ? JArray.Parse(bodyText) as JContainer
                    : JObject.Parse(bodyText));
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (string.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }

        private class EntryLimitedDictionary
        {
            private static readonly int MaximumDepth = GetMaximumDepth();
            private readonly IDictionary<string, object> _innerDictionary;
            private int _itemCount;

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                _innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                if (++_itemCount > MaximumDepth)
                {
                    throw new InvalidOperationException();
                }
                _innerDictionary.Add(key, value);
            }

            private static int GetMaximumDepth()
            {
                var appSettings = ConfigurationManager.AppSettings;
                var valueArray = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                if (valueArray != null && valueArray.Length > 0)
                {
                    int result;
                    if (int.TryParse(valueArray[0], out result))
                    {
                        return result;
                    }
                }
                return 1000; // Fallback default
            }
        }
    }
}