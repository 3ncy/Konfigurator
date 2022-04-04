using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
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
    private readonly string savefileName = "/presets";
    [SerializeField] Transform presetsPanel;
    [SerializeField] GameObject presetButtonPrefab;
    [SerializeField] Texture2D missingPicTexture;
    [SerializeField] Camera screenshotCam;
    [SerializeField] Camera mainCam;
    [SerializeField] RenderTexture screenshotRenderTexture;
    [SerializeField] Material screenshotMaterial;
    private int? selectedPresetIndex;


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

        if (File.Exists(Application.persistentDataPath + savefileName))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.OpenRead(Application.persistentDataPath + savefileName);
            if (file.Length > 0)
            {
                presets = formatter.Deserialize(file) as List<Preset>;
                file.Close();
                Debug.Log($"loaded {presets.Count} presets from {Application.persistentDataPath + savefileName}");
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

        if (presets.Count > 0)
        {
            foreach (Preset preset in presets)
            {
                AddPresetToUI(preset);
            }
            //mby pokud je souhrna sirka tlacitek vetsi nez sirka scrollview, nastavit pivot content panelu X=0.5.
            // Pokud se to ale mazanim tlacitka snizi, tak to zase nastavit na x=0. A pokud se to pridavanim presetu zvedne, tak zase na 0.5
        }
        #endregion
    }

    /// <summary>
    /// Addes <paramref name="preset"/> to the scrollable UI
    /// </summary>
    /// <param name="preset">Preset to be added</param>
    /// <param name="pic">Icon to be displayed. If null, one will be loaded from the <paramref name="preset"/> from file</param>
    private void AddPresetToUI(Preset preset, Texture2D pic = null)
    {
        GameObject presetButton = Instantiate(presetButtonPrefab);
        presetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 100);
        presetButton.transform.SetParent(presetsPanel.transform, false);

        if (pic == null)
        {
            pic = new Texture2D(256, 256);//tak akorat rozliseni pro textury, bohate staci
            if (File.Exists(preset.IconPath))
                ImageConversion.LoadImage(pic, File.ReadAllBytes(preset.IconPath));
            else
                pic = missingPicTexture;
        }


        presetButton.GetComponent<Image>().sprite = Sprite.Create(pic, new Rect(0, 0, pic.width, pic.height), new Vector2(0.5f, 0.5f));
        presetButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            LoadPreset(preset);
        });
    }

    /// <summary>
    /// Loads the <paramref name="preset"/> car to the viewport
    /// </summary>
    /// <param name="preset">The car preset to load</param>
    public void LoadPreset(Preset preset)
    {
        //Debug.Log(preset.ColorName + preset.WheelsName + preset.SpoilerName + preset.IconPath

        if (selectedPresetIndex != null) //odstraneni ramecky z minuleho oznaceneho presetu
            presetsPanel.GetChild((int)selectedPresetIndex).GetComponent<Outline>().enabled = false;

        selectedPresetIndex = presets.IndexOf(preset);

        presetsPanel.GetChild((int)selectedPresetIndex).GetComponent<Outline>().enabled = true;


        ChangeSpoiler(System.Array.IndexOf(spoilers, spoilers.First(s => s.Name == preset.SpoilerName)), false);
        ChangeWheels(System.Array.IndexOf(wheels, wheels.First(w => w.Name == preset.WheelsName)), false);
        ChangeColor(bodyColors.First(c => c.Name == preset.ColorName), true);
    }

    /// <summary>
    /// Saves current car configuration as a preset
    /// </summary>
    public void AddPreset()
    {
        Texture2D renderResult = new Texture2D(screenshotRenderTexture.width, screenshotRenderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = screenshotRenderTexture;
        renderResult.ReadPixels(new Rect(0, 0, screenshotRenderTexture.width, screenshotRenderTexture.height), 0, 0);
        renderResult.Apply();
        byte[] screenshotBytes = renderResult.EncodeToPNG();

        string iconFilename = Application.dataPath + "/screens/";
        if (!Directory.Exists(iconFilename)) //pokud neexistuje slozka screens
            Directory.CreateDirectory(iconFilename);


        iconFilename += System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";

        File.WriteAllBytes(iconFilename, screenshotBytes);

        Preset preset = new Preset
        {
            ColorName = currentColor.Name,
            WheelsName = wheels[selectedWheelIndex].Name,
            SpoilerName = spoilers[selectedSpoilerIndex].Name,
            IconPath = iconFilename
        };

        presets.Add(preset);

        AddPresetToUI(preset);

        Debug.Log($"saved {preset.ColorName} {preset.WheelsName} {preset.SpoilerName} to {preset.IconPath}");

    }

    /// <summary>
    /// Removes currently selected preset both from UI and the collection
    /// </summary>
    public void RemovePreset()
    {
        if (selectedPresetIndex == null)
            return;

        presets.RemoveAt((int)selectedPresetIndex);
        Destroy(presetsPanel.GetChild((int)selectedPresetIndex).gameObject);
        selectedPresetIndex = null;
    }

    private void SavePreset()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + savefileName, FileMode.Create);
        formatter.Serialize(file, presets);
        file.Close();
        Debug.Log("saved to " + Application.persistentDataPath + savefileName);
    }

    public void OnApplicationQuit()
    {
        //ukladani
        SavePreset();
    }

    /// <summary>
    /// Chenges car's spoiler to the one referenced via <paramref name="spoilerIndex"/>
    /// </summary>
    /// <param name="spoilerIndex">Index of the spoiler to display</param>
    /// <param name="focus">Determines whether camera should focus the newly changed spoiler</param>
    public void ChangeSpoiler(int spoilerIndex, bool focus = true)
    {
        if (focus) cameraController.FocusSpoiler();

        foreach (Transform t in spoilerHolder) 
        {
            t.gameObject.SetActive(false);
        }
        selectedSpoilerIndex = spoilerIndex;
        spoilers[spoilerIndex].Model.SetActive(true);
        ChangeColor(currentColor, false);
    }

    /// <summary>
    /// Changes car's wheels to the ones referenced via <paramref name="wheelIndex"/>
    /// </summary>
    /// <param name="wheelIndex">Index of the wheels to display</param>
    /// <param name="focus">Determines whether camera should focus one of the newly changed wheels</param>
    public void ChangeWheels(int wheelIndex, bool focus = true)
    {
        if (focus) cameraController.FocusWheel();

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

    /// <summary>
    /// Changes car and potential spoiler's color to <paramref name="c"/>
    /// </summary>
    /// <param name="c">Color to change to</param>
    /// <param name="blendMods">Whether should be the colors blended or not</param>
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
