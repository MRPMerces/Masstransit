using System.Linq;
using UnityEngine;

public class CityPair {

    public CityPair(Tile start, Tile end, Tile[] path) {
        this.start = start;
        this.end = end;
        this.path = path;

        trueDistance = Mathf.Sqrt(Mathf.Pow(start.X - end.X, 2) + Mathf.Pow(start.Y - end.Y, 2));
    }

    public Tile start { get; private set; }
    public Tile end { get; private set; }
    public Tile[] path { get; private set; }
    public Tile[] newPath { get; private set; }
    public int distance {
        get {
            return path.Count() - 1;
        }
    }

    public float trueDistance { get; private set; }

    public bool containsTiles(Tile tile1, Tile tile2) {
        return (tile1 == start && tile2 == end) || (tile1 == end && tile2 == start);
    }

    public void assignNewPath(Tile[] newPath, bool apply = false) {
        if (apply) {
            path = newPath;
            return;
        }

        this.newPath = newPath;
    }

    public void assignNewPath() {
        if (newPath != null) {
            path = newPath;
            newPath = null;
        }
    }
}
