using System.Collections.Generic;
using UnityEngine;

public class AirportGraph {
    public Dictionary<Tile, Path_Node<Tile>> nodes;

    private List<TilePair> tilePairs;

    public AirportGraph() {
        nodes = new Dictionary<Tile, Path_Node<Tile>>();
        tilePairs = new List<TilePair>();

        regenerateGraph();
    }

    public void regenerateGraph() {

        // Loop through all tiles with a city of the world and create a node if the city has a airport.
        foreach (Tile tile in World.world.tilesWithCity) {
            if (!nodes.ContainsKey(tile) && tile.city.hasAirport) {
                nodes.Add(tile, new Path_Node<Tile> { data = tile });
            }
        }


        // Now loop through all nodes again
        // Create edges for all cities with airports
        foreach (Tile tile1 in nodes.Keys) {
            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            foreach (Tile tile2 in nodes.Keys) {
                // Add the edge to our temporary (and growable!) list
                edges.Add(new Path_Edge<Tile> { cost = Mathf.Sqrt(Mathf.Pow(tile1.X - tile2.X, 2) + Mathf.Pow(tile1.Y - tile2.Y, 2)), node = nodes[tile2] });
                ///Should we use a constructor? google
            }
            nodes[tile1].edges = edges.ToArray();
        }
    }

    public void enableAirportOverlay() {
        foreach (TilePair tilePair in tilePairs) {
            tilePair.gameObject.SetActive(true);
        }

        foreach (Tile tile1 in nodes.Keys) {
            foreach (Tile tile2 in nodes.Keys) {
                if (!containsPair(tile1, tile2)) {
                    GameObject gameObject = new GameObject();

                    LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

                    lineRenderer.SetPosition(0, new Vector3(tile1.X + 0.5f, tile1.Y + 0.5f, 0));
                    lineRenderer.SetPosition(1, new Vector3(tile2.X + 0.5f, tile2.Y + 0.5f, 0));

                    lineRenderer.endColor = Color.red;
                    lineRenderer.startWidth = 0.125f;

                    lineRenderer.sortingLayerName = "Infrastructure";

                    tilePairs.Add(new TilePair(tile1, gameObject, tile2));
                }
            }
        }
    }

    public void disableAirportOverlay() {
        foreach (TilePair tilePair in tilePairs) {
            tilePair.gameObject.SetActive(false);
        }
    }

    bool containsPair(Tile tile1, Tile tile2) {
        foreach (TilePair tilePair in tilePairs) {
            if (tilePair.tile1 == tile1 && tilePair.tile2 == tile2) {
                return true;
            }
        }
        return false;
    }
}

public class TilePair {
    public TilePair(Tile tile1, GameObject gameObject, Tile tile2) {
        this.tile1 = tile1;
        this.gameObject = gameObject;
        this.tile2 = tile2;
    }

    public Tile tile1;
    public GameObject gameObject;
    public Tile tile2;
}


/// Player spesific.