using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Jobs;

public class AssetsManager : Singleton<AssetsManager>
{
    // setting
    public string jsonPath = "coding-test-frontend-unity";

    // sound data - faster access in array rather than class
    public string[] arrSongID;
    public Texture[] arrTexture;
    public AudioClip[] arrAudioClip;

    private string[] arrLastSongID;
    private Texture[] arrLastTexture;
    private AudioClip[] arrLastAudioClip;


    public Playlist[] GetPlaylist() {
        TextAsset jsonAssets = Resources.Load<TextAsset>(jsonPath);
        return JsonConvert.DeserializeObject<Playlist[]>(jsonAssets.text);
    }

    public void CacheLastData() {
        arrLastSongID = arrSongID;
        arrLastTexture = arrTexture;
        arrLastAudioClip = arrAudioClip;

        arrSongID = null;
        arrTexture = null;
        arrAudioClip = null;
    }

    public async Task PreloadAssetsInPlaylist(Playlist playlist, int startIdx = 0, int numOfSongs = -1)
    {
        int questionCnt = playlist.questions.Length;
        int endIdx = questionCnt - 1;
        if (numOfSongs == -1)
        {
            numOfSongs = questionCnt;
        } else
        {
            endIdx = startIdx + numOfSongs - 1;
        }

        Debug.Log("PreloadAssetsInPlaylist Start... load " + numOfSongs + " song(s) from index " + startIdx + " to index " + endIdx);

        List<Task> listTask = new List<Task>();

        if (arrTexture == null || arrTexture.Length == 0)
            arrTexture = new Texture[questionCnt];

        if (arrAudioClip == null || arrAudioClip.Length == 0)
            arrAudioClip = new AudioClip[questionCnt];

        if (arrSongID == null || arrSongID.Length == 0)
            arrSongID = new string[questionCnt];

        // set up download tasks
        for (int i = startIdx; i <= endIdx; i++)
        {
            int questionIdx = i;

            // for better ux: check if assets already loaded in last playlist, reuse if found
            // you can uncomment the for loop checking if there will be duplicated song in different playlist
            //if(arrLastSongID != null)
            //{
            //    // if there is last data
            //    bool isFoundInLastData = false;
            //    for (int j = 0; j < arrLastSongID.Length; j++)
            //    {
            //        if (playlist.questions[questionIdx].song.id == arrLastSongID[j])
            //        {
            //            arrSongID[questionIdx] = arrLastSongID[j];
            //            arrTexture[questionIdx] = arrLastTexture[j];
            //            arrAudioClip[questionIdx] = arrLastAudioClip[j];
            //            isFoundInLastData = true;
            //            Debug.Log("PreloadAssetsInPlaylist... song found in last data : " + arrLastSongID[j]);

            //            // stop the loop if sound found
            //            continue;
            //        }
            //    }

            //    // skip if sound founded in last data
            //    if (isFoundInLastData) continue;
            //}

            // store the song id, we can reuse the loaded data if same song is used
            arrSongID[questionIdx] = playlist.questions[questionIdx].song.id;

            listTask.Add(DownloadImageTask(playlist.questions[questionIdx].song.picture, arrTexture, questionIdx));
            listTask.Add(DonwloadAudioClipTask(playlist.questions[questionIdx].song.sample, arrAudioClip, questionIdx));
        }

        // start download with multithreading
        await Task.WhenAll(listTask);

        for (int i = 0; i < listTask.Count; i++)
        {
            // release the memory
            listTask[i].Dispose();
        }

        Debug.Log("PreloadAssetsInPlaylist Finish");

        // start download the rest of songs
        if (startIdx + numOfSongs < questionCnt)
            PreloadAssetsInPlaylist(playlist, startIdx + numOfSongs, questionCnt - numOfSongs);
    }

    static async Task DownloadImageTask(string url, Texture[] texture, int idx) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        // uncomment to test bad network
        // await Task.Delay(5000);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download url fail:" + url);
        }

        // store the downloaded data
        texture[idx] = DownloadHandlerTexture.GetContent(www);
    }

    static async Task DonwloadAudioClipTask(string url, AudioClip[] audio, int idx)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        var operation = www.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        // uncomment to test bad network
        // await Task.Delay(5000);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download url fail:" + url);
        }

        // store the downloaded data
        audio[idx] = DownloadHandlerAudioClip.GetContent(www);
    }

    
}
