using System.Collections.Generic;

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
            string page = webClient.DownloadString(url);

            string target = "id=\"_unityVersions\"";
            int start = page.IndexOf(target);
            string part = page.Substring(start + target.Length + 1);
            int end = part.IndexOf("<");

            string versions = part.Substring(0, end);

            List<string> result = new List<string>();
            foreach (string entry in versions.Split(','))
            {
                string[] version = entry.Split('.');
                if (version.Length == 3 && version[2].Contains("f"))
                {
                    string trimmedVersion = entry.Substring(0, entry.LastIndexOf("f"));
                    if (!result.Contains(trimmedVersion))
                        result.Add(trimmedVersion);
                }
            }

            ColorConsole.Msg("[Unity] Found " + result.Count + " versions of Unity.");

            return result;
        }
    }
}
