using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public enum TileType { Empty, City, River };

public class Tile : IXmlSerializable {
    public Tile(int x, int y) {
        X = x;
        Y = y;

        road = new List<Player>();
        highway = new List<Player>();
        lst = new List<Player>();
        hst = new List<Player>();
    }

    public City city { get; protected set; }

    //Infrastructure
    public List<Player> road { get; protected set; }
    public List<Player> highway { get; protected set; }
    public List<Player> lst { get; protected set; }
    public List<Player> hst { get; protected set; }


    public TileType type { get; protected set; }

    public bool isCity;

    public bool isRiver;

    public readonly int X;
    public readonly int Y;

    public void setType(TileType type) {
        if (this.type == type) {
            return;
        }

        if (this.type == TileType.City && type != TileType.City) {
            Debug.LogError("Trying to remove a city_tile, can we do this?");
            return;
        }

        this.type = type;

        if (type == TileType.River) {
            isRiver = true;
        }

        // Call the callback and let things know we've changed.
        cbTileChanged?.Invoke(this);
    }

    public void createCity() {
        setType(TileType.City);

        CityStats cityStats = World.world.newCityStats();
        city = new City(cityStats.name, cityStats.population);
        isCity = true;

        World.world.add_cityToList(this);
    }

    public void add_infrastructureOwner(NetworkType type, Player owner) {
        switch (type) {
            case NetworkType.Road:
                if (!road.Contains(owner)) {
                    road.Add(owner);
                    bleh(owner, type);
                }
                return;

            case NetworkType.Highway:
                if (!highway.Contains(owner)) {
                    highway.Add(owner);
                    bleh(owner, type);
                }
                return;

            case NetworkType.LST:
                if (!lst.Contains(owner)) {
                    lst.Add(owner);
                    bleh(owner, type);
                }
                return;

            case NetworkType.HST:
                if (!hst.Contains(owner)) {
                    hst.Add(owner);
                    bleh(owner, type);
                }
                return;

            default:
                Debug.LogError(type + " Unrecognised type");
                return;
        }
    }

    /// Rename
    void bleh(Player player, NetworkType networkType) {
        cbTileInfrastructureChanged(this, networkType, player);

        foreach (Tile tile in getNeighbours()) {
            if (tile != null && tile.hasPlayerInfrastructure(networkType, player)) {
                tile.cbTileInfrastructureChanged(tile, networkType, player);
            }
        }
    }
    public void update_infrastructureOwner(NetworkType networkType, Player oldOwner, Player newOwner) {
        switch (networkType) {
            case NetworkType.Road:
                road.Remove(oldOwner);
                road.Add(newOwner);
                break;

            case NetworkType.Highway:
                highway.Remove(oldOwner);
                highway.Add(newOwner);
                break;

            case NetworkType.LST:
                lst.Remove(oldOwner);
                lst.Add(newOwner);
                break;

            case NetworkType.HST:
                hst.Remove(oldOwner);
                hst.Add(newOwner);
                break;

            default:
                Debug.LogError(type + " Unrecognised type");
                return;
        }
    }

    public bool hasPlayerInfrastructure(NetworkType networkType, Player owner) {
        switch (networkType) {
            case NetworkType.Road:
                return road.Contains(owner);

            case NetworkType.Highway:
                return highway.Contains(owner);

            case NetworkType.LST:
                return lst.Contains(owner);

            case NetworkType.HST:
                return hst.Contains(owner);
            default:
                Debug.LogError(networkType + " Unrecognised type");
                return false;
        }
    }

