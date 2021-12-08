using UnityEngine;

public class River : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        //startRiver();
    }

    // Update is called once per frame
    void Update() {

    }


    void startRiver() {
        foreach (Tile tile in World.world.tiles) {
            if (tile != null && !tile.isCity && UnityEngine.Random.Range(0, 10) == 1) {
                tile.setType(TileType.River);
                progressRiver(tile);
                break;
            }
        }
    }

    void progressRiver(Tile tile) {
        if (tile == null) {
            Debug.LogError("tile is null");
            return;
        }

        Tile tile_n = World.world.getTileAt(tile.X, tile.Y + 1);
        if (tile_n != null && !tile_n.isCity) {
            tile_n.setType(TileType.River);
            progressRiver(tile_n);
        }
    }
}
