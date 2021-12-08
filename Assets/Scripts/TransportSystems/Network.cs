using System.Collections.Generic;
using UnityEngine;

public enum NetworkType { Road, Highway, LST, HST };
public class Network {
    public Network(int id, Player owner) {
        this.id = id;
        this.owner = owner;

        tiles = new List<Tile>();
        cityTiles = new List<Tile>();
        graph = new List<CityGraph>();
    }

    public List<Tile> tiles { get; protected set; }

    readonly List<Tile> cityTiles;

    public readonly Player owner;
    public readonly int id;
    public readonly List<CityGraph> graph;

    void creategraph() {
        if (cityTiles.Count > 1) {
            graph.Clear();
            Debug.Log(cityTiles.Count);
            foreach (Tile city1 in cityTiles) {
                CityGraph newCityGraph = new CityGraph(city1);
                graph.Add(newCityGraph);

                foreach (Tile city2 in cityTiles) {
                    if (city2 != city1 && !newCityGraph.distances.ContainsKey(city2.city)) {
                        newCityGraph.addDistance(city2.city, (int)Mathf.Sqrt(Mathf.Pow(city1.X - city2.X, 2) + Mathf.Pow(city1.Y - city2.Y, 2)));
                    }
                }
            }
        }
    }

    public void addTiles(Tile[] tilesToAdd) {
        foreach (Tile tile in tilesToAdd) {
            if (!containsTile(tile)) {
                tiles.Add(tile);

                if (tile.isCity) {
                    cityTiles.Add(tile);
                }
            }
        }
        creategraph();
    }

    public bool containsTile(Tile tile) {
        // Check that tile exist.
        if (tile == null) {
            Debug.LogError("tile = null");
            return false;
        }

        return tiles.Contains(tile);
    }
};



public class CityGraph {
    public CityGraph(Tile cityTile) {
        this.cityTile = cityTile;
        distances = new Dictionary<City, int>();
        paths = new Dictionary<Tile, Tile[]>();
    }

    public Tile cityTile { get; protected set; }
    public Dictionary<City, int> distances { get; protected set; }
    public Dictionary<Tile, Tile[]> paths { get; protected set; }

    public void addDistance(City city, int distance) {
        if (distances.ContainsKey(city)) {
            Debug.LogError("tile allready added");
            return;
        }

        distances.Add(city, distance);
    }

    public void addPath(Tile tile, Tile[] tiles) {
        if (paths.ContainsKey(tile)) {
            Debug.LogError("tile allready added");
            return;
        }

        paths.Add(tile, tiles);
    }
}
