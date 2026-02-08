using RaiNet.Network.Data;
using System.Collections.Generic;
using Totobono4.Network;
using UnityEngine;

namespace RaiNet.UI {
    public class PlayerInfosUI : MonoBehaviour {
        [SerializeField] private Transform playerInfosContent;
        [SerializeField] private Transform playerInfosElementTemplate;

        private List<PlayerInfosElementUI> playerInfosElements;

        private void Awake() {
            playerInfosElements = new List<PlayerInfosElementUI>();
        }

        private void Start() {
            for (int i = 0; i < MultiplayerManager<RaiNetPlayerData>.Instance.GetPlayerCount(); i++) {
                Transform playerInfosElementTransform = Instantiate(playerInfosElementTemplate, playerInfosContent);
                PlayerInfosElementUI playerInfosElement = playerInfosElementTransform.GetComponent<PlayerInfosElementUI>();
                playerInfosElements.Add(playerInfosElement);
                playerInfosElement.Initialize(i);
            }
        }

        public void Clean() {
            foreach (PlayerInfosElementUI playerInfosElement in playerInfosElements) {
                playerInfosElement.Clean();
            }

            Destroy(gameObject);
        }
    }
}