using System.Collections.Generic;
using System;

namespace AutomaticUDGB
{
    public class UnityVersionManager : VersionManager
    {
        public UnityVersionManager(string url) : base(url)
        {

        }

        public override List<string> FetchVersions()
        {
            ColorConsole.Msg("[Unity] Downloading list of versions...");

            webClient.Headers.Add("User-Agent", "Unity Web Player");
            string[] entries = webClient.DownloadString(url).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            List<string> result = new List<string>();
            foreach (string line in entries)
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

            ColorConsole.Msg("[Unity] Found " + result.Count + " versions of Unity.");

            return result;
        }
    }
}
