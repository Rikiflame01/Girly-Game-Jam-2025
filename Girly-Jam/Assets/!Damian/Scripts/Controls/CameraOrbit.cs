using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    public Transform pivot;

    public InputActionAsset inputActions;

    public float rotationSpeed = 0.1f;

    private float fixedY;

    private InputAction rotateAction;
    private InputAction mouseDeltaAction;

    void Start()
    {
        fixedY = transform.position.y;

        var cameraMap = inputActions.FindActionMap("Camera");
        if (cameraMap != null)
        {
            cameraMap.Enable();
        }
        else
        {
            Debug.LogError("Camera action map not found.");
            return;
        }

        rotateAction = cameraMap.FindAction("Rotate");
        mouseDeltaAction = cameraMap.FindAction("MouseDelta");

        if (rotateAction == null || mouseDeltaAction == null)
        {
            Debug.LogError("Rotate or MouseDelta action not found.");
        }

        transform.LookAt(pivot.position);
    }

    void Update()
    {
        if (rotateAction != null && rotateAction.IsPressed())
        {
            Vector2 mouseDelta = mouseDeltaAction.ReadValue<Vector2>();
            float horizontalRotation = mouseDelta.x * rotationSpeed;

            Vector3 currentPosition = transform.position;

            Vector3 offset = currentPosition - pivot.position;

            offset = Quaternion.Euler(0, horizontalRotation, 0) * offset;

            Vector3 newPosition = pivot.position + offset;
            newPosition.y = fixedY;

            transform.position = newPosition;

            transform.LookAt(pivot.position);
        }
    }
}