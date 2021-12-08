using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Era : IXmlSerializable {

    public Era(int era) {
        this.era = era;
        setupBools();
        processEra();
    }

    public int era { get; protected set; }

    public bool metro_unlocked { get; protected set; }
    public bool airport_unlocked { get; protected set; }
    public bool highways_unlocked { get; protected set; }
    public bool highSpeedTrains_unlocked { get; protected set; }

    // Progress era by 1 and update tech
    public void progress_era() {
        if (era == 5) {
            Debug.LogError("You are in the last era!");
            return;
        }

        era++;
        processEra();
    }

    void processEra() {
        if (era > 1) {
            metro_unlocked = true;
        }

        if (era > 2) {
            airport_unlocked = true;
        }

        if (era > 3) {
            highways_unlocked = true;
        }

        if (era > 4) {
            highSpeedTrains_unlocked = true;
        }
    }

    void setupBools() {
        metro_unlocked = false;
        airport_unlocked = false;
        highways_unlocked = false;
        highSpeedTrains_unlocked = false;
    }


    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Era() { }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Era", era.ToString());
    }

    public void ReadXml(XmlReader reader) {
        era = int.Parse(reader.GetAttribute("Era"));
        setupBools();
        processEra();
    }

    #endregion Saving and loading
}

// Era 1 = 1820