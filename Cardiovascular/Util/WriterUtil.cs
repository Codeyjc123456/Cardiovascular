using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardio.Util
{
    public static class WriterUtil
    {
        public static StreamWriter sw = null;

        public static void CreateText(string fileName)
        {
            sw = new StreamWriter(fileName.Replace(" ", "_").Replace(":", "_") + ".txt", true);
        }

        public static void AppendText(string content)
        {
            sw.Write(content);
        }

        public static void ExportText()
        {
            sw.Flush();
            sw.Close();
        }
    }
}
