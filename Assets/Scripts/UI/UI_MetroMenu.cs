using UnityEngine;
using UnityEngine.UI;

public class UI_MetroMenu : MonoBehaviour {
    public static UI_MetroMenu uI_MetroMenu { get; protected set; }

    public GameObject line;
    public GameObject metro;
    public GameObject station;
    public GameObject menu;

    City currentCity;

    MetroController metroController {
        get { return MetroController.metroController; }
    }

    Player player {
        get { return World.world.playerController.human; }
    }

    // Start is called before the first frame update
    void Start() {
        uI_MetroMenu = this;
        disableAll();
    }

    public void metroBuilding(City city) {

        if(city == null) {
            Debug.LogError("city is null");
            return;
        }

        menu.SetActive(true);
        /// chechk owner
        if (metroController.hasPlayerAMetroInCity(currentCity, player)) {
            line.SetActive(true);
            metro.SetActive(false);
            station.SetActive(true);

            // Disable the button if player cant afford.
            if (!player.canAfford(100000)) {
                metro.GetComponent<Button>().interactable = false;
                //terminal.color = red;
            }
        }

        else {
            line.SetActive(false);
            metro.SetActive(true);
            station.SetActive(false);
        }

        currentCity = city;

        /// change, "can be interacted", based on can afford.
    }

    public void buildMetro() {
        if (metroController.hasPlayerAMetroInCity(currentCity, player)) {
            Debug.LogError("Player allready have a metro in this city");
            return;
        }

        metroController.buildMetroInCity(currentCity, player);

        disableAll();
    }

    public void buildLine() {
        if (!metroController.hasPlayerAMetroInCity(currentCity, player)) {
            Debug.LogError("Player does not have a metro in this city");
            return;
        }

        metroController.buildLineInCity(currentCity, player);

        disableAll();
    }

    public void buildStation() {
        if (!metroController.hasPlayerAMetroInCity(currentCity, player)) {
            Debug.LogError("Player does not have a metro in this city");
            return;
        }

        /// Line need to be selected
        metroController.buildStation(currentCity, player, 0);

        disableAll();
    }

    void disableAll() {
        line.SetActive(false);
        metro.SetActive(false);
        station.SetActive(false);
        menu.SetActive(false);
    }
}
