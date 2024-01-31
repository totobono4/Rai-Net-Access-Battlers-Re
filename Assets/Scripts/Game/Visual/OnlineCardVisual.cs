using System;
using UnityEngine;

public class OnlineCardVisual : CardVisual
{
    [SerializeField] private OnlineCard onlineCard;
    [SerializeField] private Transform unknown, link, virus;

    private void Awake() {
        onlineCard.OnTileParentChanged += OnlineCard_OnTileParentChanged;
        onlineCard.OnStateChanged += OnlineCard_OnStateChanged;
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
}
