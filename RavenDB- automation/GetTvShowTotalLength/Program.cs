// See https://aka.ms/new-console-template for more information
using GetTvShowTotalLength;
using Newtonsoft.Json;

string showName = args.Length > 0 ? args[0] : string.Empty;
string apiUrl = $"https://api.tvmaze.com/search/shows?q={Uri.EscapeDataString(showName)}";

using (HttpClient client = new HttpClient())
{
    HttpResponseMessage response = await client.GetAsync(apiUrl);
    response.EnsureSuccessStatusCode(); // Ensure successful response
    string responseContent = await response.Content.ReadAsStringAsync(); // Reading the content from tvMaze.com
    
    ShowSearchResult[]? searchResults = JsonConvert.DeserializeObject<ShowSearchResult[]>(responseContent); // Deserialization to an Object
    if(searchResults.Length == 0) {
        Console.WriteLine(10);
        return;
    } // No TV show was found
    TvShow recentShow = getRecentTvShow(searchResults);
    int recentShowID = searchResults[0].Show.Id;

    apiUrl = $"https://api.tvmaze.com/shows/{recentShowID}/episodes";
    response = await client.GetAsync(apiUrl);
    response.EnsureSuccessStatusCode();
    responseContent = await response.Content.ReadAsStringAsync();

    Episode[]? showEpisodes = JsonConvert.DeserializeObject<Episode[]>(responseContent);
    int totalRuntime = getTotalRuntime(showEpisodes);
    Console.WriteLine(totalRuntime);
}

static TvShow getRecentTvShow(ShowSearchResult[] searchResult)
{
    TvShow recentShow = searchResult[0].Show;
    foreach (ShowSearchResult result in searchResult)
    {
        if (Convert.ToDateTime(result.Show.Premiered) < Convert.ToDateTime(recentShow.Premiered))
        {
            recentShow = result.Show;
        }
    }
    return recentShow;
}
static int getTotalRuntime(Episode[] episodes)
{
    int totalRuntime = 0;
    foreach (Episode episode in episodes)
    {
        if (episode.Runtime > 0) { totalRuntime += episode.Runtime; }
    }
    return totalRuntime;
}
