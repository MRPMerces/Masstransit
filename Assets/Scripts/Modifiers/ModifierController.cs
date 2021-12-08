using UnityEngine;

public class ModifierController {

    public ModifierController() {
        constructionCost = new Modifier(ModifierType.ConstructionCost);
        operatingIncome = new Modifier(ModifierType.OperatingIncome);
        ridership = new Modifier(ModifierType.Ridership);
    }

    public Modifier constructionCost { get; protected set; }
    public Modifier operatingIncome { get; protected set; }
    public Modifier ridership { get; protected set; }

    public float companyOwnership { get; protected set; }

    void resetModifiers() {
        constructionCost.value = 1f;
        operatingIncome.value = 1f;
        ridership.value = 1f;

        companyOwnership = 100f;
    }

    public void update_modifiers(Modifier[] modifiers) {
        resetModifiers();

        // Get modifiers from events
        foreach (Modifier M in modifiers) {
            switch (M.type) {
                case ModifierType.ConstructionCost:
                    constructionCost.value += M.value;
                    break;

                case ModifierType.OperatingIncome:
                    operatingIncome.value += M.value;
                    break;

                case ModifierType.Ridership:
                    ridership.value += M.value;
                    break;

                default:
                    Debug.LogError("Unknown modifier!");
                    break;
            }
        }
    }
}
