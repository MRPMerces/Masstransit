using UnityEngine;
using UnityEngine.UI;

public enum BuildMode { None, Road, Highway, LST, HST, Metro, Airport };
public class BuildModeController : MonoBehaviour {
    public static BuildModeController buildModeController { get; protected set; }

    public Button roadButton;

    private BuildMode buildMode = BuildMode.None;

    private Player player {
        get { return World.world.playerController.human; }
    }

    private Tile currentTile {
        get { return MouseController.mouseController.currentHoveredTile; }
    }

    void Start() {
        buildModeController = this;
    }

    // Update is called once per frame
    void Update() {

        ColorBlock colorBlock = new ColorBlock {
            normalColor = Color.white,
            pressedColor = Color.green,
            highlightedColor = Color.gray,
            disabledColor = Color.red,
            colorMultiplier = 1f,
            fadeDuration = 1f
        };

        roadButton.colors = colorBlock;

        roadButton.enabled = true;

        // The player exited buidling with a rigth click.
        if (Input.GetMouseButton(1)) {
            buildMode = BuildMode.None;
            return;
        }

        switch (buildMode) {
            case BuildMode.None:
                return;

            case BuildMode.Metro:
                if (Input.GetMouseButton(0) && currentTile != null && currentTile.isCity) {
                    // Send the city to the metro menu.
                    UI_MetroMenu.uI_MetroMenu.metroBuilding(currentTile.city);
                }
                return;

            case BuildMode.Airport:
                if (Input.GetMouseButton(0) && currentTile != null && currentTile.isCity) {
                    // Send the city to the metro menu.
                    UI_AirportMenu.uI_AirportMenu.airportBuilding(currentTile.city);
                }
                return;

            default:
                MouseController.mouseController.dragInfrastructure();
                return;
        }
    }

    /// <summary>
    /// Function to set the buildMode from a int.
    /// Can be used by buttons with the mode parameter set in th inspector.
    /// </summary>
    /// <param name="mode">int parameter to be casted to a BuildMode and applied to buildMode</param>
    public void setBuildMode(int mode) {
        if (buildMode == BuildMode.None) {
            buildMode = (BuildMode)mode;
        }
    }

    public void doBuild(Tile[] tiles) {

        // tiles will be null if the user cancels the build, before actually building something.
        if (tiles != null) {
            switch (buildMode) {
                case BuildMode.Road:
                    World.world.road.addTiles(tiles, player);
                    break;

                case BuildMode.Highway:
                    World.world.highway.addTiles(tiles, player);
                    break;

                case BuildMode.LST:
                    World.world.lst.addTiles(tiles, player);
                    break;

                case BuildMode.HST:
                    World.world.hst.addTiles(tiles, player);
                    break;

                default:
                    Debug.LogError("Unrecognized or invalid buildmode");
                    break;
            }
        }
        buildMode = BuildMode.None;
    }
}
