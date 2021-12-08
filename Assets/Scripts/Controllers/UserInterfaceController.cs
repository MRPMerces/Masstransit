using UnityEngine;

public class UserInterfaceController : MonoBehaviour {

    public static UserInterfaceController userInterfaceController { get; protected set; }

    //public Button buildRoad;
    //public Button buildHSTN;
    //public Button buildAirport;

    //Dropdown DropdownCity;
    //Dropdown DropdownMetro;

    // Start is called before the first frame update
    void Start() {
        userInterfaceController = this;

        ////Fetch the Dropdown GameObject the script is attached to
        //DropdownCity = GetComponent<Dropdown>();
        //DropdownMetro = GetComponent<Dropdown>();

        ////Clear the old options of the Dropdown menu
        //DropdownCity.ClearOptions();
        //DropdownMetro.ClearOptions();

        //buildRoad.onClick.AddListener(roadTrue);
        //buildHSTN.onClick.AddListener(hstndTrue);
        //buildAirport.onClick.AddListener(airportTrue);
    }

    // Update is called once per frame
    void Update() {
    }

    ////Use these for adding options to the Dropdown List
    //Dropdown.OptionData m_NewData;
    ////The list of messages for the Dropdown
    //List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();

    ////Use these for adding options to the Dropdown List
    //Dropdown.OptionData metro_data;
    ////The list of messages for the Dropdown
    //List<Dropdown.OptionData> metro_Messages = new List<Dropdown.OptionData>();





    //public void selectTypeToExpand(City city) {

    //    //Clear the old options of the Dropdown menu
    //    DropdownCity.ClearOptions();

    //    //Create a new option for the Dropdown menu which reads "What to expand?" and add to messages List
    //    m_NewData = new Dropdown.OptionData();
    //    m_NewData.text = "What to expand?";
    //    m_Messages.Add(m_NewData);

    //    //Create a new option for the Dropdown menu which reads "Metro" and add to messages List
    //    m_NewData = new Dropdown.OptionData();
    //    m_NewData.text = "Metro";
    //    m_Messages.Add(m_NewData);

    //    //Create a new option for the Dropdown menu which reads "Airport" and add to messages List
    //    m_NewData = new Dropdown.OptionData();
    //    m_NewData.text = "Airport";
    //    m_Messages.Add(m_NewData);


    //    //Take each entry in the message List
    //    foreach (Dropdown.OptionData message in m_Messages) {
    //        DropdownCity.options.Add(message);
    //    }

    //    m_Messages.Clear();

    //    DropdownCity.onValueChanged.AddListener(delegate {
    //        //Clear the old options of the Dropdown menu
    //        DropdownCity.ClearOptions();

    //        // Metro
    //        if (DropdownCity.value == 1) {
    //            expandMetro(city);
    //        }
    //        if (DropdownCity.value == 2) {

    //            //selectCity("airport");
    //        }
    //    });
    //}

    //void expandMetro(City city) {
    //    DropdownMetro.ClearOptions();
    //    if (!city.hasPlayerAMetro(PlayerController.playerController.human)) {
    //        metro_data = new Dropdown.OptionData();
    //        metro_data.text = "Build metro";
    //        metro_Messages.Add(m_NewData);

    //        //Take each entry in the message List
    //        foreach (Dropdown.OptionData message in m_Messages) {
    //            DropdownMetro.options.Add(message);
    //        }
    //        m_Messages.Clear();

    //        DropdownMetro.onValueChanged.AddListener(delegate {
    //            //Clear the old options of the Dropdown menu
    //            DropdownMetro.ClearOptions();

    //            // Metro
    //            if (DropdownMetro.value == 0) {
    //                city.buildMetro(PlayerController.playerController.human);
    //            }
    //            DropdownMetro.ClearOptions();
    //        });
    //    }

    //    else {
    //        metro_data = new Dropdown.OptionData();
    //        metro_data.text = "Add a line";
    //        metro_Messages.Add(m_NewData);

    //        metro_data = new Dropdown.OptionData();
    //        metro_data.text = "Add a station to line";
    //        metro_Messages.Add(m_NewData);
    //    }


    //}

}
