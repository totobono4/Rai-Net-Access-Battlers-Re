using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static GameBoard;

public class PlayersInfos
{
    private ulong wrongClientId;
    private List<PlayerElement> players;

    private class PlayerElement {
        public ulong clientId;
        public Team team;
        public PlayerEntity playerEntity;
        public Transform onlineCardPrefab;
        public bool ready;

        public PlayerElement() {
            clientId = 9999;
            ready = false;
        }
    }

    public PlayersInfos() {
        wrongClientId = 9999;
        players = new List<PlayerElement>();
    }

    public bool TryCreatePlayer(Team team) {
        foreach (PlayerElement player in players) if (player.team == team) return false;
        players.Add(new PlayerElement { team = team });
        return true;
    }

    private bool TryGetPlayerElementFromTeam(Team team, out PlayerElement player) {
        player = default;
        foreach (PlayerElement playerElement in players) {
            if (playerElement.team == team) {
                player = playerElement;
                return true;
            }
        }
        return false;
    }

    private bool TryGetPlayerElementFromId(ulong clientId, out PlayerElement player) {
        player = default;
        foreach(PlayerElement playerElement in players) {
            if (playerElement.clientId == clientId) {
                player = playerElement;
                return true;
            }
        }
        return false;
    }

    public bool TrySetPlayerEntityFromTeam(Team team, PlayerEntity playerEntity) {
        if (!TryGetPlayerElementFromTeam(team, out PlayerElement player)) return false;
        player.playerEntity = playerEntity;
        return true;
    }

    public bool TrySetOnlineCardPrefabFromTeam(Team team, Transform onlineCardPrefab) {
        if (!TryGetPlayerElementFromTeam(team, out PlayerElement player)) return false;
        player.onlineCardPrefab = onlineCardPrefab;
        return true;
    }

    public bool TrySetIdFromTeam(Team team, ulong clientId) {
        if (!TryGetPlayerElementFromTeam(team, out PlayerElement player)) return false;
        player.clientId = clientId;
        return true;
    }

    public bool TrySetReadyFromId(ulong clientId, bool ready) {
        if (!TryGetPlayerElementFromId(clientId, out PlayerElement player)) return false;
        player.ready = ready;
        return true;
    }

    public bool TryGetTeamFromId(ulong clientId, out Team team) {
        team = GameBoard.Team.None;
        foreach (PlayerElement playerElement in players) {
            if (playerElement.clientId == clientId) {
                team = playerElement.team;
                return true;
            }
        }
        return false;
    }

    public bool TryGetIdFromTeam(Team team, out ulong clientId) {
        clientId = wrongClientId;
        foreach (PlayerElement playerElement in players) {
            if (playerElement.team == team && playerElement.clientId != wrongClientId) {
                clientId = playerElement.clientId;
                return true;
            }
        }
        return false;
    }

    public bool TryGetPlayerEntityFromTeam(Team team, out PlayerEntity playerEntity) {
        playerEntity = null;
        foreach(PlayerElement playerElement in players) {
            if (playerElement.team == team) {
                playerEntity = playerElement.playerEntity;
                return true;
            }
        }
        return false;
    }

    public bool TryGetOnlineCardPrefabFromTeam(Team team, out Transform onlineCardPrefab) {
        onlineCardPrefab = null;
        foreach (PlayerElement playerElement in players) {
            if (playerElement.team == team) {
                onlineCardPrefab = playerElement.onlineCardPrefab;
                return true;
            }
        }
        return false;
    }

    public bool AreAllPlayersReady() {
        foreach (PlayerElement playerElement in players) if (!playerElement.ready) return false;
        return true;
    }
}
