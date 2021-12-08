using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerController() {
        players = new List<Player> {
            new Player(PlayerType.Human, "player", 0),
            new Player(PlayerType.Ai, "AI_1", 1)
        };
        human = players[0];
    }

    public List<Player> players { get; protected set; }
    public Player human { get; protected set; }

    public void update_playerModifiers(Modifier[] modifiers) {
        foreach (Player player in players) {
            player.update_modifiers(modifiers);
        }
    }

    public Player getPlayerByName(string name) {
        foreach (Player player in players) {
            if(player.name == name) {
                return player;
            }
        }

        Debug.LogError("Could not find player");
        return null;
    }
}
