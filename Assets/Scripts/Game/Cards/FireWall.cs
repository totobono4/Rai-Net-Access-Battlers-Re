public class FireWall : TerminalCard {
    public override void Action(Tile tile, out bool finished, out int tokenCost) {
        tokenCost = 0; finished = false;

        if (tile is not BoardTile) return;

        used.Value = !used.Value;

        if (used.Value) (tile as BoardTile).SetFireWall(GetTeam());
        else (tile as BoardTile).UnsetFireWall();

        tokenCost = GetTokenCost();
        finished = true;
        return;
    }

    public override bool IsActionable(Tile tile) {
        if (tile is not BoardTile) return false;

        if (!used.Value.Equals((tile as BoardTile).HasFireWall())) return false;

        if (!used.Value) {
            if ((tile as BoardTile).GetCard(out Card card) && !card.GetTeam().Equals(GetTeam())) return false;
            if (tile is ExitTile) return false;
        }
        else {
            if ((tile as BoardTile).GetFireWall() != GetTeam()) return false;
        }
        return true;
    }
}
