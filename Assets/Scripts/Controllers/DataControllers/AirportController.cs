using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportController : MonoBehaviour {

    public static AirportController airportController;

    public Dictionary<CityPair, Vehicle> cityPairs;

    // Start is called before the first frame update
    void Start() {
        airportController = this;
        cityPairs = new Dictionary<CityPair, Vehicle>();

        //findCityPairs();
    }

    public bool hasPlayerAAirport(City city, Player player) {
        if (player == null) {
            Debug.LogError("player is null");
            return false;
        }

        foreach (Airport airport in city.airports) {
            if (airport.owner == player) {
                return true;
            }
        }

        return false;
    }

    public bool hasPlayerAAirport(Tile tile, Player player) {
        if (player == null) {
            Debug.LogError("player is null");
            return false;
        }

        foreach (Airport airport in tile.city.airports) {
            if (airport.owner == player) {
                return true;
            }
        }

        return false;
    }

    public Airport get_airportByPlayer(City city, Player player) {
        if (!city.hasAirport) {
            Debug.LogError("city does not have a airport");
            return null;
        }

        foreach (Airport airport in city.airports) {
            if (airport.owner == player) {
                return airport;
            }
        }

        Debug.LogError("Cant find a airport with owner player");
        return null;
    }

    public void buildAirport(City city, Player owner) {
        if (hasPlayerAAirport(city, owner)) {
            Debug.LogError("player allready have a airport!");
            return;
        }

        city.airports.Add(new Airport(owner));
        city.airports[city.airports.Count - 1].add_runway();
        city.airports[city.airports.Count - 1].add_terminal();
        city.hasAirport = true;

        findCityPairs();
    }

    public void buildTerminal(City city, Player player) {
        if (!hasPlayerAAirport(city, player)) {
            Debug.LogError("Player: " + player + " does not have a airport in this city");
            return;
        }

        get_airportByPlayer(city, player).add_terminal();
    }

    public void buildRunway(City city, Player player) {
        if (!hasPlayerAAirport(city, player)) {
            Debug.LogError("Player: " + player + " does not have a airport in this city");
            return;
        }

        get_airportByPlayer(city, player).add_runway();
    }

    public void add_metroStationToAirport(Tile tile, Player player, int linenumber) {
        if (!player.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (tile.isCity && MetroController.metroController.hasPlayerAMetroInCity(tile.city, player) && hasPlayerAAirport(tile, player)) {
            Airport airport = get_airportByPlayer(tile.city, player);
            if (airport.metroStation) {
                Debug.LogError("Airport allready have a metrostation");
                return;
            }

            MetroController.metroController.buildStation(tile.city, player, linenumber);

            airport.add_metroStation();
        }
    }

    public void add_highwayConnectionToAirport(Tile tile, Player player) {
        if (!player.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (tile.isCity && tile.hasPlayerInfrastructure(NetworkType.Highway, player) && hasPlayerAAirport(tile, player)) {
            Airport airport = get_airportByPlayer(tile.city, player);
            if (airport.highwayConnection) {
                Debug.LogError("Airport allready have a highwayconnection");
                return;
            }
            
            airport.add_highwayConnection();
        }
    }

    public void add_trainStationToAirport(Tile tile, Player player) {
        if (!player.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (tile.isCity && tile.hasPlayerInfrastructure(NetworkType.LST, player) && hasPlayerAAirport(tile, player)) {
            Airport airport = get_airportByPlayer(tile.city, player);
            if (airport.trainStation) {
                Debug.LogError("Airport allready have a trainstation");
                return;
            }

            airport.add_trainStation();
        }
    }

    public void add_highSpeedTrainStationToAirport(Tile tile, Player player) {
        if (!player.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (tile.isCity && tile.hasPlayerInfrastructure(NetworkType.LST, player) && hasPlayerAAirport(tile, player)) {
            Airport airport = get_airportByPlayer(tile.city, player);
            if (airport.highSpeedtrainStation) {
                Debug.LogError("Airport allready have a highspeed trainstation");
                return;
            }

            airport.add_highSpeedTrainStation();
        }
    }

    /// <summary>
    /// Function to merge two airports
    /// </summary>
    /// <param name="player1">Owner of the final airport</param>
    /// <param name="player2">Airport to be merged with player 1</param>
    public void mergeAirports(City city, Player player1, Player player2) {

        // If player 1 does not have an airport in this city. Just update the ownership of player 2's airport.
        if (!hasPlayerAAirport(city, player1)) {
            get_airportByPlayer(city, player2).update_owner(player1);
        }

        // Else we need to merge them.
        else {
            get_airportByPlayer(city, player1).mergeAirport(get_airportByPlayer(city, player2));
            city.airports.Remove(get_airportByPlayer(city, player2));
        }
    }

    void findCityPairs() {
        foreach (Player player in World.world.playerController.players) {
            foreach (Tile tile1 in World.world.tilesWithCity) {
                foreach (Tile tile2 in World.world.tilesWithCity) {
                    if (hasPlayerAAirport(tile1, player) && hasPlayerAAirport(tile2, player) && !isAirportPairAdded(tile1, tile2)) {
                        cityPairs.Add(new CityPair(tile1, tile2, null), new Vehicle(VehicleType.PLANE, tile1, tile2));
                    }
                }
            }
        }
    }

    bool isAirportPairAdded(Tile tile1, Tile tile2) {
        foreach (CityPair cityPair in cityPairs.Keys) {
            if (cityPair.containsTiles(tile1, tile2)) {
                return true;
            }
        }

        return false;
    }
}
