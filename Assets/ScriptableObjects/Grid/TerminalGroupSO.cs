using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "TerminalGroupObject")]
public class TerminalGroupSO : ScriptableObject
{
    [SerializeField] private Transform terminal_0;
    [SerializeField] private Transform terminal_1;
    [SerializeField] private Transform terminal_2;
    [SerializeField] private Transform terminal_3;

    public List<Transform> GetTerminalCards () { return new List<Transform>() { terminal_0, terminal_1, terminal_2, terminal_3 }; }
}
