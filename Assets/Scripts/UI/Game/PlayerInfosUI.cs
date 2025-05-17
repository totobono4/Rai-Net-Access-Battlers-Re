using System.Collections.Generic;
using UnityEngine;

public class PlayerInfosUI : MonoBehaviour
{
    [SerializeField] private Transform playerInfosContent;
    [SerializeField] private Transform playerInfosElementTemplate;

    private List<PlayerInfosElement> playerInfosElements;

    private void Awake() {
        playerInfosElements = new List<PlayerInfosElement>();
    }

    private void Start() {
        for (int i = 0; i < MultiplayerManager.Instance.GetPlayerCount(); i++) {
            Transform playerInfosElementTransform = Instantiate(playerInfosElementTemplate, playerInfosContent);
            PlayerInfosElement playerInfosElement = playerInfosElementTransform.GetComponent<PlayerInfosElement>();
            playerInfosElements.Add(playerInfosElement);
            playerInfosElement.Initialize(i);
        }
    }

    public void Clean() {
        foreach (PlayerInfosElement playerInfosElement in playerInfosElements) {
            playerInfosElement.Clean();
        }

        Destroy(gameObject);
    }
}
