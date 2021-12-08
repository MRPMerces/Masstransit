using UnityEngine;

public class KeyInputController : MonoBehaviour {

    private Player human {
        get { return World.world.playerController.human; }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            InfrastructureOverlayController.infrastructureOverlayController.disableOverlays();
            World.world.airportGraph.disableAirportOverlay();
            TileSpriteController.tileSpriteController.enableBorder(false);
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            InfrastructureOverlayController.infrastructureOverlayController.enableOverlays(NetworkType.Road, human);
            World.world.airportGraph.disableAirportOverlay();
        }

        if (Input.GetKeyDown(KeyCode.F3)) {
            InfrastructureOverlayController.infrastructureOverlayController.enableOverlays(NetworkType.Highway, human);
            World.world.airportGraph.disableAirportOverlay();
        }

        if (Input.GetKeyDown(KeyCode.F4)) {
            InfrastructureOverlayController.infrastructureOverlayController.enableOverlays(NetworkType.LST, human);
            World.world.airportGraph.disableAirportOverlay();
        }

        if (Input.GetKeyDown(KeyCode.F5)) {
            InfrastructureOverlayController.infrastructureOverlayController.enableOverlays(NetworkType.HST, human);
            World.world.airportGraph.disableAirportOverlay();
        }

        if (Input.GetKeyDown(KeyCode.F6)) {
            World.world.airportGraph.enableAirportOverlay();
        }

        if (Input.GetKeyDown(KeyCode.F7)) {
            TileSpriteController.tileSpriteController.enableBorder(true);
        }
    }
}
