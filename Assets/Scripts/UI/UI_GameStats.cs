using UnityEngine;
using UnityEngine.UI;

public class UI_GameStats : MonoBehaviour {

    public Text gameDate;

    // Update is called once per frame
    void Update() {
        gameDate.text = SpeedController.speed.currentDate();
    }
}
