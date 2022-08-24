using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeView : MonoBehaviour
{
    public RectTransform tfmScrollContent;

    public PlaylistCellView objPlaylistCell;

    private PlaylistCellView[] playlistCells;

    public void UpdatePlaylist() {

        Playlist[] playlists = GameManager.Instance.gameData.arrPlaylist;
        playlistCells = new PlaylistCellView[playlists.Length];

        // instantiate cells
        // further improvement: if data size is large, should use infinity scroll view and reuse the cell here
        VerticalLayoutGroup layoutGp = tfmScrollContent.GetComponent<VerticalLayoutGroup>();

        float scrollHeight = layoutGp.padding.top + layoutGp.padding.bottom;
        float cellSpaceing = layoutGp.spacing;

        for (int i = 0; i < playlists.Length; i++) {
            PlaylistCellView newCell = Instantiate(objPlaylistCell, tfmScrollContent);
            newCell.UpdateView(playlists[i].playlist, i);
            playlistCells[i] = newCell;

            scrollHeight += newCell.GetComponent<RectTransform>().sizeDelta.y + cellSpaceing;
        }

        // update height
        tfmScrollContent.sizeDelta = new Vector2(tfmScrollContent.sizeDelta.x, scrollHeight);


    }

}
