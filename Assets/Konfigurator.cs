using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Konfigurator : MonoBehaviour
{
    [System.Serializable] public struct Mod
    {
        public Sprite Icon;
        public GameObject Model;
#nullable enable
        public Material? ColorMaterial;
#nullable disable
    }


    [SerializeField] Material carMaterial;
    [SerializeField] Color[] bodyColors;
    Color currentColor;
    [SerializeField] GameObject carBody;
    [SerializeField] Transform[] wheelPositions;
    [SerializeField] VerticalLayoutGroup wheelsPanel;
    int _wheelIndex;

    [SerializeField] GameObject modButtonPrefab;

    [SerializeField] HorizontalLayoutGroup colorsPanel;
    [SerializeField] Sprite colorIcon;

    [SerializeField] Mod[] wheels;
    [SerializeField] Mod[] spoilers;
    [SerializeField] Transform spoilerHolder;
    [SerializeField] VerticalLayoutGroup spoilersPanel;

    private int selectedSpoilerIndex; //compiller haze chybu, ale actually se do toho assignuje


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
            button.GetComponent<Image>().sprite = colorIcon;
            button.GetComponent<Image>().color = color;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeColor(color, true);
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

        foreach (Mod wheel in wheels)
        {
            GameObject button = Instantiate(modButtonPrefab);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(62, 50);
            button.GetComponent<Image>().sprite = wheel.Icon;
            button.transform.SetParent(wheelsPanel.transform, false);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeWheels(System.Array.IndexOf(wheels, wheel)); //ehhhh
            });
        }


        currentColor = carMaterial.color;
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
        ChangeColor(currentColor, false);
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

    public void ChangeColor(Color c, bool blendMods)
    {
        //carMaterial.color = c;
        carMaterial.DOColor(c, 1);
        //if (spoilers[selectedSpoilerIndex].ColorMaterial != null) spoilers[selectedSpoilerIndex].ColorMaterial.color = c;
        if (spoilers[selectedSpoilerIndex].ColorMaterial is not null)
        {
            if (blendMods)
                spoilers[selectedSpoilerIndex].ColorMaterial.DOColor(c, 1);
            else
                spoilers[selectedSpoilerIndex].ColorMaterial.color = c;

        }

        currentColor = c;
    }
}
