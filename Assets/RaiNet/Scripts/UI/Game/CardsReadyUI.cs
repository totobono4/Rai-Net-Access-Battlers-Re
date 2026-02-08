using RaiNet.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaiNet.UI {
    public class CardsReadyUI : MonoBehaviour {
#if PLATFORM_STANDALONE_WIN || PLATFORM_STANDALONE_LINUX
        private const string INFOS_TEXT = "Click on starting tiles to place your Link and Virus cards";
#endif
#if PLATFORM_ANDROID
    private const string INFOS_TEXT = "Tap starting tiles to place your Link and Virus cards";
#endif

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

        private void Start() {
            InputSystem inputSystem = InputSystem.Instance;

            infos.text = INFOS_TEXT;
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

        public void Clean() {
            PlayerController.OnTeamChanged -= PlayerController_OnTeamChanged;
            PlayerEntity.OnOnlineCardsPlaced -= PlayerEntity_OnOnlineCardsPlaced;
            PlayerEntity.OnCardsReady -= PlayerEntity_OnCardsReady;

            Destroy(gameObject);
        }
    }
}