using System.Collections.Generic;
using UnityEngine;

public class FireWalledTileVisual : MonoBehaviour
{
    [SerializeField] private BoardTile boardTile;

    private Dictionary<GameBoard.Team, Transform> fireWallVisualsDict = new Dictionary<GameBoard.Team, Transform>();
    [SerializeField] private List<GameBoard.Team> teams;
    [SerializeField] private List<Transform> fireWallVisuals;

    private void Awake() {
        for (int i = 0; i < teams.Count; i++) { fireWallVisualsDict.Add(teams[i], fireWallVisuals[i]); }
    }

    private void Start() {
        BoardTile.OnFireWallUpdate += FireWallUpdated;
    }

    private void FireWallUpdated(object sender, BoardTile.FireWallUpdateArgs e) {
        if (e.boardTile != boardTile) return;

        Hide();
        if (e.boardTile.HasFireWall()) Show(fireWallVisualsDict[e.fireWallTeam]);
    }

    private void Show(Transform fireWallVisual) {
        fireWallVisual.gameObject.SetActive(true);
    }

    private void Hide() {
        foreach (Transform fireWallVisual in fireWallVisuals) fireWallVisual.gameObject.SetActive(false);
    }
}
