using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSpriteController : MonoBehaviour {
    public Sprite LS_Train;
    public Sprite HS_Train;

    /// Prefab?

    Dictionary<Vehicle, GameObject> LS_Trains;
    Dictionary<Vehicle, GameObject> HS_Trains;

    // Start is called before the first frame update
    void Start() {
        LS_Trains = new Dictionary<Vehicle, GameObject>();
        HS_Trains = new Dictionary<Vehicle, GameObject>();

        SpeedController.speedController.RegisterHourTickCallback(changeVehiclePositions);
    }

    // Update is called once per frame
    void Update() {
        foreach (Network network in World.world.lst.networks) {
            foreach (KeyValuePair<CityPair, Vehicle> keyValuePair in network.vehicles) {
                if (!LS_Trains.ContainsKey(keyValuePair.Value)) {
                    GameObject gameObject = new GameObject("Test");
                    gameObject.transform.position = keyValuePair.Value.toVector3();
                    gameObject.transform.SetParent(transform, true);

                    SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sortingLayerName = "Vehicles";
                    spriteRenderer.sprite = LS_Train;

                    LS_Trains.Add(keyValuePair.Value, gameObject);
                }
            }
        }
    }

    void changeVehiclePositions() {
        foreach (Vehicle vehicle in LS_Trains.Keys) {
            LS_Trains[vehicle].transform.position = vehicle.toVector3();
        }
    }
}
