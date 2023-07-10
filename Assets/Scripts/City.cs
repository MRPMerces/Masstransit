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

    public bool hasAirport;
    public bool hasMetro;
    public bool hasHighspeedTrainstation { get; protected set; }
    public bool hasLowspeedTrainstation { get; protected set; }
    public bool hasRoadConnection { get; protected set; }
    public bool hasHighwayConnection { get; protected set; }

    public void addNetworkConnection(NetworkType type, bool remove = false) {
        switch (type) {
            case NetworkType.ROAD:
                if (remove) {
                    hasRoadConnection = false;
                }

                else {
                    hasRoadConnection = true;
                }

                return;

            case NetworkType.HIGHWAY:
                if (remove) {
                    hasHighwayConnection = false;
                }

                else {
                    hasHighwayConnection = true;
                }

                return;

            case NetworkType.LST:
                if (remove) {
                    hasLowspeedTrainstation = false;
                }

                else {
                    hasLowspeedTrainstation = true;
                }

                return;

            case NetworkType.HST:
                if (remove) {
                    hasHighspeedTrainstation = false;
                }

                else {
                    hasHighspeedTrainstation = true;
                }

                return;

            default:
                Debug.LogError(type + " Unrecognised type");
                return;
        }
    }

    public void addPopulation(int population) {
        this.population += population;
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

        if (!reader.IsEmptyElement) {
            Debug.Log("hei");
            reader.ReadEndElement();
        }
    }

    #endregion Saving and loading
}
