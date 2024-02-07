using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour {
    public static InputSystem Instance { get; private set; }

    private PlayerInputActions inputActions;
    [SerializeField] private LayerMask mousePositionLayer;

    public EventHandler OnPlayerAction;

    private bool active;

    private void Awake() {
        Instance = this;

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Action.performed += PlayerAction;

        active = true;
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

    public void PlayerAction(InputAction.CallbackContext callbackContext) {
        OnPlayerAction?.Invoke(this, EventArgs.Empty);
    }
}
