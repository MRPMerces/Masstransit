using System.Collections.Generic;
using UnityEngine;

public class InfrastructureOverlayController : MonoBehaviour {

    public static InfrastructureOverlayController infrastructureOverlayController;

    Dictionary<Tile, Dictionary<Player, GameObject>> overlays;

    Dictionary<string, Sprite> overlaySprites;

    InfrastructureSpriteController infrastructureSpriteController {
        get { return InfrastructureSpriteController.infrastructureSpriteController; }
    }

    // Start is called before the first frame update
    void Start() {
        infrastructureOverlayController = this;

        overlays = new Dictionary<Tile, Dictionary<Player, GameObject>>();

        loadSprites();
    }

    public void enableOverlays(NetworkType type, Player player) {
        // Activate all existing overlays.
        activateOverlays();

        switch (type) {
            case NetworkType.Road:
                findOverlays(infrastructureSpriteController.roadSprites, type, player, new Color(60, 46, 32));
                return;

            case NetworkType.Highway:
                findOverlays(infrastructureSpriteController.highwaySprites, type, player, new Color(60, 46, 32));
                return;

            case NetworkType.LST:
                findOverlays(infrastructureSpriteController.lstSprites, type, player, new Color(60, 46, 32));
                return;

            case NetworkType.HST:
                findOverlays(infrastructureSpriteController.hstSprites, type, player, new Color(60, 46, 32));
                return;

            default:
                Debug.LogError("Unregognised type");
                return;
        }
    }
    public void disAbleOverlays() {
        activateOverlays(false);
    }

    void activateOverlays(bool enable = true) {
        foreach (Tile tile in overlays.Keys) {
            foreach (GameObject gameObject in overlays[tile].Values) {
                gameObject.SetActive(enable);
            }
        }
    }

    void findOverlays(Dictionary<Tile, Dictionary<Player, GameObject>> spriteDictionary, NetworkType type, Player player, Color color) {

        // Find the overlay of all the new tile infrastructure constructed since the last time this function ran.
        foreach (Tile tile in spriteDictionary.Keys) {
            foreach (Player owner in spriteDictionary[tile].Keys) {
                if (owner == player) {
                    if (!(overlays.ContainsKey(tile) && overlays[tile].ContainsKey(player))) {
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
                        overlays.Add(tile, new Dictionary<Player, GameObject> { { player, gameObject } });
                    }

                    break;
                }
            }
        }
    }

    void loadSprites() {
        overlaySprites = new Dictionary<string, Sprite>();
        Sprite[] overlaysprites = Resources.LoadAll<Sprite>("Sprites/OverlaySprites/");

        foreach (Sprite sprite in overlaysprites) {
            overlaySprites[sprite.name] = sprite;
        }
    }
}
