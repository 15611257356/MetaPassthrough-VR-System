// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.Events;

namespace PassthroughCameraSamples.MultiObjectDetection
{
    [MetaCodeSample("PassthroughCameraApiSamples-MultiObjectDetection")]
    public class DetectionManager : MonoBehaviour
    {
        [SerializeField] private WebCamTextureManager m_webCamTextureManager;

        [Header("Controls configuration")]
        [SerializeField] private OVRInput.RawButton m_actionButton = OVRInput.RawButton.A;

        [Header("Ui references")]
        [SerializeField] private DetectionUiMenuManager m_uiMenuManager;

        [Header("Placement configureation")]
        [SerializeField] private GameObject m_spwanMarker;
        [SerializeField] private EnvironmentRayCastSampleManager m_environmentRaycast;
        [SerializeField] private float m_spawnDistance = 0.25f;
        [SerializeField] private AudioSource m_placeSound;

        [Header("Sentis inference ref")]
        [SerializeField] private SentisInferenceRunManager m_runInference;
        [SerializeField] private SentisInferenceUiManager m_uiInference;
        [Space(10)]
        public UnityEvent<int> OnObjectsIdentified;

        private bool m_isPaused = true;
        private List<GameObject> m_spwanedEntities = new();
        private bool m_isStarted = false;
        private bool m_isSentisReady = false;
        private float m_delayPauseBackTime = 0;

        // Add: If Passthrough window is hidden
        private bool m_passthroughHidden = false;

        private void Awake()
        {
            OVRManager.display.RecenteredPose += CleanMarkersCallBack;
        }

        private IEnumerator Start()
        {
            var sentisInference = FindAnyObjectByType<SentisInferenceRunManager>();
            while (!sentisInference.IsModelLoaded)
            {
                yield return null;
            }
            m_isSentisReady = true;
        }

        private void Update()
        {
            var hasWebCamTextureData = m_webCamTextureManager.WebCamTexture != null;

            // B: clean all the detections
            if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                ResetDetection();
            }

            // X: Pause detection
            if (OVRInput.GetDown(OVRInput.RawButton.X))
            {
                PauseDetection();
            }

            // A: Begin detecting
            if (OVRInput.GetDown(OVRInput.RawButton.A))
            {
                ResumeDetection();
            }

            // Y: Delete all passthrough windows
            if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                DeleteAllPassthroughWindows();
            }

            

            // Initial situation
            if (!m_isStarted)
            {
                if (hasWebCamTextureData && m_isSentisReady)
                {
                    m_uiMenuManager.OnInitialMenu(m_environmentRaycast.HasScenePermission());
                    m_isStarted = true;
                }
            }
            else
            {
                // put Markers
                if (OVRInput.GetUp(m_actionButton) && m_delayPauseBackTime <= 0 && !m_isPaused)
                {
                    SpwanCurrentDetectedObjects();
                }

                m_delayPauseBackTime -= Time.deltaTime;
                if (m_delayPauseBackTime <= 0)
                {
                    m_delayPauseBackTime = 0;
                }
            }

            // if paused, skip
            if (m_isPaused || !hasWebCamTextureData)
            {
                if (m_isPaused)
                {
                    m_delayPauseBackTime = 0.1f;
                }
                return;
            }

            // begin detecting
            if (!m_runInference.IsRunning())
            {
                m_runInference.RunInference(m_webCamTextureManager.WebCamTexture);
            }
        }

        private void DeleteAllPassthroughWindows()
        {
            GameObject[] windows = GameObject.FindGameObjectsWithTag("PassthroughWindow");
            foreach (var window in windows)
            {
                Destroy(window);
            }
            Debug.Log("All passthrough windows deleted.");
        }

        private void TogglePassthroughWindows()
        {
            GameObject[] windows = GameObject.FindGameObjectsWithTag("PassthroughWindow");
            m_passthroughHidden = !m_passthroughHidden;

            foreach (var window in windows)
            {
                window.SetActive(!m_passthroughHidden);
            }

            Debug.Log($"Passthrough windows {(m_passthroughHidden ? "hidden" : "shown")}");
        }

        private void CleanMarkersCallBack()
        {
            foreach (var e in m_spwanedEntities)
            {
                Destroy(e, 0.1f);
            }
            m_spwanedEntities.Clear();
            OnObjectsIdentified?.Invoke(-1);
        }

        private void SpwanCurrentDetectedObjects()
        {
            var count = 0;
            foreach (var box in m_uiInference.BoxDrawn)
            {
                if (box.WorldPos.HasValue && PlaceMarkerUsingEnvironmentRaycast(box.WorldPos, box.ClassName, box.Width, box.Height))
                {
                    count++;
                }
            }
            if (count > 0)
            {
                m_placeSound.Play();
            }
            OnObjectsIdentified?.Invoke(count);
        }

        private bool PlaceMarkerUsingEnvironmentRaycast(Vector3? position, string className, float boxWidth, float boxHeight)
        {
            if (!position.HasValue)
                return false;

            foreach (var e in m_spwanedEntities)
            {
                var markerClass = e.GetComponent<DetectionSpawnMarkerAnim>();
                if (markerClass)
                {
                    var dist = Vector3.Distance(e.transform.position, position.Value);
                    if (dist < m_spawnDistance && markerClass.GetYoloClassName() == className)
                    {
                        return false;
                    }
                }
            }

            var eMarker = Instantiate(m_spwanMarker);
            m_spwanedEntities.Add(eMarker);

            eMarker.transform.SetPositionAndRotation(position.Value, Quaternion.identity);

            var markerComp = eMarker.GetComponent<DetectionSpawnMarkerAnim>();
            markerComp.SetYoloClassName(className);
            markerComp.BoxWidth = boxWidth;
            markerComp.BoxHeight = boxHeight;

            return true;
        }

        public void OnPause(bool pause)
        {
            m_isPaused = pause;
        }

        /// <summary>
        /// Clean all the markers and quads
        /// </summary>
        public void ResetDetection()
        {
            ClearAllMarkers();
            ClearAllBoxes();
            m_isPaused = true;
            m_uiMenuManager.SetPauseState(m_isPaused);
            Debug.Log("Detection manually reset and paused.");
        }

        /// <summary>
        /// stop detecting but data remains
        /// </summary>
        public void PauseDetection()
        {
            m_isPaused = true;
            m_uiMenuManager.SetPauseState(m_isPaused);
            ClearAllBoxes();
            Debug.Log("Detection paused.");
        }

        /// <summary>
        /// begin  detecting
        /// </summary>
        public void ResumeDetection()
        {
            m_isPaused = false;
            m_uiMenuManager.SetPauseState(m_isPaused);
            Debug.Log("Detection resumed.");
        }

        private void ClearAllMarkers()
        {
            foreach (var e in m_spwanedEntities)
            {
                Destroy(e, 0.1f);
            }
            m_spwanedEntities.Clear();
            OnObjectsIdentified?.Invoke(-1);
        }

        private void ClearAllBoxes()
        {
            if (m_uiInference != null && m_uiInference.BoxDrawn != null)
            {
                m_uiInference.BoxDrawn.Clear();
                m_uiInference.DrawBox(); // refresh UI
            }
        }
    }
}
