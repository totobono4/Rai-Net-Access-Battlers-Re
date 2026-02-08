using RaiNet.Data;
using RaiNet.Game;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaiNet.Visuals {
    public class OnlineCardVisual : CardVisual {
        [SerializeField] private OnlineCard onlineCard;
        [SerializeField] private Transform unknown, link, virus, lineboostIcon, virusCheckerIcon, notFoundIcon;

        protected override void Awake() {
            base.Awake();

            onlineCard.OnTileParentChanged += OnlineCard_OnTileParentChanged;
            onlineCard.OnStateValueChanged += OnlineCard_OnStateValueChanged;
            onlineCard.OnRevealValueChanged += OnlineCard_OnRevealValueChanged;
            onlineCard.OnNotFoundValueChanged += OnlineCard_OnNotFoundValueChanged;
            onlineCard.OnCapturedValueChanged += OnlineCard_OnCapturedValueChanged;
            onlineCard.OnBoostedValueChanged += OnlineCard_OnBoostedValueChanged;
            onlineCard.OnClean += OnlineCard_OnClean;
        }

        protected override List<SpriteRenderer> InitializeSpriteRenderers() {
            return new List<SpriteRenderer>() {
            unknown.GetComponent<SpriteRenderer>(),
            link.GetComponent<SpriteRenderer>(),
            virus.GetComponent<SpriteRenderer>(),
        };
        }

        private void OnlineCard_OnTileParentChanged(object sender, Card.TileParentChangedArgs e) {
            if (e.tile is BoardTile && e.team == PlayerTeam.Blue) transform.rotation = e.tile.transform.rotation * Quaternion.Euler(0, 180, 0);
            else transform.rotation = e.tile.transform.rotation;
        }

        private void OnlineCard_OnStateValueChanged(object sender, OnlineCard.StateChangedArgs e) {
            switch (e.state) {
                case OnlineCardState.Unknown: ShowUnknown(); break;
                case OnlineCardState.Link: ShowLink(); break;
                case OnlineCardState.Virus: ShowVirus(); break;
            }
        }

        private void OnlineCard_OnBoostedValueChanged(object sender, OnlineCard.BoostUpdateArgs e) {
            HideLineBoostIcon();
            if (e.onlineCard != onlineCard) return;
            if (!e.boosted) return;
            ShowLineBoostIcon();
        }

        private void OnlineCard_OnRevealValueChanged(object sender, EventArgs e) {
            HideVirusCheckerIcon();
            if (onlineCard.GetTeam() != PlayerController.LocalInstance.GetTeam()) return;
            if (!onlineCard.IsRevealed()) return;
            if (onlineCard.IsCaptured()) return;
            ShowVirusCheckerIcon();
        }

        private void OnlineCard_OnNotFoundValueChanged(object sender, EventArgs e) {
            HideNotFoundIcon();
            if (onlineCard.GetTeam() == PlayerController.LocalInstance.GetTeam()) return;
            if (!onlineCard.IsNotFound()) return;
            ShowNotFoundIcon();
        }

        private void OnlineCard_OnCapturedValueChanged(object sender, EventArgs e) {
            if (!onlineCard.IsCaptured()) return;
            HideVirusCheckerIcon();
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

        private void ShowLineBoostIcon() {
            lineboostIcon.gameObject.SetActive(true);
        }
        private void HideLineBoostIcon() {
            lineboostIcon.gameObject.SetActive(false);
        }

        private void ShowVirusCheckerIcon() {
            virusCheckerIcon.gameObject.SetActive(true);
        }
        private void HideVirusCheckerIcon() {
            virusCheckerIcon.gameObject.SetActive(false);
        }

        private void ShowNotFoundIcon() {
            notFoundIcon.gameObject.SetActive(true);
        }
        private void HideNotFoundIcon() {
            notFoundIcon.gameObject.SetActive(false);
        }

        private void OnlineCard_OnClean(object sender, EventArgs e) {
            onlineCard.OnTileParentChanged -= OnlineCard_OnTileParentChanged;
            onlineCard.OnStateValueChanged -= OnlineCard_OnStateValueChanged;
            onlineCard.OnRevealValueChanged -= OnlineCard_OnRevealValueChanged;
            onlineCard.OnNotFoundValueChanged -= OnlineCard_OnNotFoundValueChanged;
            onlineCard.OnCapturedValueChanged -= OnlineCard_OnCapturedValueChanged;
            onlineCard.OnBoostedValueChanged -= OnlineCard_OnBoostedValueChanged;
            onlineCard.OnClean -= OnlineCard_OnClean;
        }
    }
}