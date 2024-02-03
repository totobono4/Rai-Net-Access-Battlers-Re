using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameUI : MonoBehaviour
{
    [SerializeField] Transform lobbyListContent;
    [SerializeField] Transform lobbyListElementTemplate;

    [SerializeField] Button refreshLobbyListButton;

    [SerializeField] Button quickJoinButton;

    [SerializeField] TMP_InputField codeInputField;
    [SerializeField] Button joinByCodeButton;
    private string lobbyCode;

    private List<Transform> lobbyListElementTransformList;

    private void Awake() {
        lobbyListElementTransformList = new List<Transform>();

        refreshLobbyListButton.onClick.AddListener(() => {
            LobbyManager.Instance.RefreshLobbies();
        });
        quickJoinButton.onClick.AddListener(() => {
            LobbyManager.Instance.QuickJoinLobby();
        });
        joinByCodeButton.onClick.AddListener(() => {
            LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
        });

        codeInputField.onValueChanged.AddListener((string newString) => {
            lobbyCode = newString;
        });

        lobbyCode = codeInputField.text;
    }

    private void Start() {
        LobbyManager.Instance.OnRefreshLobbiesUpdate += LobbyManager_OnRefreshLobbiesUpdate;

        if (AuthenticationService.Instance.IsSignedIn) LobbyManager.Instance.RefreshLobbies();
    }

    private void LobbyManager_OnRefreshLobbiesUpdate(object sender, LobbyManager.RefreshLobbiesUpdateArgs e) {
        foreach (Transform t in lobbyListElementTransformList) Destroy(t.gameObject);
        lobbyListElementTransformList.Clear();

        foreach(Lobby lobby in e.lobbies) {
            Transform lobbyListElementTransform = Instantiate(lobbyListElementTemplate, lobbyListContent);
            lobbyListElementTransformList.Add(lobbyListElementTransform);
            lobbyListElementTransform.GetComponent<LobbyListElement>().SetLobby(lobby);
        }
    }
}
