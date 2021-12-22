/*
public class RoadController : MonoBehaviour {

    public static RoadController road { get; protected set; }

    World world {
        get { return WorldController.worldController.world; }
    }

    float tick {
        get { return SpeedController.speed.WorldTick; }
    }

    List<roadNetwork> roadNetworks;
    // List<GameObject> lineEdges;

    private float time = 0.0f;

    void Start() {
        road = this;
        roadNetworks = new List<roadNetwork>();
        // lineEdges = new List<GameObject>();
    }

    void Update() {

        time += Time.deltaTime;

        if (time >= tick && tick != 0) {
            time -= tick;

            //Road_operation();
        }
    }

    public void buildRoad(Tile t1, Tile t2, Player owner) {

        // Check if t1 exist
        if (t1 == null) {
            Debug.LogError("-Road.buildRoad- t1  is null");
            return;
        }

        // Check if t2 exist
        if (t2 == null) {
            Debug.LogError("-Road.buildRoad- t2 is null");
            return;
        }

        // Check if the two tiles have an edge
        if (doesCitiesHaveRoadConnection(t1, t2))
            return;


        // True distance between the two tiles
        float distance = Mathf.Sqrt(Mathf.Pow(t1.X - t2.X, 2) + Mathf.Pow(t1.Y - t2.Y, 2));

        // Check if player can afford the cost
        if (owner.money < 2000 + (int)distance * 5000) {
            Debug.LogError("Insufficient funds!");
            return;
        }

        // Cost of 2K for the city conections, + 5K per tile distance
        owner.constructionCost(2000 + (int)(distance * 5000));


        roadNetwork network1 = null;
        roadNetwork network2 = null;

        // Check if one the tiles are in road networks
        foreach (roadNetwork RN in roadNetworks) {

            // Tile t1 is in a network
            if (RN.nodes.ContainsKey(t1))
                network1 = RN;

            // Tile t2 is in a network
            if (RN.nodes.ContainsKey(t2))
                network2 = RN;

            // Both tiles are in the same network
            if (RN.nodes.ContainsKey(t1) && RN.nodes.ContainsKey(t2)) {
                Debug.Log("-Road.buildRoad- Both tiles are in the same network");

                // Add edge between t1 and t2
                RN.addEdge(t1, t2);
                drawGraph(t1, t2);
                return;
            }
        }

        // Only tile t1 is in a network
        if (network1 != null && network2 == null) {
            network1.add_city(t2);

            // Add edge between t1 and t2
            network1.addEdge(t1, t2);
            drawGraph(t1, t2);
            return;
        }

        // Only tile t2 is in a network
        if (network2 != null && network1 == null) {
            network2.add_city(t1);

            // Add edge between t1 and t2
            network2.addEdge(t1, t2);
            drawGraph(t1, t2);
            return;
        }

        // Both t1 and t2 are in networks
        if (network2 != null && network1 != null) {
            mergeNetworks(network1, network2).addEdge(t1, t2);
            drawGraph(t1, t2);
            return;
        }

        // Non of the tiles are in a network
        roadNetwork newNetwork = createNewNetwork(owner);
        newNetwork.add_city(t1);
        newNetwork.add_city(t2);
        newNetwork.addEdge(t1, t2);

        // Draw the graph
        ///if(drawgrapgisenabled){
        drawGraph(t1, t2);
        ///}
        ///
    }

    roadNetwork createNewNetwork(Player owner) {

        int highestId = 0;

        /// Foreach
        // Find highest network id
        for (int i = 0; i < roadNetworks.Count; i++)
            if (roadNetworks[i].id > highestId)
                highestId = roadNetworks[i].id;

        roadNetwork newNetwork = new roadNetwork(highestId + 1, owner);
        roadNetworks.Add(newNetwork);

        return newNetwork;
    }

   /// <summary>
   /// Merges two roadNetworks.
   /// </summary>
   /// <param name="r1">Network 1. note the owner of this network, will be the owner of the merged network</param>
   /// <param name="r2">Network to be merged with network 1</param>
   /// <returns></returns>
    roadNetwork mergeNetworks(roadNetwork r1, roadNetwork r2) {

        // Check if network 1 exists.
        if (!roadNetworks.Contains(r1)) {
            Debug.LogError("-Road.mergeNetworks- network r1 does not exist");
            return null;
        }

        // Check if network 2 exists.
        if (!roadNetworks.Contains(r2)) {
            Debug.LogError("-Road.mergeNetworks- network r2 does not exist");
            return null;
        }

        // Create a new network.
        roadNetwork newNetwork = createNewNetwork(r1.owner);

        // Merge network 1 and new.
        foreach (Path_Node<Tile> N in r1.nodes.Values)
            newNetwork.add_node(N);

        // Merge network 2 and new.
        foreach (Path_Node<Tile> N in r2.nodes.Values)
            newNetwork.add_node(N);

        // Delete the old networks.
        deleteNetwork(r1);
        deleteNetwork(r2);

        return newNetwork;
    }

    void deleteNetwork(roadNetwork RN) {

        // Check if the network exists.
        if (!roadNetworks.Contains(RN)) {
            Debug.LogError("-Road.deleteNetwork- Road network does not exist");
            return;
        }

        roadNetworks.Remove(RN);
    }

    public bool doesCitiesHaveRoadConnection(Tile t1, Tile t2) {

        // Check that t1 exist.
        if (t1 == null) {
            Debug.LogError("-Road.doesCitiesHaveRoadConnection- t1 does not exist");
            return false;
        }

        // Check that t2 exist.
        if (t2 == null) {
            Debug.LogError("-Road.doesCitiesHaveRoadConnection- t2 does not exist");
            return false;
        }

        // Check if the 2 tiles are in the same network.
        foreach (roadNetwork RN in roadNetworks)
            if (RN.nodes.ContainsKey(t1) && RN.nodes.ContainsKey(t2))
                return RN.hasEdge(t1, t2);

        return false;
    }

    void drawGraph(Tile t1, Tile t2) {

        /// Circle over nodes

        GameObject go = new GameObject();
        go.transform.SetParent(this.transform, true);

        LineRenderer lr = go.AddComponent<LineRenderer>();

        lr.SetPosition(0, new Vector3(t1.X + 0.5f, t1.Y + 0.5f, -5));
        lr.SetPosition(1, new Vector3(t2.X + 0.5f, t2.Y + 0.5f, -5));

        lr.startWidth = 0.125f;

        //while (lineEdges.Count > 0) {
        //    GameObject go = lineEdges[0];
        //    lineEdges.RemoveAt(0);
        //    SimplePool.Despawn(go);
        //}

        //foreach (roadNetwork RN in roadNetworks) {
        //    foreach (Path_Node<Tile> N in RN.nodes.Values)
        //        if (N.edges != null)
        //            foreach (Path_Edge<Tile> E in N.edges) {

        //                GameObject go = new GameObject();
        //                LineRenderer lr = go.AddComponent<LineRenderer>();

        //                Tile start = N.data;
        //                Tile end = E.node.data;

        //                lr.SetPosition(0, new Vector3(start.X + 0.5f, start.Y + 0.5f, -5));
        //                lr.SetPosition(1, new Vector3(end.X + 0.5f, end.Y + 0.5f, -5));

        //                lr.startWidth = 0.125f;
        //                lr.startWidth = 0.125f;

        //                lineEdges.Add(go);
        //            }
        //}
    }

    public void Road_operation() {
        /// Cities with edges are calculates twice
        /// Ie city 1 + city 2 and city 2 + city1

        foreach (roadNetwork RN in roadNetworks) {
            foreach (Path_Node<Tile> N in RN.nodes.Values) {
                if (N.edges != null) {
                    foreach (Path_Edge<Tile> E in N.edges) {
                        int totalPopulation = N.data.city.population + E.node.data.city.population;
                        int demand = (int)(totalPopulation / (Mathf.Pow(E.cost, 2) + 4));
                        RN.owner.opereatingIncome(demand);
                    }
                }
            }

            foreach (Path_Node<Tile> N in RN.cityPaths.Values) {
                if (N.edges != null) {
                    foreach (Path_Edge<Tile> E in N.edges) {
                        int totalPopulation = N.data.city.population + E.node.data.city.population;
                        int demand = (int)(totalPopulation / (Mathf.Pow(E.cost, 2) + 4));
                        RN.owner.opereatingIncome(demand);
                    }
                }
            }
        }

public class RoadOperations : MonoBehaviour {
    // Start is called before the first frame update


    private float time = 0.0f;

    float tick {
        get { return SpeedController.speed.WorldTick; }
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;

        if (time >= tick && tick != 0) {
            time -= tick;
            Road_operation();
        }
    }

    public void Road_operation() {
        foreach (Network RN in WorldController.worldController.world.road.networks) {
            foreach (Path_Node<Tile> N in RN.nodes.Values) {
                if (N.edges != null) {
                    foreach (Path_Edge<Tile> E in N.edges) {
                        int totalPopulation = N.data.city.population + E.node.data.city.population;
                        int demand = (int)(totalPopulation / (Mathf.Pow(E.cost, 2) + 4));
                        RN.owner.opereatingIncome(demand);
                    }
                }
            }

            foreach (Path_Node<Tile> N in RN.cityPaths.Values) {
                if (N.edges != null) {
                    foreach (Path_Edge<Tile> E in N.edges) {
                        int totalPopulation = N.data.city.population + E.node.data.city.population;
                        int demand = (int)(totalPopulation / (Mathf.Pow(E.cost, 2) + 4));
                        RN.owner.opereatingIncome(demand);
                    }
                }
            }
        }
    }
}

public class roadNetwork {
    public roadNetwork(int i, Player owner) {
        id = i;
        this.owner = owner;

        nodes = new Dictionary<Tile, Path_Node<Tile>>();
    }

    public Dictionary<Tile, Path_Node<Tile>> nodes { get; protected set; }
    public Dictionary<Tile, Path_Node<Tile>> cityPaths { get; protected set; }

    public Player owner { get; protected set; }

    public int id { get; protected set; }

    // Add tile to this network.
    public void add_city(Tile t) {
        // Create node
        Path_Node<Tile> n = new Path_Node<Tile> {
            data = t
        };
        nodes.Add(t, n);
    }

    public void add_node(Path_Node<Tile> n) {
        nodes.Add(n.data, n);
    }

    public void addEdge(Tile t1, Tile t2) {

        // Check if nodes contains t1.
        if (!nodes.ContainsKey(t1)) {
            Debug.LogError("-roadNetwork.addEdge- t1 does not exist");
            return;
        }

        // Check if nodes contains t2.
        if (!nodes.ContainsKey(t2)) {
            Debug.LogError("-roadNetwork.addEdge- t2 does not exist");
            return;
        }

        // Check if edge allready exist.
        if (hasEdge(t1, t2))
            return;

        // Euclidian distance between the two tiles.
        float distance = Mathf.Sqrt(Mathf.Pow(t1.X - t2.X, 2) + Mathf.Pow(t1.Y - t2.Y, 2));

        List<Path_Edge<Tile>> edges;

        // Find t1
        /// (nodes.Keys.TryGetValue
        foreach (Tile t in nodes.Keys) {
            if (t == t1) {
                Path_Node<Tile> n = nodes[t];

                edges = new List<Path_Edge<Tile>>();

                if (n.edges != null && n.edges.Length > 0)
                    edges = n.edges.ToList();

                // Create an edge from t1 to t2.
                Path_Edge<Tile> e = new Path_Edge<Tile> {

                    // Cost is the euclidian distance between the two tiles.
                    cost = distance,
                    node = nodes[t2]
                };

                edges.Add(e);
                n.edges = edges.ToArray();

                break;
            }
        }

        // Find t2
        foreach (Tile t in nodes.Keys) {
            if (t == t2) {
                Path_Node<Tile> n = nodes[t];

                edges = new List<Path_Edge<Tile>>();

                if (n.edges != null && n.edges.Length > 0)
                    edges = n.edges.ToList();

                // Create an edge from t2 to t1
                Path_Edge<Tile> e = new Path_Edge<Tile> {

                    // Cost is the euclidian distance between the two tiles
                    cost = distance,
                    node = nodes[t1]
                };

                edges.Add(e);
                n.edges = edges.ToArray();

                break;
            }
        }
        generate_cityPaths();
    }

    // Check if there is a edge between the two tiles.
    public bool hasEdge(Tile t1, Tile t2) {
        /// Exist

        if (t1 == null)
            return false;

        if (t2 == null)
            return false;

        foreach (Path_Node<Tile> N in nodes.Values)
            if (N.data == t1 && N.edges != null)
                foreach (Path_Edge<Tile> E in N.edges)
                    if (E.node.data == t2)
                        return true;

        return false;
    }

    void generate_cityPaths() {
        cityPaths = new Dictionary<Tile, Path_Node<Tile>>();

        foreach (Path_Node<Tile> NT in nodes.Values) {
            Path_Node<Tile> CPT = new Path_Node<Tile> {
                data = NT.data
            };
            cityPaths.Add(NT.data, CPT);
        }

        foreach (Path_Node<Tile> CPT in cityPaths.Values) {

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            foreach (Path_Node<Tile> NT in nodes.Values) {
                if (NT.data == CPT.data || hasEdge(NT.data, CPT.data)) {
                    continue;
                }

                Path_Edge<Tile> e = new Path_Edge<Tile> {

                    // Euclidian distance between the two tiles.
                    /// Need to be proper cost, got from pathfinding algo
                    cost = Mathf.Sqrt(Mathf.Pow(NT.data.X - CPT.data.X, 2) + Mathf.Pow(NT.data.Y - CPT.data.Y, 2)),
                    node = NT
                };
                edges.Add(e);

            }
            CPT.edges = edges.ToArray();
        }
    }
}



    // Add edge between t1 and t2.
    public void addEdge(Tile t1, Tile t2) {

        // Check if nodes contains t1.
        if (!nodes.ContainsKey(t1)) {
            Debug.LogError("t1 does not exist");
            return;
        }

        // Check if nodes contains t2.
        if (!nodes.ContainsKey(t2)) {
            Debug.LogError("t2 does not exist");
            return;
        }

        // Check if edge allready exist.
        if (hasEdge(t1, t2))
            return;

        // Euclidian distance between the two tiles.
        float distance = Mathf.Sqrt(Mathf.Pow(t1.X - t2.X, 2) + Mathf.Pow(t1.Y - t2.Y, 2));

        List<Path_Edge<Tile>> edges;

        // Find t1
        /// (nodes.Keys.TryGetValue
        foreach (Tile T in nodes.Keys) {
            if (T == t1) {
                Path_Node<Tile> n = nodes[T];

                edges = new List<Path_Edge<Tile>>();

                if (n.edges != null && n.edges.Length > 0)
                    edges = n.edges.ToList();

                // Create an edge from t1 to t2.
                Path_Edge<Tile> e = new Path_Edge<Tile> {

                    // Cost is the euclidian distance between the two tiles.
                    cost = distance,
                    node = nodes[t2]
                };

                edges.Add(e);
                n.edges = edges.ToArray();

                break;
            }
        }

        // Find t2
        foreach (Tile T in nodes.Keys) {
            if (T == t2) {
                Path_Node<Tile> n = nodes[T];

                edges = new List<Path_Edge<Tile>>();

                if (n.edges != null && n.edges.Length > 0)
                    edges = n.edges.ToList();

                // Create an edge from t2 to t1
                Path_Edge<Tile> e = new Path_Edge<Tile> {

                    // Cost is the euclidian distance between the two tiles
                    cost = distance,
                    node = nodes[t1]
                };

                edges.Add(e);
                n.edges = edges.ToArray();

                break;
            }
        }
    }



    void drawGraph(Tile t1, Tile t2) {

        /// Circle over nodes
        if (enableDrawGraph) {
            GameObject go = new GameObject();

            go.transform.SetParent(parent.transform);
            go.name = "Tile_" + t1.X + "_" + t1.Y + " --- Tile_" + t2.X + "_" + t2.Y;

            LineRenderer lr = go.AddComponent<LineRenderer>();

            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color;
            lr.endColor = color;

            lr.SetPosition(0, new Vector3(t1.X + 0.5f, t1.Y + 0.5f, -5));
            lr.SetPosition(1, new Vector3(t2.X + 0.5f, t2.Y + 0.5f, -5));

            lr.startWidth = 0.125f;
        }
    }


    void generateNodes() {

        // Clear all nodes
        nodes.Clear();

        // There is just 1 city in this network.
        if (cities.Count < 2) {
            return;
        }

        foreach (Tile tile1 in cities) {

            // Create a new pathnode for each citytile.
            Path_Node<Tile> tile = new Path_Node<Tile>();
            tile.data = tile1;

            List<Tile> tiles = cities;
            tiles.Remove(tile1);
            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            // For each tile in cities, create a edge.
            foreach (Tile tile2 in cities) {
                Path_Edge<Tile> edge = new Path_Edge<Tile>();

                // Euclidian distance between the two tiles.
                edge.cost = Mathf.Sqrt(Mathf.Pow(tile1.X - tile2.X, 2) + Mathf.Pow(tile1.Y - tile2.Y, 2));
                edge.node.data = tile2;
                edges.Add(edge);
            }
            tile.edges = edges.ToArray();
        }
    }

public void buildNetworkConnection(Tile t1, Tile t2, Player owner) {

        // Check if t1 exist.
        if (t1 == null || !t1.isCity()) {
            Debug.LogError("t1  is null, or not a city");
            return;
        }

        // Check if t2 exist.
        if (t2 == null || !t2.isCity()) {
            Debug.LogError("t2 is null, or not a city");
            return;
        }

        // Check if the two tiles have an edge.
        if (doesCitiesHaveNetworkConnection(t1, t2)) {
            Debug.LogError("the two tiles allready have a connection");
            return;
        }

        // True distance between the two tiles.
        //float distance = Mathf.Sqrt(Mathf.Pow(t1.X - t2.X, 2) + Mathf.Pow(t1.Y - t2.Y, 2));

        // Add networkconection to cities if not allready added.
        if (t1.city.hasCityNetworkConnection(type)) {
            t1.city.add_networkConnection(type);
        }

        if (t2.city.hasCityNetworkConnection(type)) {
            t2.city.add_networkConnection(type);
        }

        /// Default?
        Network network1 = default;
        Network network2 = default;

        // Check if the tiles are in networks owned by player
        foreach (Network n in networks) {

            // Tile t1 is in a network owned by player
            if (n.tiles.Contains(t1) && n.owner == owner)
                network1 = n;

            // Tile t2 is in a network owned by player
            if (n.tiles.Contains(t2) && n.owner == owner)
                network2 = n;

            // Both tiles are in the same network owned by player
            if (n.tiles.Contains(t1) && n.tiles.Contains(t2) && n.owner == owner) {
                return;
            }
        }

        // Only tile t1 is in a network owned by player
        if (network1 != null && network2 == null) {
            network1.addTile(t2);
            return;
        }

        // Only tile t2 is in a network owned by player
        else if (network2 != null && network1 == null) {
            network2.addTile(t1);
            return;
        }

        // Both t1 and t2 are in networks owned by player
        else if (network2 != null && network1 != null) {
            //mergeNetwork(network1, network2);
            return;
        }

        else {
            // Non of the tiles are in a network owned by player
            Network newNetwork = createNewNetwork(owner);
            newNetwork.addTile(t1);
            newNetwork.addTile(t2);
        }
    }

// Check if there is a edge between the two tiles.
    public bool hasEdge(Tile t1, Tile t2) {
        /// Exist

        if (t1 == null) {
            return false;
        }

        if (t2 == null) {
            return false;
        }

        foreach (Path_Node<Tile> N in nodes) {
            if (N.data == t1 && N.edges != null) {
                foreach (Path_Edge<Tile> E in N.edges) {
                    if (E.node.data == t2) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //public static bool operator ==(City city1, City city2) {
    //    if ((object)city1 == null)
    //        return (object)city2 == null;

    //    return city1.Equals(city2);
    //}

    //public static bool operator !=(City city1, City city2) {
    //    return !(city1.name == city2.name);
    //}

    //public override bool Equals(City city)
    //{
    //    if (city == null || GetType() != city.GetType())
    //        return false;

    //    var c2 = (City)city;
    //    return (id == c2.id);
    //}

    //public override int GetHashCode() {
    //    return name.GetHashCode();
    //}


Interchange new_interchange() {
        int highestId = 0;

        // Find highest id
        foreach (Interchange I in list_Interchanges)
            if (I.id > highestId)
                highestId = I.id;

        Interchange newInterchange = new Interchange(highestId);

        list_Interchanges.Add(newInterchange);

        return newInterchange;
    }


public void add_stationToInterchange(Metrostation ms, int id) {
        // Check if station exist
        if (!get_metroline(ms.lineNumber).list_metrostations.Exists(X => X.stationNumber == ms.stationNumber)) {
            Debug.LogError("Station: " + ms.name + " does not exist");
            return;
        }

        // Make sure that the line has more than 3 stations
        if (get_metroline(ms.lineNumber).list_metrostations.Count < 4) {
            Debug.LogError("Line: " + ms.lineNumber + " have too few metrostations to be added to interchange!");
            return;
        }

        foreach (Interchange I in list_Interchanges)
            if (I.id == id) {
                I.add_lineToInterchange(ms.lineNumber);
                break;
            }

        delete_station(ms);
        updateCatchment();
    }

public void create_interchange(Metrostation ms1, Metrostation ms2) {
        // Make sure that the linumbers of ms1 and ms2 is different
        if (ms1.lineNumber == ms2.lineNumber) {
            Debug.LogError("station: " + ms1.name + " and station: " + ms2.name + " has the same linenumber!");
            return;
        }

        // Make sure that line1 has more than 3 stations
        if (get_metroline(ms1.lineNumber).list_metrostations.Count < 4) {
            Debug.LogError("-Metro.create_interchange- Line: " + ms1.lineNumber + " has to few stations!");
            return;
        }

        // Make sure that line2 has more than 3 stations
        if (get_metroline(ms2.lineNumber).list_metrostations.Count < 4) {
            Debug.LogError("-Metro.create_interchange- Line: " + ms2.lineNumber + " has to few stations!");
            return;
        }

        // Create a new interchange
        Interchange newInterchange = new_interchange();

        // Add the stations to the new interchange
        newInterchange.add_lineToInterchange(ms1.lineNumber);
        newInterchange.add_lineToInterchange(ms2.lineNumber);

        // Delete the stations
        delete_station(ms1);
        delete_station(ms2);
    }

    void delete_station(Metrostation ms) {
        List<Metrostation> list = get_metroline(ms.lineNumber).list_metrostations;

        // Check if station exist
        if (!list.Exists(X => X.stationNumber == ms.stationNumber)) {
            Debug.LogError("Station: " + ms.name + " does not exist");
            return;
        }

        for (int i = 0; i < list.Count; i++)
            if (list[i].lineNumber == ms.lineNumber) {
                list.RemoveAt(i);
                break;
            }

        /// list.Remove(new Metrostation() { lineNumber = ms.lineNumber });
        /// foreach (Metrostation MS in list) if (MS.lineNumber == ms.lineNumber) get_metroline(ms.lineNumber).remove(MS);
    }

    //void creategraph() {
    //    if (cityTiles.Count > 1) {
    //        graph.Clear();
    //        foreach (Tile city1 in cityTiles) {
    //            CityGraph newCityGraph = new CityGraph(city1);
    //            graph.Add(newCityGraph);

    //            foreach (Tile city2 in cityTiles) {
    //                if (city2 != city1 && !newCityGraph.distances.ContainsKey(city2.city)) {
    //                    newCityGraph.addDistance(city2.city, (int)Mathf.Sqrt(Mathf.Pow(city1.X - city2.X, 2) + Mathf.Pow(city1.Y - city2.Y, 2)));
    //                }
    //            }
    //        }
    //    }
    //}

//public class CityGraph {
//    public CityGraph(Tile cityTile) {
//        this.cityTile = cityTile;
//        distances = new Dictionary<City, int>();
//        paths = new Dictionary<Tile, Tile[]>();
//    }

//    public Tile cityTile { get; protected set; }
//    public Dictionary<City, int> distances { get; protected set; }
//    public Dictionary<Tile, Tile[]> paths { get; protected set; }

//    public void addDistance(City city, int distance) {
//        if (distances.ContainsKey(city)) {
//            Debug.LogError("tile allready added");
//            return;
//        }

//        distances.Add(city, distance);
//    }

//    public void addPath(Tile tile, Tile[] tiles) {
//        if (paths.ContainsKey(tile)) {
//            Debug.LogError("tile allready added");
//            return;
//        }

//        paths.Add(tile, tiles);
//    }
//}

public class AirportBuildController : MonoBehaviour {

    public static AirportBuildController airportBuildController { get; protected set; }
    // Start is called before the first frame update
    void Start() {
        airportBuildController = this;
    }

    // Update is called once per frame
    void Update() {

    }

    public void buildAirport(City city, Player player) {
        if (city.hasPlayerAAirport(player)) {
            Debug.LogError("Player: " + player + " allready have a airport in this city");
            return;
        }

        city.buildAirport(player);
    }

    public void buildTerminal(City city, Player player) {
        if (!city.hasPlayerAAirport(player)) {
            Debug.LogError("Player: " + player + " does not have a airport in this city");
            return;
        }

        city.get_airportByPlayer(player).add_terminal();
    }

    public void buildRunway(City city, Player player) {
        if (!city.hasPlayerAAirport(player)) {
            Debug.LogError("Player: " + player + " does not have a airport in this city");
            return;
        }

        city.get_airportByPlayer(player).add_runway();
    }
}

*/
