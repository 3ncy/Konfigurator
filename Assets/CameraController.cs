using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraSpots; //tohle by slo dat jako array transformu tech spotu a ne getovat, pokud je to velkej vykonostni problem
    int selectedSpot = 0;
    [SerializeField] Transform car;
    [SerializeField] float speedX;
    [SerializeField] float speedY;

    public GameObject a;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedSpot = (selectedSpot == 0) ? cameraSpots.childCount - 1 : selectedSpot - 1;
            //transform.SetPositionAndRotation(cameraSpots.GetChild(selectedSpot).position, cameraSpots.GetChild(selectedSpot).rotation);
            transform.DOMove(cameraSpots.GetChild(selectedSpot).position, 1.5f);
            transform.DORotateQuaternion(cameraSpots.GetChild(selectedSpot).rotation, 1.5f);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedSpot = (selectedSpot == cameraSpots.childCount - 1) ? 0 : selectedSpot + 1;
            //transform.SetPositionAndRotation(cameraSpots.GetChild(selectedSpot).position, cameraSpots.GetChild(selectedSpot).rotation);
            transform.DOMove(cameraSpots.GetChild(selectedSpot).position, 1.5f);
            transform.DORotateQuaternion(cameraSpots.GetChild(selectedSpot).rotation, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.LookAt(car);
        }


        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(car.transform.position, Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * speedX);

            //transform.RotateAround(car.transform.position, Vector3.MoveTowards(transform.right, car.transform.position, 20), Input.GetAxis("Mouse Y") * Time.deltaTime * speedY);


            a.transform.right = transform.right;
            float moveDegrees = -Input.GetAxis("Mouse Y") * Time.deltaTime * speedY;

            float cameraAngle = Quaternion.Angle(transform.rotation, a.transform.rotation);
            Debug.Log(cameraAngle > 1 && cameraAngle < 89);

            //transform.RotateAround(car.transform.position, a.transform.right, (transform.localRotation.eulerAngles.x > 0 && transform.localRotation.eulerAngles.x < 90) ? degrees : 0);
            //transform.RotateAround(car.transform.position, a.transform.right, degrees);
            transform.RotateAround(car.transform.position, a.transform.right, (cameraAngle + moveDegrees > 1 && cameraAngle + moveDegrees < 89) ? moveDegrees : 0);





        }
    }
}
