using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportSpriteController : MonoBehaviour {
    public Sprite plane;

    Dictionary<Vehicle, GameObject> planes;
    Dictionary<CityPair, GameObject> lines;

    // Start is called before the first frame update
    void Start() {
        planes = new Dictionary<Vehicle, GameObject>();
        lines = new Dictionary<CityPair, GameObject>();

        SpeedController.speedController.RegisterHourTickCallback(changePlanePositions);
    }

    // Update is called once per frame
    void Update() {
        foreach (KeyValuePair<CityPair, Vehicle> keyValuePair in AirportController.airportController.cityPairs) {
            if (!planes.ContainsKey(keyValuePair.Value)) {
                GameObject gameObject = new GameObject("Plane");
                gameObject.transform.position = keyValuePair.Value.toVector3();
                gameObject.transform.SetParent(transform, true);

                SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Vehicles";
                spriteRenderer.sprite = plane;

                planes.Add(keyValuePair.Value, gameObject);
                addLine(keyValuePair.Key);
            }
        }
    }

    void changePlanePositions() {
        foreach (Vehicle vehicle in planes.Keys) {
            planes[vehicle].transform.position = vehicle.toVector3();
        }
    }

    void addLine(CityPair cityPair) {
        GameObject gameObject = new GameObject(cityPair.start.name + "-" + cityPair.end.name);
        //gameObject.transform.position = Vector3.zero;
        gameObject.transform.SetParent(transform, true);

        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.SetPosition(0, new Vector3(cityPair.start.X + 0.5f, cityPair.start.Y + 0.5f, 0));
        lineRenderer.SetPosition(1, new Vector3(cityPair.end.X + 0.5f, cityPair.end.Y + 0.5f, 0));

        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 1f;

        lineRenderer.sortingLayerName = "Infrastructure";

        lines.Add(cityPair, gameObject);
    }

    public void enableAirportOverlay(bool enable = true) {
        foreach (GameObject gameObject in lines.Values) {
            gameObject.SetActive(enable);
        }
    }
}
