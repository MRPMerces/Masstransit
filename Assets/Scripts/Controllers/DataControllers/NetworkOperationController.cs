using UnityEngine;

public class NetworkOperationController : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        SpeedController.speedController.RegisterHourTickCallback(operations);
    }

    void operations() {
        World.world.road.operation(1f);
        World.world.highway.operation(1f);
        World.world.lst.operation(1f);
        World.world.hst.operation(1f);
    }
}
