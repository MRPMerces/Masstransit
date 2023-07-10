using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Look at quills furntiure.xml reading and replace this one with that one i tinkerino.

public class SaveController : MonoBehaviour {
    private void OnEnable() {
        //createNewWorld();

    }

    public void saveWorld() {
        XmlSerializer serializer = new XmlSerializer(typeof(World));
        FileStream stream = new FileStream(Application.dataPath + "/Resources/Data/SaveGame00.xml", FileMode.Create);

        //serializer.Serialize(stream, WorldController.worldController.world);
        stream.Close();
    }

    void createWorldFromSaveFile() {

        FileStream stream = new FileStream(Application.dataPath + "/Resources/Data/SaveGame00.xml", FileMode.Open);

        XmlSerializer serializer = new XmlSerializer(typeof(World));

        WorldController.worldController.createWorld((World)serializer.Deserialize(stream));
        stream.Close();
    }
    void createNewWorld() {
        WorldController.worldController.createWorld(new World(25, 25));
    }

    /// Needs a callback for when non saved data should be loaded, like the TileSpriteController. ??
}