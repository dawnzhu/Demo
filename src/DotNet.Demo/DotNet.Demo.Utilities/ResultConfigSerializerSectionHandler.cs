using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace DotNet.Demo.Utilities
{
    public class ResultConfigSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            IEnumerable<XmlNode> elements;
            try
            {
                elements = section.ChildNodes.Cast<XmlNode>();
            }
            catch (Exception)
            {
                return new List<ResultConfig>();
            }
            var list = (from property in elements
                        where property.Name == "action"
                        select new ResultConfig
                        {
                            Name = ((XmlElement)property).GetAttribute("name"),
                            Expiry = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("expiry")) ? "" : ((XmlElement)property).GetAttribute("expiry"),
                            PropertyNames = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("properties")) || ((XmlElement)property).GetAttribute("properties") == "*" 
                            ? null 
                            : ((XmlElement)property).GetAttribute("properties").Split(',', '，', ' ').Where(pn => pn.Length > 0).ToArray(),
                            IgnoreNames = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("ignores")) 
                            ? null 
                            : ((XmlElement)property).GetAttribute("ignores").Split(',', '，', ' ').Where(pn => pn.Length > 0).ToArray(),
                            CacheTime = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("cachetime")) 
                            ? 0 
                            : Convert.ToInt32(((XmlElement)property).GetAttribute("cachetime"))
                        }).GroupBy(p => p.Name).Select(g => g.First()).ToList();
            return list;
        }
    }

    public class ResultConfigsSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var retlist = new List<ResultConfig>();
            IEnumerable<XmlNode> configs;
            try
            {
                configs = section.ChildNodes.Cast<XmlNode>();
            }
            catch (Exception)
            {
                return retlist;
            }
            foreach (var config in configs)
            {
                IEnumerable<XmlNode> classes;
                try
                {
                    var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ((XmlElement) config).GetAttribute("configSource");
                    var doc = new XmlDocument();
                    doc.Load(path);
                    classes = doc.SelectSingleNode(config.Name).ChildNodes.Cast<XmlNode>();
                }
                catch (Exception)
                {
                    continue;
                }
                var list = (from property in classes
                            where property.Name == "action"
                            select new ResultConfig
                            {
                                Name = ((XmlElement)property).GetAttribute("name"),
                                Expiry = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("expiry")) ? "" : ((XmlElement)property).GetAttribute("expiry"),
                                PropertyNames = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("properties")) || ((XmlElement)property).GetAttribute("properties") == "*"
                                ? null
                                : ((XmlElement)property).GetAttribute("properties").Split(',', '，', ' ').Where(pn => pn.Length > 0).ToArray(),
                                IgnoreNames = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("ignores"))
                                ? null
                                : ((XmlElement)property).GetAttribute("ignores").Split(',', '，', ' ').Where(pn => pn.Length > 0).ToArray(),
                                CacheTime = string.IsNullOrEmpty(((XmlElement)property).GetAttribute("cachetime"))
                                ? 0
                                : Convert.ToInt32(((XmlElement)property).GetAttribute("cachetime"))
                            }).GroupBy(p => p.Name).Select(g => g.First()).ToList();
                retlist.AddRange(list);
            }
            return retlist;
        }
    }

    public class ResultConfig
    {
        public string Name { get; set; }

        public string Expiry { get; set; }

        public string[] PropertyNames { get; set; }

        public string[] IgnoreNames { get; set; }

        public int CacheTime { get; set; }
    }
}
