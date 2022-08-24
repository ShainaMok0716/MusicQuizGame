using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Playlist
{
    [JsonProperty("id")]
    public string id { get; set; }
    [JsonProperty("questions")]
    public Question[] questions { get; set; }
    [JsonProperty("playlist")]
    public string playlist { get; set; }
}

[System.Serializable]
public struct Question
{
    [JsonProperty("id")]
    public string id { get; set; }
    [JsonProperty("answerIndex")]
    public int answerIndex { get; set; }
    [JsonProperty("choices")]
    public Choice[] choices { get; set; }
    [JsonProperty("song")]
    public Song song { get; set; }

}

[System.Serializable]
public struct Choice
{
    [JsonProperty("artist")]
    public string artist { get; set; }
    [JsonProperty("title")]
    public string title { get; set; }
}

[System.Serializable]
public struct Song
{
    [JsonProperty("id")]
    public string id { get; set; }
    [JsonProperty("title")]
    public string title { get; set; }
    [JsonProperty("artist")]
    public string artist { get; set; }
    [JsonProperty("picture")]
    public string picture { get; set; }
    [JsonProperty("sample")]
    public string sample { get; set; }

}