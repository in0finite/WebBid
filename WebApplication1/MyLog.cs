using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class MyLog
    {

        public static void Log(string text)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                path = System.IO.Path.Combine(path, "weblog.txt");
                System.IO.File.AppendAllText(path, text + "\n");
            }
            catch
            {

            }
        }

    }
}