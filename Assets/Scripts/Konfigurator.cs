using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Konfigurator : MonoBehaviour
{
    [System.Serializable]
    public struct Mod
    {
        public string Name;
        public Sprite Icon;
        public GameObject Model;
#nullable enable
        public Material? ColorMaterial;
#nullable disable
    }

    [System.Serializable]
    public struct color
    {
        public string Name;
        public Color Color;
    }


    [SerializeField] CameraController cameraController;

    [SerializeField] Material carMaterial;
    [SerializeField] color[] bodyColors;
    color currentColor;
    [SerializeField] GameObject carBody;
    [SerializeField] Transform[] wheelPositions;
    [SerializeField] VerticalLayoutGroup wheelsPanel;
    //int _wheelIndex;

    [SerializeField] GameObject modButtonPrefab;

    [SerializeField] HorizontalLayoutGroup colorsPanel;
    [SerializeField] Sprite colorIcon;

    [SerializeField] Mod[] wheels;
    [SerializeField] Mod[] spoilers;
    [SerializeField] Transform spoilerHolder;
    [SerializeField] VerticalLayoutGroup spoilersPanel;

    private int selectedSpoilerIndex;
    private int selectedWheelIndex;

    [Header("Saves")]

    private List<Preset> presets;
    private string filename = "/presets";
    [SerializeField] Transform presetsPanel;
    [SerializeField] GameObject presetButtonPrefab;
    //[SerializeField] Camera screenshotCam;
    [SerializeField] RenderTexture screenshotTexture;
    [SerializeField] private int? selectedPresetIndex; //todo: odstranit serializefield, nemusi byt viditelne

    // Start is called before the first frame update
    void Start()
    {
        //init buttons

        foreach (color c in bodyColors)
        {
            GameObject button = Instantiate(modButtonPrefab);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 62);
            button.transform.SetParent(colorsPanel.transform, false);
            button.GetComponent<Image>().sprite = colorIcon;
            button.GetComponent<Image>().color = c.Color;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangeColor(c, true);
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

        currentColor = bodyColors.First(c => c.Color == carMaterial.color);


        #region load settings

        if (File.Exists(Application.persistentDataPath + filename))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + filename);
            if (file.Length > 0)
            {
                presets = formatter.Deserialize(file) as List<Preset>;
                file.Close();
                Debug.Log($"loaded {presets.Count} presets from {Application.persistentDataPath + filename}");
            }
            else
            {
                presets = new();// lol nova syntaxe
                Debug.Log("stream is MT");
            }
        }
        else
        {
            presets = new();
            Debug.Log("file doens't exist");
        }
        //todo: zesvetlit ten panel configuraci kdyz tam nic neni.

        if (presets.Count > 0)
        {
            foreach (Preset preset in presets)
            {
                AddPresetToUI(preset);
            }
            //todo: pokud je souhrna sirka tlacitek vetsi nez sirka scrollview, nastavit pivot content panelu X=0.5.
            // Pokud se to ale mazanim tlacitka snizi, tak to zase nastavit na x=0. A pokud se to pridavanim presetu zvedne, tak zase na 0.5
        }
        #endregion
    }

    private void AddPresetToUI(Preset preset, Texture2D pic = null)
    {
        GameObject presetButton = Instantiate(presetButtonPrefab);
        presetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 100);
        presetButton.transform.SetParent(presetsPanel.transform, false);
        //todo: nastavit ikonu
        if (pic == null)
        {
            pic = new Texture2D(256, 256);//todo:magicka cisla, asi predelat. anebo taky apis ne, tohle se nebude menit
            //todo: check jestli soubor existuje 
            ImageConversion.LoadImage(pic, File.ReadAllBytes(preset.IconPath));
        }


        presetButton.GetComponent<Image>().sprite = Sprite.Create(pic, new Rect(0, 0, pic.width, pic.height), new Vector2(0.5f, 0.5f));
        presetButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            LoadPreset(preset);
        });
    }

    public void LoadPreset(Preset preset)
    {
        if (selectedPresetIndex != null) //odstraneni ramecky z minuleho oznaceneho presetu
            presetsPanel.GetChild((int)selectedPresetIndex).GetComponent<Outline>().enabled = false;

        selectedPresetIndex = presets.IndexOf(preset);

        presetsPanel.GetChild((int)selectedPresetIndex).GetComponent<Outline>().enabled = true;


        ChangeColor(bodyColors.First(c => c.Name == preset.ColorName), true);
        ChangeSpoiler(System.Array.IndexOf(spoilers, spoilers.First(s => s.Name == preset.SpoilerName)));
        ChangeWheels(System.Array.IndexOf(wheels, wheels.First(w => w.Name == preset.WheelsName)));

        //todo: jeste loadnout to auto
        Debug.Log(preset.ColorName + preset.WheelsName + preset.SpoilerName + preset.IconPath);
    }

    public void AddPreset()
    {
        Preset preset = new Preset
        {
            ColorName = currentColor.Name,
            WheelsName = wheels[selectedWheelIndex].Name,
            SpoilerName = spoilers[selectedSpoilerIndex].Name,
            IconPath = @"C:\Users\encyk\Development\Unity\Konfigurator\Assets\screens\a.png"
        };

        presets.Add(preset);

        AddPresetToUI(preset);//todo: predavat tu texturu

        //ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/ooo.png");
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(Application.dataPath);

        //StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        //RenderTexture renderTexture = screenshotCam.targetTexture;
        //Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        //Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        //renderResult.ReadPixels(rect, 0, 0);
        ////RenderTexture.ReleaseTemporary(renderTexture);

        Texture2D renderResult = new Texture2D(screenshotTexture.width, screenshotTexture.height);
        Rect rect = new Rect(0, 0, screenshotTexture.width, screenshotTexture.height);
        RenderTexture.active = screenshotTexture;
        renderResult.ReadPixels(rect, 0, 0);
        renderResult.Apply();

        RenderTexture.active = null;



        byte[] screenshotBytes = renderResult.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/screens/a.png", screenshotBytes);
    }

    public void RemovePreset()
    {
        if (selectedPresetIndex == null)
            return;


        presets.RemoveAt((int)selectedPresetIndex);
        Destroy(presetsPanel.GetChild((int)selectedPresetIndex).gameObject);

    }

    public void OnApplicationQuit()
    {
        //ukladani

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + filename, FileMode.Create);
        formatter.Serialize(file, presets);
        file.Close();
        Debug.Log("saved to " + Application.persistentDataPath + filename);
    }

    public void ChangeSpoiler(int spoilerIndex)
    {
        cameraController.FocusSpoiler();

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
        cameraController.FocusWheel();

        foreach (Transform wheelHolder in wheelPositions)
        {
            foreach (Transform wheel in wheelHolder)
            {
                wheel.gameObject.SetActive(false);
            }

            wheelHolder.GetChild(wheelIndex).gameObject.SetActive(true);
        }

        selectedWheelIndex = wheelIndex;
    }

    public void ChangeColor(color c, bool blendMods)
    {
        carMaterial.DOColor(c.Color, 1);

        if (spoilers[selectedSpoilerIndex].ColorMaterial != null)
        {
            if (blendMods)
                spoilers[selectedSpoilerIndex].ColorMaterial.DOColor(c.Color, 1);
            else
                spoilers[selectedSpoilerIndex].ColorMaterial.color = c.Color;
        }

        currentColor = c;
    }
}
