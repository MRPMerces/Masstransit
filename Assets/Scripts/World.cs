using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class World : IXmlSerializable {
    public World(int Width, int Height) {
        this.Width = Width;
        this.Height = Height;
        world = this;

        tiles = new Tile[Width, Height];

        // Populate Tile array with (Width * Height) tiles
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                tiles[x, y] = new Tile(x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
                tiles[x, y].RegisterTileInfrastructureChanged(OnTileInfrastructureChanged);
            }
        }

        Debug.Log("World created with " + (Width * Height) + " tiles.");
        tilesWithCity = new List<Tile>();

        tech = new Era(5);

        tileGraph = new Path_TileGraph();
        airportGraph = new AirportGraph();

        road = new NetworkManager(NetworkType.ROAD, 5000, 1000);
        highway = new NetworkManager(NetworkType.HIGHWAY, 10000, 7500);
        lst = new NetworkManager(NetworkType.LST, 30000, 15000);
        hst = new NetworkManager(NetworkType.HST, 100000, 100000);

        playerController = new PlayerController();
    }

    public static World world { get; protected set; }

    // The pathfinding graph used to navigate our world map.
    public Path_TileGraph tileGraph;
    public AirportGraph airportGraph;

    public NetworkManager road;
    public NetworkManager highway;
    public NetworkManager lst;
    public NetworkManager hst;

    public PlayerController playerController;

    public Era tech;

    // A two-dimensional array to hold our tiles.
    public Tile[,] tiles { get; protected set; }

    // All the tiles with cities in them.
    public List<Tile> tilesWithCity { get; protected set; }

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    // Add a tile to the list of tiles with cities.
    public void add_cityToList(Tile tile) {
        tilesWithCity.Add(tile);
    }

    public Tile getTileAt(int x, int y) {
        return (0 > x || x >= Width || Height <= y || y < 0) ? null : tiles[x, y];
    }

    /// <summary>
    /// Gets the tile at the unity-space coordinates
    /// </summary>
    /// <returns>The tile at world coordinate.</returns>
    /// <param name="vector3">Unity World-Space coordinates.</param>
    public Tile getTileAt(Vector3 vector3) {
        return getTileAt(Mathf.FloorToInt(vector3.x + 0.5f), Mathf.FloorToInt(vector3.y + 0.5f));
    }

    /// <summary>
    /// Gets the tile at the unity-space coordinates
    /// </summary>
    /// <returns>The tile at world coordinate.</returns>
    /// <param name="vector2">Tile coordinates.</param>
    public Tile getTileAt(Vector2 vector2) {
        return getTileAt(Mathf.FloorToInt(vector2.x + 0.5f), Mathf.FloorToInt(vector2.y + 0.5f));
    }

    // Create new stats for a new city
    public (string, int) newCityStats() {
        return ("city" + tilesWithCity.Count.ToString(), 100 * UnityEngine.Random.Range(1, 250));
    }

    #region Callbacks

    Action<Tile> cbTileChanged;
    Action<Tile, NetworkType, Player> cbTileInfrastructureChanged;

    public void RegisterTileChanged(Action<Tile> callbackfunc) {
        cbTileChanged += callbackfunc;
    }

    public void UnregisterTileChanged(Action<Tile> callbackfunc) {
        cbTileChanged -= callbackfunc;
    }

    public void RegisterTileInfrastructureChanged(Action<Tile, NetworkType, Player> callbackfunc) {
        cbTileInfrastructureChanged += callbackfunc;
    }

    public void UngisterTileInfrastructureChanged(Action<Tile, NetworkType, Player> callbackfunc) {
        cbTileInfrastructureChanged -= callbackfunc;
    }

    // Gets called whenever ANY tile changes
    void OnTileChanged(Tile t) {
        if (cbTileChanged == null) {
            return;
        }

        cbTileChanged(t);
    }

    void OnTileInfrastructureChanged(Tile tile, NetworkType type, Player player) {
        cbTileInfrastructureChanged?.Invoke(tile, type, player);
    }

    #endregion Callbacks

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private World() {
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Height.ToString());

        writer.WriteStartElement("Tiles");
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader) {

        Width = int.Parse(reader.GetAttribute("Width"));
        Height = int.Parse(reader.GetAttribute("Height"));

        //setupWorld(Width, Height);

        // We are in the "Tiles" element, so read elements until we run out of "Tile" nodes.
        if (reader.ReadToDescendant("Tile")) {
            // We have at least one tile, so do something with it.

            do {
                //Debug.Log("Name: " + reader.Name);
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));

                XmlSerializer serializer = new XmlSerializer(typeof(Tile));

                tiles[x, y] = (Tile)serializer.Deserialize(reader);

                Debug.Log(reader.NodeType + "Name: " + reader.Name + "value: " + reader.Value);

            } while (reader.ReadToNextSibling("Tile"));
        }
    }

    #endregion Saving and loading
}

// Struct to temp store name and population
//public struct CityStats {

//    public CityStats(string n, int p) {
//        name = n;
//        population = p;
//    }

//    public string name { get; }
//    public int population { get; }
//}