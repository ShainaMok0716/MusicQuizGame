using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject objImgLoading;
    public GameObject objSongLoading;

    public RawImage imgSong;
    public Button[] arrButton;
    public Text[] arrTextChoice;

    private int _questionIdx = 0;
    private Question question;

    private ColorBlock cbCorrect = ColorBlock.defaultColorBlock;
    private ColorBlock cbIncorrect = ColorBlock.defaultColorBlock;
    private ColorBlock cbIdle = ColorBlock.defaultColorBlock;

    private bool _isShowingResult = false;

    private void Awake()
    {
        // set color block for result display
        cbCorrect.normalColor = Color.green;
        cbIncorrect.normalColor = Color.red;
        cbIdle.normalColor = Color.white;
        cbCorrect.highlightedColor = Color.green;
        cbIncorrect.highlightedColor = Color.red;
        cbIdle.highlightedColor = Color.white;
    }

    private void Start()
    {
        objImgLoading.SetActive(false);
        objSongLoading.SetActive(false);

        // set up button calblack
        for (int i = 0; i < arrButton.Length; i++) {
            int idx = i;
            arrButton[i].onClick.AddListener(delegate { ShowResult(idx); });
        }

        _isShowingResult = false;
    }

    public void UpdateView(int questionIdx, Question question) {

        StopCoroutine(CheckImgDownloadedIE());
        StopCoroutine(CheckSongDownloadedIE());

        _isShowingResult = false;
        _questionIdx = questionIdx;
        this.question = question;

        // update button ui
        for (int i = 0; i < arrButton.Length; i++) {
            arrButton[i].colors = cbIdle;
            arrTextChoice[i].text = question.choices[i].title + "\n" + question.choices[i].artist;
        }

        // update img
        imgSong.texture = AssetsManager.Instance.arrTexture[questionIdx];
        if (AssetsManager.Instance.arrTexture[questionIdx] == null) {
            // if download is not completed, show loading ui
            objImgLoading.SetActive(true);
            // wait until donwload finish
            StartCoroutine(CheckImgDownloadedIE());
        } else {
            objImgLoading.SetActive(false);
        }

        // play sound
        if (AssetsManager.Instance.arrTexture[questionIdx] == null)
        {
            audioSource.Stop();
            // if download is not completed, show loading ui
            objSongLoading.SetActive(true);
            // wait until donwload finish
            StartCoroutine(CheckSongDownloadedIE());
        } else {
            audioSource.clip = AssetsManager.Instance.arrAudioClip[questionIdx];
            objSongLoading.SetActive(false);
            audioSource.Play();
        }
    }

    public void ShowResult(int answer) {
        // showing result, do not accept any answer
        // we might need to return here too if the download is not completed depends on the requirement.
        if (_isShowingResult == true) return;
        _isShowingResult = true;

        // record player answer
        GameManager.Instance.gameData.RecordPlayerAnswer(answer);

        // update view
        EventSystem.current.SetSelectedGameObject(null);
        if (question.answerIndex == answer) {
            // correct
            arrButton[answer].colors = cbCorrect;
        } else {
            // incorrect
            arrButton[answer].colors = cbIncorrect;
            arrButton[question.answerIndex].colors = cbCorrect;
        }

        // next question
        GameManager.Instance.NextQuestion();
    }

    public void ResetView() {
        audioSource.Stop();

        for (int i = 0; i < arrButton.Length; i++)
        {
            arrButton[i].colors = cbIdle;
        }
    }

    IEnumerator CheckImgDownloadedIE() {
        yield return new WaitUntil(() => AssetsManager.Instance.arrTexture[_questionIdx] != null);
        objImgLoading.SetActive(false);
        imgSong.texture = AssetsManager.Instance.arrTexture[_questionIdx];
    }

    IEnumerator CheckSongDownloadedIE() {
        yield return new WaitUntil(() => AssetsManager.Instance.arrAudioClip[_questionIdx] != null);
        objSongLoading.SetActive(false);
        audioSource.clip = AssetsManager.Instance.arrAudioClip[_questionIdx];
        audioSource.Play();
    }
}
