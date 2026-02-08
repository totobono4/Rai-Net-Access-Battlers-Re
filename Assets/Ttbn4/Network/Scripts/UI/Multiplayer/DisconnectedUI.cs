using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ttbn4.Network.UI {
    public class DisconnectedUI<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
        [SerializeField] private Enum scenes;
        [SerializeField] private TextMeshProUGUI disconnectStatusText;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private bool disconnectHost;

        public EventHandler OnClean;

        protected virtual void Awake() {
            mainMenuButton.onClick.AddListener(() => {
                OnClean?.Invoke(this, EventArgs.Empty);
                NetworkSceneLoader.Load(NetworkSceneLoader.Scene.MainMenuScene);
            });
        }

        protected virtual void Start() {
            if (SceneManager.GetActiveScene().name == NetworkSceneLoader.Scene.LobbyRoomScene.ToString() && !disconnectHost && NetworkManager.Singleton.IsHost) {
                Hide();
                return;
            }

            if (NetworkManager.Singleton.IsHost) {
                disconnectStatusText.text = "Client have been disconnected";
                MultiplayerManager<TCustomData>.Instance.OnClientDisconnect += MultiplayerManager_OnClientDisconnect;
            }
            else {
                disconnectStatusText.text = "You have been disconnected";
                MultiplayerManager<TCustomData>.Instance.OnHostDisconnect += MultiplayerManager_OnHostDisconnect;
            }

            Hide();
        }

        private void MultiplayerManager_OnClientDisconnect(object sender, EventArgs e) {
            Show();
        }

        private void MultiplayerManager_OnHostDisconnect(object sender, EventArgs e) {
            Show();
        }

        protected virtual void Show() {
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        public virtual void Clean() {
            MultiplayerManager<TCustomData>.Instance.OnClientDisconnect -= MultiplayerManager_OnClientDisconnect;
            MultiplayerManager<TCustomData>.Instance.OnHostDisconnect -= MultiplayerManager_OnHostDisconnect;

            Destroy(gameObject);
        }
    }
}