using System.Net.Http.Headers;
using CheeseBot.GitHub.Entities;
using CheeseBot.GitHub.Models;
using Newtonsoft.Json.Linq;

namespace CheeseBot.GitHub
{
    public class GitHubClient
    {
        private readonly HttpClient _httpClient;

        private const string BaseApiUrl = "https://api.github.com";
        private const string GetRepositoryContentUrl = "/repos/Zackattak01/CheeseBot/contents/{0}";

        public GitHubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> ExecuteAsync(HttpMethod method, string endpoint)
        {
            var request = new HttpRequestMessage(method, BaseApiUrl + endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            request.Headers.UserAgent.Add(ProductInfoHeaderValue.Parse("CheeseBot"));
            
            using var resp = await _httpClient.SendAsync(request);
            resp.EnsureSuccessStatusCode();

            var jsonStr = await resp.Content.ReadAsStringAsync();
            
            return jsonStr;
        }

        public async Task<(GitHubSourceType Type, RepositoryContentModel[] Models)> GetRepositoryContentsAsync(string path)
        {
            var jsonStr = await ExecuteAsync(HttpMethod.Get, string.Format(GetRepositoryContentUrl, path));
            var token = JToken.Parse(jsonStr);

            if (token is JArray)
                return (GitHubSourceType.Dir, token.ToObject<RepositoryContentModel[]>());
            else
                return (GitHubSourceType.File, new[] { token.ToObject<RepositoryContentModel>() });
        }
    }
}