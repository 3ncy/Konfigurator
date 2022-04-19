using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConfiguration : MonoBehaviour
{
    [SerializeField] public Mod[] Wheels;
    [SerializeField] public Transform[] WheelPositions;
    public int selectedWheelIndex;
    [SerializeField] public Mod[] Spoilers;
    [SerializeField] public Transform SpoilerHolder;
    public int selectedSpoilerIndex;
    [SerializeField] public PaintJob[] PaintJobs;
    public PaintJob currentPaintJob;
    [SerializeField] public Material Material;
}
