using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class MecanimRecorder : MonoBehaviour
{
  public AnimationClip AnimationFile;
  public Animator Animator;
  public IK IK;
  public float recordingInterval = .2f;

  private Animation ikAnimation;
  private AnimationCurve[] muscleCurves;
  private AnimationCurve
      motionTCurveX = new AnimationCurve(), motionTCurveY = new AnimationCurve(),
      motionTCurveZ = new AnimationCurve(), motionQCurveX = new AnimationCurve(),
      motionQCurveY = new AnimationCurve(), motionQCurveZ = new AnimationCurve(),
      motionQCurveW = new AnimationCurve();

  private void Awake()
    => ikAnimation = GetComponent<Animation>();

  private void Start()
  {
    IK.enabled = true;
    ikAnimation.Play();

    muscleCurves = new AnimationCurve[HumanTrait.MuscleCount];

    // Init muscle curves
    for (int i = 0; i < muscleCurves.Length; i++)
      muscleCurves[i] = new AnimationCurve();

    // Start recording
    StartCoroutine(RecordingTick());
  }

  private IEnumerator RecordingTick()
  {
    Debug.Log("Mecanim Recording STARTED");

    float recordingStartTime = Time.time;

    while (true)
    {
      yield return new WaitForEndOfFrame(); // Wait other scripts (IK etc.)

      float timestamp = Time.time - recordingStartTime;

      // Stop recording when the IK animation ends
      if (timestamp > ikAnimation.clip.length)
        break;

      // Get characters HumanPose
      HumanPoseHandler poseHandler = new HumanPoseHandler(Animator.avatar, Animator.transform);
      HumanPose pose = new HumanPose();
      poseHandler.GetHumanPose(ref pose);

      // Add keyframes to curves
      for (int i = 0; i < pose.muscles.Length; i++)
        muscleCurves[i].AddKey(timestamp, pose.muscles[i]);

      motionTCurveX.AddKey(timestamp, pose.bodyPosition.x);
      motionTCurveY.AddKey(timestamp, pose.bodyPosition.y);
      motionTCurveZ.AddKey(timestamp, pose.bodyPosition.z);
      motionQCurveX.AddKey(timestamp, pose.bodyRotation.x);
      motionQCurveY.AddKey(timestamp, pose.bodyRotation.y);
      motionQCurveZ.AddKey(timestamp, pose.bodyRotation.z);
      motionQCurveW.AddKey(timestamp, pose.bodyRotation.w);

      yield return new WaitForSecondsRealtime(recordingInterval);
    }

    Debug.Log("Mecanim Recording ENDED");

    SaveAnimation();
  }

  private void SaveAnimation()
  {
    AnimationFile.ClearCurves();

    AnimationFile.SetCurve("", typeof(Animator), "Spine Front-Back", muscleCurves[0]);
    AnimationFile.SetCurve("", typeof(Animator), "Spine Left-Right", muscleCurves[1]);
    AnimationFile.SetCurve("", typeof(Animator), "Spine Twist Left-Right", muscleCurves[2]);
    AnimationFile.SetCurve("", typeof(Animator), "Neck Nod Down-Up", muscleCurves[9]);
    AnimationFile.SetCurve("", typeof(Animator), "Neck Tilt Left-Right", muscleCurves[10]);
    AnimationFile.SetCurve("", typeof(Animator), "Neck Turn Left-Right", muscleCurves[11]);
    AnimationFile.SetCurve("", typeof(Animator), "Head Nod Down-Up", muscleCurves[12]);
    AnimationFile.SetCurve("", typeof(Animator), "Head Tilt Left-Right", muscleCurves[13]);
    AnimationFile.SetCurve("", typeof(Animator), "Head Turn Left-Right", muscleCurves[14]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Arm Down-Up", muscleCurves[39]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Arm Front-Back", muscleCurves[40]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Arm Twist In-Out", muscleCurves[41]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Forearm Stretch", muscleCurves[42]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Forearm Twist In-Out", muscleCurves[43]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Hand Down-Up", muscleCurves[44]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Hand In-Out", muscleCurves[45]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Arm Down-Up", muscleCurves[48]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Arm Front-Back", muscleCurves[49]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Arm Twist In-Out", muscleCurves[50]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Forearm Stretch", muscleCurves[51]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Forearm Twist In-Out", muscleCurves[52]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Hand Down-Up", muscleCurves[53]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Hand In-Out", muscleCurves[54]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Upper Leg Front-Back", muscleCurves[21]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Upper Leg In-Out", muscleCurves[22]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Upper Leg Twist In-Out", muscleCurves[23]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Lower Leg Stretch", muscleCurves[24]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Lower Leg Twist In-Out", muscleCurves[25]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Foot Up-Down", muscleCurves[26]);
    AnimationFile.SetCurve("", typeof(Animator), "Left Foot Twist In-Out", muscleCurves[27]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Upper Leg Front-Back", muscleCurves[29]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Upper Leg In-Out", muscleCurves[30]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Upper Leg Twist In-Out", muscleCurves[31]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Lower Leg Stretch", muscleCurves[32]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Lower Leg Twist In-Out", muscleCurves[33]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Foot Up-Down", muscleCurves[34]);
    AnimationFile.SetCurve("", typeof(Animator), "Right Foot Twist In-Out", muscleCurves[35]);
    AnimationFile.SetCurve("", typeof(Animator), "RootT.x", motionTCurveX);
    AnimationFile.SetCurve("", typeof(Animator), "RootT.y", motionTCurveY);
    AnimationFile.SetCurve("", typeof(Animator), "RootT.z", motionTCurveZ);
    AnimationFile.SetCurve("", typeof(Animator), "RootQ.x", motionQCurveX);
    AnimationFile.SetCurve("", typeof(Animator), "RootQ.y", motionQCurveY);
    AnimationFile.SetCurve("", typeof(Animator), "RootQ.z", motionQCurveZ);
    AnimationFile.SetCurve("", typeof(Animator), "RootQ.w", motionQCurveW);
  }
}
