using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct PaintJob
{
    public string Name;
    public Color Color;
}

public class PaintjobButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Button button;

    public void Initialize(PaintJob info, Action onClick, Transform parentPanel)
    {
        icon.color = info.Color;
        button.onClick.AddListener(() => onClick());
        transform.SetParent(parentPanel, false);
    }
}
