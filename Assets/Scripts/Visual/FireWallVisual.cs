using System.Collections.Generic;
using UnityEngine;

public class FireWallVisual : MonoBehaviour
{
    [SerializeField] private Tile Reference;

    private Dictionary<GameBoard.Team, Transform> fireWallVisualsDict = new Dictionary<GameBoard.Team, Transform>();
    [SerializeField] private List<GameBoard.Team> teams;
    [SerializeField] private List<Transform> fireWallVisuals;

    private void Awake() {
        for (int i = 0; i < teams.Count; i++) { fireWallVisualsDict.Add(teams[i], fireWallVisuals[i]); }
    }

    private void Start() {
        if (Reference.TryGetComponent(out BoardTile boardTile)) {
            boardTile.OnFireWallUpdate += FireWallUpdated;
        }
    }

    private void FireWallUpdated(object sender, BoardTile.FireWallUpdateArgs e) {
        foreach (Transform fireWallVisual in fireWallVisualsDict.Values) { Hide(fireWallVisual); }
        if (e.boardTile == Reference && e.fireWalled) {
            Show(fireWallVisualsDict[e.fireWallTeam]);
        }
    }

    private void Show(Transform fireWallVisual) {
        fireWallVisual.gameObject.SetActive(true);
    }

    private void Hide(Transform fireWallVisual) {
        fireWallVisual.gameObject.SetActive(false);
    }
}
