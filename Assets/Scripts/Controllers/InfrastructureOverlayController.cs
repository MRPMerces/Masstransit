using System.Collections.Generic;
using UnityEngine;

public class InfrastructureOverlayController : MonoBehaviour {

    public static InfrastructureOverlayController infrastructureOverlayController;

    Dictionary<Tile, GameObject> overlay;

    Dictionary<string, Sprite> overlaySprites;

    InfrastructureSpriteController infrastructureSpriteController {
        get { return InfrastructureSpriteController.infrastructureSpriteController; }
    }

    // Start is called before the first frame update
    void Start() {
        infrastructureOverlayController = this;

        overlay = new Dictionary<Tile, GameObject>();

        loadSprites();
    }

    public void enableOverlays(NetworkType type, Player player) {
        disableOverlays();

        switch (type) {
            case NetworkType.Road:
                findOverlays(infrastructureSpriteController.roadSprites, type, player, new Color(60, 46, 32));
                break;

            case NetworkType.Highway:
                findOverlays(infrastructureSpriteController.highwaySprites, type, player, new Color(60, 46, 32));
                break;

            case NetworkType.LST:
                findOverlays(infrastructureSpriteController.lstSprites, type, player, new Color(60, 46, 32));
                break;

            case NetworkType.HST:
                findOverlays(infrastructureSpriteController.hstSprites, type, player, new Color(60, 46, 32));
                break;

            default:
                Debug.LogError("Unregognised type");
                return;
        }

    }

    public void disableOverlays() {
        changeSprites(infrastructureSpriteController.roadSprites, false);
        changeSprites(infrastructureSpriteController.highwaySprites, false);
        changeSprites(infrastructureSpriteController.lstSprites, false);
        changeSprites(infrastructureSpriteController.hstSprites, false);
    }

    void changeSprites(Dictionary<Tile, Dictionary<Player, GameObject>> spriteDictionary, bool enable) {
        foreach (Tile tile in spriteDictionary.Keys) {
            foreach (GameObject gameObject in spriteDictionary[tile].Values) {
                gameObject.SetActive(enable);
            }
        }
    }

    void findOverlays(Dictionary<Tile, Dictionary<Player, GameObject>> spriteDictionary, NetworkType type, Player player, Color color) {

        /// We are destroyng all the gameobjects for every single infrastructure tile. And then respawning them. Maybe bad?
        foreach (GameObject gameObject in overlay.Values) {
            Destroy(gameObject);
        }
        overlay.Clear();

        foreach (Tile tile in spriteDictionary.Keys) {
            foreach (Player owner in spriteDictionary[tile].Keys) {
                if (owner == player) {
                    GameObject gameObject = new GameObject { name = "Tile_" + tile.X + "_" + tile.Y + "overlay" };
                    gameObject.transform.position = tile.toVector3();
                    gameObject.transform.SetParent(transform, true);

                    // Add a Sprite Renderer
                    SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

                    // Set the sprite of the gameobject's spriterenderer to the sprite that overlaySprites spits out with the given spritname.
                    spriteRenderer.sortingLayerName = "Infrastructure";
                    spriteRenderer.sprite = overlaySprites["overlay_" + infrastructureSpriteController.findSprite(tile, player, type)];
                    spriteRenderer.color = color;

                    // Add our tile/GO pair to the dictionary.
                    overlay.Add(tile, gameObject);

                    break;
                }
            }
        }
    }

    void loadSprites() {
        overlaySprites = new Dictionary<string, Sprite>();
        Sprite[] infrastructureprites = Resources.LoadAll<Sprite>("Sprites/TileInfrastructureSprites/");
        Sprite[] overlaysprites = Resources.LoadAll<Sprite>("Sprites/OverlaySprites/");

        foreach (Sprite sprite in overlaysprites) {
            overlaySprites[sprite.name] = sprite;
        }
    }
}
