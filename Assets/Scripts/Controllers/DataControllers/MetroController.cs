using UnityEngine;

public class MetroController : MonoBehaviour {

    public static MetroController metroController { get; protected set; }
    // Start is called before the first frame update
    void Start() {
        metroController = this;
    }

    // Create a metrosystem in city
    public void buildMetroInCity(City city, Player player) {

        // Check if player allready have a metro system in this city
        if (hasPlayerAMetroInCity(city, player)) {
            Debug.LogError("player allready have a metro in this city");
            return;
        }

        // Check if metroes are unlocked
        if (!World.world.tech.metro_unlocked) {
            Debug.LogError("Metro is not unlocked!");
            return;
        }

        // Check if player can afford the cost
        if (!player.canAffordConstructionCost(100000)) {
            Debug.LogError("Insufficient funds!");
            return;
        }

        city.metros.Add(new Metro(player));

        if (!city.hasMetro) {
            city.hasMetro = true;
        }

        // Create a metroline in system and builds two stations.
        get_metroByPlayer(city, player).add_metroline(city.usedLines.Count);
        city.usedLines.Add(city.usedLines.Count);

        // One time cost of 40K to start building a metro system
        player.constructionCost(40000);
    }

    public void buildLineInCity(City city, Player player) {
        if (!hasPlayerAMetroInCity(city, player)) {
            Debug.LogError("Player does not have a metro in this city");
            return;
        }

        get_metroByPlayer(city, player).add_metroline(city.usedLines.Count);
        city.usedLines.Add(city.usedLines.Count);

        player.constructionCost(30000);
    }

    public void buildStation(City city, Player player, int line) {
        if (!hasPlayerAMetroInCity(city, player)) {
            Debug.LogError("Player does not have a metro in this city");
            return;
        }

       get_metroByPlayer(city, player).add_metrostationToLine(line);
    }



    public bool hasPlayerAMetroInCity(City city, Player player) {
        if (player == null) {
            Debug.LogError("player is null");
            return false;
        }

        if (city == null) {
            Debug.LogError("city is null");
            return false;
        }

        if (!city.hasMetro) {
            return false;
        }

        foreach (Metro metro in city.metros) {
            if (metro.owner == player) {
                return true;
            }
        }
        return false;
    }


    public Metro get_metroByPlayer(City city, Player player) {
        if (!city.hasMetro) {
            Debug.LogError("City does not have a metro");
            return null;
        }

        foreach (Metro metro in city.metros) {
            if (metro.owner == player) {
                return metro;
            }
        }

        Debug.LogError("Cant find a metro with owner player");
        return null;
    }

    /// <summary>
    /// Function to merge two metro systems
    /// </summary>
    /// <param name="player1">Owner of the final metro</param>
    /// <param name="player2">metro to be merged with player 1</param>
    public void mergeMetroes(City city, Player player1, Player player2) {
        Metro metro1 = get_metroByPlayer(city, player1);
        Metro metro2 = get_metroByPlayer(city, player2);

        if (metro1 == null || metro2 == null) {
            Debug.LogError("metro does not exist");
            return;
        }

        metro1.lines.AddRange(metro2.lines);
        metro1.updateCatchment();

        city.metros.Remove(metro2);
    }

    public int amountOfMetroLinesInCity(City city) {
        int lines = 0;
        foreach (Metro metro in city.metros) {
            lines += metro.get_amountOfMetrolinesInSystem();
        }
        return lines;
    }

    public int amountOfMetroStationsInCity(City city) {
        int stations = 0;
        foreach (Metro metro in city.metros) {
            stations += metro.get_amountOfMetrostationsInSystem();
        }
        return stations;
    }

    public int get_totalMetroCathment(City city) {
        int cathment = 0;
        foreach (Metro metro in city.metros) {
            cathment += metro.catchment;
        }
        return cathment;
    }
}
