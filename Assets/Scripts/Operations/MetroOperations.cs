using UnityEngine;

public class MetroOperations : MonoBehaviour {

    private void Start() {
        SpeedController.speedController.RegisterDayTickCallback(metro_Operations);
    }

    public void metro_Operations() {

        /// Capacity should be city wide.
        foreach (Tile tile in World.world.tilesWithCity) {
            City city = tile.city;
            if (city.hasMetro) {
                foreach (Metro metro in city.metros) {
                    // calculate capacity
                    int capacity = metro.get_amountOfMetrolinesInSystem() * 100000;

                    // calculate usage
                    float usage;

                    float ridershipMod = metro.owner.modifiers.globalModifiers[ModifierType.Ridership];

                    int mod = modifiers();

                    // Below capacity and great enoth population
                    if (metro.catchment * ridershipMod < capacity && metro.catchment < city.population)
                        usage = (metro.catchment / mod) * ridershipMod;

                    // Above capacity and great enoth population
                    else if ((metro.catchment / mod) * ridershipMod > capacity && metro.catchment < city.population)
                        usage = capacity;

                    // To small population
                    else

                        /// Needs catchment.
                        usage = (city.population / mod) * ridershipMod;
                    // Each rider pays 1 money each per gametick
                    metro.owner.opereatingIncome(usage);
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
