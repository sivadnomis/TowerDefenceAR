using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField] [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    public GameObject findingSquare;
    //public GameObject findingLabel;

    public GameObject foundSquare;
    //public GameObject foundLabel;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    public ARRaycastManager m_RaycastManager;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    private void Start()
    {
        if (Application.isEditor)
        {
            spawnedObject = Instantiate(m_PlacedPrefab, Camera.main.transform.position+Camera.main.transform.forward+Vector3.down, Quaternion.Euler(Vector3.zero));

            foundSquare.SetActive(false);
            //foundLabel.SetActive(false);
            findingSquare.SetActive(false);
            //findingLabel.SetActive(false);

            enabled = false;
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif

        touchPosition = default;
        return false;
    }

    void Update()
    {
        var _screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 1);

        if (TryGetTouchPosition(out Vector2 touchPosition))
        {
            if (m_RaycastManager.Raycast(_screenCentre, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                if (spawnedObject == null)
                {
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);

                    foundSquare.SetActive(false);
                    //foundLabel.SetActive(false);
                    findingSquare.SetActive(false);
                    //findingLabel.SetActive(false);

                    enabled = false;
                }
                else
                {
                    spawnedObject.transform.position = hitPose.position;
                }
            }
        }
        else
        {
            if (m_RaycastManager.Raycast(_screenCentre, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                foundSquare.SetActive(true);
                //foundLabel.SetActive(true);
                findingSquare.SetActive(false);
                //findingLabel.SetActive(false);

                foundSquare.transform.position = hitPose.position;
                foundSquare.transform.rotation = hitPose.rotation;
            }
            else
            {
                foundSquare.SetActive(false);
                //foundLabel.SetActive(false);
                findingSquare.SetActive(true);
                //findingLabel.SetActive(true);

                findingSquare.transform.position = Camera.main.ScreenToWorldPoint(_screenCentre);
                findingSquare.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}