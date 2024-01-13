using System;
using Unity.Netcode;

public class BoardTile : PlayTile {
    private NetworkVariable<GameBoard.Team> fireWallTeam;

    public static EventHandler<FireWallUpdateArgs> OnFireWallUpdate;
    public class FireWallUpdateArgs : EventArgs {
        public BoardTile boardTile;
        public GameBoard.Team fireWallTeam;
    }

    protected override void Awake() {
        base.Awake();

        fireWallTeam = new NetworkVariable<GameBoard.Team>();
        fireWallTeam.Value = GameBoard.Team.None;
        fireWallTeam.OnValueChanged += FireWallUpdate;
    }

    private void FireWallUpdate(GameBoard.Team previous, GameBoard.Team current) {
        SetFireWall(current);
    }

    public bool HasFireWall() {
        return fireWallTeam.Value != GameBoard.Team.None;
    }

    public void SetFireWall(GameBoard.Team fireWallTeam) {
        this.fireWallTeam.Value = fireWallTeam;
        FireWallUpdate();
    }

    public void UnsetFireWall() {
        fireWallTeam.Value = GameBoard.Team.None;
        FireWallUpdate();
    }

    public GameBoard.Team GetFireWall() { return fireWallTeam.Value; }

    private void FireWallUpdate() {
        OnFireWallUpdate?.Invoke(this, new FireWallUpdateArgs { boardTile = this, fireWallTeam = fireWallTeam.Value });
    }
}
