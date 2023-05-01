using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRLeftController : MonoBehaviour
{
    public GameObject Reticle;
    public Transform XROriginTransform;
    public InputActionAsset inputActions;
    public float turnAngle = 45.0f;

    private GameObject currReticle = null;
    private InputAction teleportAction;
    private InputAction quickTurnAction;
    private bool teleportTriggered = false;
    private bool quickTurnTriggered = false;

    void Start()
    {
        teleportAction = inputActions.FindAction("Teleport");
        teleportAction.Enable();

        quickTurnAction = inputActions.FindAction("QuickTurn");
        quickTurnAction.Enable();
    }

    void Update()
    {
        Teleport();
        QuickTurn();
    }

    void Teleport()
    {
        Ray myRay = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;

        if (!Physics.Raycast(myRay, out rayHit, 2.0f) || rayHit.collider.name != "Floor")
        {
            if (currReticle != null)
            {
                Destroy(currReticle);
                currReticle = null;
            }
            
            return;
        }

        if (currReticle == null)
        {
            currReticle = Instantiate(Reticle, rayHit.point + new Vector3(0.0f, 0.01f, 0.0f), Reticle.transform.rotation);
        }
        else
        {
            currReticle.transform.position = rayHit.point + new Vector3(0.0f, 0.01f, 0.0f);
        }

        if (teleportAction.ReadValue<float>() > 0.7 && !teleportTriggered)
        {
            XROriginTransform.position = new Vector3(rayHit.point.x, XROriginTransform.position.y, rayHit.point.z);
            teleportTriggered = true;
        }
        else if (teleportAction.ReadValue<float>() <= 0.7)
        {
            teleportTriggered = false;
        }
    }

    void QuickTurn()
    {
        float thumbstickX = quickTurnAction.ReadValue<Vector2>().x;

        if (thumbstickX > 0.9 && !quickTurnTriggered)
        {
            XROriginTransform.Rotate(Vector3.up, turnAngle);
            quickTurnTriggered = true;
        }
        else if (thumbstickX < -0.9 && !quickTurnTriggered)
        {
            XROriginTransform.Rotate(Vector3.up, -turnAngle);
            quickTurnTriggered = true;
        }
        else if (thumbstickX < 0.2 && thumbstickX > -0.2)
        {
            quickTurnTriggered = false;
        }
    }
}
