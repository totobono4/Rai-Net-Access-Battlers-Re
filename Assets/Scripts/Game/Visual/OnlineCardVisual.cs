using System;
using UnityEngine;

public class OnlineCardVisual : CardVisual
{
    [SerializeField] private OnlineCard onlineCard;
    [SerializeField] private Transform unknown, link, virus, revealed, notfound;

    private void Awake() {
        onlineCard.OnTileParentChanged += OnlineCard_OnTileParentChanged;
        onlineCard.OnStateChanged += OnlineCard_OnStateChanged;
        onlineCard.OnRevealValueChanged += OnlineCard_OnRevealValueChanged;
        onlineCard.OnNotFoundValueChanged += OnlineCard_OnNotFoundValueChanged;
    }

    private void OnlineCard_OnTileParentChanged(object sender, Card.TileParentChangedArgs e) {
        if (e.tile is BoardTile && e.team == Team.Blue) transform.rotation = e.tile.transform.rotation * Quaternion.Euler(0, 180, 0);
        else transform.rotation = e.tile.transform.rotation;
    }

    private void OnlineCard_OnStateChanged(object sender, OnlineCard.StateChangedArgs e) {
        switch(e.state) {
            case OnlineCard.CardState.Unknown: ShowUnknown(); break;
            case OnlineCard.CardState.Link: ShowLink(); break;
            case OnlineCard.CardState.Virus: ShowVirus(); break;
        }
    }

    private void OnlineCard_OnRevealValueChanged(object sender, EventArgs e) {
        if (onlineCard.IsRevealed() && onlineCard.GetTeam() == PlayerController.LocalInstance.GetTeam()) {
            ShowReveal();
        }
        else {
            HideReveal();
        }
    }

    private void OnlineCard_OnNotFoundValueChanged(object sender, EventArgs e) {
        if (onlineCard.GetTeam() != PlayerController.LocalInstance.GetTeam()) ShowNotFound();
    }

    private void ShowUnknown() {
        unknown.gameObject.SetActive(true);
        link.gameObject.SetActive(false);
        virus.gameObject.SetActive(false);
    }

    private void ShowLink() {
        unknown.gameObject.SetActive(false);
        link.gameObject.SetActive(true);
        virus.gameObject.SetActive(false);

    }

    private void ShowVirus() {
        unknown.gameObject.SetActive(false);
        link.gameObject.SetActive(false);
        virus.gameObject.SetActive(true);
    }

    private void ShowReveal() {
        revealed.gameObject.SetActive(true);
    }

    private void HideReveal() {
        revealed.gameObject.SetActive(false);
    }

    private void ShowNotFound() {
        notfound.gameObject.SetActive(true);
    }
}
