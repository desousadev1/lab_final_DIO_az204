using Newtonsoft.Json;

public class MovieResult
{
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("title")]
    public string? Title { get; set; }
    
    [JsonProperty("year")]
    public int Year { get; set; }
    
    [JsonProperty("video")]
    public string? Video { get; set; }
    
    [JsonProperty("thumb")]
    public string? Thumb { get; set; }
}