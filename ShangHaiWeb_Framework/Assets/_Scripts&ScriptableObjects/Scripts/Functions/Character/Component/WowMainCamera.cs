/* 
*** Script Made by : Hesa ***
*/
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WowMainCamera : MonoBehaviour
{
    public Transform target;

    public float targetHeight = 0;

    public float maxDistance = 100;

    public float minDistance = -50;

    public float xSpeed = 250.0f;

    public float ySpeed = 120.0f;

    public int yMinLimit = -90;

    public int yMaxLimit = 90;

    public int zoomRate = 40;

    public float zoomDampening = 10.0f;

    public float x;

    public float y;

    public float currentDistance;

    public float desiredDistance;

    private float correctedDistance;

    public float dragSpeed = 10;

    public bool allowControl = true;

    public bool allowZoom = true;
    
    public bool allowDrag = true;

    public bool AgreewithViewOnEnable = false;

    //private float damping = 1f;

    private void OnEnable()
    {
        if (AgreewithViewOnEnable)
        {
            AgreewithView();
        }
    }
    private void AgreewithView()
    {
        bool tempControl = allowControl;
        bool tempZoom = allowZoom;
        allowZoom = false;
        allowControl = false;
        x = transform.eulerAngles.y;
        y = transform.eulerAngles.x;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - targetHeight, transform.position.z);
        GameObject tempTarget = new GameObject();
        pos += transform.forward * desiredDistance;
        tempTarget.transform.position = pos;
        Transform originTarget = target;
        target = tempTarget.transform;
        tempTarget.transform.DOMove(originTarget.position, 1f).OnComplete(() =>
        {
            target = originTarget;
            Destroy(tempTarget);
            allowZoom = tempZoom;
            allowControl = tempControl;
        });
    }
    void LateUpdate()
    {
        if (allowControl)
        {
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }
        }

        y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

        x = x % 360;

        y = y % 360;

        // set camera rotation
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        //if (allowDrag)
        //{
        //    if (Input.GetMouseButton(2))
        //    {

        //        target.transform.Translate(-Input.GetAxis("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxis("Mouse Y") * Time.deltaTime * dragSpeed, 0, this.transform);

        //    }
        //}

        // calculate the desired distance
        if (allowZoom)
        {
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * 20;
        }
        //gameObject.camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate*50;

        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

        correctedDistance = desiredDistance;

        // calculate desired camera position
        Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + new Vector3(0, -targetHeight, 0));

        //target.transform.eulerAngles = transform.eulerAngles;

        // if there was a collision, correct the camera position and calculate the corrected distance
        bool isCorrected = false;


        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

        // recalculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -targetHeight - 0.05f, 0));

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        transform.rotation = rotation;

        transform.position = position;


    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }


}