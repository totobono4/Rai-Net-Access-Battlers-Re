using System.Collections.Generic;
using UnityEngine;

public class TerminalGroup : TileMap {
    [SerializeField] private TerminalGroupSO terminalGroupSO;
    private List<TerminalCard> terminalCards = new List<TerminalCard>();

    protected override void Awake() {
        base.Awake();

        List<Transform> transforms = terminalGroupSO.GetTerminalCards();
        for (int i  = 0; i < transforms.Count; i++) {
            TerminalCard terminalCard = Instantiate(transforms[i]).GetComponent<TerminalCard>();
            terminalCard.SetTileParent(GetTile(0,i));
            terminalCards.Add(terminalCard);
        }
    }

    public List<TerminalCard> GetTerminalCards() { return terminalCards; }
}
