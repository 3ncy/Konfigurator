using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Konfigurator : MonoBehaviour
{
    [System.Serializable] public struct Mod
    {
        public Texture Icon;
        public GameObject Model;
        public Material? ColorMaterial;
    }

    [SerializeField] Material carMaterial;
    [SerializeField] Color[] bodyColors;
    [SerializeField] GameObject carBody;
    //[SerializeField] GameObject[] wheelObjets;
    [SerializeField] Transform[] wheelPositions;
    int _wheelIndex;

    [SerializeField] HorizontalLayoutGroup colorsPanel;
    [SerializeField] GameObject colorButtonPrefab;

    [SerializeField] Mod[] wheels;
    [SerializeField] Mod[] spoilers;
    [SerializeField] Transform spoilerHolder;
    int selectedSpoilerIndex;


    int _i = 0;

    // Start is called before the first frame update
    void Start()
    {
        //celou tuhle logiku prolly presunout do samostatneho skriptu, ktery se bude starat o ui

        //init buttons
        foreach (Color color in bodyColors)
        {
            GameObject button = Instantiate(colorButtonPrefab);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 62);
            button.transform.SetParent(colorsPanel.transform, false);
            button.GetComponent<Image>().color = color;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeColor(color);
            });
        }


        //todo: nastavit vsem modum a autu stejnou barvu 
    }

    // Update is called once per frame
    void Update()
    {

        //tohle se obv presune do UI
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _i = (_i == bodyColors.Length - 1) ? 0 : _i + 1;
            ChangeColor(bodyColors[_i]);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _wheelIndex = (_wheelIndex == wheelPositions[0].childCount - 1) ? 0 : _wheelIndex + 1;

            ChangeWheels(_wheelIndex);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            selectedSpoilerIndex = (selectedSpoilerIndex == spoilers.Length - 1) ? 0 : selectedSpoilerIndex + 1;
            ChangeSpoiler(selectedSpoilerIndex);
        }
    }

    public void ChangeSpoiler(int spoilerIndex)
    {
        foreach(Transform spoiler in spoilerHolder) //asi neni idealni, ale na pokud bude potreba optimalizace, tak to nejak pujde. Pokud presunu UI 
                                                    //do samostatne classtky, tak si tu musu necht promennou lastSpoiler a podle te to budu vypinat.
                                                    //tohle je asi hlavne rychlej prototyp
        {
            spoiler.gameObject.SetActive(false);
        }
        Debug.Log(spoilers[spoilerIndex].Model.activeSelf);
        spoilers[selectedSpoilerIndex].Model.SetActive(true);
        Debug.Log(spoilers[spoilerIndex].Model.name);
        Debug.Log(spoilers[spoilerIndex].Model.activeSelf);

        //todo: ejak poresit obarvovani spoileru //probs si ukladat barvu na kterou bylo posledne zmeneno a tady zavolat ChangeColor(ta barva).
    }

    public void ChangeWheels(int wheelIndex) //mozna ne pres index? (-_-)?  //a tohle je prej "scratching head" emoticon
                                             // ted me napadlo ze v souvislosti s dynamickym ui, tak bych mohl mit list prefabu a pri startu aplikcae
                                             // pridat ty prefaby kolum (a mby si na ne keepovat referenci??)
    {
        foreach(Transform wheelHolder in wheelPositions)
        {
            foreach(Transform wheel in wheelHolder)
            {
                wheel.gameObject.SetActive(false);
            }

            wheelHolder.GetChild(wheelIndex).gameObject.SetActive(true);
        }
    }

    public void ChangeColor(Color c)
    {


        carMaterial.color = c;
        if(spoilers[selectedSpoilerIndex].ColorMaterial != null) spoilers[selectedSpoilerIndex].ColorMaterial.color = c;
    }
}
