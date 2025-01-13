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
            // obtain data
            // parse line by line
            // maintain # indents by watching for < (+1) and /> (-1)
            // apply indents accordingly
            // create new file
            // dump data to new file (optionally ask if save to original file
            //List<string> data = GetData("C:\\Users\\FFaruqi\\Desktop\\LAD\\LAD_App\\LAD\\Web.config");
            //data = ApplyIndents(data);
            //Console.WriteLine(string.Join("\n", data));
            TestApplyIndents();
        }

        static List<string> GetData(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File does not exist!");
            }
            List<string> res = new List<string>();
            char[] toRemove = { '\t', ' ', '\n' };

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                string[] lines;

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    lines = temp.GetString(b).Split('\n');
                    foreach (string line in lines)
                    {
                        res.Add(line.Trim(toRemove));
                    }
                }
            }

            return res;
        }

        static List<string> ApplyIndents(List<string> data)
        {
            int numIndents = 0;
            string line;
            bool hasBegin;
            bool hasCloseEnd;
            bool hasClose;

            for (int i = 0; i < data.Count; i++)
            {
                line = data[i]; 

                if (line.Length < 4) continue;

                hasBegin = line[0] == '<' && line[1] != '/';
                hasCloseEnd = (line[line.Length - 1] == '>' && line[line.Length - 2] == '/');
                hasClose = (line[0] == '<' && line[1] == '/');

                if (hasClose) numIndents--;
                data[i] = (new string('\t', numIndents)) + data[i];

                if (hasBegin && hasCloseEnd) continue;
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
        }
    }
}
