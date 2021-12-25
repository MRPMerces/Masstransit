using UnityEngine;

public class KeyInputController : MonoBehaviour {

    public static KeyInputController keyInputController;

    public GameObject mainMenu;

    private Player human {
        get { return World.world.playerController.human; }
    }

    // Start is called before the first frame update
    void Start() {
        keyInputController = this;
    }

    // Update is called once per frame
    void Update() {
        #region Overlays
        if (Input.GetKeyDown(KeyCode.F1)) {
            disableOverlays();
            InfrastructureSpriteController.infrastructureSpriteController.enableSprites();
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            disableOverlays();
            InfrastructureSpriteController.infrastructureSpriteController.enableOverlays(NetworkType.Road, human);
        }

        if (Input.GetKeyDown(KeyCode.F3)) {
            disableOverlays();
            InfrastructureSpriteController.infrastructureSpriteController.enableOverlays(NetworkType.Highway, human);
        }

        if (Input.GetKeyDown(KeyCode.F4)) {
            disableOverlays();
            InfrastructureSpriteController.infrastructureSpriteController.enableOverlays(NetworkType.LST, human);
        }

        if (Input.GetKeyDown(KeyCode.F5)) {
            disableOverlays();
            InfrastructureSpriteController.infrastructureSpriteController.enableOverlays(NetworkType.HST, human);
        }

        if (Input.GetKeyDown(KeyCode.F6)) {
            World.world.airportGraph.enableAirportOverlay();
        }

        if (Input.GetKeyDown(KeyCode.F7)) {
            disableOverlays();
            TileSpriteController.tileSpriteController.enableBorder();
        }

        #endregion Overlays

        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            SpeedController.speedController.changeSpeed(3);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            SpeedController.speedController.changeSpeed(2);
        }

        if (Input.GetKeyDown(KeyCode.Keypad9)) {
            SpeedController.speedController.changeSpeed(1);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            SpeedController.speedController.changeSpeed(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            mainMenu.SetActive(!mainMenu.activeSelf);
        }
    }

    public void disableOverlays() {
        InfrastructureSpriteController.infrastructureSpriteController.disableOverlays();

        World.world.airportGraph.disableAirportOverlay();
        TileSpriteController.tileSpriteController.enableBorder(false);
    }
}
