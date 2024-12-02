using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
namespace SafeStreet.Data.Migration

{
    public class YoutubeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public YoutubeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["apikey"];  // Retrieve the API key from configuration
        }

        /// <summary>
        /// Fetches videos from YouTube API based on a search query.
        /// </summary>
        /// <param name="query">Search term for videos.</param>
        /// <returns>List of YouTube videos.</returns>
        public async Task<List<YouTubeVideo>> GetVideosAsync(string query)
        {
            // URL encode the query to handle special characters
            string encodedQuery = Uri.EscapeDataString(query);

            // Construct the YouTube API request URL
            string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={encodedQuery}&key={_apiKey}&maxResults=5";

            // Initialize a list to hold the videos
            var videos = new List<YouTubeVideo>();

            // Add static videos (hardcoded examples)
            videos.Add(new YouTubeVideo
            {
                Title = "Sweet Potato & Black Bean Chili Recipe",
                Description = "A hearty and healthy Sweet Potato & Black Bean Chili recipe that's perfect for any occasion.",
                VideoId = "md63lafRoqM"  // Static video ID for Sweet Potato & Black Bean Chili
            });

            // Example video with the specific video ID "9NbBoQ-R_0s"
            videos.Add(new YouTubeVideo
            {
                Title = "Vegan Sweet Potato & Black Bean Chili",
                Description = "Learn how to make a delicious vegan Sweet Potato & Black Bean Chili with this simple recipe.",
                VideoId = "9NbBoQ-R_0s"  // Static video ID for Vegan Sweet Potato & Black Bean Chili
            });

            // Fetch dynamic videos from the YouTube API
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var youtubeResponse = JsonConvert.DeserializeObject<YouTubeResponse>(jsonResponse);

                // Check if we received any valid video data
                if (youtubeResponse != null && youtubeResponse.Items != null)
                {
                    // Map the API response to the YouTubeVideo model
                    var apiVideos = youtubeResponse.Items.Select(item => new YouTubeVideo
                    {
                        Title = item.Snippet.Title,
                        Description = item.Snippet.Description,
                        VideoId = item.Id.VideoId
                    }).ToList();

                    // Add the dynamic API videos to our list of videos
                    videos.AddRange(apiVideos);
                }
            }

            return videos;
        }
    }

    // Model for YouTube API Response
    public class YouTubeResponse
    {
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public Snippet Snippet { get; set; }
        public Id Id { get; set; }
    }

    public class Snippet
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Id
    {
        public string VideoId { get; set; }
    }

    public class YouTubeVideo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoId { get; set; }
    }
}

