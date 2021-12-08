using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Metroline : IXmlSerializable {

    public Metroline(int ln) {
        lineNumber = ln;
        stations = new List<Metrostation>();
    }

    public List<Metrostation> stations { get; protected set; }

    public int lineNumber { get; protected set; }

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Metroline() {
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("LineNumber", lineNumber.ToString());

        writer.WriteStartElement("Stations");
        foreach (Metrostation station in stations) {
            writer.WriteStartElement("Station");
            station.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader) {
        lineNumber = int.Parse(reader.GetAttribute("LineNumber"));

        while (reader.Read()) {

            // We are in the "Stations" element, so read elements until we run out of "Station" nodes.
            if (reader.ReadToDescendant("Station")) {
                // We have at least one station, so do something with it.

                do {
                    // move outside loop?
                    XmlSerializer serializer = new XmlSerializer(typeof(Metrostation));

                    stations.Add((Metrostation)serializer.Deserialize(reader));
                    stations[stations.Count - 1].ReadXml(reader);
                } while (reader.ReadToNextSibling("Station"));
            }
        }
    }

    #endregion Saving and loading
}
