using UnityEngine;

public class PlayerInfosUI : MonoBehaviour
{
    [SerializeField] private Transform playerInfosContent;
    [SerializeField] private Transform playerInfosElementTemplate;

    private void Start() {
        for (int i = 0; i < MultiplayerManager.Instance.GetPlayerCount(); i++) {
            Transform playerInfosElement = Instantiate(playerInfosElementTemplate, playerInfosContent);
            playerInfosElement.GetComponent<PlayerInfosElement>().Initialize(i);
        }
    }
}
