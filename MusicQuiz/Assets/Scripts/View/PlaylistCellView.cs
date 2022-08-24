using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistCellView : MonoBehaviour
{
    public Button button;
    public Text textName;

    public int myPlaylistIdx;

    public void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void UpdateView(string name, int idx) {
        textName.text = name;
        myPlaylistIdx = idx;
    }

    void OnClick() {
        GameManager.Instance.StartQuiz(myPlaylistIdx);
    }
}
