using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TerminalGroup : TileMap {
    [SerializeField] private TerminalGroupSO terminalGroupSO;
    private List<TerminalCard> terminalCards = new List<TerminalCard>();

    protected override void Awake() {
        base.Awake();
    }

    public void InstantiateTerminalCards() {
        List<Transform> transforms = terminalGroupSO.GetTerminalCards();
        for (int i = 0; i < transforms.Count; i++) {
            Transform terminalCardTransform = Instantiate(transforms[i]);

            NetworkObject terminalCardNetwork = terminalCardTransform.GetComponent<NetworkObject>();
            terminalCardNetwork.Spawn();

            TerminalCard terminalCard = terminalCardTransform.GetComponent<TerminalCard>();

            terminalCard.SetTileParent(GetTile(0, i));
            terminalCards.Add(terminalCard);
        }
    }

    public List<TerminalCard> GetTerminalCards() { return terminalCards; }
}
