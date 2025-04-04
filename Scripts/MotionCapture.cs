using System.Collections;
using UnityEditor;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Animation))]
public class MotionCapture : MonoBehaviour
{
  public float GizmoSize = .1f;
  public float recordingInterval = .015f;
  [Header("Trackers")]
  public Transform LeftLeg;
  public Transform RightLeg;
  public Transform LeftHand;
  public Transform RightHand;
  public Transform Head;
  public Transform Chest;
  public Transform Pelvis;
  [Space]
  public MotionCaptureData MotionCaptureData;

  private bool isRecording = false;

  private void Update()
  {
    if (SteamVR_Input.GetStateDown("CalibratePose", SteamVR_Input_Sources.Any, true))
    {
      MotionCaptureData.LeftLeg.CalibrationPosition = LeftLeg != null ? LeftLeg.position : Vector3.zero;
      MotionCaptureData.RightLeg.CalibrationPosition = RightLeg != null ? RightLeg.position : Vector3.zero;
      MotionCaptureData.LeftHand.CalibrationPosition = LeftHand != null ? LeftHand.position : Vector3.zero;
      MotionCaptureData.RightHand.CalibrationPosition = RightHand != null ? RightHand.position : Vector3.zero;
      MotionCaptureData.Head.CalibrationPosition = Head != null ? Head.position : Vector3.zero;
      MotionCaptureData.Chest.CalibrationPosition = Chest != null ? Chest.position : Vector3.zero;
      MotionCaptureData.Pelvis.CalibrationPosition = Pelvis != null ? Pelvis.position : Vector3.zero;
      MotionCaptureData.LeftLeg.CalibrationRotation = LeftLeg != null ? LeftLeg.rotation : Quaternion.identity;
      MotionCaptureData.RightLeg.CalibrationRotation = RightLeg != null ? RightLeg.rotation : Quaternion.identity;
      MotionCaptureData.LeftHand.CalibrationRotation = LeftHand != null ? LeftHand.rotation : Quaternion.identity;
      MotionCaptureData.RightHand.CalibrationRotation = RightHand != null ? RightHand.rotation : Quaternion.identity;
      MotionCaptureData.Head.CalibrationRotation = Head != null ? Head.rotation : Quaternion.identity;
      MotionCaptureData.Chest.CalibrationRotation = Chest != null ? Chest.rotation : Quaternion.identity;
      MotionCaptureData.Pelvis.CalibrationRotation = Pelvis != null ? Pelvis.rotation : Quaternion.identity;

      // Save scriptable object
      EditorUtility.SetDirty(MotionCaptureData);
      AssetDatabase.SaveAssets();

      Debug.Log("Pose Calibrated");
    }

    if (SteamVR_Input.GetStateDown("Record", SteamVR_Input_Sources.Any, true))
    {
      if (!isRecording)
        StartCoroutine(RecordingTick());
      else
      {
        // Stop recording
        isRecording = false;

        // Save scriptable object
        EditorUtility.SetDirty(MotionCaptureData);
        AssetDatabase.SaveAssets();

        Animation animation = GetComponent<Animation>();

        // Convert animation curves to animation clip
        AnimationClip animationClip = animation.clip;
        MotionCaptureData.ConvertToAnimationClip(animationClip);

        // Play recorded animation
        animation.AddClip(animationClip, animationClip.name);
        animation.Play(animationClip.name);

        // Set IK active
        IK ik = FindObjectOfType<IK>();
        ik.enabled = true;
      }
    }
  }

  /// <summary>
  /// Coroutine that records IK positions to motion capture data object
  /// </summary>
  private IEnumerator RecordingTick()
  {
    Debug.Log("REC STARTED");

    float recordingStartTime = Time.time;
    MotionCaptureData.Clear();
    isRecording = true;

    while (isRecording)
    {
      float timestamp = Time.time - recordingStartTime;

      if (LeftLeg) MotionCaptureData.LeftLeg.AddKeyframe(timestamp, LeftLeg.localPosition, LeftLeg.localRotation);
      if (RightLeg) MotionCaptureData.RightLeg.AddKeyframe(timestamp, RightLeg.localPosition, RightLeg.localRotation);
      if (LeftHand) MotionCaptureData.LeftHand.AddKeyframe(timestamp, LeftHand.localPosition, LeftHand.localRotation);
      if (RightHand) MotionCaptureData.RightHand.AddKeyframe(timestamp, RightHand.localPosition, RightHand.localRotation);
      if (Head) MotionCaptureData.Head.AddKeyframe(timestamp, Head.localPosition, Head.localRotation);
      if (Chest) MotionCaptureData.Chest.AddKeyframe(timestamp, Chest.localPosition, Chest.localRotation);
      if (Pelvis) MotionCaptureData.Pelvis.AddKeyframe(timestamp, Pelvis.localPosition, Pelvis.localRotation);

      yield return new WaitForSecondsRealtime(recordingInterval);
    }

    Debug.Log("REC ENDED");
  }

