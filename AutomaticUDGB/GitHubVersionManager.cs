using System.Collections.Generic;
using Newtonsoft.Json;

namespace AutomaticUDGB
{
    public class GitHubVersionManager : VersionManager
    {

        public GitHubVersionManager(string url) : base(url)
        {

        }

        public override List<string> FetchVersions()
        {

            ColorConsole.Msg("Downloading GitHub Versions...");

            webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string jsonString = webClient.DownloadString(url);

            GithubTree githubTree = JsonConvert.DeserializeObject<GithubTree>(jsonString);
            List<string> result = new List<string>();

            foreach (GithubFile file in githubTree.tree)
            {
                if (!file.path.StartsWith("3") && !file.path.StartsWith("4"))
                    result.Add(file.path.Substring(0, file.path.LastIndexOf(".")));
            }

            ColorConsole.Msg("Finished fetching Unity Versions from GitHub.");

            return result;
        }
    }
}
