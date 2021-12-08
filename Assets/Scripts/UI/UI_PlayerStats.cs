using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour {

    public Text playerName;
    public Text playerMoney;

    Player player {
        get { return World.world.playerController.human; }
    }

    // Use this for initialization
    void Start() {
        // Register our callback so that our money display gets updated whenever human.money updates.
        player.RegisterMoneyUpdatedCallback(onMoneyUpdate);

        // Display playername and player money.
        playerName.text = "Name: " + player.name;
        playerMoney.text = "Money: " + player.money;
    }

    void onMoneyUpdate() {

        // Display player money.
        playerMoney.text = "Money: " + player.money;
    }
}
