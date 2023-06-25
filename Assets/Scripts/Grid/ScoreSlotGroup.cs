using System.Collections.Generic;

public class ScoreSlotGroup : TileMap
{
    List<OnlineCard> onlineCards = new List<OnlineCard>();

    public void AddOnlineCard(OnlineCard onlineCard) {
        int width = GetWidth();

        onlineCard.SetTileParent(GetTile(onlineCards.Count % width, onlineCards.Count / width));
        onlineCards.Add(onlineCard);
    }
}
