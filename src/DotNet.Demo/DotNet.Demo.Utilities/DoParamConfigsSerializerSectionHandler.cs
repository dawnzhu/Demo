using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Demo.Utilities
{
    public class DoParamConfigsSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var dic = new Dictionary<string, ParamConfig>();
            IEnumerable<XmlNode> configs;
            try
            {
                configs = section.ChildNodes.Cast<XmlNode>();
            }
            catch (Exception)
            {
                return new List<ParamConfig>();
            }
            foreach (var config in configs)
            {
                IEnumerable<XmlNode> classes;
                try
                {
                    var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ((XmlElement)config).GetAttribute("configSource");
                    var doc = new XmlDocument();
                    doc.Load(path);
                    classes = doc.SelectSingleNode(config.Name).ChildNodes.Cast<XmlNode>(); //config.ChildNodes.Cast<XmlNode>();
                }
                catch (Exception)
                {
                    continue;
                }
                foreach (var c in classes)
                {
                    if (c.Name != "class") continue;
                    foreach (var m in c.ChildNodes.Cast<XmlNode>())
                    {
                        if (m.Name != "method") continue;
                        var pc = new ParamConfig
                        {
                            Params = new Dictionary<string, ParamInfo>(),
                            GroupParams = new Dictionary<string, ParamInfo>(),
                            Sorts = new Dictionary<string, ParamInfo>()
                        };
                        foreach (var p in m.ChildNodes.Cast<XmlNode>())
                        {
                            switch (p.Name)
                            {
                                case "param":
                                    pc.Params.Add(((XmlElement)p).GetAttribute("key"), new ParamInfo
                                    {
                                        Name = ((XmlElement)p).GetAttribute("name"),
                                        TypeString = ((XmlElement)p).GetAttribute("type"),
                                        Symbol = ((XmlElement)p).GetAttribute("symbol")
                                    });
                                    break;
                                case "groupParam":
                                    pc.GroupParams.Add(((XmlElement)p).GetAttribute("key"), new ParamInfo
                                    {
                                        Name = ((XmlElement)p).GetAttribute("name"),
                                        TypeString = (((XmlElement)p).GetAttribute("type")),
                                        Symbol = ((XmlElement)p).GetAttribute("symbol")
                                    });
                                    break;
                                case "sort":
                                    var key = ((XmlElement)p).GetAttribute("key");
                                    if (string.IsNullOrEmpty(key))
                                        key = Guid.NewGuid().ToString();
                                    pc.Sorts.Add(key, new ParamInfo
                                    {
                                        Name = ((XmlElement)p).GetAttribute("name"),
                                        Symbol = ((XmlElement)p).GetAttribute("symbol")
                                    });
                                    break;
                            }
                        }
                        var dickey = ((XmlElement)c).GetAttribute("key") + "." + ((XmlElement)m).GetAttribute("key");
                        if (!dic.ContainsKey(dickey))
                        {
                            dic.Add(dickey, pc);
                        }
                    }
                }
            }
            return dic;
        }
    }
}
