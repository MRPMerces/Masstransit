using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Metro : IXmlSerializable {
    public Metro(Player owner) {
        lines = new List<Metroline>();

        this.owner = owner;
    }

    public Player owner { get; protected set; }

    public List<Metroline> lines { get; protected set; }

    public int catchment { get; protected set; }

    public int get_amountOfMetrostationsInSystem() {
        int stations = 0;
        foreach (Metroline ML in lines) {
            stations += ML.stations.Count;
        }

        return stations;
    }

    public int get_amountOfMetrolinesInSystem() {

        return lines.Count;
    }

    public void add_metroline(int line) {
        lines.Add(new Metroline(line));

        // Add 2 stations to the newly created line
        add_metrostationToLine(line);
        add_metrostationToLine(line);
    }

    public void add_metrostationToLine(int lineNumber) {

        if (!owner.canAffordConstructionCost(30000)) {
            Debug.LogError("Insufficient funds!");
            return;
        }

        Metroline line = get_metroline(lineNumber);

        if (line == null) {
            Debug.LogError("Line: " + lineNumber + " does not exist in this system");
            return;
        }

        line.stations.Add(new Metrostation(lineNumber, line.stations.Count));
        owner.constructionCost(30000);
        updateCatchment();
    }

    public Metroline get_metroline(int ln) {
        /// list_metrolines.find();
        foreach (Metroline M in lines)
            if (M.lineNumber == ln)
                return M;

        Debug.LogError("Line: " + ln + " does not exist!");
        return null;
    }

    public void updateCatchment() {
        catchment = get_amountOfMetrostationsInSystem() * 10000;
    }

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Metro() { }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Owner", owner.name);

        writer.WriteStartElement("Lines");
        foreach (Metroline line in lines) {
            writer.WriteStartElement("Line");
            line.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader) {

        owner = World.world.playerController.getPlayerByName(reader.GetAttribute("Owner"));

        // We have allready set the owner.
        while (reader.Read()) {

            // We are in the "Lines" element, so read elements until we run out of "Line" nodes.
            if (reader.ReadToDescendant("Line")) {
                // We have at least one metroline, so do something with it.

                XmlSerializer serializer = new XmlSerializer(typeof(Metroline));

                do {
                    lines.Add((Metroline)serializer.Deserialize(reader));
                    lines[lines.Count - 1].ReadXml(reader);

                } while (reader.ReadToNextSibling("Line"));
            }
        }

        updateCatchment();
    }

    #endregion Saving and loading
}
