using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConfiguratorUI : MonoBehaviour
{
    [SerializeField] ModButton modButtonPrefab;
    [SerializeField] PaintjobButton paintjobButton;
    [Header("Button Panels")]
    [SerializeField] VerticalLayoutGroup spoilersPanel;
    [SerializeField] VerticalLayoutGroup wheelsPanel;
    [SerializeField] HorizontalLayoutGroup colorsPanel;
    
    [Space(5f)]

    [SerializeField] ConfigurationManager configManager;
    [SerializeField] PresetManager presetManager;

    internal void InitializeUI(CarConfiguration carConfiguration)
    {
        //init buttons
        foreach (PaintJob c in carConfiguration.PaintJobs)
        {
            PaintjobButton paintjob = Instantiate(paintjobButton);
            paintjob.Initialize(c, () => configManager.ChangeColor(c, true), colorsPanel.transform);
        }

        foreach (Mod spoiler in carConfiguration.Spoilers)
        {
            ModButton modButton = Instantiate(modButtonPrefab);
            modButton.Initialize(spoiler, () => configManager.ChangeSpoiler(System.Array.IndexOf(carConfiguration.Spoilers, spoiler)), spoilersPanel.transform);
        }

        foreach (Mod wheel in carConfiguration.Wheels)
        {
            ModButton modButton = Instantiate(modButtonPrefab);
            modButton.Initialize(wheel, () => configManager.ChangeWheels(System.Array.IndexOf(carConfiguration.Wheels, wheel)), wheelsPanel.transform);
        }

        carConfiguration.currentPaintJob = carConfiguration.PaintJobs.First(c => c.Color == carConfiguration.Material.color);
    }
}
