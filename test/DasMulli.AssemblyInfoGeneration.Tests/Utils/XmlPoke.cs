using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
#pragma warning disable 168

namespace DasMulli.AssemblyInfoGeneration.Tests.Utils
{
    internal class XmlPoke
    {
        private string XmlInputPath { get; }

        private string Query { get; }

        private string Value { get; }

        /// <summary>
        /// The namespaces for XPath query's prefixes.
        /// </summary>
        private string Namespaces { get; }

        public XmlPoke(string xmlInputPath, string query, string value, string namespaces)
        {
            XmlInputPath = xmlInputPath ?? throw new ArgumentNullException(nameof(xmlInputPath));
            Query = query ?? throw new ArgumentNullException(nameof(query));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Namespaces = namespaces;
        }
        /// <summary>
        /// Executes the XMLPoke task.
        /// </summary>
        /// <returns>true if transformation succeeds.</returns>
        public bool Execute()
        {
            // Load the XPath Document
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                using (FileStream fs = new FileStream(XmlInputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlReaderSettings xrs = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };
                    using (XmlReader sr = XmlReader.Create(fs, xrs))
                    {
                        xmlDoc.Load(sr);
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            XPathNavigator nav = xmlDoc.CreateNavigator();
            XPathExpression expr;

            try
            {
                // Create the expression from query
                expr = nav.Compile(Query);
            }
            catch (Exception e)
            {
                return false;
            }

            // Create the namespace manager and parse the input.
            var xmlNamespaceManager = new XmlNamespaceManager(nav.NameTable);

            // Arguments parameters
            try
            {
                LoadNamespaces(ref xmlNamespaceManager, Namespaces);
            }
            catch (Exception e)
            {
                return false;
            }

            try
            {
                expr.SetContext(xmlNamespaceManager);
            }
            catch (XPathException e)
            {
                return false;
            }

            XPathNodeIterator iter = nav.Select(expr);

            while (iter.MoveNext())
            {
                try
                {
                    iter.Current.InnerXml = Value;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            
            if (iter.Count > 0)
            {
#if RUNTIME_TYPE_NETCORE
                using (Stream stream = File.Create(_xmlInputPath.ItemSpec))
                {
                    xmlDoc.Save(stream);
                }
#else
                xmlDoc.Save(XmlInputPath);
#endif
            }

            return true;
        }

        /// <summary>
        /// Loads the namespaces specified at Namespaces parameter to XmlNSManager.
        /// </summary>
        /// <param name="namespaceManager">The namespace manager to load namespaces to.</param>
        /// <param name="namepaces">The namespaces as XML snippet.</param>
        private static void LoadNamespaces(ref XmlNamespaceManager namespaceManager, string namepaces)
        {
            var doc = new XmlDocument();
            try
            {
                var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };
                using (XmlReader reader = XmlReader.Create(new StringReader("<Namespaces>" + namepaces + "</Namespaces>"), settings))
                {
                    doc.Load(reader);
                }
            }
            catch (XmlException xe)
            {
                throw new ArgumentException($"The specified Namespaces attribute is not a well-formed XML fragment", xe);
            }

            XmlNodeList xnl = doc.SelectNodes("/Namespaces/*[local-name() = 'Namespace']");

            for (int i = 0; i < xnl?.Count; i++)
            {
                XmlNode xn = xnl[i];

                const string prefixAttr = "Prefix";
                XmlAttribute prefix = xn.Attributes?[prefixAttr];
                if (prefix == null)
                {
                    throw new ArgumentException($@"The specified Namespaces attribute does not have attribute ""{ prefixAttr }"".");
                }

                const string uriAttr = "Uri";
                XmlAttribute uri = xn.Attributes[uriAttr];
                if (uri == null)
                {
                    throw new ArgumentException($@"The specified Namespaces attribute doesn't have attribute ""{ uriAttr }"".");
                }

                namespaceManager.AddNamespace(prefix.Value, uri.Value);
            }
        }
    }
}
