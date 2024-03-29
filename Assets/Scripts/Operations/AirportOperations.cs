﻿using System.Collections.Generic;
using UnityEngine;

public class AirportOperations : MonoBehaviour {

    private void Start() {
        SpeedController.speedController.RegisterDayTickCallback(airport_Operations);
    }

    public void airport_Operations() {
        foreach (KeyValuePair<Tile, Path_Node<Tile>> kvp in World.world.airportGraph.nodes) {
            if (kvp.Value.edges != null) {
                foreach (Path_Edge<Tile> E in kvp.Value.edges) {
                    int totalPopulation = kvp.Key.city.population + E.node.data.city.population;
                    int demand = (int)(totalPopulation / (Mathf.Pow(E.cost, 2) + 4));

                    /// Hard coded to player, maek sure to not add airport to ai yet.
                    kvp.Key.city.airports[0].owner.opereatingIncome(demand);
                }
            }
        }
    }
}
