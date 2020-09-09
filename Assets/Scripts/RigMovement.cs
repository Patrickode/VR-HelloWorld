using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RigMovement : MonoBehaviour
{
    [SerializeField] private XRRig rig = null;
    [SerializeField] private CapsuleCollider coll = null;
    [SerializeField] private Rigidbody rBody = null;
    [Tooltip("Extra height to add to the collider. 0 = exactly level with the player's eyes.")]
    [SerializeField] private float extraCollHeight = 0.2f;
    [SerializeField] [Range(0, 0.9f)] private float minTriggerPress = 0.25f;
    [SerializeField] private float bodyAccel = 5f;
    [SerializeField] private float maxBodySpeed = 5f;
    private float bodyDrag = 0f;

    private XRNode leftHandNode = XRNode.LeftHand;
    private XRNode rightHandNode = XRNode.RightHand;
    private Quaternion leftHandRot;
    private Quaternion rightHandRot;
    private float leftTriggerThresh;
    private float rightTriggerThresh;

    private Vector3 rDir;
    private Vector3 lDir;

    private void Update()
    {
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(leftHandNode);
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(rightHandNode);

        leftHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out leftHandRot);
        rightHandDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rightHandRot);

        leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandPos);
        rightHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandPos);

        leftHandDevice.TryGetFeatureValue(CommonUsages.trigger, out leftTriggerThresh);
        rightHandDevice.TryGetFeatureValue(CommonUsages.trigger, out rightTriggerThresh);

        Debug.DrawRay(leftHandPos, lDir);
        Debug.DrawRay(rightHandPos, rDir);
    }

    private void FixedUpdate()
    {
        coll.height = rig.cameraInRigSpaceHeight + extraCollHeight;
        Vector3 collCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        collCenter.y = coll.height / 2;
        coll.center = collCenter;

        lDir = (leftHandRot * Vector3.forward);
        rDir = (rightHandRot * Vector3.forward);
        TryMovePlayer(leftTriggerThresh, lDir);
        TryMovePlayer(rightTriggerThresh, rDir);
    }

    private void TryMovePlayer(float threshold, Vector3 deviceDir)
    {
        if (threshold >= minTriggerPress)
        {
            transform.position += deviceDir.normalized * 10 * Time.deltaTime;
            ////Calculated using https://forum.unity.com/threads/terminal-velocity.34667/#post-1869897
            //bodyDrag = rBody.velocity.magnitude / maxBodySpeed;
            //Vector3 force = deviceDir * bodyAccel;
            //Vector3 resistance = bodyDrag * rBody.velocity;
            //rBody.velocity += (force - resistance) * Time.deltaTime;
        }
    }
}
