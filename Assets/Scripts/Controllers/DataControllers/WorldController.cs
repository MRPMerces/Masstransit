using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {
    public static WorldController worldController { get; protected set; }

    World world;

    // Start is called before the first frame update
    void OnEnable() {
        worldController = this;
        this.world = new World(25, 25);
        // createWorld(new World(25, 25));

        randomizeCityTiles();

        Debug.Log(world.tilesWithCity.Count + " Cities");

        // Center the camera.
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Height / 2, Camera.main.transform.position.z);
    }

    public void createWorld(World world) {
        this.world = world;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void saveWorld() {
        //XmlSerializer serializer = new XmlSerializer(typeof(World));
        //FileStream stream = new FileStream(Application.dataPath + "/Resources/Data/SaveGame00.xml", FileMode.Create);

        // serializer.Serialize(stream, WorldController.worldController.world);
        //stream.Close();
    }

    /// <summary>
    /// Randomize witch tiles is cities
    /// </summary>
    public void randomizeCityTiles() {
        foreach (Tile tile in world.tiles) {
            if (!tile.hasACityNeighbour() && UnityEngine.Random.Range(0, 10) == 0) {
                tile.createCity();
            }
        }
    }  
}