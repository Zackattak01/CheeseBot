using System.Text;
using CheeseBot.GitHub.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CheeseBot.GitHub.Models
{
    public class RepositoryContentModel
    {
        private string _content;

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public GitHubSourceType Type { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("content")]
        public string Content
        {
            get => _content is not null ? Encoding.UTF8.GetString(Convert.FromBase64String(_content)) : null;
            set => _content = value;
        }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
        
        
    }
}