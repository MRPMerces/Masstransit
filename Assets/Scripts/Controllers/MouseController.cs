using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {
    public static MouseController mouseController { get; protected set; }

    private BuildModeController buildModeController {
        get { return BuildModeController.buildModeController; }
    }

    public GameObject overlayPrefab;

    GameObject hoveredPreviewGameObject;

    // The world-position of the mouse last frame.
    Vector3 lastFramePosition;
    Vector3 currFramePosition;

    // The world-position start of our left-mouse drag operation
    Vector3 dragStartPosition;

    List<GameObject> dragPreviewGameObjects;

    List<Tile> dragTiles;

    bool canceled = false;

    // The currently hovered tile
    // NOTE migth be null!
    public Tile currentHoveredTile {
        get {
            return World.world.getTileAt(currFramePosition);
        }
    }

    // Grabs the currentHoveredTile, if it is a city tile.
    // NOTE migth be null!
    public Tile currentHoveredCityTile {
        get {
            if (currentHoveredTile != null && currentHoveredTile.isCity) {
                return currentHoveredTile;
            }
            return null;
        }
    }

    // Use this for initialization
    void Start() {
        dragPreviewGameObjects = new List<GameObject>();

        hoveredPreviewGameObject = SimplePool.Spawn(overlayPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        hoveredPreviewGameObject.name = "HoveredTilePreviewGameObject";
        hoveredPreviewGameObject.transform.SetParent(transform, true);
        hoveredPreviewGameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        hoveredPreviewGameObject.SetActive(false);

        mouseController = this;
    }

    // Update is called once per frame
    void Update() {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateMouseHover();
        UpdateCameraMovement();

        // Save the mouse position from this frame
        // We don't use currFramePosition because we may have moved the camera.
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }

    void UpdateMouseHover() {
        if (currentHoveredCityTile != null) {
            hoveredPreviewGameObject.SetActive(true);
            hoveredPreviewGameObject.transform.position = currentHoveredTile.toVector3();
        }

        else {
            hoveredPreviewGameObject.SetActive(false);
        }
    }

    void UpdateCameraMovement() {
        // Handle screen panning
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {   // Right or Middle Mouse Button
            Camera.main.transform.Translate(lastFramePosition - currFramePosition);
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
    }

    public void dragInfrastructure() {

        // Activate the tile borders.
        TileSpriteController.tileSpriteController.enableBorder(true);

        // If we're over a UI element, then bail out from this.
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        // Clean up old drag previews
        cleanUpPreviews();

        // If the player presses escape. Quit the drag function.
        if (Input.GetKeyDown(KeyCode.Escape)) {
            buildModeController.doBuild(null);
        }

        // quit Drag
        if (Input.GetMouseButton(1)) {
            dragStartPosition = currFramePosition;
            canceled = true;
            return;
        }

        // Start Drag
        if (Input.GetMouseButtonDown(0)) {
            dragStartPosition = currFramePosition;
            canceled = false;
        }

        if (!canceled) {
            int start_x = Mathf.FloorToInt(dragStartPosition.x + 0.5f);
            int start_y = Mathf.FloorToInt(dragStartPosition.y + 0.5f);
            int end_x = Mathf.FloorToInt(currFramePosition.x + 0.5f);
            int end_y = Mathf.FloorToInt(currFramePosition.y + 0.5f);

            // We may be dragging in the "wrong" direction, so flip things if needed.
            if (end_x < start_x) {
                int temp = end_x;
                end_x = start_x;
                start_x = temp;
            }

            if (end_y < start_y) {
                int temp = end_y;
                end_y = start_y;
                start_y = temp;
            }

            // end_x is always greater than start_x same with y

            if (Input.GetMouseButton(0)) {

                dragTiles = new List<Tile>();

                createPreview(start_x, start_y);
                dragTiles.Add(World.world.getTileAt(start_x, start_y));
                // Display a preview of the drag area
                if (start_x != end_x) {
                    for (int x = start_x; x <= end_x; x++) {
                        createPreview(x, start_y);
                        dragTiles.Add(World.world.getTileAt(x, start_y));
                    }
                }

                else if (start_y != end_y) {
                    for (int y = start_y; y <= end_y; y++) {
                        createPreview(start_x, y);
                        dragTiles.Add(World.world.getTileAt(start_x, y));
                    }
                }
            }

            // End Drag
            if (Input.GetMouseButtonUp(0) && dragTiles.Count > 0) {
                buildModeController.doBuild(dragTiles.ToArray());
                TileSpriteController.tileSpriteController.enableBorder(false);
            }
        }
    }

    void cleanUpPreviews() {
        while (dragPreviewGameObjects.Count > 0) {
            SimplePool.Despawn(dragPreviewGameObjects[0]);
            dragPreviewGameObjects.RemoveAt(0);
        }
    }

    /// <summary>
    /// Display the building hint on top of tile's position.
    /// </summary>
    /// <param name="tile"></param>
    void createPreview(Tile tile) {
        if (tile != null) {
            GameObject gameObject = SimplePool.Spawn(overlayPrefab, tile.toVector3(), Quaternion.identity);
            gameObject.transform.SetParent(transform, true);
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            dragPreviewGameObjects.Add(gameObject);
        }
    }

    /// <summary>
    /// Display the building hint on top of the vector3 position.
    /// </summary>
    /// <param name="vector3"></param>
    void createPreview(Vector3 vector3) {
        GameObject gameObject = SimplePool.Spawn(overlayPrefab, vector3, Quaternion.identity);
        gameObject.transform.SetParent(transform, true);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        dragPreviewGameObjects.Add(gameObject);
    }

    /// <summary>
    /// Display the building hint on top a vector3 position.
    /// </summary>
    /// <param name="x">x component of the vector3.</param>
    /// <param name="y">y component of the vector3.</param>
    void createPreview(int x, int y) {
        GameObject gameObject = SimplePool.Spawn(overlayPrefab, new Vector3(x, y, 0), Quaternion.identity);
        gameObject.transform.SetParent(transform, true);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        dragPreviewGameObjects.Add(gameObject);
    }
}

/// update the color of the cursorcircle based on can we build?