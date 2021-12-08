using UnityEngine;

public class MetroOperations : MonoBehaviour {

    float tick {
        get { return SpeedController.speed.worldTick; }
    }

    private float time = 0.0f;

    void Update() {
        time += Time.deltaTime;

        if (time >= tick && tick != 0) {
            time -= tick;
            metro_Operations();
        }
    }

    public void metro_Operations() {

        /// Capacity should be city wide.
        foreach (Tile T in World.world.tilesWithCity) {
            City C = T.city;
            if (C.hasMetro) {
                foreach (Metro metro in C.metros) {
                    // calculate capacity
                    int capacity = metro.get_amountOfMetrolinesInSystem() * 100000;

                    // calculate usage
                    float usage;

                    float ridershipMod = metro.owner.modifiers.ridership.value;

                    int mod = modifiers();

                    // Below capacity and great enoth population
                    if (metro.catchment * ridershipMod < capacity && metro.catchment < C.population)
                        usage = (metro.catchment / mod) * ridershipMod;

                    // Above capacity and great enoth population
                    else if ((metro.catchment / mod) * ridershipMod > capacity && metro.catchment < C.population)
                        usage = capacity;

                    // To small population
                    else

                        /// Needs catchment.
                        usage = (C.population / mod) * ridershipMod;
                    // Each rider pays 1 money each per gametick
                    metro.owner.opereatingIncome((int)usage);
                }
            }
        }
    }

    int modifiers() {
        int modifier = 0;

        // eraMod.
        modifier += 4;

        if (modifier != 0)
            return modifier;
        else
            return 1;
    }
}
