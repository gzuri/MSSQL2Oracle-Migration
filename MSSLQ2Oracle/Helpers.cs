using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSLQ2Oracle
{
    public static class Helpers
    {
        public static string ConvertNaming(string oldName)
        {
            return string.Concat(oldName.Select((x, i) =>
            {
                if (i > 0 && char.IsUpper(x) && !(x == 'D' && oldName[i - 1] == 'I'))
                {
                    return "_" + x.ToString().ToUpperInvariant();
                }
                return x.ToString().ToUpperInvariant();
            }));
        }

        public static void WriteStatementsToFile(List<string> lines, string path, string fileName)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filePath))
            {
                foreach (var line in lines)
                    sw.WriteLine(line);
            }
        }
    }
}
