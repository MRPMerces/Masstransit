using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NetworkManager {
    public NetworkManager(NetworkType type, int stationCost, int tileCost) {
        this.type = type;

        this.stationCost = stationCost;
        this.tileCost = tileCost;

        networks = new List<Network>();
        playerTiles = new Dictionary<Player, List<Tile>>();
    }

    readonly NetworkType type;

    readonly int stationCost;
    readonly int tileCost;
    public List<Network> networks;

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

        float cost;
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
                if (type != NetworkType.ROAD && tile.hasPlayerInfrastructure(NetworkType.ROAD, owner)) {
                    cost = tileCost * 0.75f;
                }

                else {
                    cost = tileCost;
                }
            }

            // Check if the owner can afford the cost.
            if (!owner.canAffordConstructionCost(cost)) {
                Debug.LogError("Insufficient funds!");
                return;
            }

            owner.constructionCost(cost);

            // Player can afford the cost, so add the infrastructure.
            tile.add_infrastructureOwner(type, owner);
            tilesToCheck.AddRange(tile.getNeighbours());

            // Add the tile to the player.
            playerTiles[owner].Add(tile);
        }

        mergeCheck(tilesToCheck.ToArray(), tilesToAdd, owner);
    }

    void mergeCheck(Tile[] tilesToCheck, Tile[] tilesToAdd, Player owner) {
        // Check if a neighbouring tile allready is in a network.
        List<Network> neighbouringNetworks = new List<Network>();
        foreach (Tile tileToCheck in tilesToCheck) {
            if (tileToCheck != null && tileToCheck.hasPlayerInfrastructure(type, owner)) {
                foreach (Network network in networks) {
                    if (!neighbouringNetworks.Contains(network) && network.containsTile(tileToCheck)) {
                        neighbouringNetworks.Add(network);
                        break;
                    }
                }
            }
        }

        switch (neighbouringNetworks.Count) {
            case 0:
                // Create a new network and add the tiles to this network.
                createNewNetwork(owner).addTiles(tilesToAdd);
                return;

            case 1:
                // Add the tiles to this network.
                neighbouringNetworks[0].addTiles(tilesToAdd);
                return;

            default:
                // Create a new network and merge it with the others and then add all the tiles.
                mergeNetworks(createNewNetwork(owner), neighbouringNetworks.ToArray()).addTiles(tilesToAdd);
                return;
        }
    }

    // Create a new network
    Network createNewNetwork(Player owner) {
        Network newNetwork = new Network(type, owner);
        networks.Add(newNetwork);

        return newNetwork;
    }

    /// <summary>
    /// Merge theNetwork and networksToMerge.
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

        Dictionary<CityPair, Vehicle> existingVehicles = new Dictionary<CityPair, Vehicle>();

        foreach (Network network in networksToMerge) {
            foreach (KeyValuePair<CityPair, Vehicle> keyValuePair in network.vehicles) {
                existingVehicles.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        // Merge the networks.
        foreach (Network network in networksToMerge) {

            // Check if network exists.
            if (!networks.Contains(network)) {
                Debug.LogError("network does not exist");
                continue;
            }

            // Merge theNetwork with network.
            theNetwork.addTiles(network.tiles.ToArray(), false);

            // Delete network
            deleteNetwork(network);
        }

        theNetwork.vehicles = existingVehicles;
        theNetwork.networkGraph.reGenerateNetwork();

        return theNetwork;
    }

    /// <summary>
    /// Delete network network.
    /// </summary>
    /// <param name="network">Network to be deleted</param>
    void deleteNetwork(Network network) {

        // Check if the network exists.
        if (!networks.Contains(network)) {
            Debug.LogError("network does not exist");
            return;
        }

        networks.Remove(network);
    }

    public void operation(float modifier, float speed = 0.0833333333333333f) {
        //speed = 0.13f;

        // operation tick once per ingame hour.
        /// Multithread?
        Tile currentTile;
        foreach (Network network in networks) {
            foreach (KeyValuePair<CityPair, Vehicle> keyValuePair in network.vehicles) {

                currentTile = World.world.getTileAt(keyValuePair.Value.position);
                if (currentTile == null) {
                    Debug.LogError("currentTile is null");
                    continue;
                }

                if (keyValuePair.Value.direction.x > 0) {
                    keyValuePair.Value.translate(speed, 0);
                }

                else if (keyValuePair.Value.direction.x < 0) {
                    keyValuePair.Value.translate(-speed, 0);
                }

                else if (keyValuePair.Value.direction.y > 0) {
                    keyValuePair.Value.translate(0, speed);
                }

                else if (keyValuePair.Value.direction.y < 0) {
                    keyValuePair.Value.translate(0, -speed);
                }

                else {
                    Debug.LogError("We dont have a direction?");
                    /// Just find a direction?
                }

                Debug.Log("pos: " + keyValuePair.Value.position.x.ToString());

                if (currentTile != World.world.getTileAt(keyValuePair.Value.position)) {
                    // We have moved to a new tile.

                    currentTile = World.world.getTileAt(keyValuePair.Value.position);

                    if (currentTile == keyValuePair.Value.destinationTile) {
                        // We have arrived at our destination.

                        // Find the total population of the 2 cities and divide by the distance squared. Multiply this by the modifier.
                        float demand = (keyValuePair.Key.start.city.population + keyValuePair.Key.end.city.population) / Mathf.Pow(keyValuePair.Key.distance, 2) * modifier;
                        network.owner.opereatingIncome(demand);

                        Debug.Log("Overshoot: " + keyValuePair.Value.position.x.ToString());

                        // Keep extra movement.

                        float xx = keyValuePair.Value.position.x - currentTile.X;
                        float yy = keyValuePair.Value.position.y - currentTile.Y;

                        if (keyValuePair.Value.direction.x == 0) {
                            keyValuePair.Value.translate(0, -(2 * yy));
                        }

                        else {
                            keyValuePair.Value.translate(-(2 * xx), 0);
                        }

                        // Reverse the start and end tile and the path.
                        keyValuePair.Value.reverse();

                        if (keyValuePair.Key.newPath != null) {
                            keyValuePair.Key.assignNewPath();
                        }
                    }

                    // We have moved to a new tile. Lets get a new nextTile.
                    if (keyValuePair.Value.reversed) {
                        for (int i = keyValuePair.Key.path.Count() - 1; i > 0; i--) {
                            if (keyValuePair.Key.path[i] == currentTile) {
                                keyValuePair.Value.nextTile = keyValuePair.Key.path[i - 1];
                            }
                        }
                    }

                    else {
                        for (int i = 0; i < keyValuePair.Key.path.Count(); i++) {
                            if (keyValuePair.Key.path[i] == currentTile) {
                                keyValuePair.Value.nextTile = keyValuePair.Key.path[i + 1];
                            }
                        }
                    }

                    Vector2 direction = Vector2.zero;

                    if (currentTile.X != keyValuePair.Value.nextTile.X) {
                        if (currentTile.X > keyValuePair.Value.nextTile.X) {
                            direction.x = -1;
                        }

                        else {
                            direction.x = 1;
                        }
                    }

                    if (currentTile.Y != keyValuePair.Value.nextTile.Y) {
                        if (currentTile.Y > keyValuePair.Value.nextTile.Y) {
                            direction.y = -1;
                        }

                        else {
                            direction.y = 1;
                        }
                    }

                    keyValuePair.Value.direction = direction;
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
