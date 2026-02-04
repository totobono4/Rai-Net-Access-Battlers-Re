using System;
using Unity.Netcode;

public abstract class TerminalCard : Card {
    protected NetworkVariable<bool> used;

    public EventHandler OnUsedValueChanged;

    protected override void Awake() {
        base.Awake();

        used = new NetworkVariable<bool>(false);
        used.OnValueChanged += Used_OnValueChanged;
    }

    private void Used_OnValueChanged(bool previousValue, bool newValue) {
        OnUsedValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsUsed() {
        return used.Value;
    }

    protected void SetUsed() {
        used.Value = true;
    }
    protected void UnsetUsed() {
        used.Value = false;
    }

    public override void Clean() {
        used.OnValueChanged -= Used_OnValueChanged;

        base.Clean();
    }
}
