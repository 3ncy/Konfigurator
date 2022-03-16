using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Konfigurator : MonoBehaviour
{
    [System.Serializable]
    public struct Mod
    {
        public Sprite Icon;
        public GameObject Model;
        public Material? ColorMaterial;
    }


    [SerializeField] Material carMaterial;
    [SerializeField] Color[] bodyColors;
    Color currentColor;
    [SerializeField] GameObject carBody;
    //[SerializeField] GameObject[] wheelObjets;
    [SerializeField] Transform[] wheelPositions;
    int _wheelIndex;

    [SerializeField] GameObject modButtonPrefab;

    [SerializeField] HorizontalLayoutGroup colorsPanel;

    [SerializeField] Mod[] wheels;
    [SerializeField] Mod[] spoilers;
    [SerializeField] Transform spoilerHolder;
    [SerializeField] VerticalLayoutGroup spoilersPanel;
   
    int selectedSpoilerIndex;


    // Start is called before the first frame update
    void Start()
    {
        //celou tuhle logiku prolly presunout do samostatneho skriptu, ktery se bude starat o ui

        //init buttons
        foreach (Color color in bodyColors)
        {
            GameObject button = Instantiate(modButtonPrefab);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 62);
            button.transform.SetParent(colorsPanel.transform, false);
            button.GetComponent<Image>().color = color;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeColor(color);
            });
        }

        foreach (Mod spoiler in spoilers)
        {
            GameObject button = Instantiate(modButtonPrefab);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(62, 50);
            button.GetComponent<Image>().sprite = spoiler.Icon;
            button.transform.SetParent(spoilersPanel.transform, false);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeSpoiler(System.Array.IndexOf(spoilers, spoiler)); //ehhhh
            });
        }

        //todo: nastavit vsem modum a autu stejnou barvu 

        currentColor = bodyColors.FirstOrDefault();
    }

    // Update is called once per frame
    void Update()
    {

        //tohle se obv presune do UI

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _wheelIndex = (_wheelIndex == wheelPositions[0].childCount - 1) ? 0 : _wheelIndex + 1;

            ChangeWheels(_wheelIndex);
        }
    }

    public void ChangeSpoiler(int spoilerIndex)
    {
        foreach (Transform t in spoilerHolder) //asi neni idealni, ale na pokud bude potreba optimalizace, tak to nejak pujde. Pokud presunu UI 
                                               //do samostatne classtky, tak si tu musu necht promennou lastSpoiler a podle te to budu vypinat.
                                               //tohle je asi hlavne rychlej prototyp
        {
            t.gameObject.SetActive(false);
        }
        selectedSpoilerIndex = spoilerIndex;           //tohle by moglo jit predelat v souvislosti s komentarem ?
        spoilers[spoilerIndex].Model.SetActive(true);
        ChangeColor(currentColor);
    }

    public void ChangeWheels(int wheelIndex) //mozna ne pres index? (-_-)?  //a tohle je prej "scratching head" emoticon
                                             // ted me napadlo ze v souvislosti s dynamickym ui, tak bych mohl mit list prefabu a pri startu aplikcae
                                             // pridat ty prefaby kolum (a mby si na ne keepovat referenci??)
    {
        foreach (Transform wheelHolder in wheelPositions)
        {
            foreach (Transform wheel in wheelHolder)
            {
                wheel.gameObject.SetActive(false);
            }

            wheelHolder.GetChild(wheelIndex).gameObject.SetActive(true);
        }
    }

    public void ChangeColor(Color c)
    {
        carMaterial.color = c;
        if (spoilers[selectedSpoilerIndex].ColorMaterial != null) spoilers[selectedSpoilerIndex].ColorMaterial.color = c;
        currentColor = c;
        Debug.Log($"current color {c}");
    }
}
