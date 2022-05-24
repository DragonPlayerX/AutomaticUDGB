using System.Net;
using System.Collections.Generic;

namespace AutomaticUDGB
{
    public abstract class VersionManager
    {
        protected string url;
        protected WebClient webClient = new WebClient();

        public abstract List<string> FetchVersions();

        public VersionManager(string url) => this.url = url;
    }
}
