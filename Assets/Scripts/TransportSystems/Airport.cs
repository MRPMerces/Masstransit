using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Airport : IXmlSerializable {

    public Airport(Player owner) {
        World.world.airportGraph.regenerateGraph();

        this.owner = owner;
    }

    public Player owner { get; protected set; }

    public int runways { get; protected set; }
    public int terminals { get; protected set; }

    public bool metroStation { get; protected set; }
    public bool trainStation { get; protected set; }

    public bool highSpeedtrainStation { get; protected set; }
    public bool highwayConnection { get; protected set; }

    public void update_owner(Player newOwner) {
        owner = newOwner;
    }

    public void add_metroStation() {
        if (metroStation) {
            Debug.LogError("Airport allready have a train station");
            return;
        }

        metroStation = true;
    }

    public void add_trainStation() {
        // If city has a train connection, then we can add it here as well for a price.
        if (!owner.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (trainStation) {
            Debug.LogError("Airport allready have a train station");
            return;
        }

        owner.constructionCost(0);

        trainStation = true;
    }

    public void add_highSpeedTrainStation() {
        if (!owner.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (highSpeedtrainStation) {
            Debug.LogError("Airport allready have a highspeed trainstation");
            return;
        }

        owner.constructionCost(0);

        highSpeedtrainStation = true;
    }

    public void add_highwayConnection() {
        // If city has a highway connection, then we can add it here as well for a price.
        if (!owner.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        if (highwayConnection) {
            Debug.LogError("Airport allready have a highwayconnection");
            return;
        }

        owner.constructionCost(0);

        highwayConnection = true;
    }

    public void add_runway() {
        if (!owner.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        owner.constructionCost(0);

        runways++;
    }

    public void add_terminal() {
        if (!owner.canAffordConstructionCost(0)) {
            Debug.LogError("Insufficient funds");
            return;
        }

        owner.constructionCost(0);

        terminals++;
    }

    public void mergeAirport(Airport airport) {
        runways += airport.runways;
        terminals += airport.terminals;

        if (airport.metroStation) {
            metroStation = true;
        }

        if (airport.trainStation) {
            trainStation = true;
        }

        if (airport.highwayConnection) {
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
