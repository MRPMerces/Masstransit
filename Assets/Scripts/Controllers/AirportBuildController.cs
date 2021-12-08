using UnityEngine;

public class AirportBuildController : MonoBehaviour {

    public static AirportBuildController airportBuildController { get; protected set; }
    // Start is called before the first frame update
    void Start() {
        airportBuildController = this;
    }

    // Update is called once per frame
    void Update() {

    }

    public void buildAirport(City city, Player player) {
        if (city.hasPlayerAAirport(player)) {
            Debug.LogError("Player: " + player + " allready have a airport in this city");
            return;
        }

        city.buildAirport(player);
    }

    public void buildTerminal(City city, Player player) {
        if (!city.hasPlayerAAirport(player)) {
            Debug.LogError("Player: " + player + " does not have a airport in this city");
            return;
        }

        city.get_airportByPlayer(player).add_terminal();
    }

    public void buildRunway(City city, Player player) {
        if (!city.hasPlayerAAirport(player)) {
            Debug.LogError("Player: " + player + " does not have a airport in this city");
            return;
        }

        city.get_airportByPlayer(player).add_runway();
    }
}