    public bool hasACityNeighbour() {
        foreach (Tile tile in getNeighbours(true)) {
            if (tile != null && tile.isCity) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Function that finds the neighbours
    /// </summary>
    /// <param name="diagOkay">Do the caller want the diagonale neighbours in the array</param>
    /// <returns>A array of neighbouring tiles NOTE! some tiles migth be null
    /// </returns>
    public Tile[] getNeighbours(bool diagOkay = false) {
        Tile[] tiles;

        if (diagOkay == false) {
            tiles = new Tile[4];   // Tile order: N E S W
        }
        else {
            tiles = new Tile[8];   // Tile order : N E S W NE SE SW NW
        }

        tiles[0] = World.world.getTileAt(X, Y + 1);
        tiles[1] = World.world.getTileAt(X + 1, Y);
        tiles[2] = World.world.getTileAt(X, Y - 1);
        tiles[3] = World.world.getTileAt(X - 1, Y);

        if (diagOkay == true) {
            tiles[4] = World.world.getTileAt(X + 1, Y + 1);
            tiles[5] = World.world.getTileAt(X + 1, Y - 1);
            tiles[6] = World.world.getTileAt(X - 1, Y - 1);
            tiles[7] = World.world.getTileAt(X - 1, Y + 1);
        }

        return tiles;
    }

    public Tile North {
        get {
            return World.world.getTileAt(X, Y + 1);
        }
    }

    public Tile South {
        get {
            return World.world.getTileAt(X, Y - 1);
        }
    }

    public Tile East {
        get {
            return World.world.getTileAt(X + 1, Y);
        }
    }

    public Tile West {
        get {
            return World.world.getTileAt(X - 1, Y);
        }
    }

    public Vector3 toVector3() {
        return new Vector3(X, Y, 0);
    }

    public Vector2 toVector2() {
        return new Vector2(X, Y);
    }

    public string name {
        get {
            return "Tile_" + X + "_" + Y;
        }
    }

    #region Callbacks

    Action<Tile> cbTileChanged;
    Action<Tile, NetworkType, Player> cbTileInfrastructureChanged;

    /// <summary>
    /// Register a function to be called back when our tile type changes.
    /// </summary>
    public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback) {
        cbTileChanged -= callback;
    }

    public void RegisterTileInfrastructureChanged(Action<Tile, NetworkType, Player> callbackfunc) {
        cbTileInfrastructureChanged += callbackfunc;
    }

    public void UngisterTileInfrastructureChanged(Action<Tile, NetworkType, Player> callbackfunc) {
        cbTileInfrastructureChanged -= callbackfunc;
    }

    #endregion Callbacks

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Tile() { }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteAttributeString("Type", ((int)type).ToString());

        if (isCity) {
            writer.WriteStartElement("City");
            city.WriteXml(writer);
            writer.WriteEndElement();
        }

        if (road.Count > 0) {
            writer.WriteStartElement("Road");
            foreach (Player player in road) {
                writer.WriteAttributeString("Owner", player.name);
            }
            writer.WriteEndElement();
        }

        if (highway.Count > 0) {
            writer.WriteStartElement("Highway");
            foreach (Player player in highway) {
                writer.WriteAttributeString("Owner", player.name);
            }
            writer.WriteEndElement();
        }

        if (lst.Count > 0) {
            writer.WriteStartElement("Lst");
            foreach (Player player in lst) {
                writer.WriteAttributeString("Owner", player.name);
            }
            writer.WriteEndElement();
        }

        if (hst.Count > 0) {
            writer.WriteStartElement("Hst");
            foreach (Player player in hst) {
                writer.WriteAttributeString("Owner", player.name);
            }
            writer.WriteEndElement();
        }
    }

    public void ReadXml(XmlReader reader) {
        Debug.Log("hei");
        setType((TileType)int.Parse(reader.GetAttribute("Type")));

        if (reader.ReadToDescendant("City")) {
            isCity = true;
            World.world.add_cityToList(this);

            XmlSerializer serializer = new XmlSerializer(typeof(City));

            // The tile is a city tile so create a new city.
            city = (City)serializer.Deserialize(reader);
        }

        Debug.Log("Name: " + reader.Name);

        if (reader.ReadToDescendant("Road")) {
            // We have at least one road owner, so do something with it.

            do {
                Debug.Log("has road");
                add_infrastructureOwner(NetworkType.Road, World.world.playerController.getPlayerByName(reader.GetAttribute("Owner")));
            } while (reader.ReadToNextSibling("Road"));
        }

        if (reader.ReadToDescendant("Highway")) {
            // We have at least one highway owner, so do something with it.

            do {
                add_infrastructureOwner(NetworkType.Highway, World.world.playerController.getPlayerByName(reader.GetAttribute("Owner")));
            } while (reader.ReadToNextSibling("Highway"));
        }

        if (reader.ReadToDescendant("Lst")) {
            // We have at least one lst owner, so do something with it.

            do {
                add_infrastructureOwner(NetworkType.LST, World.world.playerController.getPlayerByName(reader.GetAttribute("Owner")));
            } while (reader.ReadToNextSibling("Lst"));
        }

        if (reader.ReadToDescendant("Hst")) {
            // We have at least one hst owner, so do something with it.

            do {
                add_infrastructureOwner(NetworkType.HST, World.world.playerController.getPlayerByName(reader.GetAttribute("Owner")));
            } while (reader.ReadToNextSibling("Hst"));
        }

        if (!reader.IsEmptyElement) {
            Debug.Log("hei");
            reader.ReadEndElement();
        }
    }

    #endregion Saving and loading
}
