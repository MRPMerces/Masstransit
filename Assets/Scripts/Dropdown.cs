////Create a new Dropdown GameObject by going to the Hierarchy and clicking Create>UI>Dropdown. Attach this script to the Dropdown GameObject.

//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class MetroExpand : MonoBehaviour {

//    //Use these for adding options to the Dropdown List
//    Dropdown.OptionData m_NewData;
//    //The list of messages for the Dropdown
//    List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();


//    //This is the Dropdowns
//    Dropdown dropdownExpand;
//    Dropdown dropdownSelect;
//    Dropdown dropdownMetro;
//    Dropdown dropdownAirport;

//    World World {
//        get { return WorldController.worldController.world; }
//    }

//    void Start() {
//        selectTypeToExpand();
//    }

//    void selectTypeToExpand() {
//        //Fetch the Dropdown GameObject the script is attached to
//        dropdownExpand = GetComponent<Dropdown>();

//        //Clear the old options of the Dropdown menu
//        dropdownExpand.ClearOptions();

//        /// Fetc the dropdown again, with differetn name, to get arond addlistener
//        /// 


//        //Create a new option for the Dropdown menu which reads "Expand" and add to messages List
//        m_NewData = new Dropdown.OptionData();
//        m_NewData.text = "Expand";
//        m_Messages.Add(m_NewData);

//        //Create a new option for the Dropdown menu which reads "Metro" and add to messages List
//        m_NewData = new Dropdown.OptionData();
//        m_NewData.text = "Metro";
//        m_Messages.Add(m_NewData);

//        //Create a new option for the Dropdown menu which reads "Airport" and add to messages List
//        m_NewData = new Dropdown.OptionData();
//        m_NewData.text = "Airport";
//        m_Messages.Add(m_NewData);

//        //Take each entry in the message List
//        foreach (Dropdown.OptionData message in m_Messages)
//            dropdownExpand.options.Add(message);

//        m_Messages.Clear();

//        dropdownExpand.onValueChanged.AddListener(delegate {
//            //Clear the old options of the Dropdown menu
//            dropdownExpand.ClearOptions();

//            if (dropdownExpand.value == 1) {

//                selectCity("metro");
//            }
//            if (dropdownExpand.value == 2) {

//                selectCity("airport");
//            }
//        });
//    }



//    void selectCity(string type) {
//        Destroy(dropdownExpand);
//        /// Return a city to make reusable.


//        //Fetch the Dropdown GameObject the script is attached to
//        dropdownSelect = GetComponent<Dropdown>();

//        //Clear the old options of the Dropdown menu
//        dropdownSelect.ClearOptions();

//        foreach (Tile T in World.tilesWithCity) {
//            //Create a new option for the Dropdown menu which reads city name
//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = T.city.name;
//            m_Messages.Add(m_NewData);
//        }

//        //Take each entry in the message List
//        foreach (Dropdown.OptionData message in m_Messages)
//            dropdownSelect.options.Add(message);

//        m_Messages.Clear();

//        City city = World.tilesWithCity[dropdownSelect.value].city;

//        dropdownSelect.onValueChanged.AddListener(delegate {
//            //Clear the old options of the Dropdown menu
//            dropdownSelect.ClearOptions();
//            Destroy(dropdownSelect);

//            if (type == "metro")
//                expandMetro(city);
//            if (type == "airport")
//                expandAirport(city);
//        });
//    }

//    void expandMetro(City c) {
//        //Fetch the Dropdown GameObject the script is attached to
//        dropdownMetro = GetComponent<Dropdown>();

//        //Clear the old options of the Dropdown menu
//        dropdownMetro.ClearOptions();

//        if (!c.hasMetro) {
//            //Create a new option for the Dropdown menu which reads "Expand" and add to messages List
//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Expand";
//            m_Messages.Add(m_NewData);


//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Build a metro system";
//            m_Messages.Add(m_NewData);
//        }

//        else {
//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Add line";
//            m_Messages.Add(m_NewData);

//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Add station to line";
//            m_Messages.Add(m_NewData);
//        }

//        //Take each entry in the message List
//        foreach (Dropdown.OptionData message in m_Messages) {
//            //Add each entry to the Dropdown
//            dropdownMetro.options.Add(message);
//        }
//        m_Messages.Clear();

//        dropdownMetro.onValueChanged.AddListener(delegate {
//            //Clear the old options of the Dropdown menu
//            dropdownMetro.ClearOptions();

//            if (!c.hasMetro && dropdownMetro.value == 1) {
//                c.buildMetro();
//                Destroy(dropdownMetro);
//            }

//            else
//                switch (dropdownMetro.value) {
//                    case 1:
//                        Destroy(dropdownMetro);
//                        c.metro.add_metroline();
//                        break;
//                }
//            selectTypeToExpand();
//        });
//    }

//    void expandAirport(City c) {
//        //Fetch the Dropdown GameObject the script is attached to
//        dropdownAirport = GetComponent<Dropdown>();

//        //Clear the old options of the Dropdown menu
//        dropdownAirport.ClearOptions();

//        if (!c.hasAirport) {
//            //Create a new option for the Dropdown menu which reads "Expand" and add to messages List
//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Expand";
//            m_Messages.Add(m_NewData);


//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Build airport";
//            m_Messages.Add(m_NewData);
//        }

//        else {
//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Build terminal";
//            m_Messages.Add(m_NewData);

//            m_NewData = new Dropdown.OptionData();
//            m_NewData.text = "Build runway";
//            m_Messages.Add(m_NewData);
//        }

//        //Take each entry in the message List
//        foreach (Dropdown.OptionData message in m_Messages) {
//            //Add each entry to the Dropdown
//            dropdownAirport.options.Add(message);
//        }
//        m_Messages.Clear();

//        dropdownAirport.onValueChanged.AddListener(delegate {
//            //Clear the old options of the Dropdown menu
//            dropdownAirport.ClearOptions();

//            if (dropdownAirport.value == 1 && !c.hasAirport) {
//                c.buildAirport();
//                Destroy(dropdownAirport);
//            }

//            else
//                switch (dropdownAirport.value) {
//                    case 1:
//                        Destroy(dropdownAirport);
//                        c.airport.add_runway();
//                        break;

//                    case 2:
//                        c.airport.add_terminal();
//                        Destroy(dropdownAirport);
//                        break;
//                }
//            selectTypeToExpand();
//        });
//    }
//}


