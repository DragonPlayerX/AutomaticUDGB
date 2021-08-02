using System.IO;
using System.Collections.Generic;
using System.Text;

namespace AutomaticUDGB
{
    public class UnityVersionManager : VersionManager
    {

        public UnityVersionManager(string url) : base(url)
        {

        }

        public override List<string> FetchVersions()
        {

            string tempFile = Path.GetTempFileName();

            ColorConsole.Msg("Downloading Unity Versions to " + tempFile);

            webClient.Headers.Add("User-Agent", "Unity Web Player");
            webClient.DownloadFile(url, tempFile);

            List<string> result = new List<string>();

            foreach (string line in File.ReadAllLines(tempFile, Encoding.UTF8))
            {
                string[] contents = line.ToLower().Replace("\"", "").Split(',');
                if (contents[1].Equals("add") && contents[2].Equals("file") && contents[5].Equals("unity"))
                {
                    if (contents[6].Contains("."))
                    {
                        string[] version = contents[6].Split('.');
                        if (version.Length == 3 && version[2].Contains("f"))
                        {
                            string trimmedVersion = contents[6].Substring(0, contents[6].LastIndexOf("f"));
                            if (!result.Contains(trimmedVersion))
                                result.Add(trimmedVersion);
                        }
                    }
                }
            }

            ColorConsole.Msg("Finished downloading Unity Versions.");

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
                ColorConsole.Msg("Deleted temp file " + tempFile);
            }

            return result;
        }
    }
}
