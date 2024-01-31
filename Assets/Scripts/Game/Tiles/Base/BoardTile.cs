using System;
using Unity.Netcode;

public class BoardTile : PlayTile {
    private NetworkVariable<Team> fireWallTeam;

    public static EventHandler<FireWallUpdateArgs> OnFireWallUpdate;
    public class FireWallUpdateArgs : EventArgs {
        public BoardTile boardTile;
        public Team fireWallTeam;
    }

    protected override void Awake() {
        base.Awake();

        fireWallTeam = new NetworkVariable<Team>();
        fireWallTeam.Value = Team.None;
        fireWallTeam.OnValueChanged += FireWallTeam_OnValueChanged;
    }

    private void FireWallTeam_OnValueChanged(Team previous, Team current) {
        SetFireWall(current);
    }

    public bool HasFireWall() {
        return fireWallTeam.Value != Team.None;
    }

    public void SetFireWall(Team fireWallTeam) {
        this.fireWallTeam.Value = fireWallTeam;
        FireWallUpdate();
    }

    public void UnsetFireWall() {
        fireWallTeam.Value = Team.None;
        FireWallUpdate();
    }

    public Team GetFireWall() { return fireWallTeam.Value; }

    private void FireWallUpdate() {
        OnFireWallUpdate?.Invoke(this, new FireWallUpdateArgs { boardTile = this, fireWallTeam = fireWallTeam.Value });
    }
}
