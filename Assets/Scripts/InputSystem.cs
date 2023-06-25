using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour {
    private PlayerInputActions inputActions;
    [SerializeField] LayerMask mousePositionLayer;

    public EventHandler OnPlayerAction;

    private void Awake() {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Action.performed += PlayerAction;
    }

    public Vector3 GetMouseWorldPosition() {
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