  private void OnDrawGizmos()
  {
    if (!MotionCaptureData)
      return;

    // Draw gizmos that represents calibrated pose from motion capture data object
    // IK positions
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(MotionCaptureData.LeftLeg.CalibrationPosition, GizmoSize);
    Gizmos.DrawWireSphere(MotionCaptureData.RightLeg.CalibrationPosition, GizmoSize);
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(MotionCaptureData.LeftHand.CalibrationPosition, GizmoSize);
    Gizmos.DrawWireSphere(MotionCaptureData.RightHand.CalibrationPosition, GizmoSize);
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(MotionCaptureData.Head.CalibrationPosition, GizmoSize);
    Gizmos.DrawWireSphere(MotionCaptureData.Chest.CalibrationPosition, GizmoSize);
    Gizmos.DrawWireSphere(MotionCaptureData.Pelvis.CalibrationPosition, GizmoSize);
    // Hand lines and spheres
    //Gizmos.color = Color.green;
    //Vector3 handCenter = MotionCaptureData.LeftHand.CalibrationPosition + (MotionCaptureData.RightHand.CalibrationPosition - MotionCaptureData.LeftHand.CalibrationPosition) / 2;
    //float shoulderOffset = ((MotionCaptureData.LeftLeg.CalibrationPosition - MotionCaptureData.RightLeg.CalibrationPosition) / 2).magnitude; // Same as feetOffset
    //Vector3 rHandShoulder = handCenter + (MotionCaptureData.RightHand.CalibrationPosition - handCenter).normalized * shoulderOffset;
    //Vector3 lHandShoulder = handCenter + (MotionCaptureData.LeftHand.CalibrationPosition - handCenter).normalized * shoulderOffset;
    //Gizmos.DrawLine(rHandShoulder, MotionCaptureData.RightHand.CalibrationPosition);
    //Gizmos.DrawLine(lHandShoulder, MotionCaptureData.LeftHand.CalibrationPosition);
    //Gizmos.DrawSphere(rHandShoulder, GizmoSize / 3);
    //Gizmos.DrawSphere(lHandShoulder, GizmoSize / 3);
    //// Spine Lines
    //Gizmos.color = Color.red;
    //Gizmos.DrawLine(rHandShoulder, lHandShoulder);
    //Gizmos.DrawLine(rHandShoulder, MotionCaptureData.Pelvis.CalibrationPosition);
    //Gizmos.DrawLine(lHandShoulder, MotionCaptureData.Pelvis.CalibrationPosition);
    //// Leg lines and spheres
    //Gizmos.color = Color.green;
    //Vector3 feetCenter = MotionCaptureData.RightLeg.CalibrationPosition + (MotionCaptureData.LeftLeg.CalibrationPosition - MotionCaptureData.RightLeg.CalibrationPosition) / 2;
    //Vector3 rUpperLeg = new Vector3(MotionCaptureData.RightLeg.CalibrationPosition.x, MotionCaptureData.Pelvis.CalibrationPosition.y, MotionCaptureData.RightLeg.CalibrationPosition.z);
    //Vector3 lUpperLeg = new Vector3(MotionCaptureData.LeftLeg.CalibrationPosition.x, MotionCaptureData.Pelvis.CalibrationPosition.y, MotionCaptureData.LeftLeg.CalibrationPosition.z);
    //Gizmos.DrawLine(rUpperLeg, MotionCaptureData.RightLeg.CalibrationPosition);
    //Gizmos.DrawLine(lUpperLeg, MotionCaptureData.LeftLeg.CalibrationPosition);
    //Gizmos.DrawSphere(rUpperLeg, GizmoSize / 3);
    //Gizmos.DrawSphere(lUpperLeg, GizmoSize / 3);
  }
}
