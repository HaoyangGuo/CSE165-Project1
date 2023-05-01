using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRRightController : MonoBehaviour
{
    public GameObject Chair;
    public GameObject OculusRift;
    public InputActionAsset inputActions;
    public Material highlightMaterial;

    private GameObject currSelectedObject = null;
    private GameObject currSpawnedObject = null;

    private InputAction spawnChairAction;
    private InputAction spawnOculusRiftAction;
    private InputAction indexTriggerAction;
    private InputAction handTriggerAction;
    private InputAction rightJoyStickAction;

    private bool selectTriggered = false;
    private bool spawnTriggered = false;

    private Renderer currSelectedObjectRenderer;
    private Material[] currSelectedObjectOriginalMaterials;


    // Start is called before the first frame update
    void Start()
    {
        spawnChairAction = inputActions.FindAction("SpawnChair");
        spawnChairAction.Enable();

        spawnOculusRiftAction = inputActions.FindAction("SpawnOculusRift");
        spawnOculusRiftAction.Enable();

        indexTriggerAction = inputActions.FindAction("IndexTrigger");
        indexTriggerAction.Enable();

        handTriggerAction = inputActions.FindAction("HandTrigger");
        handTriggerAction.Enable();

        rightJoyStickAction = inputActions.FindAction("RightJoyStick");
        rightJoyStickAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        spawnObject();
        updateSpawnedObject();

        selectObject();
        updateSelectedObject();
    }

    void spawnObject()
    {
        if (currSelectedObject != null) return;

        Ray myRay = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(myRay, out rayHit, 2.0f))
        {
            return;
        }

        if (!spawnTriggered)
        {
            if (spawnChairAction.IsPressed() && !spawnOculusRiftAction.IsPressed())
            {
                currSpawnedObject = Instantiate(Chair, transform.position + transform.forward * 1.5f,
                    Chair.transform.rotation);
                currSpawnedObject.GetComponent<Rigidbody>().angularDrag = 50.0f;

                spawnTriggered = true;


            }
            else if (!spawnChairAction.IsPressed() && spawnOculusRiftAction.IsPressed())
            {
                currSpawnedObject = Instantiate(OculusRift, transform.position + transform.forward * 1.5f,
                    OculusRift.transform.rotation);
                currSpawnedObject.GetComponent<Rigidbody>().angularDrag = 50.0f;

                spawnTriggered = true;
            }
        }

        if (spawnTriggered && !spawnChairAction.IsPressed() &&
            !spawnOculusRiftAction.IsPressed() && currSpawnedObject == null)
        {
            spawnTriggered = false;
        }
    }

    void updateSpawnedObject()
    {
        if (currSpawnedObject != null)
        {
            currSpawnedObject.transform.position = transform.position + transform.forward * 1.5f;

            Vector2 joystickInput = rightJoyStickAction.ReadValue<Vector2>();
            float rotationSpeed = 50.0f;
            currSpawnedObject.transform.Rotate(Vector3.up, joystickInput.x * rotationSpeed * Time.deltaTime);
            currSpawnedObject.transform.Rotate(Vector3.right, -joystickInput.y * rotationSpeed * Time.deltaTime);

            if (indexTriggerAction.IsPressed())
            {
                currSpawnedObject = null;
            }
        }
    }

    void selectObject()
    {
        if (currSpawnedObject != null) return;

        Ray myRay = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;

        if (!Physics.Raycast(myRay, out rayHit, 2.0f) || rayHit.collider.name.Contains("Floor") ||
            rayHit.collider.name.Contains("Wall"))
        {
            return;
        }

        if (!selectTriggered)
        {
            if (indexTriggerAction.IsPressed() && handTriggerAction.IsPressed())
            {
                currSelectedObject = rayHit.collider.gameObject;
                currSelectedObject.GetComponent<Rigidbody>().angularDrag = 50.0f;
                selectTriggered = true;
            }
        }
    }

    void updateSelectedObject()
    {
        if (currSelectedObject != null && indexTriggerAction.IsPressed() && handTriggerAction.IsPressed())
        {
            currSelectedObject.transform.position = transform.position + transform.forward * 1.5f;
            Vector2 joystickInput = rightJoyStickAction.ReadValue<Vector2>();
            float rotationSpeed = 50.0f;
            currSelectedObject.transform.Rotate(Vector3.up, joystickInput.x * rotationSpeed * Time.deltaTime);
            currSelectedObject.transform.Rotate(Vector3.right, -joystickInput.y * rotationSpeed * Time.deltaTime);

            if (spawnChairAction.IsPressed())
            {
                currSelectedObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            }
            else if (spawnOculusRiftAction.IsPressed())
            {
                currSelectedObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
        else
        {
            currSelectedObject = null;
            selectTriggered = false;
        }
    }

}