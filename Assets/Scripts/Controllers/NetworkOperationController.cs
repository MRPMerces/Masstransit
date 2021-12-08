using UnityEngine;

public class NetworkOperationController : MonoBehaviour {

    float time = 0.0f;

    float tick {
        get { return SpeedController.speed.worldTick; }
    }


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;

        if (time >= tick && tick != 0) {
            time -= tick;

            World.world.road.operation(1f);
            World.world.highway.operation(1f);
            World.world.lst.operation(1f);
            World.world.hst.operation(1f);
        }
    }
}
