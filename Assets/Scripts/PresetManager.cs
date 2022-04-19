using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class PresetManager : MonoBehaviour
{
    [SerializeField] ScreenshotManager screenshotManager;
    [SerializeField] ConfigurationManager configurationManager;
    private readonly string savefileName = "/presets";
    [SerializeField] GameObject presetButtonPrefab;
    [SerializeField] Transform presetsPanel;
    [SerializeField] Texture2D missingPicTexture;

    private List<Preset> presets;
    private int? selectedPresetIndex;


    private void Start()
    {
        LoadPresets();
    }

    public void OnApplicationQuit()
    {
        SavePresets();
    }


    /// <summary>
    /// Loads all saved presets from <see cref="savefileName"/>
    /// </summary>
    /// <returns>List of loaded <see cref="Preset"/>s</returns>
    public void LoadPresets()
    {
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
        }
    }


    /// <summary>
    /// Adds <paramref name="preset"/> to the scrollable UI
    /// </summary>
    /// <param name="preset">Preset to be added</param>
    /// <param name="pic">Icon to be displayed. If null, one will be loaded from the <paramref name="preset"/> from file</param>
    private void AddPresetToUI(Preset preset, Texture2D pic = null)
    {
        GameObject presetButton = Instantiate(presetButtonPrefab);
        //presetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 100);
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

            //odstraneni ramecku z minuleho oznaceneho presetu a oznaceni aktualniho presetu
            if (selectedPresetIndex != null) 
                presetsPanel.GetChild((int)selectedPresetIndex).GetComponent<Outline>().enabled = false;

            selectedPresetIndex = presets.IndexOf(preset);

            presetsPanel.GetChild((int)selectedPresetIndex).GetComponent<Outline>().enabled = true;


            //loadnuti barev atd na auto ve viewportu
            configurationManager.LoadPreset(preset);
        });
    }


    /// <summary>
    /// Saves <see cref="presets"/> to a file 
    /// </summary>
    private void SavePresets()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + savefileName, FileMode.Create);
        formatter.Serialize(file, presets);
        file.Close();
        Debug.Log("saved to " + Application.persistentDataPath + savefileName);
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


    /// <summary>
    /// Saves current car configuration as a preset
    /// </summary>
    public void CreatePreset()
    {
        //timhle si nejsem jistej, asi by si k tomu nemel PresetManager pristupovat takhle na primo, ale nevim jak to predat kdyz tuhle metodu volam z UI z buttonu
        // mozna by slo ji volat z ConfiguratorUI classky, ale ta by se spis mela starat o configurator, ne o presety
        //  nebo mit PresetUI odkud by se to volalo, ale kde by to zase PresetUI vzal tyhle data
        PaintJob paintJob = configurationManager.currentCarConfig.currentPaintJob;
        Mod wheel = configurationManager.currentCarConfig.Wheels[configurationManager.currentCarConfig.selectedWheelIndex];
        Mod spoiler = configurationManager.currentCarConfig.Spoilers[configurationManager.currentCarConfig.selectedSpoilerIndex];



        string iconFilename = screenshotManager.TakeAndSaveScreenshot();

        Preset preset = new Preset
        {
            ColorName = paintJob.Name,
            WheelsName = wheel.Name,
            SpoilerName = spoiler.Name,
            IconPath = iconFilename
        };

        presets.Add(preset);

        AddPresetToUI(preset);

        Debug.Log($"saved {preset.ColorName} {preset.WheelsName} {preset.SpoilerName} to {preset.IconPath}");
    }
}
