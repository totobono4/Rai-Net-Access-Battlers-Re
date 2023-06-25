using System.Collections.Generic;
using UnityEngine;

public class TerminalGroup : TileMap {
    [SerializeField] private TerminalGroupSO terminalGroupSO;

    protected override void Awake() {
        base.Awake();

        List<Transform> transforms = terminalGroupSO.GetTerminalCards();
        for (int i  = 0; i < transforms.Count; i++) {
            Instantiate(transforms[i]).GetComponent<TerminalCard>().SetTileParent(GetTile(0,i));
        }
    }
}
