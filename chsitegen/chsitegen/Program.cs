using System;
using System.IO;
using MarkdownSharp;
using System.Xml;
using System.Collections.Generic;

namespace chsitegen
{
    class MainClass
    {
        private bool settingsCreated;
        public static void Main(string[] args)
        {
            Console.WriteLine("Chloride Site Generator");
            Console.WriteLine();
            Console.Write("Checking for site configuration...");
            XmlDocument confd = new XmlDocument();

            if (!File.Exists("site.xml"))
            {
                Console.WriteLine(" Nonexistant");
                Console.WriteLine();
                StreamWriter confsw = File.CreateText("site.xml");
                settingsCreated = false;
                Console.Write("Friendly site name: ");
                AddXmlNode("SiteName", Console.ReadLine(), confd.DocumentElement);
                Console.Write("Website footer: ");
                AddXmlNode("SiteFooter", Console.ReadLine(), confd.DocumentElement);
                Console.WriteLine("--Available themes--");
                foreach (string d in Directory.GetDirectories("themes"))
                {
                    Console.WriteLine(d);
                }
                Console.Write("Select a theme: ");
                AddXmlNode("SiteTheme", Console.ReadLine(), confd.DocumentElement);
            }
            else
            {
                Console.WriteLine(" OK");
                StreamReader confsr = File.OpenText("site.xml");
                confd.LoadXml(confsr.ReadToEnd());
            }

            SortedDictionary<int, KeyValuePair<string, string>> pages = new Dictionary<int, KeyValuePair<string, string>>(); //Unconventional use but meh
            Console.Write("Counting pages...");
            foreach (string f in Directory.GetFiles("pages"))
            {
                string file = Path.GetFileNameWithoutExtension(f);
                int sortnum = Convert.ToInt32(file.Substring(0, 3));
                KeyValuePair<string, string> extendedkvp = new KeyValuePair<string, string>();
                extendedkvp.Key = file.Substring(3);
                extendedkvp.Value = extendedkvp.Key.ToLowerInvariant().Replace(" ","") + ".html";
                pages.Add(sortnum, extendedkvp);
            }
            Console.WriteLine(" " + pages.Count + " pages found");
        }

        private void AddXmlNode(string name, string content, XmlElement parentelem)
        {
            XmlNode xmn = parentelem.OwnerDocument.CreateNode(XmlNodeType.Element, name, "");
            xmn.InnerText = content;
            parentelem.AppendChild(xmn);
        }
    }
}
