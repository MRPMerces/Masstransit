using System.Collections.Generic;
using UnityEngine;

public class NetworkController {
    public NetworkController(NetworkType type, int stationCost, int tileCost) {
        this.type = type;

        this.stationCost = stationCost;
        this.tileCost = tileCost;

        networks = new List<Network>();
        playerTiles = new Dictionary<Player, List<Tile>>();
    }

    readonly NetworkType type;

    readonly int stationCost;
    readonly int tileCost;
    readonly List<Network> networks;

    readonly Dictionary<Player, List<Tile>> playerTiles;

    /// <summary>
    /// Add tiles to the network controller
    /// </summary>
    /// <param name="tilesToAdd">Tile[] to be added</param>
    /// <param name="owner">Owner of the tiles in tilesToAdd</param>
    public void addTiles(Tile[] tilesToAdd, Player owner) {

        if (owner == null) {
            Debug.LogError("owner is null");
            return;
        }

        if (!playerTiles.ContainsKey(owner)) {
            playerTiles.Add(owner, new List<Tile>());
        }

        List<Tile> tilesToCheck = new List<Tile>();

        double cost;
        // Subtract the cost and add the infrastructure to the tiles.
        foreach (Tile tile in tilesToAdd) {
            if (tile == null || tile.hasPlayerInfrastructure(type, owner)) {
                continue;
            }

            if (tile.isCity) {
                cost = stationCost;
                tile.city.addNetworkConnection(type);
            }

            else {
                if (type != NetworkType.Road && tile.hasPlayerInfrastructure(NetworkType.Road, owner)) {
                    cost = tileCost * 0.75;
                }

                else {
                    cost = tileCost;
                }
            }

            // Check if the owner can afford the cost.
            if (!owner.canAfford((int)cost)) {
                Debug.LogError("Insufficient funds!");
                return;
            }

            owner.constructionCost((int)cost);

            // Player can afford the cost, so add the infrastructure.
            tile.add_infrastructureOwner(type, owner);
            tilesToCheck.AddRange(tile.getNeighbours());

            // Add the tile to the player.
            playerTiles[owner].Add(tile);
        }

        // Check if a neighbouring tile allready is in a network.
        List<Network> neighbouringNetworks = new List<Network>();
        foreach (Tile tileToCheck in tilesToCheck) {
            if (tileToCheck != null && tileToCheck.hasPlayerInfrastructure(type, owner)) {
                foreach (Network network in networks) {
                    if (!neighbouringNetworks.Contains(network) && network.tiles.Contains(tileToCheck)) {
                        neighbouringNetworks.Add(network);
                        break;
                    }
                }
            }
        }

        if (neighbouringNetworks.Count == 0) {
            // Create a new network and add the tiles to this network.
            createNewNetwork(owner).addTiles(tilesToAdd);
            return;
        }

        if (neighbouringNetworks.Count == 1) {
            // Add the tiles to this network.
            neighbouringNetworks[0].addTiles(tilesToAdd);
            return;
        }

        if (neighbouringNetworks.Count > 1) {
            // Create a new network and merge it with the others and then add all the tiles.
            mergeNetworks(createNewNetwork(owner), neighbouringNetworks.ToArray()).addTiles(tilesToAdd);
            return;
        }
    }

    // Create a new network
    Network createNewNetwork(Player owner) {

        int highestId = 0;

        // Find highest network id
        foreach (Network n in networks) {
            if (n.id > highestId) {
                highestId = n.id;
            }
        }

        Network newNetwork = new Network(highestId + 1, owner);
        networks.Add(newNetwork);

        return newNetwork;
    }


    /// <summary>
    /// Merge network n and n[].
    /// </summary>
    /// <param name="theNetwork"></param>
    /// <param name="networksToMerge"></param>
    /// <returns>The merged network</returns>
    Network mergeNetworks(Network theNetwork, Network[] networksToMerge) {

        // Check if theNetwork exists.
        if (!networks.Contains(theNetwork)) {
            Debug.LogError("theNetwork does not exist");
            return default;
        }

        // Merge the networks.
        foreach (Network network in networksToMerge) {

            // Check if network exists.
            if (!networks.Contains(network)) {
                Debug.LogError("network does not exist");
            }

            else {
                // Merge theNetwork with network.
                theNetwork.addTiles(network.tiles.ToArray());

                // Delete network
                deleteNetwork(network);
            }
        }

        return theNetwork;
    }

    /// <summary>
    /// Delete network n.
    /// </summary>
    /// <param name="n">Network to be deleted</param>
    void deleteNetwork(Network network) {

        // Check if the network exists.
        if (!networks.Contains(network)) {
            Debug.LogError("network does not exist");
            return;
        }

        networks.Remove(network);
    }

    public void operation(float modifier) {
        float demand;

        foreach (Network network in networks) {
            foreach (CityGraph city1 in network.graph) {
                foreach (KeyValuePair<City, int> city2 in city1.distances) {
                    if (city2.Value <= 0) {
                        Debug.LogError("distance is 0");
                    }

                    else {
                        // Find the total population of the 2 cities and divide by the distance squared. Multiply this by the modifier.
                        demand = (city1.cityTile.city.population + city2.Key.population) / Mathf.Pow(city2.Value, 2 * modifier);
                        network.owner.opereatingIncome((int) demand);
                    }
                }
            }
        }
    }

    public bool areCitiesConnected(Tile tile1, Tile tile2) {

        // Check that t1 exist.
        if (tile1 == null) {
            Debug.LogError("tile1 = null");
            return false;
        }

        // Check that t2 exist.
        if (tile2 == null) {
            Debug.LogError("tile2 = null");
            return false;
        }

        // Check if the 2 tiles are in the same network.
        foreach (Network network in networks) {
            if (network.containsTile(tile1) && network.containsTile(tile2)) {
                return true;
            }
        }

        return false;
    }
}

/// Color the cities with different colors, corresponding to the different networks.

/// graph of cities
/// Dictionary<City, Dictionary<City, int>>
/// 


