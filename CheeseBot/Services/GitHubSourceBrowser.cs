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
        private const char GithubLineSelectorChar = '#';
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
            path = GetPathWithoutLineSelector(path, out var lineSelection);
            
            var uri = new Uri(GithubRawRepoBaseLink + GetPathWithExtension(path));

            if (_contentCache.TryGetValue(uri, out var cachedFile))
            {
                if (lineSelection is not null)
                    return new GitHubSourceFileSelection(cachedFile, lineSelection);
                
                return cachedFile;
            }
                
            
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var sourceFile = new GitHubSourceFile(uri, path.Split('/').Last(), content);
            _contentCache.Add(uri, sourceFile);
            
            Logger.LogInformation($"Fetched github source file: {sourceFile.Filename}. Caching contents for {MinutesToCacheContent} minutes");
            
            _scheduler.Schedule(DateTime.Now.AddMinutes(MinutesToCacheContent), () =>
            {
                Logger.LogInformation($"Cached content for {sourceFile.Filename} expired after {MinutesToCacheContent} minutes.");
                _contentCache.Remove(uri);
                return Task.CompletedTask;
            });

            if (lineSelection is not null)
            {
                return new GitHubSourceFileSelection(sourceFile, lineSelection);
            }
            
            return sourceFile;
        }

        public string GetSourceLink(string path)
        {
            var lineSelectorIndex = path.LastIndexOf(GithubLineSelectorChar);
            
            if (lineSelectorIndex == -1)
                return GithubRepoBaseLink + path;

            var pathWithoutSelection = path[..lineSelectorIndex];
            var pathWithExtension = GetPathWithExtension(pathWithoutSelection);
            return GithubRepoBaseLink + pathWithExtension + path[(lineSelectorIndex)..];
        }

        // simple check to add a file extension if one was not provided
        private static string GetPathWithExtension(string path)
        {
            if (path.Length > CSharpExtension.Length)
            {
                if (path[^CSharpExtension.Length..] == CSharpExtension)
                    return path;
                
                if (path.Length > CsprojExtension.Length && path[^CsprojExtension.Length..] == CsprojExtension)
                    return path;
            }


            return path += CSharpExtension;
        }

        private static string GetPathWithoutLineSelector(string path, out GitHubLineSelection selection)
        {
            
            var lastIndexOfLineSelector = path.LastIndexOf(GithubLineSelectorChar);
            if (lastIndexOfLineSelector != -1)
            {
                var pathWithoutLineSelector = path[..lastIndexOfLineSelector];
                GitHubLineSelection.TryParse(path[(lastIndexOfLineSelector + 1)..], out selection);
                return pathWithoutLineSelector;
                
            }

            selection = null;
            return path;
        }
    }
}