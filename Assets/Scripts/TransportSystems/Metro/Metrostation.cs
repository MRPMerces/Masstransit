using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Metrostation : IXmlSerializable {

    public Metrostation(int lineNumber, int stationNumber) {
        this.lineNumber = lineNumber;
        this.stationNumber = stationNumber;
        name = "L" + lineNumber.ToString() + "_S" + stationNumber.ToString();
    }

    public string name { get; protected set; }

    public int lineNumber { get; protected set; }
    public int stationNumber { get; protected set; }

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Metrostation() {
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("LineNumber", lineNumber.ToString());
        writer.WriteAttributeString("StationNumber", stationNumber.ToString());
    }

    public void ReadXml(XmlReader reader) {
        lineNumber = int.Parse(reader.GetAttribute("LineNumber"));
        stationNumber = int.Parse(reader.GetAttribute("StationNumber"));
        name = "L" + reader.GetAttribute("LineNumber") + "_S" + reader.GetAttribute("StationNumber");
    }

    #endregion Saving and loading
}
