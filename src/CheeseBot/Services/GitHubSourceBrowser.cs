using System.Text;
using CheeseBot.GitHub;
using CheeseBot.GitHub.Entities;

namespace CheeseBot.Services
{
    public class GitHubSourceBrowser : CheeseBotService
    {
        private const string DefaultSourceDirectory = "src/CheeseBot";
        private const string GithubRepoBaseLink = "https://github.com/Zackattak01/CheeseBot/tree/main/src/CheeseBot";
        private const string CSharpExtension = ".cs";
        private const string CsprojExtension = ".csproj";
        private const char GithubLineSelectorChar = '#';
        private const int MinutesToCacheContent = 30;

        private readonly GitHubClient _client;
        private readonly SchedulingService _scheduler;
        private readonly Dictionary<string, GitHubSource> _contentCache;
        private readonly HashSet<int> _scheduledCacheRemovalTasks;

        public GitHubSourceBrowser(GitHubClient client, SchedulingService scheduler, ILogger<GitHubSourceBrowser> logger) : base(logger)
        {
            _client = client;
            _scheduler = scheduler;
            _scheduledCacheRemovalTasks = new HashSet<int>();
            _contentCache = new Dictionary<string, GitHubSource>();
        }

        public async Task<GitHubSource> GetPathAsync(string path)
        {
            path = NormalizePath(path, out var lineSelection);
            
            if (_contentCache.TryGetValue(path, out var cachedFile))
            {
                if (lineSelection is not null)
                {
                    return cachedFile switch
                    {
                        GitHubSourceFile cachedSourceFile => new GitHubSourceFileSelection(cachedSourceFile, lineSelection),
                        GitHubSourceDirectory => cachedFile,
                        _ => throw new Exception("Github source cache contains an unexpected type!")
                    };
                }
                
                return cachedFile;
            }

            var (type, models) =  await _client.GetRepositoryContentsAsync(path);
            
            GitHubSource sourceFile;
            if (type == GitHubSourceType.File)
                sourceFile = new GitHubSourceFile(path, models[0].Name, models[0].Content);
            else
            {
                var files = new List<GitHubSource>(models.Length);
                foreach (var model in models)
                {
                    var filePath = string.Concat(path, "/", model.Name);
                    
                    if (model.Type == GitHubSourceType.File)
                        files.Add(new GitHubSourceFile(filePath, model.Name, string.Empty));
                    else if (model.Type == GitHubSourceType.Dir)
                        files.Add(new GitHubSourceDirectory(filePath, model.Name, Array.Empty<GitHubSource>()));
                }

                sourceFile = new GitHubSourceDirectory(path, path.Split('/').Last(), files);
            }

            AddFileToCache(path, sourceFile);
            
            if (lineSelection is not null && sourceFile is GitHubSourceFile file)
                sourceFile = new GitHubSourceFileSelection(file, lineSelection);
            
            return sourceFile;
        }

        public void AddFileToCache(string path, GitHubSource sourceFile)
        {
            Logger.LogInformation("Fetched github source file: {0}. Caching contents for {1} minutes", sourceFile.Filename, MinutesToCacheContent);
            _contentCache.Add(path, sourceFile);
            
            var scheduledTask = _scheduler.Schedule(DateTime.Now.AddMinutes(MinutesToCacheContent), scheduledTask =>
            {
                Logger.LogInformation("Cached content for {0} expired after {1} minutes.", sourceFile.Filename, MinutesToCacheContent);
                _contentCache.Remove(path);
                _scheduledCacheRemovalTasks.Remove(scheduledTask.Id);
                return Task.CompletedTask;
            });

            _scheduledCacheRemovalTasks.Add(scheduledTask.Id);
        }

        public void ClearContentCache()
        {
            foreach (var taskId in _scheduledCacheRemovalTasks)
                _scheduler.CancelScheduledTask(taskId);

            _scheduledCacheRemovalTasks.Clear();
            
            _contentCache.Clear();
        }

        public static string GetSourceLink(string path)
        {
            var lineSelectorIndex = path.LastIndexOf(GithubLineSelectorChar);
            
            if (lineSelectorIndex == -1)
                return GithubRepoBaseLink + path;

            var pathWithoutSelection = path[..lineSelectorIndex];
            var pathWithExtension = GetPathWithExtension(pathWithoutSelection);
            return GithubRepoBaseLink + pathWithExtension + path[(lineSelectorIndex)..];
        }

        private static string GetPathWithExtension(string path)
        {
            if (path.Length > CSharpExtension.Length)
            {
                if (path[^CSharpExtension.Length..] == CSharpExtension)
                    return path;
                
                if (path.Length > CsprojExtension.Length && path[^CsprojExtension.Length..] == CsprojExtension)
                    return path;
            }


            return path + CSharpExtension;
        }

        private static string NormalizePath(string path, out GitHubLineSelection selection)
        {
            selection = null;
            
            if (path is null)
                return DefaultSourceDirectory;

            path = GetPathWithoutSelection(path, out selection);

            if (!path.EndsWith('/'))
                path += '/';

            var pathSegments = new List<string>();

            if (!path.StartsWith(DefaultSourceDirectory))
            {
                pathSegments.Add("src");
                pathSegments.Add("CheeseBot");
            }
            
            var currentSegment = new StringBuilder();
            foreach (var c in path)
            {
                var currentCharIsSlash = c == '/';

                if (currentCharIsSlash)
                {
                    var currentSegmentStr = currentSegment.ToString();
                    currentSegment.Clear();
                    if (string.IsNullOrEmpty(currentSegmentStr) || string.IsNullOrWhiteSpace(currentSegmentStr) || currentSegmentStr == ".")
                        continue;
                    else if (currentSegmentStr == "..")
                    {
                        var index = pathSegments.Count - 1;
                        if (index >= 0)
                            pathSegments.RemoveAt(index);
                        continue;
                    }

                    pathSegments.Add(currentSegmentStr);
                }
                else
                    currentSegment.Append(c);
            }

            return string.Join("/", pathSegments);
        }

        private static string GetPathWithoutSelection(string path, out GitHubLineSelection selection)
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