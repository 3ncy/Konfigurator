using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


public class ModButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Button button;


    public void Initialize(Mod mod, Action onClick, Transform parentPanel)
    {
        icon.sprite = mod.Icon;
        button.onClick.AddListener(() => onClick());
        transform.SetParent(parentPanel, false);
    }
}
