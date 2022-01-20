using System.IO;

namespace DGP.Genshin.DataViewer.Helpers
{
    public class File2
    {
        public File2(string fullPath)
        {
            FullPath = fullPath;
        }

        public string FullPath { get; set; }
        public string FullFileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath);
            }
        }

        public string FileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath)
                    .Replace("Excel", "")
                    .Replace("Config", "")
                    .Replace("Data", "");
            }
        }

        public string Read()
        {
            return File.ReadAllText(FullPath);
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
