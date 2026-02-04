public class RaiNetPauseTabUI : PauseTabUI
{
    public override void Show() {
        InputSystem.Instance.SetInactive();
        base.Show();
    }

    public override void Hide() {
        InputSystem.Instance.SetActive();
        base.Hide();
    }
}
