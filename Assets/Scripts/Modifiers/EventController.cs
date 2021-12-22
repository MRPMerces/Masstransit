using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {
    public static EventController eventController { get; protected set; }

    List<Event> activeEvents;
    List<Event> disaster_events;
    List<Event> events;
    Event disasterEvent;

    // Start is called before the first frame update
    void Start() {
        activeEvents = new List<Event>();
        disaster_events = new List<Event>();
        eventController = this;

        createEvents();
    }

    // Update is called once per frame
    void Update() {
        disaster();
    }

    Modifier[] getEventModifiers() {
        List<Modifier> modifiers = new List<Modifier>();
        foreach (Event E in activeEvents) {
            modifiers.AddRange(E.modifiers);
        }
        return modifiers.ToArray();
    }

    /// <summary>
    /// Updates everything that needs to be updated if a event is added
    /// </summary>
    void eventAdded() {
        World.world.playerController.update_playerModifiers(getEventModifiers());
    }

    void disaster() {

        bool disaster = false;
        if (disaster) {
            activeEvents.Add(disaster_events[UnityEngine.Random.Range(0, disaster_events.Count)]);
            eventAdded();
        }
    }

    void createEvents() {
        Modifier[] modifiers;

        // Pandemic
        modifiers = new Modifier[2];
        modifiers[0] = new Modifier(ModifierType.ConstructionCost, 0.5f);
        modifiers[1] = new Modifier(ModifierType.Ridership, 0.1f);
        /// hstrain = 0
        /// lstrain = 0.1
        /// road = 0.3
        disaster_events.Add(new Event(modifiers, "event_pandemic", "event_disaster", SpeedController.speedController.currentDay, (int)(1825 / World.world.tech.era)));

        // Era change
        modifiers = new Modifier[2];
        modifiers[0] = new Modifier(ModifierType.PopulationGrowth, 2f);
        modifiers[1] = new Modifier(ModifierType.Ridership, 0.5f);
        /// hstrain = 0
        /// lstrain = 0.1
        /// road = 0.3
        disaster_events.Add(new Event(modifiers, "event_eraChange", "event_trigger", SpeedController.speedController.currentDay, 365));
    }




    //void loadEventsFromXml(XmlReader reader) {

    //}
}
