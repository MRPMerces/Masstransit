using System.Collections.Generic;
using UnityEngine;

public class InfrastructureSpriteController : MonoBehaviour {

    public static InfrastructureSpriteController infrastructureSpriteController;

    Dictionary<Tile, Dictionary<Player, GameObject>> roadSprites;
    Dictionary<Tile, Dictionary<Player, GameObject>> roadOverlays;

    Dictionary<Tile, Dictionary<Player, GameObject>> highwaySprites;
    Dictionary<Tile, Dictionary<Player, GameObject>> highwayOverlays;

    Dictionary<Tile, Dictionary<Player, GameObject>> lstSprites;
    Dictionary<Tile, Dictionary<Player, GameObject>> lstOverlays;

    Dictionary<Tile, Dictionary<Player, GameObject>> hstSprites;
    Dictionary<Tile, Dictionary<Player, GameObject>> hstOverlays;

    Dictionary<string, Sprite> sprites;

    // Start is called before the first frame update
    private void Start() {
        infrastructureSpriteController = this;

        roadSprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        roadOverlays = new Dictionary<Tile, Dictionary<Player, GameObject>>();

        highwaySprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        highwayOverlays = new Dictionary<Tile, Dictionary<Player, GameObject>>();

        lstSprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        lstOverlays = new Dictionary<Tile, Dictionary<Player, GameObject>>();

        hstSprites = new Dictionary<Tile, Dictionary<Player, GameObject>>();
        hstOverlays = new Dictionary<Tile, Dictionary<Player, GameObject>>();

        loadSprites();

        // Register our callback so that the InfrastructureOverlayController gets updated whenever a tile's infrastructure updates.
        World.world.RegisterTileInfrastructureChanged(OnTileInfrastructureChanged);
    }

    private void OnTileInfrastructureChanged(Tile tile, NetworkType type, Player player) {
        /// Add build time for infrastructure, when that is done, this implementation is fine. Dont remove the callback.
        switch (type) {
            case NetworkType.Road:
                processSprite(roadSprites, player, tile, type, "road_", Color.clear);
                processSprite(roadOverlays, player, tile, type, "overlay_", new Color(60, 46, 32));
                return;

            case NetworkType.Highway:
                processSprite(highwaySprites, player, tile, type, "highway_", Color.clear);
                processSprite(highwayOverlays, player, tile, type, "overlay_", new Color(60, 46, 32));
                return;

            case NetworkType.LST:
                processSprite(lstSprites, player, tile, type, "lst_", Color.clear);
                processSprite(lstOverlays, player, tile, type, "overlay_", new Color(60, 46, 32));
                return;

            case NetworkType.HST:
                processSprite(hstSprites, player, tile, type, "hst_", Color.clear);
                processSprite(hstOverlays, player, tile, type, "overlay_", new Color(60, 46, 32));
                return;

            default:
                Debug.LogError("Unregognised type");
                return;
        }
    }

    private void processSprite(Dictionary<Tile, Dictionary<Player, GameObject>> spriteMap, Player player, Tile tile, NetworkType type, string spriteType, Color color) {
        if (!spriteMap.ContainsKey(tile)) {
            GameObject gameObject = new GameObject(spriteType == "overlay_" ? tile.name + "_overlay" : tile.name);
            gameObject.transform.position = tile.toVector3();
            gameObject.transform.SetParent(transform, true);

            // Add a Sprite Renderer
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Infrastructure";

            // Add our tile/GO pair to the dictionary.
            spriteMap.Add(tile, new Dictionary<Player, GameObject> { { player, gameObject } });
        }

        // Debug only remove when all the sprites are added.
        if (!sprites.ContainsKey(spriteType + findSprite(tile, player, type))) {
            spriteMap[tile][player].GetComponent<SpriteRenderer>().sprite = sprites[spriteType];
        }

        else {
            // Set the sprite of the gameobject's spriterenderer to the sprite that infrastructureSprites spits out with the given spritname.
            spriteMap[tile][player].GetComponent<SpriteRenderer>().sprite = sprites[spriteType + findSprite(tile, player, type)];
        }

        if (color != Color.clear) {
            spriteMap[tile][player].GetComponent<SpriteRenderer>().color = color;
        }
    }

