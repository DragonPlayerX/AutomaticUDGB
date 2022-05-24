using System.Collections.Generic;
using System.Linq;
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
            ColorConsole.Msg("[GitHub] Downloading list of versions...");

            webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string jsonString = webClient.DownloadString(url);
            GithubTree githubTree = JsonConvert.DeserializeObject<GithubTree>(jsonString);

            ColorConsole.Msg("[GitHub] Fetched from " + githubTree.Url);

            int totalSize = 0;
            List<string> result = new List<string>();

            foreach (GithubFile file in githubTree.FileTree)
            {
                List<string> parsedVersion = file.Path.Split('.').ToList();
                if (parsedVersion.Count != 4)
                    continue;

                parsedVersion.RemoveAt(parsedVersion.Count - 1);
                if (parsedVersion.Any(v => !v.All(char.IsNumber)))
                    continue;

                if (parsedVersion[0].StartsWith("3") || parsedVersion[0].StartsWith("4"))
                    continue;

                result.Add(file.Path.Substring(0, file.Path.LastIndexOf(".")));
                totalSize += file.Size;
            }

            ColorConsole.Msg("[GitHub] Found " + result.Count + " versions of Unity on GitHub. Total size " + (totalSize / 1024 / 1024) + "mb.");

            return result;
        }
    }
}
