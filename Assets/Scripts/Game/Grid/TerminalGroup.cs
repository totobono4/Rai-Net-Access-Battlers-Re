using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TerminalGroup : TileMap {
    [SerializeField] private TerminalGroupSO terminalGroupSO;
    private List<TerminalCard> terminalCards;

    protected override void Awake() {
        base.Awake();

        terminalCards = new List<TerminalCard>();
    }

    public void InstantiateTerminalCards() {
        List<Transform> transforms = new List<Transform>(terminalGroupSO.GetTerminalCards());
        List<int[]> validCoords = GetValidCoords();

        for (int i = 0; i < validCoords.Count; i++) {
            int x = validCoords[i][0];
            int y = validCoords[i][1];

            Transform terminalCardTransform = Instantiate(transforms[i]);

            NetworkObject terminalCardNetwork = terminalCardTransform.GetComponent<NetworkObject>();
            terminalCardNetwork.Spawn();

            TerminalCard terminalCard = terminalCardTransform.GetComponent<TerminalCard>();

            terminalCard.SetTileParent(GetTile(x, y));
            terminalCards.Add(terminalCard);
        }
    }

    public List<TerminalCard> GetTerminalCards() { return terminalCards; }
}
