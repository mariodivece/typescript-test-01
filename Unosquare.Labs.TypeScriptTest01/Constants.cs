using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unosquare.Labs.TypeScriptTest01
{
    /// <summary>
    /// Defines application-wide constants and properties
    /// </summary>
    static public class Constants
    {
        private static string m_DataDirectory;

        public const string ServerUrl = "http://localhost:9696/";

        public const string WebApiRelativePath = "/api/";

        public static string HtmlRootPath
        {
            get
            {
                var assemblyPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
#if DEBUG
                // This lets you edit the files without restarting the server.
                return Path.GetFullPath(Path.Combine(assemblyPath, "..\\..\\wwwroot"));
#else
                // This is when you have deployed ythe server.
                return Path.Combine(assemblyPath, "html");
#endif
            }
        }

        public static string DataDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_DataDirectory))
                {
                    var localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    m_DataDirectory = Path.Combine(localAppDataDirectory, Assembly.GetEntryAssembly().GetName().Name);
                    if (Directory.Exists(m_DataDirectory) == false)
                    {
                        Directory.CreateDirectory(m_DataDirectory);
                    }
                }

                return m_DataDirectory;
            }
        }

        public static string DatabaseFullPath
        {
            get { return Path.Combine(DataDirectory, "database.sqlite"); }
        }
    }
}
