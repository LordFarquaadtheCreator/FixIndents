using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
            List<string> data = GetData("C:\\Users\\FFaruqi\\Desktop\\LAD\\LAD_App\\LAD\\Web.config");
            
        }

        static List<string> GetData(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File does not exist!");
            }
            List<string> res = new List<string>();

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                string line;

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    line = temp.GetString(b);
                    res.Add(line);
                }
            }

            return res;
        }
        
        static 
    }
}
