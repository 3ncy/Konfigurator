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
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;

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


    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0) // zoomovani kamery
        {
            transform.position += transform.forward * (Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime);
        }


        if (Input.GetMouseButtonDown(0))
        {
            //abych si omylem neposunul kameru kdyz klikam
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            moving = true;
        }
        if (Input.GetMouseButton(0) && moving)
        {
            //abych si omylem neposunul kameru kdyz klikam
            if (EventSystem.current.IsPointerOverGameObject())
                return;


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
            degX = Mathf.Lerp(degX, 0, Time.deltaTime * deceleration);
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

    public void FocusSpoiler()
    {
        transform.DOMove(spoilerCamSpot.position, 1f);
        transform.DORotateQuaternion(spoilerCamSpot.rotation, 1f);
    }

    public void FocusWheel()
    {
        //todo: kdyz byde cas, tak zaridit aby to jelo k *nejblizsimu* kolu, aby se omezilo prejizdeni skrz auto
        transform.DOMove(wheelCamSpot.position, 1f);
        transform.DORotateQuaternion(wheelCamSpot.rotation, 1f);
    }
}
