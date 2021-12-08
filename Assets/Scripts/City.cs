using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class City : IXmlSerializable {
    public City(string name, int population) {
        this.name = name;
        this.population = population;

        metros = new List<Metro>();
        airports = new List<Airport>();
        usedLines = new List<int>();

        hasMetro = false;
        hasAirport = false;
    }

    public List<Metro> metros { get; protected set; }

    public List<Airport> airports { get; protected set; }

    public List<int> usedLines { get; protected set; }

    public string name { get; protected set; }

    public int population { get; protected set; }
    public int numberOfTrainStations { get; protected set; }

    public bool hasAirport { get; protected set; }
    public bool hasMetro;
    public bool hasHighspeedTrainstation { get; protected set; }
    public bool hasLowspeedTrainstation { get; protected set; }
    public bool hasRoadConnection { get; protected set; }
    public bool hasHighwayConnection { get; protected set; }

    public bool hasPlayerAAirport(Player player) {
        if (player == null) {
            Debug.LogError("-City.hasPlayerAAirport- Player is null");
            return false;
        }

        foreach (Airport airport in airports) {
            if (airport.owner == player) {
                return true;
            }
        }

        return false;
    }

    public Airport get_airportByPlayer(Player player) {
        if (!hasAirport) {
            Debug.LogError("-City.get_airportByPlayer- City does not have a airport");
            return null;
        }

        foreach (Airport airport in airports) {
            if (airport.owner == player) {
                return airport;
            }
        }

        Debug.LogError("-City.get_airportByPlayer- Cant find a airport with owner player");
        return null;
    }

    public void buildAirport(Player owner) {
        if (hasPlayerAAirport(owner)) {
            Debug.LogError("Player allready have a airport!");
            return;
        }

        airports.Add(new Airport(owner));
        airports[airports.Count - 1].add_runway();
        airports[airports.Count - 1].add_terminal();
        hasAirport = true;
    }

    /// <summary>
    /// Function to merge two airports
    /// </summary>
    /// <param name="player1">Owner of the final airport</param>
    /// <param name="player2">Airport to be merged with player 1</param>
    public void mergeAirports(Player player1, Player player2) {

        // If player 1 does not have an airport in this city. Just update the ownership of player 2's airport.
        if (!hasPlayerAAirport(player1)) {
            get_airportByPlayer(player2).update_owner(player1);
        }

        // Else we need to merge them.
        else {
            get_airportByPlayer(player1).mergeAirport(get_airportByPlayer(player2));
            airports.Remove(get_airportByPlayer(player2));
        }
    }

    public void addNetworkConnection(NetworkType type) {
        switch (type) {
            case NetworkType.Road:
                if (!hasRoadConnection) {
                    hasRoadConnection = true;
                }

                break;

            case NetworkType.Highway:
                if (!hasHighwayConnection) {
                    hasHighwayConnection = true;
                }

                break;

            case NetworkType.LST:
                if (!hasLowspeedTrainstation) {
                    hasLowspeedTrainstation = true;
                }

                break;

            case NetworkType.HST:
                if (!hasHighspeedTrainstation) {
                    hasHighspeedTrainstation = true;
                }

                break;

            default:
                Debug.LogError(type + " Unrecognised type");
                return;
        }
    }

    public void update_population(int pop) {
        population += pop;
    }

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private City() { }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Name", name);
        writer.WriteAttributeString("Population", population.ToString());

        if (hasMetro) {
            writer.WriteStartElement("Metroes");

            foreach (Metro metro in metros) {
                writer.WriteStartElement("Metro");
                metro.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        if (hasAirport) {
            writer.WriteStartElement("Airports");

            foreach (Airport airport in airports) {
                writer.WriteStartElement("Airport");
                airport.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }

    public void ReadXml(XmlReader reader) {
        name = reader.GetAttribute("Name");
        population = int.Parse(reader.GetAttribute("Population"));

        // We are in the "Metroes" element, so read elements until we run out of "Metro" nodes.
        if (reader.ReadToDescendant("Metro")) {
            // We have at least one metro, so do something with it.

            hasMetro = true;
            XmlSerializer serializer = new XmlSerializer(typeof(Metro));

            do {
                metros.Add((Metro)serializer.Deserialize(reader));
                metros[metros.Count - 1].ReadXml(reader);

            } while (reader.ReadToNextSibling("Metro"));
        }
    }

    #endregion Saving and loading
}
