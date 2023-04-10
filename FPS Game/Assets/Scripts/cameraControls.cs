using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class cameraControls : MonoBehaviour
{
    [SerializeField] int sensHor;
    [SerializeField] int sensVer;

    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    [SerializeField] bool invertY;

    float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get Input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVer;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

        //Input -> rotation float
        if (invertY)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        //Clamp camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

        //rotate on x-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //Rotate on y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