    private string findSprite(Tile tile, Player player, NetworkType networkType) {

        // Get the neighbourng tiles so that we can check the adjacent tiles for infrastructure.
        Tile[] tiles = tile.getNeighbours();
        Tile north = tiles[0];
        Tile east = tiles[1];
        Tile south = tiles[2];
        Tile west = tiles[3];

        if (checkTile(north, networkType, player)) {
            if (checkTile(south, networkType, player)) {
                if (checkTile(east, networkType, player)) {
                    if (checkTile(west, networkType, player)) {
                        // 4X junction
                        return "x_junction";
                    }

                    // 3X junction
                    return "north_east_south";
                }

                if (checkTile(west, networkType, player)) {
                    // 3X junction
                    return "north_south_west";
                }

                // straight
                return "north_south";
            }

            if (checkTile(east, networkType, player)) {
                if (checkTile(west, networkType, player)) {
                    // 3X junction
                    return "north_east_west";
                }

                // Rigth
                return "north_east";
            }

            if (checkTile(west, networkType, player)) {
                // Rigth
                return "north_west";
            }

            // Deadend
            return "north_dead_end";
        }

        if (checkTile(south, networkType, player)) {
            if (checkTile(east, networkType, player)) {
                if (checkTile(west, networkType, player)) {
                    // 3X junction
                    return "east_south_west";
                }

                // Rigth
                return "east_south";
            }

            if (checkTile(west, networkType, player)) {
                // Rigth
                return "south_west";
            }

            // Deadend
            return "south_dead_end";
        }

        if (checkTile(east, networkType, player)) {
            if (checkTile(west, networkType, player)) {
                // straight
                return "east_west";
            }

            // Deadend
            return "east_dead_end";
        }

        if (checkTile(west, networkType, player)) {
            // Deadend
            return "west_dead_end";
        }

        return "";
    }

    private bool checkTile(Tile tile, NetworkType networkType, Player player) {
        return tile != null && tile.hasPlayerInfrastructure(networkType, player);
    }

    private void loadSprites() {
        sprites = new Dictionary<string, Sprite>();

        Sprite[] infrastructureprites = Resources.LoadAll<Sprite>("Sprites/TileInfrastructureSprites/");
        Sprite[] overlaysprites = Resources.LoadAll<Sprite>("Sprites/OverlaySprites/");

        foreach (Sprite sprite in infrastructureprites) {
            sprites[sprite.name] = sprite;
        }

        foreach (Sprite sprite in overlaysprites) {
            sprites[sprite.name] = sprite;
        }
    }

    public void enableOverlays(NetworkType type, Player player, bool enable = true) {
        if (enable) {
            // Activate all existing overlays.
            enableSprites(false);

            switch (type) {
                case NetworkType.Road:
                    activateGameobjects(roadSprites, true);

                    return;

                case NetworkType.Highway:
                    activateGameobjects(roadSprites, true);

                    return;

                case NetworkType.LST:
                    activateGameobjects(roadSprites, true);

                    return;

                case NetworkType.HST:
                    activateGameobjects(roadSprites, true);

                    return;

                default:
                    Debug.LogError("Unregognised type");
                    return;
            }
        }

        enableSprites();
        disableOverlays();
    }

    public void disableOverlays() {
        activateGameobjects(roadOverlays, false);
        activateGameobjects(highwayOverlays, false);
        activateGameobjects(lstOverlays, false);
        activateGameobjects(hstOverlays, false);
    }

    public void enableSprites(bool enable = true) {
        activateGameobjects(roadSprites, enable);
        activateGameobjects(highwaySprites, enable);
        activateGameobjects(lstSprites, enable);
        activateGameobjects(hstSprites, enable);
    }

    void activateGameobjects(Dictionary<Tile, Dictionary<Player, GameObject>> spriteDictionary, bool enable) {
        foreach (Tile tile in spriteDictionary.Keys) {
            foreach (GameObject gameObject in spriteDictionary[tile].Values) {
                gameObject.SetActive(enable);
            }
        }
    }
}
