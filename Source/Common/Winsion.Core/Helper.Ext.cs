using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Winsion.Core
{
    abstract partial class Helper
    {
        /// <summary>
        /// if not exist return null
        /// </summary>
        /// <param name="currentDomainFileName"></param>
        /// <returns></returns>
        public static string GetCurrentDomainFilePathFor(string currentDomainFileName)
        {
            var path = currentDomainFileName;
            if (File.Exists(path))
            {
                return GetFullPath(path);
            }
            var dom = AppDomain.CurrentDomain;
            path = Path.Combine(dom.BaseDirectory, currentDomainFileName);
            if (File.Exists(path))
            {
                return GetFullPath(path);
            }
            path = Path.Combine(dom.BaseDirectory, dom.RelativeSearchPath ?? "", currentDomainFileName);
            if (File.Exists(path))
            {
                return GetFullPath(path);              
            }
            return null;
        }

        private static string GetFullPath(string fileName)
        {
            if (Path.IsPathRooted(fileName) == false)
            {
                return Path.GetFullPath(fileName);
            }
            else
            {
                return fileName;
            }
        }
    }
}
