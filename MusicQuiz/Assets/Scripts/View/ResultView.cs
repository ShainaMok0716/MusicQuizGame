using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    public Text textScore;
    public Text[] textPlayerAnswer;
    public Button btnNext;
    public Button btnAgain;

    private void Start()
    {
        btnNext.onClick.AddListener(onNextClick);
        btnAgain.onClick.AddListener(onAgainClick);
    }

    public void UpdateView() {
        textScore.text = "Your Score: " + GameManager.Instance.gameData.playerScore;
        for (int i = 0; i < GameManager.Instance.gameData.playerAnswer.Length; i++) {
            textPlayerAnswer[i].text = "Q" + (i + 1) + ": " + GameManager.Instance.gameData.playerAnswer[i];
        }
    }

    void onNextClick() {
        GameManager.Instance.NextPlaylist();
    }

    void onAgainClick() {
        GameManager.Instance.PlayAgain();
    }
}
