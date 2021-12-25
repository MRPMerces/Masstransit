using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public enum PlayerType { Human, Ai };

public class Player : IXmlSerializable {
    public Player(PlayerType type, string name, int id, Modifier[] characterModifiers = null) {
        this.characterModifiers = characterModifiers;
        this.type = type;
        this.name = name;

        //policies = new PolicyController;
        modifiers = new ModifierController();
        companyOwnership = 1f;
        actualMoney = 100000000;

        loan1 = null;
        loan2 = null;
        loan3 = null;
    }

    public PlayerType type { get; protected set; }

    public string name { get; protected set; }

    /// <summary>
    /// All modifiers that affect the player.
    /// </summary>
    public ModifierController modifiers { get; protected set; }

    /// <summary>
    ///  Modifiers that the characther has. eg railway empire characheter spesific modifiers
    /// </summary>
    readonly Modifier[] characterModifiers;

    public Loan loan1 { get; set; }
    public Loan loan2 { get; set; }
    public Loan loan3 { get; set; }

    private float actualMoney;

    /// <summary>
    /// Returns the player::money as a integer.
    /// </summary>
    public int money {
        get {
            return (int)actualMoney;
        }
        protected set {
        }
    }

    public float totalExpenditure { get; protected set; }

    float companyOwnership;
    public float publicOwnership { get; protected set; }

    /// <summary>
    /// Subtract construction cost from balance.
    /// Use this if the constructionCost modifier should be applied.
    /// </summary>
    public void constructionCost(int amount) {
        float expendeture = (amount * modifiers.constructionCost.value);
        if (actualMoney - expendeture < 0) {
            Debug.LogError("Insufficient funds!");
            return;
        }

        actualMoney -= expendeture;
        totalExpenditure += expendeture;

        // Call the callback and let things know we've updated.
        if (cbMoneyUpdated != null && type == PlayerType.Human) {
            cbMoneyUpdated();
        }

        if (type == PlayerType.Human) {
            Debug.Log("- " + expendeture + " Money");
        }
    }

    public void constructionCost(float amount) {
        float expendeture = (amount * modifiers.constructionCost.value);
        if (actualMoney - expendeture < 0) {
            Debug.LogError("Insufficient funds!");
            return;
        }

        actualMoney -= expendeture;
        totalExpenditure += expendeture;

        // Call the callback and let things know we've updated.
        if (cbMoneyUpdated != null && type == PlayerType.Human) {
            cbMoneyUpdated();
        }

        if (type == PlayerType.Human) {
            Debug.Log("- " + expendeture + " Money");
        }
    }

    /// <summary>
    /// Add opereating income to balance.
    /// </summary>
    public void opereatingIncome(int amount) {
        if (amount > 0) {
            actualMoney += amount * modifiers.operatingIncome.value * companyOwnership;

            if (type == PlayerType.Human) {
                Debug.Log("+ " + (amount * modifiers.operatingIncome.value * companyOwnership) + " Money");
            }
        }

        if (amount < 0) {
            if (type == PlayerType.Human) {
                Debug.Log("- " + amount + " Money");
            }
            actualMoney += amount;
        }

        // Call the callback and let things know we've updated.
        if (cbMoneyUpdated != null && type == PlayerType.Human) {
            cbMoneyUpdated();
        }
    }

    /// <summary>
    /// Add opereating income to balance.
    /// </summary>
    public void opereatingIncome(float amount) {
        if (amount > 0) {
            actualMoney += amount * modifiers.operatingIncome.value * companyOwnership;

            if (type == PlayerType.Human) {
                //Debug.Log("+ " + (amount * modifiers.operatingIncome.value * companyOwnership) + " Money");
            }
        }

        if (amount < 0) {
            if (type == PlayerType.Human) {
                Debug.Log("- " + amount + " Money");
            }
            actualMoney += amount;
        }

        // Call the callback and let things know we've updated.
        if (cbMoneyUpdated != null && type == PlayerType.Human) {
            cbMoneyUpdated();
        }
    }

    /// <summary>
    /// Method to check whenever a player can afford the construction cost.
    /// </summary>
    /// <param name="amount">The constructioncost to test</param>
    /// <returns>ool can/ can't afford</returns>
    public bool canAffordConstructionCost(int amount) {
        return amount * modifiers.constructionCost.value < actualMoney;
    }

    /// <summary>
    /// Method to check whenever a player can afford the construction cost.
    /// </summary>
    /// <param name="amount">The constructioncost to test</param>
    /// <returns>ool can/ can't afford</returns>
    public bool canAffordConstructionCost(float amount) {
        return amount * modifiers.constructionCost.value < actualMoney;
    }

    //public PolicyController policies { get; protected set; }

    /// <summary>
    /// Function to update all the player spesific modifiers.
    /// </summary>
    public void update_modifiers(Modifier[] externalModifiers) {

        List<Modifier> tempModifiers = new List<Modifier>();
        //modifiers.Add(policies.policies);
        tempModifiers.AddRange(externalModifiers);

        modifiers.update_modifiers(tempModifiers.ToArray());
    }

    public void update_companyOwnership(float f) {
        if (f > publicOwnership) {
            Debug.LogError("Buying more stock than the public has");
            return;
        }
        /// error
        companyOwnership += f;
        publicOwnership -= f;
    }

    // The function we callback any time player.human.money updates changes
    Action cbMoneyUpdated;

    /// <summary>
    /// Register a function to be called back when our tile type changes.
    /// </summary>
    public void RegisterMoneyUpdatedCallback(Action callback) {
        cbMoneyUpdated += callback;
    }

    /// <summary>
    /// Unregister a callback.
    /// </summary>
    public void UnregisterMoneyUpdatedCallback(Action callback) {
        cbMoneyUpdated -= callback;
    }

    #region Saving and loading

    /// <summary>
    /// Default constructor for saving and loading.
    /// </summary>
    private Player() { }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("Type", ((int)type).ToString());
        writer.WriteAttributeString("Name", name);
        writer.WriteAttributeString("Money", actualMoney.ToString());
        writer.WriteAttributeString("TotalExpenditure", totalExpenditure.ToString());
    }

    public void ReadXml(XmlReader reader) {
        type = (PlayerType)int.Parse(reader.GetAttribute("Type"));
        name = reader.GetAttribute("Name");
        actualMoney = float.Parse(reader.GetAttribute("Money"));
        totalExpenditure = int.Parse(reader.GetAttribute("TotalExpenditure"));
    }

    #endregion Saving and loading
}
