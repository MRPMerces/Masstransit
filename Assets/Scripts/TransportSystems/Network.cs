using System.Collections.Generic;
using UnityEngine;

public enum NetworkType { Road, Highway, LST, HST }
public class Network {
    public Network(NetworkType networkType, Player owner) {
        this.owner = owner;

        tiles = new List<Tile>();
        cityTiles = new List<Tile>();
        networkGraph = new NetworkGraph(this);
        vehicles = new Dictionary<CityPair, Vehicle>();

        switch (networkType) {
            case NetworkType.Road:
                vehicleType = VehicleType.LSC;
                break;

            case NetworkType.Highway:
                vehicleType = VehicleType.HSC;
                break;

            case NetworkType.LST:
                vehicleType = VehicleType.LST;
                break;

            case NetworkType.HST:
                vehicleType = VehicleType.HST;
                break;

            default:
                break;
        }
    }

    public List<Tile> tiles { get; protected set; }

    public List<Tile> cityTiles { get; protected set; }

    public readonly Player owner;
    //public readonly List<CityGraph> graph;

    public NetworkGraph networkGraph;

    public Dictionary<CityPair, Vehicle> vehicles;

    VehicleType vehicleType;

    public void addTiles(Tile[] tilesToAdd, bool generateGraph = true) {
        foreach (Tile tile in tilesToAdd) {
            if (!containsTile(tile)) {
                tiles.Add(tile);

                if (tile.isCity) {
                    cityTiles.Add(tile);
                }
            }
        }

        if (generateGraph) {
            networkGraph.reGenerateNetwork();
        }

        reGenerateVehicles();
    }

    void reGenerateVehicles() {
        foreach (CityPair cityPair in networkGraph.cityPairs) {
            if (!vehicles.ContainsKey(cityPair)) {
                vehicles.Add(cityPair, new Vehicle(vehicleType, cityPair.start, cityPair.end));

                Vector2 direction = Vector2.zero;
                Tile currentTile = cityPair.start;
                Tile nextTile = cityPair.path[1];

                if (currentTile.X != nextTile.X) {
                    if (currentTile.X > nextTile.X) {
                        direction.x = -1;
                    }

                    else {
                        direction.x = 1;
                    }
                }

                if (currentTile.Y != nextTile.Y) {
                    if (currentTile.Y > nextTile.Y) {
                        direction.y = -1;
                    }

                    else {
                        direction.y = 1;
                    }
                }

                vehicles[cityPair].nextTile = nextTile;
                vehicles[cityPair].direction = direction;
            }
        }
    }

    public bool containsTile(Tile tile) {
        // Check that tile exist.
        if (tile == null) {
            Debug.LogError("tile = null");
            return false;
        }

        return tiles.Contains(tile);
    }

    public bool containsCityTile(Tile tile) {
        // Check that tile exist.
        if (tile == null) {
            Debug.LogError("tile = null");
            return false;
        }

        if (tile.city == null) {
            return false;
        }

        return cityTiles.Contains(tile);
    }
}
