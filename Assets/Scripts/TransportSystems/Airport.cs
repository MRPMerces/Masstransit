using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Airport : IXmlSerializable  {

    public Airport(Player owner) {
        World.world.airportGraph.regenerateGraph();

        this.owner = owner;
    }

    public Player owner { get; protected set; }

    public int runways { get; protected set; }
    public int terminals { get; protected set; }

    public bool metroStation { get; protected set; }
    public bool trainStation { get; protected set; }
    public bool highwayConnection { get; protected set; }

    public void update_owner(Player newOwner) {
        owner = newOwner;
    }

    public void add_metroStation() { }

    public void add_trainStation() { }

    public void add_highwayConnection() {
        if (!owner.canAfford(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }
        if (highwayConnection) {
            Debug.LogError("Airport allready have a highwayconnection");
            return;
        }
        highwayConnection = true;
    }

    public void add_runway() {
        if (!owner.canAfford(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        runways++;
    }

    public void add_terminal() {
        if (!owner.canAfford(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        terminals++;
    }

    public void mergeAirport(Airport airport) {
        runways += airport.runways;
        terminals += airport.terminals;

        if(!metroStation && airport.metroStation) {
            metroStation = true;
        }

        if (!trainStation && airport.trainStation) {
            trainStation = true;
        }

        if (!highwayConnection && airport.highwayConnection) {
            highwayConnection = true;
        }
    }

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Airport() {
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        
    }

    public void ReadXml(XmlReader reader) {
    }

    #endregion Saving and loading
}
