using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public class ConfigurationManager : MonoBehaviour
{
    [SerializeField] ConfiguratorUI configuratorUI;
    [SerializeField] PresetManager presetManager;
    [SerializeField] public CarConfiguration currentCarConfig; //tohle je public jenom prechodne, nez prijdu na to, jak to udelat nejspis
    [SerializeField] CameraController cameraController;

    private void Start()
    {
        configuratorUI.InitializeUI(currentCarConfig);
    }



    /// <summary>
    /// Loads a car <paramref name="preset"/> to the viewport
    /// </summary>
    /// <param name="preset">The car preset to load</param>
    public void LoadPreset(Preset preset)
    {
        //Debug.Log(preset.ColorName + preset.WheelsName + preset.SpoilerName + preset.IconPath

        ChangeSpoiler(System.Array.IndexOf(currentCarConfig.Spoilers, currentCarConfig.Spoilers.First(s => s.Name == preset.SpoilerName)), false);
        ChangeWheels(System.Array.IndexOf(currentCarConfig.Wheels, currentCarConfig.Wheels.First(w => w.Name == preset.WheelsName)), false);
        ChangeColor(currentCarConfig.PaintJobs.First(c => c.Name == preset.ColorName), true);
    }



    /// <summary>
    /// Chenges car's spoiler to the one referenced via <paramref name="spoilerIndex"/>
    /// </summary>
    /// <param name="spoilerIndex">Index of the spoiler to display</param>
    /// <param name="focus">Determines whether camera should focus the newly changed spoiler</param>
    public void ChangeSpoiler(int spoilerIndex, bool focus = true)
    {
        if (focus) cameraController.FocusSpoiler();

        foreach (Transform t in currentCarConfig.SpoilerHolder)
        {
            t.gameObject.SetActive(false);
        }
        currentCarConfig.selectedSpoilerIndex = spoilerIndex;
        currentCarConfig.Spoilers[spoilerIndex].Model.SetActive(true);
        ChangeColor(currentCarConfig.currentPaintJob, false);
    }


    /// <summary>
    /// Changes car's wheels to the ones referenced via <paramref name="wheelIndex"/>
    /// </summary>
    /// <param name="wheelIndex">Index of the wheels to display</param>
    /// <param name="focus">Determines whether camera should focus one of the newly changed wheels</param>
    public void ChangeWheels(int wheelIndex, bool focus = true)
    {
        if (focus) cameraController.FocusWheel();

        foreach (Transform wheelHolder in currentCarConfig.WheelPositions)
        {
            foreach (Transform wheel in wheelHolder)
            {
                wheel.gameObject.SetActive(false);
            }

            wheelHolder.GetChild(wheelIndex).gameObject.SetActive(true);
        }

        currentCarConfig.selectedWheelIndex = wheelIndex;
    }


    /// <summary>
    /// Changes car and potential spoiler's color to <paramref name="paintJob"/>
    /// </summary>
    /// <param name="paintJob">Color to change to</param>
    /// <param name="blendMods">Whether should be the colors blended or not</param>
    public void ChangeColor(PaintJob paintJob, bool blendMods)
    {
        currentCarConfig.Material.DOColor(paintJob.Color, 1);

        if (currentCarConfig.Spoilers[currentCarConfig.selectedSpoilerIndex].ColorMaterial != null)
        {
            if (blendMods)
                currentCarConfig.Spoilers[currentCarConfig.selectedSpoilerIndex].ColorMaterial.DOColor(paintJob.Color, 1);
            else
                currentCarConfig.Spoilers[currentCarConfig.selectedSpoilerIndex].ColorMaterial.color = paintJob.Color;
        }

        currentCarConfig.currentPaintJob = paintJob;
    }
}
