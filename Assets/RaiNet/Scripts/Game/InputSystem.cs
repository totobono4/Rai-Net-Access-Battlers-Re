using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RaiNet.Game {
    public class InputSystem : MonoBehaviour {
        public static InputSystem Instance { get; private set; }

        private PlayerInputActions inputActions;
        [SerializeField] private LayerMask mousePositionLayer;

        public enum PlayerActionType {
            Action,
            SecondaryAction
        }

        public EventHandler<PlayerActionEventArgs> OnPlayerAction;

        public class PlayerActionEventArgs : EventArgs {
            public PlayerActionType playerActionType;
        }

        private bool active;

        private void Awake() {
            Instance = this;

            inputActions = new PlayerInputActions();
            inputActions.Player.Enable();

            inputActions.Player.Action.performed += PlayerAction;
            inputActions.Player.SecondaryAction.performed += PlayerAction;

            active = true;
        }

        private void OnDestroy() {
            inputActions.Player.Action.performed -= PlayerAction;
            inputActions.Player.SecondaryAction.performed -= PlayerAction;
            inputActions.Player.Disable();
            Instance = null;
        }

        public void SetActive() {
            active = true;
        }
        public void SetInactive() {
            active = false;
        }

        public Vector3 GetMouseWorldPosition() {
            if (!active) return Vector3.positiveInfinity;

            Vector2 mousePos = inputActions.Player.Hover.ReadValue<Vector2>();
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, float.MaxValue, mousePositionLayer)) {
                return hitInfo.point;
            }
            return Vector3.positiveInfinity;
        }

        private void PlayerAction(InputAction.CallbackContext context) {
            if (!Enum.TryParse(context.action.name, out PlayerActionType playerActionType)) return;

            if (!context.action.WasPressedThisFrame()) return;

            OnPlayerAction?.Invoke(this, new PlayerActionEventArgs {
                playerActionType = playerActionType
            });
        }
    }
}