using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using DotNet.Standard.NSmart.Utilities;

namespace DotNet.Demo.Utilities
{
    public class DoConfigsSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var dbConfigs = new Dictionary<string, DoConfigDbs>();
            IEnumerable<XmlNode> xmlNodes;
            try
            {
                xmlNodes = section.ChildNodes.Cast<XmlNode>();
            }
            catch (Exception)
            {
                return new Dictionary<string, DoConfigDbs>();
            }
            foreach (var node in xmlNodes)
            {
                if (node.Name != "dbs") continue;
                var element = (XmlElement) node;
                var model = new DoConfigDbs
                {
                    ConnectionString = element.GetAttribute("connectionString"),
                    ProviderName = element.GetAttribute("providerName"),
                    Adds = new List<DoConfigDb>()
                };
                var name = element.GetAttribute("name");
                foreach (var xmlNode in node.ChildNodes.Cast<XmlNode>())
                {
                    if(xmlNode.Name != "add") continue;
                    element = (XmlElement)xmlNode;
                    if (!element.HasAttribute("connectionString")
                        && (!element.HasAttribute("readConnectionString") || !element.HasAttribute("writeConnectionString")))
                    {
                        continue;
                    }
                    if (!element.HasAttribute("providerName"))
                    {
                        continue;
                    }
                    var doConfigDb = new DoConfigDb();
                    if (element.HasAttribute("connectionString"))
                    {
                        doConfigDb.ReadConnectionString = element.GetAttribute("connectionString");
                        doConfigDb.WriteConnectionString = doConfigDb.ReadConnectionString;
                    }
                    else
                    {
                        doConfigDb.ReadConnectionString = element.GetAttribute("readConnectionString");
                        doConfigDb.WriteConnectionString = element.GetAttribute("writeConnectionString");
                    }
                    doConfigDb.Name = element.GetAttribute("name");
                    doConfigDb.ProviderName = element.GetAttribute("providerName");
                    model.Adds.Add(doConfigDb);
                }
                dbConfigs.Add(name, model);
            }
            return dbConfigs;
        }
    }
}
