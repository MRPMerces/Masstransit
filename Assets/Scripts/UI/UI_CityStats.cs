using UnityEngine;
using UnityEngine.UI;

public class UI_CityStats : MonoBehaviour {

    public Text cityName;
    public Text cityPopulation;
    public Text cityAirport;
    public Text cityMetro;
    public Text cityMetrolines;
    public Text cityMetrostations;

    Tile currentTile {
        get { return MouseController.mouseController.currentHoveredCityTile; }
    }

    MetroController metroController {
        get { return MetroController.metroController; }
    }

    // Update is called once per frame
    void Update() {
        if (currentTile != null) {
            City currentCity = currentTile.city;

            cityName.text = "City name: " + currentCity.name;
            cityPopulation.text = "City population: " + currentCity.population;
            cityAirport.text = "City has airport: " + currentCity.hasAirport.ToString();
            cityMetro.text = "City has a metro: " + currentCity.hasMetro.ToString();

            if (currentCity.hasMetro) {
                cityMetrolines.text = "City has: " + metroController.amountOfMetroLinesInCity(currentCity) + " metrolines";
                cityMetrostations.text = "City has: " + metroController.amountOfMetroStationsInCity(currentCity) + " metrostations";
            }
            return;
        }

        cityName.text = "City name: ";
        cityPopulation.text = "City population: ";
        cityAirport.text = "City has airport: ";
        cityMetro.text = "City has a metro: ";
        cityMetrolines.text = "City has: " + 0 + " metrolines";
        cityMetrostations.text = "City has: " + 0 + " metrostations";
    }
}
