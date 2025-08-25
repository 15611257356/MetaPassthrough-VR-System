using UnityEngine;

public class DeleteButton : MonoBehaviour
{
    private GameObject targetQuad;
    private OVRCameraRig ovrRig;
    private Transform rightHandAnchor;
    private Transform leftHandAnchor;

    [Header("Raycast")]
    public float rayDistance = 100f;
    public LayerMask hitMask = ~0;

    // Assign target Quad when this button is created
    public void Init(GameObject quad)
    {
        targetQuad = quad;
    }

    void Awake()
    {
        // Cache OVRCameraRig and hand anchors
        ovrRig = FindObjectOfType<OVRCameraRig>();
        if (ovrRig != null)
        {
            rightHandAnchor = ovrRig.rightHandAnchor;
            leftHandAnchor = ovrRig.leftHandAnchor;
        }

        // Ensure BoxCollider exists
        var bc = GetComponent<BoxCollider>();
        if (bc == null) bc = gameObject.AddComponent<BoxCollider>();
        bc.isTrigger = false;
    }

    void LateUpdate()
    {
        if (targetQuad != null)
        {
            MeshRenderer quadRenderer = targetQuad.GetComponentInChildren<MeshRenderer>();
            if (quadRenderer != null)
            {
                Bounds bounds = quadRenderer.bounds;
                Vector3 topRight = bounds.center + targetQuad.transform.right * bounds.extents.x + targetQuad.transform.up * bounds.extents.y;
                transform.position = topRight + targetQuad.transform.forward * 0.01f;
            }

            if (Camera.main != null)
            {
                transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            }

            float worldSize = 0.05f;
            transform.localScale = Vector3.one * worldSize;

            var bc = GetComponent<BoxCollider>();
            if (bc != null)
            {
                bc.size = Vector3.one;
            }
        }
    }

    void Update()
    {
        HandleRaycastClick();
    }

    void HandleRaycastClick()
    {
        if (rightHandAnchor == null || leftHandAnchor == null)
        {
            ovrRig = FindObjectOfType<OVRCameraRig>();
            if (ovrRig != null)
            {
                rightHandAnchor = ovrRig.rightHandAnchor;
                leftHandAnchor = ovrRig.leftHandAnchor;
            }
        }

        if (rightHandAnchor != null)
        {
            CheckControllerHit(rightHandAnchor, OVRInput.Controller.RTouch, OVRInput.Button.PrimaryIndexTrigger);
        }

        if (leftHandAnchor != null)
        {
            CheckControllerHit(leftHandAnchor, OVRInput.Controller.LTouch, OVRInput.Button.PrimaryIndexTrigger);
        }
    }

    void CheckControllerHit(Transform handAnchor, OVRInput.Controller controller, OVRInput.Button triggerButton)
    {
        Vector3 origin = handAnchor.position;
        Vector3 dir = handAnchor.forward;

        Ray ray = new Ray(origin, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, hitMask))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (OVRInput.GetDown(triggerButton, controller))
                {
                    DeleteTarget();
                }
            }
        }
    }

    void DeleteTarget()
    {
        if (targetQuad != null)
        {
            Destroy(targetQuad);
            targetQuad = null;
        }

        Destroy(gameObject);
    }
}
