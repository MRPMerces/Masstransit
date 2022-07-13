using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Priority_Queue;

/// <summary>
///  Multithread pathA* ???
/// </summary>

public class NetworkGraph {
    // This class constructs a simple path-finding compatible graph of our network. Each tile is a node. Each neighbour from a tile is linked via an edge connection.

    // Mostly following this pseusocode:
    // https://en.wikipedia.org/wiki/A*_search_algorithm

    public Dictionary<Tile, Path_Node<Tile>> nodes;

    Network network;

    public List<CityPair> cityPairs;

    public NetworkGraph(Network network) {
        this.network = network;
        cityPairs = new List<CityPair>();

        generateGraphFromNetwork();
    }

    //public void regenerateDistances() {
    //    generateGraphFromNetwork();

    //    Dictionary<Tile, List<Tile>> visited = new Dictionary<Tile, List<Tile>>();


    //    foreach (Tile tile1 in network.cityTiles) {
    //        visited.Add(tile1, new List<Tile>());
    //        foreach (Tile tile2 in network.cityTiles) {
    //            if (!visited.ContainsKey(tile2) && !visited[tile1].Contains(tile2)) {
    //                visited[tile1].Add(tile2);
    //                pathA(nodes[tile1], nodes[tile2]);
    //            }
    //        }
    //    }
    //}

    public void reGenerateNetwork() {
        generateGraphFromNetwork();

        Dictionary<Tile, List<Tile>> visited = new Dictionary<Tile, List<Tile>>();

        foreach (Tile tile1 in network.cityTiles) {
            visited.Add(tile1, new List<Tile>());
            foreach (Tile tile2 in network.cityTiles) {

                if (!visited.ContainsKey(tile2) && !visited[tile1].Contains(tile2)) {
                    visited[tile1].Add(tile2);
                    if(!cityPairExist(tile1, tile2)) {
                        cityPairs.Add(new CityPair(tile2, tile1, pathA(nodes[tile1], nodes[tile2])));
                    }
                }
            }
        }
    }

    bool cityPairExist(Tile tile1, Tile tile2) {
        foreach (CityPair cityPair in cityPairs) {
            if (cityPair.containsTiles(tile1, tile2)) {
                cityPair.assignNewPath(pathA(nodes[tile1], nodes[tile2]));
                return true;
            }
        }

        return false;
    }

    void generateGraphFromNetwork() {
        nodes = new Dictionary<Tile, Path_Node<Tile>>(network.tiles.Count);

        foreach (Tile tileNode in network.tiles) {
            nodes.Add(tileNode, new Path_Node<Tile> { data = tileNode });
        }

        // Now loop through all nodes again and create edges for neighbours.

        foreach (Tile tileNode in nodes.Keys) {
            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>(4);

            // Get a list of neighbours for the tile
            foreach (Tile tile1 in tileNode.getNeighbours()) {
                // Create an edge to the relevant node.
                if (tile1 != null && network.containsTile(tile1)) {
                    edges.Add(new Path_Edge<Tile> { cost = 1, node = nodes[tile1] });
                }
            }
            nodes[tileNode].edges = edges.ToArray();
        }
    }

    Tile[] pathA(Path_Node<Tile> startNode, Path_Node<Tile> endNode) {

        // Make sure our start/end tiles are in the list of nodes!
        if (startNode == null) {
            Debug.LogError("Path_AStar: The starting tile isn't in the list of nodes!");

            return null;
        }

        if (endNode == null) {
            Debug.LogError("Path_AStar: The ending tile isn't in the list of nodes!");
            return null;
        }

        List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>>();
        SimplePriorityQueue<Path_Node<Tile>> OpenSet = new SimplePriorityQueue<Path_Node<Tile>>();
        Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();
        Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();
        Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float>();

        OpenSet.Enqueue(startNode, 0);

        foreach (Path_Node<Tile> node in nodes.Values) {
            g_score.Add(node, Mathf.Infinity);
            f_score.Add(node, Mathf.Infinity);
        }

        g_score[startNode] = 0;
        f_score[startNode] = heuristic_cost_estimate(startNode, endNode);

        while (OpenSet.Count > 0) {
            Path_Node<Tile> currentNode = OpenSet.Dequeue();

            if (currentNode == endNode) {
                // We have reached our goal! Let's convert this into an actual sequene of tiles.
                return reconstruct_path(Came_From, currentNode);
            }

            ClosedSet.Add(currentNode);

            foreach (Path_Edge<Tile> edge_neighbor in currentNode.edges) {

                if (ClosedSet.Contains(edge_neighbor.node)) {
                    continue; // ignore this already completed neighbor
                }

                if (OpenSet.Contains(edge_neighbor.node) && g_score[currentNode] >= g_score[edge_neighbor.node]) {
                    continue;
                }

                Came_From[edge_neighbor.node] = currentNode;
                g_score[edge_neighbor.node] = g_score[currentNode];
                f_score[edge_neighbor.node] = g_score[edge_neighbor.node] + heuristic_cost_estimate(edge_neighbor.node, endNode);

                if (!OpenSet.Contains(edge_neighbor.node)) {
                    OpenSet.Enqueue(edge_neighbor.node, f_score[edge_neighbor.node]);
                }

                else {
                    OpenSet.UpdatePriority(edge_neighbor.node, f_score[edge_neighbor.node]);
                }
            }
        }

        // If we reached here, it means that we've burned through the entire OpenSet without ever reaching a point where current == goal.
        // This happens when there is no path from start to goal

        Debug.LogError("Cant find path from: " + startNode.data.name + " to: " + endNode.data.name);
        return null;
    }

    float heuristic_cost_estimate(Path_Node<Tile> nodeA, Path_Node<Tile> nodeB) {
        return Mathf.Sqrt(Mathf.Pow(nodeA.data.X - nodeB.data.X, 2) + Mathf.Pow(nodeA.data.Y - nodeB.data.Y, 2));
    }

    Tile[] reconstruct_path(Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From, Path_Node<Tile> endNode) {
        // We want to go backwards through Came_From map, until we reach the "end" of that map. Which will be the start node!

        List<Tile> path = new List<Tile> {
            endNode.data
        };

        while (Came_From.ContainsKey(endNode)) {
            // Came_From is a map, where the key => value relation is realy saying some_node => we_got_there_from_this_node

            endNode = Came_From[endNode];
            path.Add(endNode.data);
        }

        return path.ToArray();
    }
}
