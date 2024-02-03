using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameUI : MonoBehaviour
{
    [SerializeField] Button createGameButton;
    [SerializeField] TMP_InputField lobbyNameInputField;
    [SerializeField] Toggle privateLobbyToggle;

    private string lobbyName;
    private bool isPrivate;

    private void Awake() {
        isPrivate = false;
        privateLobbyToggle.isOn = isPrivate;

        createGameButton.onClick.AddListener(() => {
            LobbyManager.Instance.CreateLobby(lobbyName, isPrivate);
        });
        lobbyNameInputField.onValueChanged.AddListener((string newString) => {
            lobbyName = newString;
        });
        privateLobbyToggle.onValueChanged.AddListener((bool newState) => {
            isPrivate = newState;
        });
    }

    private void Start() {
        lobbyName = MultiplayerManager.Instance.GetPlayerName() + "'s lobby";
        lobbyNameInputField.text = lobbyName;
    }
}
