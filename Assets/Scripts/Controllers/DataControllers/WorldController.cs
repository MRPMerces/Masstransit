using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {
    World world;

    static bool loadWorldbool = false;

    // Start is called before the first frame update
    void OnEnable() {

        if (loadWorldbool) {
            loadWorldbool = false;
            createWorldFromSaveFile();
        }

        else {
            createEmptyWorld();
        }

        Debug.Log(world.tilesWithCity.Count + " Cities");

        // Center the camera.
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Height / 2, Camera.main.transform.position.z);
    }

    void createEmptyWorld() {
        world = new World(25, 25);

        randomizeCityTiles();
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
    
    #region Saving and loading

    public void newWorld() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        createEmptyWorld();
    }

    public void saveWorld() {
        XmlSerializer serializer = new XmlSerializer(typeof(World));
        FileStream stream = new FileStream(Application.dataPath + "/Resources/Data/SaveGame00.xml", FileMode.Create);

        serializer.Serialize(stream, world);
        stream.Close();
    }

    public void loadWorld() {

        // Reload the scene to reset all the data.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        loadWorldbool = true;
    }

    void createWorldFromSaveFile() {

        FileStream stream = new FileStream(Application.dataPath + "/Resources/Data/SaveGame00.xml", FileMode.Open);

        XmlSerializer serializer = new XmlSerializer(typeof(World));

        world = (World)serializer.Deserialize(stream);
        stream.Close();
    }

    #endregion Saving and loading
}

/// Look at quills furntiure.xml reading and replace this one with that one i tinkerino.