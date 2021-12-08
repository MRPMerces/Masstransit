using System.Collections.Generic;
using UnityEngine;

public class Interchange {
    public Interchange(int i) {
        id = i;
        list_interchange_lines = new List<int>();
    }

    public List<int> list_interchange_lines { get; protected set; }

    public int id { get; protected set; }

    public bool trainstation { get; protected set; }

    public void add_trainstationToInterchange() {
        trainstation = true;
    }

    public void add_lineToInterchange(int lineNumber) {
        foreach (int VIL in list_interchange_lines)
            if (VIL == lineNumber) {
                Debug.LogError("Line: " + lineNumber + " is allerady added to interchange");
                return;
            }
        list_interchange_lines.Add(lineNumber);

        ///Sort interchange
    }
}
