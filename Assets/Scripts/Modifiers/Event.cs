using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Event {
    /// <summary>
    /// Constructor to create a new event.
    /// </summary>
    /// <param name="modifiers">Array of all the modifiers the event has.</param>
    /// <param name="name">Name of the event</param>
    public Event(Dictionary<ModifierType, float> modifiers, string name, string type, int startDay, int duration) {
        this.modifiers = modifiers;
        this.name = name;
        this.type = type;
        this.startDay = startDay;
        this.duration = duration;
    }

    public readonly Dictionary<ModifierType, float> modifiers;

    public readonly int startDay;
    public readonly int duration;

    public readonly string name;

    // Type of event. E.g type_disaster.
    public readonly string type;
}
