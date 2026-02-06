using UnityEngine;

[CreateAssetMenu(fileName = "MultiplayerConfigSO", menuName = "Ttbn4/Network/MultiplayerConfig")]
public class MultiplayerConfigSO : ScriptableObject
{
    [SerializeField] private int MaxPlayerCount;
    [SerializeField] private int MinPlayerCount;

    public int GetMaxPlayerCount() { return MaxPlayerCount; }
    public int GetMinPlayerCount() { return MinPlayerCount; }
}
