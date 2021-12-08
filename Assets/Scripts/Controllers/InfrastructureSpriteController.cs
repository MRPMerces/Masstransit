using System.Collections.Generic;
using UnityEngine;

public class InfrastructureSpriteController : MonoBehaviour {

    public static InfrastructureSpriteController infrastructureSpriteController;

    public Dictionary<Tile, Dictionary<Player, GameObject>> roadSprites { get; protected set; }
    public Dictionary<Tile, Dictionary<Player, GameObject>> highwaySprites { get; protected set; }
    public Dictionary<Tile, Dictionary<Player, GameObject>> lstSprites { get; protected set; }
    public Dictionary<Tile, Dictionary<Player, GameObject>> hstSprites { get; protected set; }

    Dictionary<string, Sprite> infrastructureSprites;

    // Start is called before the first frame update
    void Start() {
        infrastructureSpriteController = this;

        roadSprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        highwaySprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        lstSprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        hstSprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();

        loadSprites();
    }

    public void OnTileInfrastructureChanged(Tile tile, NetworkType type) {

        /// Add build time for infrastructure, when that is done, this implementation is fine. Dont remove the callback.
        switch (type) {
            case NetworkType.Road:
                if (tile.road.Count > 0) {
                    processSprite(roadSprites, tile.road, tile, type, "road_");
                }

                break;

            case NetworkType.Highway:
                if (tile.highway.Count > 0) {
                    processSprite(highwaySprites, tile.highway, tile, type, "highway_");
                }

                break;

            case NetworkType.LST:
                if (tile.lst.Count > 0) {
                    processSprite(lstSprites, tile.lst, tile, type, "lst_");
                }

                break;

            case NetworkType.HST:
                if (tile.hst.Count > 0) {
                    processSprite(hstSprites, tile.hst, tile, type, "hst_");
                }

                break;

            default:
                Debug.LogError("Unregognised type");
                return;
        }
    }

    void processSprite(Dictionary<Tile, Dictionary<Player, GameObject>> overlay, List<Player> players, Tile tile, NetworkType type, string spriteType) {
        foreach (Player player in players) {

            if (!overlay.ContainsKey(tile)) {
                GameObject gameObject = new GameObject { name = "Tile_" + tile.X + "_" + tile.Y };
                gameObject.transform.position = tile.toVector3();
                gameObject.transform.SetParent(transform, true);

                // Add a Sprite Renderer
                gameObject.AddComponent<SpriteRenderer>().sortingLayerName = "Infrastructure";

                // Add our tile/GO pair to the dictionary.
                overlay.Add(tile, new Dictionary<Player, GameObject> { { player, gameObject } });
            }

            // Debug only remove when all the sprites are added.
            if (!infrastructureSprites.ContainsKey(spriteType + findSprite(tile, player, type))) {
                overlay[tile][player].GetComponent<SpriteRenderer>().sprite = infrastructureSprites[spriteType];
            }

            else {
                // Set the sprite of the gameobject's spriterenderer to the sprite that infrastructureSprites spits out with the given spritname.
                overlay[tile][player].GetComponent<SpriteRenderer>().sprite = infrastructureSprites[spriteType + findSprite(tile, player, type)];
            }
        }
    }

    public string findSprite(Tile tile, Player player, NetworkType networkType) {

        // Get the neighbourng tiles so that we can check the adjacent tiles for infrastructure.
        Tile[] tiles = tile.getNeighbours();
        Tile north = tiles[0];
        Tile east = tiles[1];
        Tile south = tiles[2];
        Tile west = tiles[3];

        if (checkTile(north, networkType, player)) {
            if (checkTile(south, networkType, player)) {
                if (checkTile(east, networkType, player)) {

                    // 4X junction
                    if (checkTile(west, networkType, player)) {
                        return "x_junction";
                    }

                    // 3X junction
                    return "north_east_south";
                }

                // 3X junction
                if (checkTile(west, networkType, player)) {
                    return "north_south_west";
                }

                // straight
                return "north_south";
            }

            if (checkTile(east, networkType, player)) {

                // 3X junction
                if (checkTile(west, networkType, player)) {
                    return "north_east_west";
                }

                // Rigth
                return "north_east";
            }

            // Rigth
            if (checkTile(west, networkType, player)) {
                return "north_west";
            }

            // Deadend
            return "north_dead_end";
        }

        if (checkTile(south, networkType, player)) {
            if (checkTile(east, networkType, player)) {

                // 3X junction
                if (checkTile(west, networkType, player)) {
                    return "east_south_west";
                }

                // Rigth
                return "east_south";
            }

            // Rigth
            if (checkTile(west, networkType, player)) {
                return "south_west";
            }

            // Deadend
            return "south_dead_end";
        }

        if (checkTile(east, networkType, player)) {

            // straight
            if (checkTile(west, networkType, player)) {
                return "east_west";
            }

            // Deadend
            return "east_dead_end";
        }

        // Deadend
        if (checkTile(west, networkType, player)) {
            return "west_dead_end";
        }

        return "";
    }

    bool checkTile(Tile tile, NetworkType networkType, Player player) {
        return tile != null && tile.hasPlayerInfrastructure(networkType, player);
    }


    void loadSprites() {
        infrastructureSprites = new Dictionary<string, Sprite>();
        Sprite[] infrastructureprites = Resources.LoadAll<Sprite>("Sprites/TileInfrastructureSprites/");
        Sprite[] overlaysprites = Resources.LoadAll<Sprite>("Sprites/OverlaySprites/");

        foreach (Sprite sprite in infrastructureprites) {
            infrastructureSprites[sprite.name] = sprite;
        }
    }
}
