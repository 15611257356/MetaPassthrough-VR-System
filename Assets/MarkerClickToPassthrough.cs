using System.Collections.Generic;
using UnityEngine;
using PassthroughCameraSamples.MultiObjectDetection;

public class MarkerClickToPassthrough : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject PassthroughWindowPrefab;
    public GameObject DeleteButtonPrefab;

    private GameObject previewPassthroughWindow;
    private static List<GameObject> permanentPassthroughWindows = new List<GameObject>();

    // Show preview window when hovering marker
    public void ShowPreview()
    {
        if (previewPassthroughWindow != null) return;
        previewPassthroughWindow = CreatePassthroughWindow(false);
    }

    // Hide preview window when unhovering marker
    public void HidePreview()
    {
        if (previewPassthroughWindow != null)
        {
            Destroy(previewPassthroughWindow);
            previewPassthroughWindow = null;
        }
    }

    // On marker click → create or fix a window
    public void ClickMarker()
    {
        if (previewPassthroughWindow != null)
        {
            var window = previewPassthroughWindow;
            previewPassthroughWindow = null;

            permanentPassthroughWindows.Add(window);
            window.name = "[Permanent]" + window.name;
        }
        else
        {
            var window = CreatePassthroughWindow(true);
            permanentPassthroughWindows.Add(window);
            window.name = "[Permanent]" + window.name;
        }
    }

    private GameObject CreatePassthroughWindow(bool permanent)
    {
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;

        GameObject window = Instantiate(PassthroughWindowPrefab, spawnPos, spawnRot);

        // Set passthrough layer
        int passthroughLayer = LayerMask.NameToLayer("passthrough");
        SetLayerRecursively(window, passthroughLayer);

        // Scale window using marker data
        var markerData = GetComponentInParent<DetectionSpawnMarkerAnim>();
        if (markerData != null)
        {
            float worldWidth = markerData.BoxWidth / 1000f;
            float worldHeight = markerData.BoxHeight / 1000f;
            window.transform.localScale = new Vector3(worldWidth, worldHeight, 1);
        }

        // Rotate window to face the camera
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        if (cameraRig != null)
        {
            Transform cam = cameraRig.centerEyeAnchor;
            if (cam != null)
            {
                window.transform.LookAt(cam);
                window.transform.Rotate(0, 180, 0);
            }
        }

        // Register DisplayQuad
        Transform quadTransform = window.transform.Find("DisplayQuad");
        if (quadTransform != null)
        {
            if (quadTransform.GetComponent<PassthroughProjectionSurface>() == null)
                quadTransform.gameObject.AddComponent<PassthroughProjectionSurface>();
        }

        // Add delete button
        AddDeleteButton(window);

        if (!permanent)
        {
            window.name = "[Preview]" + window.name;
        }

        window.SetActive(false);
        window.SetActive(true);

        return window;
    }

    private void AddDeleteButton(GameObject window)
    {
        if (DeleteButtonPrefab == null) return;

        // Compute top-right corner in world space
        MeshRenderer mr = window.GetComponentInChildren<MeshRenderer>();
        if (mr == null)
        {
            Transform dq = window.transform.Find("DisplayQuad");
            if (dq != null) mr = dq.GetComponent<MeshRenderer>();
        }
        if (mr == null) return;

        Bounds b = mr.bounds;
        Vector3 topRightWorld = b.center + window.transform.right * b.extents.x + window.transform.up * b.extents.y;
        Vector3 placePos = topRightWorld + window.transform.forward * 0.01f;

        // Instantiate delete button
        GameObject button = Instantiate(DeleteButtonPrefab, placePos, Quaternion.identity);
        button.name = "DeleteButton";

        if (Camera.main != null)
            button.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

        float worldButtonSize = 0.03f;
        button.transform.localScale = Vector3.one * worldButtonSize;

        BoxCollider bc = button.GetComponent<BoxCollider>();
        if (bc == null) bc = button.AddComponent<BoxCollider>();
        bc.isTrigger = false;
        bc.size = Vector3.one;

        // Force to Default layer for raycast detection
        int defaultLayer = LayerMask.NameToLayer("Default");
        button.layer = defaultLayer;
        foreach (Transform t in button.transform)
            t.gameObject.layer = defaultLayer;

        var deleter = button.GetComponent<DeleteButton>();
        if (deleter != null)
        {
            deleter.Init(window);
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    public static void RemovePermanentWindow(GameObject window)
    {
        permanentPassthroughWindows.Remove(window);
    }
}
