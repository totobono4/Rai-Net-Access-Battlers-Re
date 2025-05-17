using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardsReadyUI : MonoBehaviour
{
    private PlayerEntity playerEntity;
    [SerializeField] private TextMeshProUGUI infos;
    [SerializeField] private Button readyButton;

    private void Awake() {
        readyButton.interactable = false;
        readyButton.onClick.AddListener(() => {
            playerEntity.SetCardsReady();
        });

        PlayerController.OnTeamChanged += PlayerController_OnTeamChanged;
        PlayerEntity.OnOnlineCardsPlaced += PlayerEntity_OnOnlineCardsPlaced;
        PlayerEntity.OnCardsReady += PlayerEntity_OnCardsReady;
    }

    private void OnDestroy() {
        PlayerController.OnTeamChanged -= PlayerController_OnTeamChanged;
        PlayerEntity.OnOnlineCardsPlaced -= PlayerEntity_OnOnlineCardsPlaced;
        PlayerEntity.OnCardsReady -= PlayerEntity_OnCardsReady;
    }

    private void Start() {
        InputSystem inputSystem = InputSystem.Instance;

        infos.text = $"Place your link cards with {inputSystem.GetActionbinding()} and virus cards with {inputSystem.GetSecondaryActionbinding()}";
    }

    private void PlayerController_OnTeamChanged(object sender, PlayerController.TeamChangedArgs e) {
        if (!sender.Equals(PlayerController.LocalInstance)) return;

        playerEntity = GameBoard.Instance.GetPlayerEntityByTeam(e.team);
    }

    private void PlayerEntity_OnOnlineCardsPlaced(object sender, PlayerEntity.OnlineCardsPlacedArgs e) {
        if (!sender.Equals(playerEntity)) return;
        readyButton.interactable = e.onlineCardsPlaced;
    }

    private void PlayerEntity_OnCardsReady(object sender, PlayerEntity.CardsReadyArgs e) {
        if (!sender.Equals(playerEntity)) return;
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
