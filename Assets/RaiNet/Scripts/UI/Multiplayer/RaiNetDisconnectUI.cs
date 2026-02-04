public class RaiNetDisconnectUI : DisconnectedUI<RaiNetPlayerData>
{
    protected override void Show() {
        if (InputSystem.Instance != null) InputSystem.Instance.SetInactive();
        base.Show();
    }
}
