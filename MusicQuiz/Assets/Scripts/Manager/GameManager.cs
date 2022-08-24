using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameManager : Singleton<GameManager>
{
    public enum View
    {
        Welcome,
        Game,
        Result
    }

    // ui views
    public WelcomeView welcomeView;
    public GameView gameView;
    public ResultView resultView;

    // global game data
    public GameData gameData;

    // setting
    public int nextQWaitingMs = 1000;
    
    private void Start()
    {
        // Start of everything

        // init game data
        gameData = new GameData();

        // load json data
        gameData.arrPlaylist = AssetsManager.Instance.GetPlaylist();

        // update view
        SwitchToView(View.Welcome);
        welcomeView.UpdatePlaylist();
    }

    public async void StartQuiz(int playlistIdx) {

        int lastPlaylistIdx = gameData.currPlaylistIdx;

        // update game data
        gameData.startNewPlaylist(playlistIdx);

        // load music and image if playlistIdx id different from last time
        if(lastPlaylistIdx != playlistIdx)
        {
            // There are two ways to load assets before start game
            
            // 1. only load the first song and start, other songs will be downlaoded asynchronous after the quiz started 
            // you can custom number of songs to pre download
            await AssetsManager.Instance.PreloadAssetsInPlaylist(gameData.currPlaylist, 0, 1);

            // 2. start after downloading all songs by uncomment this:
            // await AssetsManager.Instance.PreloadAssetsInPlaylist(gameData.currPlaylist);

            // TODO: if network is really bad, we might need to show loading ui here
            // other there will be a LoadingManager storing a loading stack and control a global LoadingView
        }

        // update view
        SwitchToView(View.Game);
        gameView.UpdateView(gameData.currQuestionIdx, gameData.currQuestion);

    }

    public async void NextQuestion()
    {
        // delay to show correct answer
        await Task.Delay(nextQWaitingMs);

        // check if last question
        if (gameData.IsLastQuestion()) {
            // show result view
            SwitchToView(View.Result);
            gameView.ResetView();
            resultView.UpdateView();
        }
        else {
            // next question 

            // update game data
            gameData.NextQuestion();

            // update gameview
            gameView.UpdateView(gameData.currQuestionIdx, gameData.currQuestion);
        }
    }

    public void NextPlaylist() {
        SwitchToView(View.Welcome);

        // unload unused assets
        Resources.UnloadUnusedAssets();

        // for better ux: cached last played songs
        AssetsManager.Instance.CacheLastData();

        // You can update the player score to server here

    }

    public void PlayAgain() {
        StartQuiz(gameData.currPlaylistIdx);

        // You can update the player score to server here

    }

    private void SwitchToView(View trgView) {
        welcomeView.gameObject.SetActive(trgView == View.Welcome);
        gameView.gameObject.SetActive(trgView == View.Game);
        resultView.gameObject.SetActive(trgView == View.Result);
    }
}
