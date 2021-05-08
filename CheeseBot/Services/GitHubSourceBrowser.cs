using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CheeseBot.GitHub;
using Microsoft.Extensions.Logging;

namespace CheeseBot.Services
{
    public class GitHubSourceBrowser : CheeseBotService
    {
        private const string GithubRawRepoBaseLink = "https://raw.githubusercontent.com/Zackattak01/CheeseBot/main/CheeseBot/";
        private const string GithubRepoBaseLink = "https://github.com/Zackattak01/CheeseBot/tree/main/CheeseBot/";
        private const string CSharpExtension = ".cs";
        private const string CsprojExtension = ".csproj";
        private const int MinutesToCacheContent = 30;
        
        private readonly HttpClient _httpClient;
        private readonly SchedulingService _scheduler;
        private readonly Dictionary<Uri, GitHubSourceFile> _contentCache;

        public GitHubSourceBrowser(HttpClient httpClient, SchedulingService scheduler, ILogger<GitHubSourceBrowser> logger) : base(logger)
        {
            _httpClient = httpClient;
            _scheduler = scheduler;
            _contentCache = new Dictionary<Uri, GitHubSourceFile>();
        }

        public async Task<GitHubSourceFile> GetFileContents(string path)
        {
            var uri = new Uri(GithubRawRepoBaseLink + GetPathWithExtension(path));
            
            if (_contentCache.TryGetValue(uri, out var cachedFile))
                return cachedFile;
            
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
                return null;

            Logger.LogInformation($"Fetched github source file. Caching contents for {MinutesToCacheContent} minutes");
            
            var content = await response.Content.ReadAsStringAsync();
            var sourceFile = new GitHubSourceFile(uri, path.Split('/').Last(), content);
            _contentCache.Add(uri, sourceFile);
            
            _scheduler.Schedule(DateTime.Now.AddMinutes(MinutesToCacheContent), () =>
            {
                Logger.LogInformation($"Cached content for {sourceFile.Filename} expired after {MinutesToCacheContent} minutes.");
                _contentCache.Remove(uri);
                return Task.CompletedTask;
            });
            
            return sourceFile;
        }

        public string GetSourceLink(string path)
        {
            path = GetPathWithExtension(path);
            return GithubRepoBaseLink + path;
        }

        // simple check to add a file extension if one was not provided
        private static string GetPathWithExtension(string path)
        {
            if ((path.Length > CSharpExtension.Length && path[^CSharpExtension.Length..] != CSharpExtension) && 
                (path.Length > CsprojExtension.Length && path[^CsprojExtension.Length..] != CsprojExtension))
                path += CSharpExtension;

            return path;
        }

        
    }
}