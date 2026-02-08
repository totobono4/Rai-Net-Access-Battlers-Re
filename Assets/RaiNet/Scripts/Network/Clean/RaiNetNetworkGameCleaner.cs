using RaiNet.Game;
using RaiNet.Network.Data;
using RaiNet.UI;
using System;
using Ttbn4.Network.Clean;
using UnityEngine;

namespace RaiNet.Network.Clean {
    public class RaiNetNetworkGameCleaner : NetworkGameCleaner<RaiNetPlayerData> {
        [SerializeField] CardsReadyUI cardsReadyUI;
        [SerializeField] GameOverUI gameOverUI;
        [SerializeField] PlayerInfosUI playerInfosUI;

        protected override void Start() {
            base.Start();
            gameOverUI.OnClean += GameOverUI_OnClean;
        }

        private void GameOverUI_OnClean(object sender, EventArgs e) {
            Clean();
        }

        protected override void Clean() {
            base.Clean();
            gameOverUI.OnClean -= GameOverUI_OnClean;

            GameManager.Instance.Clean();
            cardsReadyUI.Clean();
            gameOverUI.Clean();
            playerInfosUI.Clean();
        }
    }
}