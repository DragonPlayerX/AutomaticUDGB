namespace AutomaticUDGB
{
    public class GithubTree
    {
        public string sha;
        public string url;
        public GithubFile[] tree;
        public bool truncated;
    }

    public class GithubFile
    {
        public string path;
        public string mode;
        public string blob;
        public string sha;
        public string size;
        public string url;
    }
}
