using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private bool isSelected = false;
    private bool isDragging = false;
    private Camera mainCamera;
    private Plane groundPlane;
    private Rigidbody rb;
    private int flatLayerMask;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        flatLayerMask = LayerMask.GetMask("Flat");
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isSelected = !isSelected;
                    Debug.Log($"Object {(isSelected ? "selected" : "deselected")}: {gameObject.name}");

                    if (isSelected)
                    {
                        isDragging = true;
                        SnapToMousePosition();
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.None;
                        isDragging = false;
                    }
                }
                else if (isSelected)
                {
                    rb.constraints = RigidbodyConstraints.None;
                    isSelected = false;
                    isDragging = false;
                    Debug.Log("Object deselected by clicking elsewhere.");
                }
            } else if (isSelected) {
                rb.constraints = RigidbodyConstraints.None;
                isSelected = false;
                isDragging = false;
                Debug.Log("Object deselected by clicking elsewhere.");
            }
        }

        if (isSelected && isDragging)
        {
            SnapToMousePosition();
        }

        if (isSelected)
        {
            HandleRotation();
        }
    }

    void SnapToMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            targetPoint.y = transform.position.y;
            transform.position = targetPoint;
            CheckCollisions();
        }
    }

    void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 currentRotation = transform.eulerAngles;
            float targetYRotation = Mathf.Round(currentRotation.y / 90f) * 90f + 90f;

            transform.eulerAngles = new Vector3(currentRotation.x, targetYRotation, currentRotation.z);
        }
    }

    void CheckCollisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2f, transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject)
            {
                Vector3 direction = transform.position - hitCollider.transform.position;
                if (direction.magnitude < 1f)
                {
                    transform.position += direction.normalized * Time.deltaTime * 5f;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
    }
}