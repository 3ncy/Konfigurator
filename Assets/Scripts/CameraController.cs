using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraSpots; //tohle by slo dat jako array transformu tech spotu a ne getovat, pokud je to velkej vykonostni problem
    [SerializeField] Transform car;
    [SerializeField] float speedX;
    [SerializeField] float speedY;
    [SerializeField] float scrollSpeed;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    [SerializeField] Transform wheelCamSpot;
    [SerializeField] Transform spoilerCamSpot;

    [SerializeField] Transform orbitCenter;

    [SerializeField] float deceleration;
    [SerializeField] float speedFluctuation;
    float degX;
    float avgDegX;
    float degY;
    float avgDegY;
    bool moving = false;

    private bool isCursorValid => !EventSystem.current.IsPointerOverGameObject();

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0) // zoomovani kamery
        {
            float zoom = Input.mouseScrollDelta.y;
            if (zoom != 0 && isCursorValid)
            {
                ZoomCamera(zoom);
            }
        }


        if (Input.GetMouseButtonDown(0) && isCursorValid)
        {
            moving = true;
        }
        if (Input.GetMouseButton(0) && moving && isCursorValid)
        {
            transform.LookAt(car);


            //orbitovani
            degX = Input.GetAxis("Mouse X") * Time.deltaTime * speedX;
            avgDegX = Mathf.Lerp(avgDegX, degX, Time.deltaTime * speedFluctuation); //popr Mathf.SmoothDamp


            //tilteni
            orbitCenter.right = transform.right;
            degY = -Input.GetAxis("Mouse Y") * Time.deltaTime * speedY;
            avgDegY = Mathf.Lerp(avgDegY, degY, Time.deltaTime * speedFluctuation);

        }
        else
        {
            if (moving)
            {
                degX = avgDegX;
                degY = avgDegY;
                moving = false;
            }
            degX = Mathf.Lerp(degX, 0, Time.deltaTime * deceleration); //brzdeni setrvacnosti
            degY = Mathf.Lerp(degY, 0, Time.deltaTime * deceleration);

            if (Mathf.Abs(degX) < 0.00001f) degX = 0;
            if (Mathf.Abs(degY) < 0.00001f) degY = 0;
        }


        //orbitovani
        transform.RotateAround(car.position, Vector3.up, degX);

        //titleni
        float cameraAngle = Quaternion.Angle(transform.rotation, orbitCenter.rotation);
        transform.RotateAround(car.transform.position, transform.right, (cameraAngle + degY > 1 && cameraAngle + degY < 89) ? degY : 0);

        if (transform.position.y < 0.1) //vetsinu casu to funguje bez toho, ale kdyz se hra malinko kousne a deltaTime se zvedne,tak se kamera mohla dostat pod zem
        {
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            transform.LookAt(car);
        }
    }

    private void ZoomCamera(float zoom)
    {
        Vector3 zoomDir = scrollSpeed * zoom * transform.forward;
        float dist = Vector3.Distance(transform.position + zoomDir, car.position);
        if (dist > minZoom && dist < maxZoom)
            transform.position += zoomDir;
    }


    /// <summary>
    /// Changes camera position to get the car Spoiler in the frame
    /// </summary>
    public void FocusSpoiler()
    {
        transform.DOMove(spoilerCamSpot.position, 1f);
        transform.DORotateQuaternion(spoilerCamSpot.rotation, 1f);
    }

    /// <summary>
    /// Changes camera position to get one of the cars' Wheels in the frame
    /// </summary>
    public void FocusWheel()
    {
        transform.DOMove(wheelCamSpot.position, 1f);
        transform.DORotateQuaternion(wheelCamSpot.rotation, 1f);
    }
}
