using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraSpots; //tohle by slo dat jako array transformu tech spotu a ne getovat, pokud je to velkej vykonostni problem
    //int selectedSpot = 0;
    [SerializeField] Transform car;
    [SerializeField] float speedX;
    [SerializeField] float speedY;
    [SerializeField] float scrollSpeed;
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;


    [SerializeField] Transform wheelCamSpot;
    [SerializeField] Transform spoilerCamSpot;

    public Transform orbitCenter;

    [SerializeField] Camera camera;

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0) // zoomovani kamery
        {
            transform.position += transform.forward * (Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime);
        }


        if (Input.GetMouseButton(0))
        {
            //abych si omylem neposunul kameru kdyz klikam
            if (EventSystem.current.IsPointerOverGameObject())
                return;


            transform.LookAt(orbitCenter);


            transform.RotateAround(car.transform.position, Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * speedX);


            orbitCenter.right = transform.right;

            float moveDegrees = -Input.GetAxis("Mouse Y") * Time.deltaTime * speedY;

            float cameraAngle = Quaternion.Angle(transform.rotation, orbitCenter.rotation);
            transform.RotateAround(car.transform.position, transform.right,
                (cameraAngle + moveDegrees > 1 && cameraAngle + moveDegrees < 89) ? moveDegrees : 0);
        }

    }

    public void FocusSpoiler()
    {
        transform.DOMove(spoilerCamSpot.position, 1f);
        transform.DORotateQuaternion(spoilerCamSpot.rotation, 1f);
    }

    public void FocusWheel()
    {
        transform.DOMove(wheelCamSpot.position, 1f);
        transform.DORotateQuaternion(wheelCamSpot.rotation, 1f);
    }
}
