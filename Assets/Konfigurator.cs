using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Konfigurator : MonoBehaviour
{
    [SerializeField] Material carMaterial;
    [SerializeField] Color[] bodyColors;
    [SerializeField] GameObject carBody;
    //[SerializeField] GameObject[] wheelObjets;
    [SerializeField] Transform[] wheelPositions;
    int _wheelIndex;


    int _i = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //tohle se obv presune do UI
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _i = (_i == bodyColors.Length - 1) ? 0 : _i + 1;
            ChangeColor(bodyColors[_i]);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _wheelIndex = (_wheelIndex == wheelPositions[0].childCount - 1) ? 0 : _wheelIndex + 1;

            ChangeWheels(_wheelIndex);
        }
    }

    public void ChangeWheels(int wheelIndex) //mozna ne pres index? (-_-)?  //a tohle je prej "scratching head" emoticon
                                             // ted me napadlo ze v souvislosti s dynamickym ui, tak bych mohl mit list prefabu a pri startu aplikcae
                                             // pridat ty prefaby kolum (a mby si na ne keepovat referenci??)
    {
        foreach(Transform wheelHolder in wheelPositions)
        {
            foreach(Transform wheel in wheelHolder)
            {
                wheel.gameObject.SetActive(false);
            }

            wheelHolder.GetChild(wheelIndex).gameObject.SetActive(true);
        }
    }

    public void ChangeColor(Color c)
    {
        carMaterial.color = c;
    }
}
