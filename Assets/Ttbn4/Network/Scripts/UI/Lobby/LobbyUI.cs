using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI<TCustomData> : MonoBehaviour where TCustomData : struct, IEquatable<TCustomData>, INetworkSerializable {
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        playerNameInputField.text = MultiplayerManager<TCustomData>.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newString) => {
            MultiplayerManager<TCustomData>.Instance.SetPlayerName(newString);
        });
    }
}
