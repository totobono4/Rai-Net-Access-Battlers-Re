public class RaiNetDisconnectUI : DisconnectedUI
{
    protected override void Show() {
        if (InputSystem.Instance != null) InputSystem.Instance.SetInactive();
        base.Show();
    }
}
