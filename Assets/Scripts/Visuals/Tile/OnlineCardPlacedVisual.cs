using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineCardPlacedVisual : MonoBehaviour {
    [SerializeField] StartTile startTile;
    [SerializeField] Transform onlineCardPlaced;

    private void Start() {
        startTile.OnOnlineCardPlaced += StartTile_OnOnlineCardPlaced;

        PlayerController.OnTeamChanged += PlayerController_OnTeamChanged;
    }

    private void PlayerController_OnTeamChanged(object sender, PlayerController.TeamChangedArgs e) {
        if (!sender.Equals(PlayerController.LocalInstance)) return;
        if (startTile.GetTeam() != e.team) return;
        Show();
    }

    private void StartTile_OnOnlineCardPlaced(object sender, StartTile.OnlineCardPlacedArgs e) {
        Hide();
        if (e.startTile != startTile) return;
        if (PlayerController.LocalInstance.GetTeam() != startTile.GetTeam()) return;
        if (e.onlineCardPlaced) return;
        Show();
    }

    private void OnDestroy() {
        startTile.OnOnlineCardPlaced -= StartTile_OnOnlineCardPlaced;
    }

    private void Show() {
        onlineCardPlaced.gameObject.SetActive(true);
    }
    private void Hide() {
        onlineCardPlaced.gameObject.SetActive(false);
    }
}
