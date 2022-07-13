using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_RoadController : MonoBehaviour {

    private Player ai {
        get { return World.world.playerController.players[1]; }
    }

    MetroController metroController {
        get { return MetroController.metroController; }
    }

    List<Tile> tiles;

    bool b = true;


    // Start is called before the first frame update
    void Start() {
        tiles = World.world.tilesWithCity.ToList();
    }

    // Update is called once per frame
    void Update() {
        if (b) {
            //aiBuildRoad();
            //aiBuildRoad();
            //aiBuildRoad();
            //aiBuildRoad();
            //aiBuildRoad();

            //aiBuildlstn();
            //aiBuildlstn();
            //aiBuildlstn();
            //aiBuildlstn();
            //aiBuildlstn();

            //aiBuildHw();
            //aiBuildHw();
            //aiBuildHw();
            //aiBuildHw();
            //aiBuildHw();

            //aiBuildMetro();
            //aiBuildMetro();
            //aiBuildMetro();
            //aiBuildMetro();
            //aiBuildMetro();

            b = false;
        }
    }

    void aiBuildRoad() {
        int pop1 = 0;
        int pop2;
        Tile t1 = null;
        Tile t2 = null;
        foreach (Tile tile in tiles) {
            if (tile.city.population > pop1) {
                if (t1 != null && t2 != null && !World.world.road.areCitiesConnected(tile, t1)) {
                    //continue;
                }
                t2 = t1;
                t1 = tile;

                pop2 = pop1;
                pop1 = tile.city.population;
            }
        }
        tiles.Remove(t1);
        tiles.Remove(t2);


        Tile tile2 = t1;
        List<Tile> tiles2 = new List<Tile>();

        if (t1.X > t2.X) {
            do {
                tile2 = World.world.getTileAt(tile2.X - 1, t1.Y);
                tiles2.Add(tile2);
            }
            while (tile2.X > t2.X);
        }

        else if (t1.X < t2.X) {
            do {
                tile2 = World.world.getTileAt(tile2.X + 1, t1.Y);
                tiles2.Add(tile2);
            }
            while (tile2.X < t2.X);
        }

        if (t1.Y > t2.Y) {
            do {
                tile2 = World.world.getTileAt(t2.X, tile2.Y - 1);
                tiles2.Add(tile2);
            }
            while (tile2.Y > t2.Y);
        }

        else if (t1.Y < t2.Y) {
            do {
                tile2 = World.world.getTileAt(t2.X, tile2.Y + 1);
                tiles2.Add(tile2);
            }
            while (tile2.Y < t2.Y);
        }

        World.world.road.addTiles(tiles2.ToArray(), ai);
    }

    void aiBuildlstn() {
        int pop1 = 0;
        int pop2 = 0;
        Tile t1 = null;
        Tile t2 = null;
        foreach (Tile tile in tiles) {
            if (tile.city.population > pop1) {
                if (t1 != null && t2 != null && !World.world.lst.areCitiesConnected(tile, t1)) {
                    //continue;
                }
                t2 = t1;
                t1 = tile;

                pop2 = pop1;
                pop1 = tile.city.population;
            }
        }
        tiles.Remove(t1);
        tiles.Remove(t2);

        //world.lstn.buildNetworkConnection(t1, t2, ai);
    }

    void aiBuildHw() {
        int pop1 = 0;
        int pop2 = 0;
        Tile t1 = null;
        Tile t2 = null;
        foreach (Tile tile in tiles) {
            if (tile.city.population > pop1) {
                if (t1 != null && t2 != null && !World.world.highway.areCitiesConnected(tile, t1)) {
                    //continue;
                }
                t2 = t1;
                t1 = tile;

                pop2 = pop1;
                pop1 = tile.city.population;
            }
        }
        tiles.Remove(t1);
        tiles.Remove(t2);

        //world.highway.buildNetworkConnection(t1, t2, ai);
    }

    void aiBuildMetro() {

        int pop = 0;
        Tile t = null;

        foreach (Tile tile in World.world.tilesWithCity) {
            if (tile.city.population > pop && !metroController.hasPlayerAMetroInCity(tile.city, ai)) {
                t = tile;

                pop = tile.city.population;
            }
        }

        if (t != null) {
            metroController.buildMetroInCity(t.city, ai);
        }
    }
}
