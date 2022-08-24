using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Playlist[] arrPlaylist; // all transformed json data stored here
    public Playlist currPlaylist;
    public int currPlaylistIdx = -1;

    public Question currQuestion;
    public int currQuestionIdx;

    // player data
    public int playerScore;
    public bool[] playerAnswer;

    public bool IsLastQuestion() {
        return currPlaylist.questions.Length == currQuestionIdx + 1;
    }

    public void startNewPlaylist(int idx) {
        // reset current data & player data
        currPlaylistIdx = idx;
        // copy a the playlist struct for faster access
        currPlaylist = arrPlaylist[idx];
        currQuestionIdx = 0;
        // copy a the playlist struct for faster access
        currQuestion = currPlaylist.questions[currQuestionIdx];

        playerAnswer = new bool[currPlaylist.questions.Length];
        playerScore = 0;
    }

    public void NextQuestion() {
        currQuestionIdx++;
        // copy a the question struct for faster access
        currQuestion = currPlaylist.questions[currQuestionIdx];
    }

    public void RecordPlayerAnswer(int playerAns) {
        if (playerAns == currQuestion.answerIndex)
        {
            playerScore++;
            playerAnswer[currQuestionIdx] = true;

        } else {
            playerAnswer[currQuestionIdx] = false;
        }

    }

}
