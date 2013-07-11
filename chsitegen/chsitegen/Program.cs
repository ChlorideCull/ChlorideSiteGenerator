using System;
using System.IO;
using MarkdownSharp;
using System.Xml;
using System.Collections.Generic;

namespace chsitegen
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Chloride Site Generator");
            Console.WriteLine();
            Console.Write("Checking for site configuration...");
            XmlDocument confd = new XmlDocument();
            confd.InnerXml = "<SiteSettings></SiteSettings>";
            MarkdownOptions mdo = new MarkdownOptions();
            mdo.AutoHyperlink = true;
            mdo.AutoNewlines = true;
            mdo.EncodeProblemUrlCharacters = true;
            mdo.LinkEmails = true;
            Markdown md = new Markdown(mdo);

            if (!File.Exists("site.xml"))
            {
                Console.WriteLine(" Nonexistant");
                Console.WriteLine();
                StreamWriter confsw = File.CreateText("site.xml");
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
                confd.Save(confsw.BaseStream);
            }
            else
            {
                Console.WriteLine(" OK");
                StreamReader confsr = File.OpenText("site.xml");
                confd.LoadXml(confsr.ReadToEnd());
            }

            SortedDictionary<int, string[]> pages = new SortedDictionary<int, string[]>(); //Unconventional use but meh
            Console.Write("Counting pages...");
            foreach (string f in Directory.GetFiles("pages"))
            {
                string file = Path.GetFileNameWithoutExtension(f);
                int sortnum = Convert.ToInt32(file.Substring(0, 3));
                string[] values = new string[] { file.Substring(3), file.Substring(3).ToLowerInvariant().Replace(" ","") + ".html", f };
                pages.Add(sortnum, values);
            }
            Console.WriteLine(" " + pages.Count + " pages found");
            Console.Write("Generating pages...");
            if (Directory.Exists("html"))
            {
                Directory.Delete("html", true);
            }
            Directory.CreateDirectory("html");

            string SiteTitle = confd.DocumentElement["SiteName"].InnerText;
            Console.WriteLine(SiteTitle);
            string SiteFooter = confd.DocumentElement["SiteFooter"].InnerText;
            Console.WriteLine(SiteFooter);
            string SiteTheme = confd.DocumentElement["SiteTheme"].InnerText;
            Console.WriteLine(SiteTheme);
            string TemplateHTML = "";

            foreach (string f in Directory.GetFiles("themes/" + SiteTheme + "/"))
            {
                if (Path.GetFileName(f) == "index.html")
                {
                    TemplateHTML = File.OpenText(f).ReadToEnd();
                }
                else
                {
                    File.Copy(f, "html/" + Path.GetFileName(f), true);
                }
                Console.ReadKey();
            }

            foreach (KeyValuePair<int, string[]> kvp in pages)
            {
                string TemporaryHTML = TemplateHTML;
                string csginfo = "Generated with Chloride Page Generator at " + DateTime.UtcNow.ToShortTimeString() + ", " + DateTime.UtcNow.ToShortDateString() + " (UTC)";
                TemporaryHTML = TemporaryHTML.Replace("$WEBSITE_TITLE", SiteTitle).Replace("$WEBSITE_FOOTER", SiteFooter).Replace("$PAGE_URL", kvp.Value[1]).Replace("$PAGE_TITLE", kvp.Value[0]).Replace("$CSG_INFO", csginfo);
                TemporaryHTML = TemporaryHTML.Replace("$PAGE_CONTENT", md.Transform(File.OpenText(kvp.Value[2]).ReadToEnd()));
                File.WriteAllText("html/" + kvp.Value[1], TemporaryHTML);
            }
        }

        private static void AddXmlNode(string name, string content, XmlElement parentelem)
        {
            XmlNode xmn = parentelem.OwnerDocument.CreateNode(XmlNodeType.Element, name, "");
            xmn.InnerText = content;
            parentelem.AppendChild(xmn);
        }
    }
}
