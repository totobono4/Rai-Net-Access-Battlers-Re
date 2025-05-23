using System.Collections.Generic;
using UnityEngine;

public class FireWalledTileVisual : MonoBehaviour
{
    [SerializeField] private BoardTile boardTile;

    private Dictionary<PlayerTeam, Transform> fireWallVisualsDict = new Dictionary<PlayerTeam, Transform>();
    [SerializeField] private List<PlayerTeam> teams;
    [SerializeField] private List<Transform> fireWallVisuals;

    private void Awake() {
        for (int i = 0; i < teams.Count; i++) { fireWallVisualsDict.Add(teams[i], fireWallVisuals[i]); }
    }

    private void Start() {
        BoardTile.OnFireWallUpdate += BoardTile_OnFireWallUpdate;
    }

    private void BoardTile_OnFireWallUpdate(object sender, BoardTile.FireWallUpdateArgs e) {
        if (e.boardTile != boardTile) return;
        Hide();
        if (!e.boardTile.HasFireWall()) return;
        Show(fireWallVisualsDict[e.fireWallTeam]);
    }

    private void OnDestroy() {
        BoardTile.OnFireWallUpdate -= BoardTile_OnFireWallUpdate;
    }

    private void Show(Transform fireWallVisual) {
        fireWallVisual.gameObject.SetActive(true);
    }
    private void Hide() {
        foreach (Transform fireWallVisual in fireWallVisuals) fireWallVisual.gameObject.SetActive(false);
    }
}
