using UnityEngine;
using System.Collections.Generic;

public enum ModifierType { ConstructionCost, OperatingIncome, Ridership, PopulationGrowth }

public class ModifierManager {

    public ModifierManager() {
        globalModifiers = new Dictionary<ModifierType, float>();

        globalModifiers[ModifierType.ConstructionCost] = 1f;
        globalModifiers[ModifierType.OperatingIncome] = 1f;
        globalModifiers[ModifierType.Ridership] = 1f;
    }

    public Dictionary<ModifierType, float> globalModifiers { get; protected set; }

    //public Modifier constructionCost { get; protected set; }
    //public Modifier operatingIncome { get; protected set; }
    //public Modifier ridership { get; protected set; }

    public float companyOwnership { get; protected set; }

    void resetModifiers() {
        globalModifiers[ModifierType.ConstructionCost] = 1f;
        globalModifiers[ModifierType.OperatingIncome] = 1f;
        globalModifiers[ModifierType.Ridership] = 1f;

        companyOwnership = 100f;
    }

    public void update_modifiers(Dictionary<ModifierType, float> modifiers) {
        foreach (KeyValuePair<ModifierType, float> modifier in modifiers) {
            globalModifiers[modifier.Key] += modifier.Value;
        }
    }

    public void update_modifiers(ModifierType type, float value) {
        globalModifiers[type] += value;
    }
}