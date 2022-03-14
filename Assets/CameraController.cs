using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraSpots;
    int selectedSpot = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedSpot = (selectedSpot == cameraSpots.childCount - 1) ? 0 : selectedSpot + 1;
            transform.position = cameraSpots.GetChild(selectedSpot).position;
            transform.rotation = cameraSpots.GetChild(selectedSpot).rotation;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedSpot = (selectedSpot == 0) ? cameraSpots.childCount - 1 : selectedSpot - 1;
            transform.position = cameraSpots.GetChild(selectedSpot).position;
            transform.rotation = cameraSpots.GetChild(selectedSpot).rotation;
        }
    }
}
