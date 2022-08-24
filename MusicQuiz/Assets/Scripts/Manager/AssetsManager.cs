using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;

public class AssetsManager : Singleton<AssetsManager>
{
    // setting
    private string jsonPath = "coding-test-frontend-unity";
    private int maxCacheCnt = 8; // you can cache the song if there will be duplicated song in different playlist

    // song data - faster access using array rather than class
    public string[] arrSongID;
    public Texture[] arrTexture;
    public AudioClip[] arrAudioClip;

    // cached song data
    private int nextCacheIdx = 0;
    private string[] cachedSongID; 
    private Texture[] cachedTexture;
    private AudioClip[] cachedAudioClip;

    public Playlist[] GetPlaylist() {
        TextAsset jsonAssets = Resources.Load<TextAsset>(jsonPath);
        return JsonConvert.DeserializeObject<Playlist[]>(jsonAssets.text);
    }

    public void CacheLastData() {
        if (maxCacheCnt > 0)
        {
            // cache the song data in current playlist

            if (cachedSongID == null)
            {
                cachedSongID = new string[maxCacheCnt];
                cachedTexture = new Texture[maxCacheCnt];
                cachedAudioClip = new AudioClip[maxCacheCnt];
            }

            for (int i = 0; i < arrSongID.Length; i++) {
                if (GetCachedAseetsIdx(arrSongID[i]) == -1) {
                    // if data is not cached before
                    Debug.Log("Cache Song: " + arrSongID[i] + " at index " + nextCacheIdx);

                    cachedSongID[nextCacheIdx] = arrSongID[i];
                    cachedTexture[nextCacheIdx] = arrTexture[i];
                    cachedAudioClip[nextCacheIdx] = arrAudioClip[i];
                    nextCacheIdx++;
                    if (nextCacheIdx > maxCacheCnt - 1)
                        nextCacheIdx = 0;
                }
            }
        }
    }

    public async Task PreloadAssetsInPlaylist(Playlist playlist, int startIdx = 0, int numOfSongs = -1)
    {
        // clear last song data if start loading from index 0
        if (startIdx == 0) {
            arrSongID = null;
            arrTexture = null;
            arrAudioClip = null;
        }

        int questionCnt = playlist.questions.Length;
        int endIdx = questionCnt - 1;
        if (numOfSongs == -1)
        {
            numOfSongs = questionCnt;
        } else {
            endIdx = startIdx + numOfSongs - 1;
        }

        Debug.Log("PreloadAssetsInPlaylist Start... load " + numOfSongs + " song(s) from index " + startIdx + " to index " + endIdx + ", maxCacheCnt:" + maxCacheCnt);

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
            if (maxCacheCnt > 0)
            {
                int cachedIdx = GetCachedAseetsIdx(playlist.questions[questionIdx].song.id);
                if (cachedIdx > -1)
                {
                    arrSongID[questionIdx] = cachedSongID[cachedIdx];
                    arrTexture[questionIdx] = cachedTexture[cachedIdx];
                    arrAudioClip[questionIdx] = cachedAudioClip[cachedIdx];
                    // stop the loop if sound found in cached data
                    continue;
                }
            }
            
            // store the song id, we can reuse the loaded data if same song is used
            arrSongID[questionIdx] = playlist.questions[questionIdx].song.id;

            // add dowload task to list
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

    int GetCachedAseetsIdx(string songID) {
        if (cachedSongID != null)
        {
            // if there is last data
            for (int j = 0; j < cachedSongID.Length; j++)
            {
                if (songID == cachedSongID[j])
                {
                    Debug.Log("GetCachedAseetsIdx ... song found in cache data : " + cachedSongID[j]);
                    // stop the loop if sound found
                    return j;
                }
            }
        }
        return -1;
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
