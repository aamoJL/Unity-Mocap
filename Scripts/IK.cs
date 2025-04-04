using UnityEngine;

public class IK : MonoBehaviour
{
    public Transform IKLLeg, IKRLeg, IKLHand, IKRHand, IKHips, IKChest, IKLookAt;

    // Tracker position offsets to character bones
    private Vector3 hipsPosOffset, rLegPosOffset, lLegPosOffset, rHandPosOffset, lHandPosOffset;
    // Tracker rotation offsets to character bones
    private Quaternion hipsRotOffset, rLegRotOffset, lLegRotOffset, rHandRotOffset, lHandRotOffset, chestRotOffset, headRotOffset;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        MotionCaptureData mocapData = FindObjectOfType<MotionCapture>().MotionCaptureData;

        SetPosRotOffsets(mocapData);
    }

    private void Start()
    {
        animator.enabled = true;
    }

    /// <summary>
    /// Sets position and rotation offsets from a motion capture data object
    /// </summary>
    public void SetPosRotOffsets(MotionCaptureData data)
    {
        if (data == null)
            return;

        // From bone to tracker
        hipsPosOffset = data.Pelvis.CalibrationPosition - animator.GetBoneTransform(HumanBodyBones.Hips).position;
        rLegPosOffset = data.RightLeg.CalibrationPosition - animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
        lLegPosOffset = data.LeftLeg.CalibrationPosition - animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;

        // Vector between hand ik points with controller grip offset
        Vector3 ikHandVector = (data.RightHand.CalibrationPosition +
            data.RightHand.CalibrationRotation * IKRHand.GetChild(0).localPosition) -
            (data.LeftHand.CalibrationPosition + data.LeftHand.CalibrationRotation * IKLHand.GetChild(0).localPosition);
        Vector3 charHandVector = animator.GetBoneTransform(HumanBodyBones.RightHand).position -
            animator.GetBoneTransform(HumanBodyBones.LeftHand).position;
        float handLengthDifference = (charHandVector.magnitude - ikHandVector.magnitude) / 2;
        rHandPosOffset = ikHandVector.normalized * handLengthDifference;
        lHandPosOffset = -ikHandVector.normalized * handLengthDifference;

        hipsRotOffset = Quaternion.Inverse(data.Pelvis.CalibrationRotation);
        rLegRotOffset = Quaternion.Inverse(data.RightLeg.CalibrationRotation);
        lLegRotOffset = Quaternion.Inverse(data.LeftLeg.CalibrationRotation);
        rHandRotOffset = Quaternion.Inverse(data.RightHand.CalibrationRotation);
        lHandRotOffset = Quaternion.Inverse(data.LeftHand.CalibrationRotation);
        chestRotOffset = Quaternion.Inverse(data.Chest.CalibrationRotation);
        headRotOffset = Quaternion.Inverse(data.Head.CalibrationRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 hipPos = animator.GetBoneTransform(HumanBodyBones.Hips).position;
        Vector3 rotatedHipOffset = IKHips.rotation * hipsRotOffset * hipsPosOffset;
        Vector3 hipMovement = IKHips.position - hipPos;
        Vector3 hipsIKOffset = hipMovement - rotatedHipOffset;

        animator.SetBoneLocalRotation(HumanBodyBones.Hips, IKHips.rotation * hipsRotOffset);
        animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(IKHips.rotation * hipsRotOffset) *
            IKChest.rotation * chestRotOffset);
        animator.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.Inverse(IKChest.rotation * chestRotOffset) *
            IKLookAt.rotation * headRotOffset);

        Vector3 rLegTrackerOffset = IKRLeg.rotation * rLegRotOffset * rLegPosOffset;
        Vector3 lLegTrackerOffset = IKLLeg.rotation * lLegRotOffset * lLegPosOffset;
        animator.SetIKPosition(AvatarIKGoal.RightFoot, IKRLeg.position - hipsIKOffset - rLegTrackerOffset);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, IKRLeg.rotation * rLegRotOffset);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, IKLLeg.position - hipsIKOffset - lLegTrackerOffset);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, IKLLeg.rotation * lLegRotOffset);

        Vector3 rHandTrackerOffset = IKRHand.rotation * rHandRotOffset * rHandPosOffset;
        Vector3 lHandTrackerOffset = IKLHand.rotation * lHandRotOffset * lHandPosOffset;
        animator.SetIKPosition(AvatarIKGoal.RightHand, IKRHand.GetChild(0).position - hipsIKOffset + rHandTrackerOffset);
        animator.SetIKRotation(AvatarIKGoal.RightHand, IKRHand.GetChild(0).rotation);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, IKLHand.GetChild(0).position - hipsIKOffset + lHandTrackerOffset);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, IKLHand.GetChild(0).rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
    }

    private void LateUpdate()
    {
        // Pelvis position
        Vector3 rotatedHipOffset = IKHips.rotation * hipsRotOffset * hipsPosOffset;
        Vector3 hipMovement = IKHips.position - animator.GetBoneTransform(HumanBodyBones.Hips).position;
        animator.GetBoneTransform(HumanBodyBones.Hips).position =
            animator.GetBoneTransform(HumanBodyBones.Hips).position + hipMovement - rotatedHipOffset;
    }
}
