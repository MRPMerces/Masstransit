using UnityEngine;

public enum VehicleType { LST, HST, LSC, HSC, PLANE, FERRY }

public class Vehicle {
    public Vehicle(VehicleType vehicleType, Tile startTile, Tile destinationTile) {
        this.vehicleType = vehicleType;
        this.startTile = startTile;
        this.destinationTile = destinationTile;

        position = new Vector2(startTile.X, startTile.Y);
        direction = Vector2.zero;
        reversed = false;

        Debug.Log(startTile.name);
        Debug.Log(destinationTile.name);
    }

    public VehicleType vehicleType { get; private set; }

    public Tile startTile { get; private set; }
    public Tile destinationTile { get; private set; }

    public Tile nextTile;

    public Vector2 position { get; private set; }
    public Vector2 direction;

    public bool reversed { get; private set; }

    public void reverse() {
        Tile temp = startTile;
        startTile = destinationTile;
        destinationTile = temp;

        reversed = !reversed;
    }

    public void translate(float x, float y) {
        Vector2 newPos = new Vector2(position.x + x, position.y + y);
        position = newPos;
    }

    public Vector3 toVector3() {
        return new Vector3(position.x, position.y, 0);
    }
}
