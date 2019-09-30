using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class ARKitController : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    private ARSessionOrigin arOrgin;
    private Pose placementPose;

    private bool placementPoseIsValid = false;

    // Use this for initialization
    private void Start()
    {
        arOrgin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdataPlacementPose();
        UpdatePlacementIndictor();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            placePbject();
        }
    }

    private void placePbject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndictor()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdataPlacementPose()
    {
        var screenCenter = Camera.main.ViewportPointToRay(new Vector3(5f, 0f, 5f));
        var hits = new List<ARRaycastHit>();
        arOrgin.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = Camera.current.transform.forward;
            var cameraBraring = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBraring);
        }
    }
}