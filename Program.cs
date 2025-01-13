using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace FixIndents
{
    class Program
    {
        static void Main(string[] args)
        {
            try 
            {
                TestApplyIndents();
            } catch (Exception _)
            {
                Console.WriteLine("Tests failed, please revert to a stable build");
                return;
            }

            Console.WriteLine("Path to file to fix");
            string path = Console.ReadLine();
            Console.WriteLine("Path to save output to (absolute path)");
            string outPath = Console.ReadLine();
            
            List<string> data = GetData(path);
            data = ApplyIndents(data);
            SaveToFile(data, "../../../../" + outPath);
        }

        static List<string> GetData(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File does not exist!");
            }
            List<string> res = new List<string>();
            char[] toRemove = { '\t', ' ', '\n' };

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    res.Add(line.Trim(toRemove));
                }
            }
            return res;
        }

        static void SaveToFile(List<string> data, string path)
        {
            File.WriteAllLines(path, data);
        }

        static List<string> ApplyIndents(List<string> data)
        {
            Dictionary<string, string> toIgnore = new Dictionary<string, string>();
            toIgnore.Add("<?", "?>");
            toIgnore.Add("<!--", "-->");
            toIgnore.Add("<", "/>");

            int numIndents = 0;
            string line;
            bool hasBegin;
            bool hasCloseEnd;
            bool hasClose;
            bool ignore;

            for (int i = 0; i < data.Count; i++)
            {
                ignore = false;
                line = data[i];
                if (line.Length < 4) continue;

                hasBegin = line[0] == '<' && line[1] != '/';
                hasCloseEnd = (line[line.Length - 1] == '>' && line[line.Length - 2] == '/');
                hasClose = (line[0] == '<' && line[1] == '/');

                if (hasClose || hasCloseEnd && !hasBegin) numIndents--;
                data[i] = (new string('\t', numIndents)) + data[i];

                foreach (string key in toIgnore.Keys) {
                    if (line.Contains(key) && line.Contains(toIgnore[key])) ignore = true;
                }

                if (ignore) continue;
                
                if (hasBegin) numIndents++;
            }

            return data;
        }

        static bool ListEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count){
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!comparer.Equals(list1[i], list2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        static void TestApplyIndents()
        {
            List<string> testInput = new List<string>{"<asdasds />",
                "<asdasdasdasdasdasdas>",
                "<asdasdasdasdas />",
                "</asdasdasdsad>"};
            List<string> testAnswer = new List<string> { "<asdasds />",
                "<asdasdasdasdasdasdas>",
                "\t<asdasdasdasdas />",
                "</asdasdasdsad>" };

            Debug.Assert(ListEqual(ApplyIndents(testInput), testAnswer) == true);
            
            testInput = new List<string>{ "<div>", "<div/>", "<div/>", "</div>", "<div />", "</>" };
            testAnswer = new List<string> { "<div>", "\t<div/>", "\t<div/>", "</div>", "<div />", "</>" };
            Debug.Assert(ListEqual(ApplyIndents(testInput), testAnswer) == true);

            testInput = new List<string> { "<div>", "<div>", "<div>", "</div>", "</div>", "</div>" };
            testAnswer = new List<string> { "<div>", "\t<div>", "\t\t<div>", "\t\t</div>", "\t</div>", "</div>" };
            Debug.Assert(ListEqual(ApplyIndents(testInput), testAnswer) == true);

            testInput = new List<string> { "<div>", "</div>", "<div>", "</div>", "<div>", "</div>" };
            Debug.Assert(ListEqual(ApplyIndents(testInput), testInput) == true);

            testInput = new List<string> { "<div>", "<div>", "</div>", "</div>", "<div>", "</div>" };
            testAnswer = new List<string> { "<div>", "\t<div>", "\t</div>", "</div>", "<div>", "</div>" };
            Debug.Assert(ListEqual(ApplyIndents(testInput), testAnswer) == true);

            testInput = new List<string> { "<configSections>", "<sectionGroup name=\"businessObjects\">", "<sectionGroup name=\"crystalReports\">",
                "<section name=\"rptBuildProvider\" type=\"CrystalDecisions.Shared.RptBuildProviderHandler, CrystalDecisions.Shared, Version=13.0.2000.0, " +
                "Culture=neutral, PublicKeyToken=692fbea5521e1304, Custom=null\"/>", "</sectionGroup>", "</sectionGroup>", "<sectionGroup name=\"elmah\">",
                "<section name=\"security\" requirePermission=\"false\" type=\"Elmah.SecuritySectionHandler, Elmah\"/>",
                "<section name=\"errorLog\" requirePermission=\"false\" type=\"Elmah.ErrorLogSectionHandler, Elmah\" />",
                "<section name=\"errorMail\" requirePermission=\"false\" type=\"Elmah.ErrorMailSectionHandler, Elmah\" />",
                "<section name=\"errorFilter\" requirePermission=\"false\" type=\"Elmah.ErrorFilterSectionHandler, Elmah\"/>",
                "</sectionGroup>", "</configSections>"
            };
            testAnswer = new List<string> { "<configSections>", "\t<sectionGroup name=\"businessObjects\">", "\t\t<sectionGroup name=\"crystalReports\">",
                "\t\t\t<section name=\"rptBuildProvider\" type=\"CrystalDecisions.Shared.RptBuildProviderHandler, CrystalDecisions.Shared, Version=13.0.2000.0, " +
                "Culture=neutral, PublicKeyToken=692fbea5521e1304, Custom=null\"/>", "\t\t</sectionGroup>", "\t</sectionGroup>", "\t<sectionGroup name=\"elmah\">",
                "\t\t<section name=\"security\" requirePermission=\"false\" type=\"Elmah.SecuritySectionHandler, Elmah\"/>",
                "\t\t<section name=\"errorLog\" requirePermission=\"false\" type=\"Elmah.ErrorLogSectionHandler, Elmah\" />",
                "\t\t<section name=\"errorMail\" requirePermission=\"false\" type=\"Elmah.ErrorMailSectionHandler, Elmah\" />",
                "\t\t<section name=\"errorFilter\" requirePermission=\"false\" type=\"Elmah.ErrorFilterSectionHandler, Elmah\"/>",
                "\t</sectionGroup>", "</configSections>"
            };
            Debug.Assert(ListEqual(ApplyIndents(testInput), testAnswer) == true);
        }
    }
}
