using Newtonsoft.Json;

namespace AutomaticUDGB
{
    public class GithubTree
    {
        [JsonProperty("url")]
        public string Url;
        [JsonProperty("tree")]
        public GithubFile[] FileTree;
    }

    public class GithubFile
    {
        [JsonProperty("path")]
        public string Path;
        [JsonProperty("size")]
        public string Size;
    }
}
