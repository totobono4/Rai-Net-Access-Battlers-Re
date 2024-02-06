using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameUI : MonoBehaviour
{
    [SerializeField] Transform lobbyListContent;
    [SerializeField] Transform lobbyListElementTemplate;

    [SerializeField] Button quickJoinButton;

    [SerializeField] TMP_InputField codeInputField;
    [SerializeField] Button joinByCodeButton;
    private string lobbyCode;

    private List<Transform> lobbyListElementTransformList;

    private void Awake() {
        lobbyListElementTransformList = new List<Transform>();

        quickJoinButton.onClick.AddListener(() => {
            LobbyManager.Instance.QuickJoinLobby();
        });
        joinByCodeButton.onClick.AddListener(() => {
            LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
        });

        codeInputField.onValueChanged.AddListener((string newString) => {
            codeInputField.text = newString.ToUpper();
            lobbyCode = newString.ToUpper();
        });

        lobbyCode = codeInputField.text;
    }

    private void Start() {
        LobbyManager.Instance.OnRefreshLobbiesUpdate += LobbyManager_OnRefreshLobbiesUpdate;
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
