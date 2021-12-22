using UnityEngine;
using UnityEngine.UI;

public class UI_AirportMenu : MonoBehaviour {
    // Start is called before the first frame update

    public static UI_AirportMenu uI_AirportMenu { get; protected set; }
    public GameObject terminal;
    public GameObject airport;
    public GameObject runway;
    public GameObject menu;

    City city;

    private Player player {
        get { return World.world.playerController.human; }
    }

    void Start() {
        uI_AirportMenu = this;

        disableAll();
    }

    public void airportBuilding(City currentCity) {
        menu.SetActive(true);
        /// chechk owner
        if (currentCity.hasAirport) {
            terminal.SetActive(true);
            airport.SetActive(false);
            runway.SetActive(true);

            // Disable the button if player cant afford.
            if (!player.canAffordConstructionCost(0)) {
                terminal.GetComponent<Button>().interactable = false;
                //terminal.color = red;
            }
        }

        else {
            terminal.SetActive(false);
            airport.SetActive(true);
            runway.SetActive(false);
        }
        city = currentCity;

        /// change can be interacted based on can afford.
    }

    public void buildAirport() {
        AirportController.airportController.buildAirport(city, player);

        disableAll();
    }

    public void buildTerminal() {
        AirportController.airportController.buildTerminal(city, player);

        disableAll();
    }

    public void buildRunway() {
        AirportController.airportController.buildRunway(city, player);

        disableAll();
    }

    void disableAll() {
        terminal.SetActive(false);
        airport.SetActive(false);
        runway.SetActive(false);
        menu.SetActive(false);
    }
}
