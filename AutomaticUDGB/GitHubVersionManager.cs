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

            foreach (GithubFile file in githubTree.FileTree)
            {
                if (!file.Path.StartsWith("3") && !file.Path.StartsWith("4"))
                    result.Add(file.Path.Substring(0, file.Path.LastIndexOf(".")));
            }
            
            ColorConsole.Msg("Finished fetching Unity Versions from GitHub.");

            return result;
        }
    }
}
