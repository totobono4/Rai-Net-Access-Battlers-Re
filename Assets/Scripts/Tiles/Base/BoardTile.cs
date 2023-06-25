using System;
public class BoardTile : PlayTile {
    private bool fireWalled;
    private GameBoard.Team fireWallTeam;

    public EventHandler<FireWallUpdateArgs> OnFireWallUpdate;
    public class FireWallUpdateArgs : EventArgs {
        public BoardTile boardTile;
        public bool fireWalled;
        public GameBoard.Team fireWallTeam;
    }

    protected override void Awake() {
        base.Awake();
        fireWalled = false;
    }

    public bool HasFireWall() {
        return fireWalled;
    }

    public void SetFireWall(GameBoard.Team fireWallTeam) {
        this.fireWallTeam = fireWallTeam;
        fireWalled = true;
        FireWallUpdate();
    }

    public void UnsetFireWall() {
        fireWalled = false;
        FireWallUpdate();
    }

    public GameBoard.Team GetFireWall() { return fireWallTeam; }
    private void FireWallUpdate() {
        OnFireWallUpdate?.Invoke(this, new FireWallUpdateArgs { boardTile = this, fireWalled = fireWalled, fireWallTeam = fireWallTeam });
    }
}
