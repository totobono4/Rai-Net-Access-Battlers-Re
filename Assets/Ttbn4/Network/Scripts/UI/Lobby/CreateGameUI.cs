using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ttbn4.Network.UI {
    public class CreateGameUI<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
        [SerializeField] Button createGameButton;
        [SerializeField] TMP_InputField lobbyNameInputField;
        [SerializeField] Toggle privateLobbyToggle;

        private string lobbyName;
        private bool isPrivate;

        private void Awake() {
            isPrivate = false;
            privateLobbyToggle.isOn = isPrivate;

            createGameButton.onClick.AddListener(() => {
                LobbyManager<TCustomData>.Instance.CreateLobby(lobbyName, isPrivate);
            });
            lobbyNameInputField.onValueChanged.AddListener((string newString) => {
                lobbyName = newString;
            });
            privateLobbyToggle.onValueChanged.AddListener((bool newState) => {
                isPrivate = newState;
            });
        }

        private void Start() {
            lobbyName = MultiplayerManager<TCustomData>.Instance.GetPlayerName() + "'s lobby";
            lobbyNameInputField.text = lobbyName;
        }
    }
}