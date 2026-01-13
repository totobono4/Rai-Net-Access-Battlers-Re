using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListElement : MonoBehaviour
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyPlayerCountText;
    [SerializeField] private TextMeshProUGUI lobbyBuildVersionText;
    [SerializeField] private TextMeshProUGUI lobbyNetworkCompatibleText;

    private Lobby lobby;

    private void Awake() {
        joinLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.JoinLobbyById(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayerCountText.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
        lobbyBuildVersionText.text = LobbyManager.Instance.GetLobbyHostBuildVersion(lobby);
        lobbyNetworkCompatibleText.text = LobbyManager.Instance.IsLobbyNeworkCompatible(lobby) ? "Yes" : "No";
    }
}
