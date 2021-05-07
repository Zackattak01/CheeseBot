using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class GitHubSourceBrowser : CheeseBotService
    {
        private const string GithubRawRepoBaseLink = "https://raw.githubusercontent.com/Zackattak01/CheeseBot/main/CheeseBot/";
        private const string GithubRepoBaseLink = "https://github.com/Zackattak01/CheeseBot/tree/main/CheeseBot/";
        private const string CSharpExtension = ".cs";
        private const string CsprojExtension = ".csproj";
        
        private readonly HttpClient _httpClient;

        public GitHubSourceBrowser(HttpClient httpClient, ILogger<GitHubSourceBrowser> logger) : base(logger)
        {
            _httpClient = httpClient;
        }

        public async Task<(string content, string filename)> GetFileContents(string path)
        {
            // simple check to add a file extension if one was not provided
            path = GetPathWithExtension(path);
            
            // await _httpClient.GetStringAsync(GithubRawRepoBaseLink + path)
            var response = await _httpClient.GetAsync(GithubRawRepoBaseLink + path);
            if (!response.IsSuccessStatusCode)
                return (null, null);

            return (await response.Content.ReadAsStringAsync(), path.Split('/').Last());
        }

        public string GetSourceLink(string path)
        {
            path = GetPathWithExtension(path);
            return GithubRepoBaseLink + path;
        }

        private static string GetPathWithExtension(string path)
        {
            if ((path.Length > 3 && path[^3..] != CSharpExtension) && (path.Length > 7 && path[^7..] != CsprojExtension))
                path += CSharpExtension;

            return path;
        }

        
    }
}