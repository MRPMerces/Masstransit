﻿using UnityEngine;

public class PopulationController : MonoBehaviour {

    MetroController metroController {
        get { return MetroController.metroController; }
    }

    private float time = 0.0f;

    float tick {
        get { return SpeedController.speed.worldTick; }
    }

    int findPopulationRoof(City city) {
        int roof = 75000;

        if (city.hasMetro)
            roof += metroController.get_totalMetroCathment(city) * 2;

        // 2 * numberOfTrainStations * 15000.
        roof += city.numberOfTrainStations * 30000;

        if (city.hasHighspeedTrainstation)
            roof += 100000;

        if (city.hasAirport) {
            /// for now
            roof += 100000;
        }

        return roof;
    }

    int growthThisTick(City city) {
        float growth = 0f;
        int roof = findPopulationRoof(city);

        // Positive.

        // 1 % annually (birth rate).
        growth += city.population / 36500;

        growth += World.world.tech.era * 2;

        if (city.hasHighspeedTrainstation)
            growth += 50;

        if (city.hasMetro)
            growth += metroController.get_totalMetroCathment(city) / 2000;

        // Negative.

        // 1 % of pop above roof.
        if (city.population > roof)
            growth -= roof - city.population / 100;

        return (int)growth;
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;

        if (time >= tick && tick != 0) {
            time -= tick;

            foreach (Tile T in World.world.tilesWithCity) {
                T.city.update_population(growthThisTick(T.city));
            }
        }
    }
}

/*
 * increase
 *      era change
 *      each transit type has a population roof
 *      new transit type
 *      new transit expansion
 *         
 *      metro will grow towards twice the cathment
 */
