using System;
using Unity.Netcode;

public class BoardTile : PlayTile {
    private NetworkVariable<PlayerTeam> fireWallTeam;

    public static EventHandler<FireWallUpdateArgs> OnFireWallUpdate;
    public class FireWallUpdateArgs : EventArgs {
        public BoardTile boardTile;
        public PlayerTeam fireWallTeam;
    }

    protected override void Awake() {
        base.Awake();

        fireWallTeam = new NetworkVariable<PlayerTeam>();
        fireWallTeam.Value = PlayerTeam.None;
        fireWallTeam.OnValueChanged += FireWallTeam_OnValueChanged;
    }

    private void FireWallTeam_OnValueChanged(PlayerTeam previous, PlayerTeam current) {
        SetFireWall(current);
    }

    public bool HasFireWall() {
        return fireWallTeam.Value != PlayerTeam.None;
    }

    public void SetFireWall(PlayerTeam fireWallTeam) {
        this.fireWallTeam.Value = fireWallTeam;
        FireWallUpdate();
    }

    public void UnsetFireWall() {
        fireWallTeam.Value = PlayerTeam.None;
        FireWallUpdate();
    }

    public PlayerTeam GetFireWall() { return fireWallTeam.Value; }

    private void FireWallUpdate() {
        OnFireWallUpdate?.Invoke(this, new FireWallUpdateArgs { boardTile = this, fireWallTeam = fireWallTeam.Value });
    }
}
