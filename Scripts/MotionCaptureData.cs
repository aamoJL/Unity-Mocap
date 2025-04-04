using UnityEngine;

[CreateAssetMenu(fileName = "MotionCaptureData", menuName = "MotionCaptureData")]
public class MotionCaptureData : ScriptableObject
{
    [System.Serializable]
    public class MotionCaptureDataObject
    {
        public Vector3 CalibrationPosition;
        public Quaternion CalibrationRotation;
        public AnimationCurve 
            PosCurveX, PosCurveY, PosCurveZ,
            RotCurveX, RotCurveY, RotCurveZ; // Euler

        /// <summary>
        /// Adds keyframes to AnimationCurves
        /// </summary>
        public void AddKeyframe(float time, Vector3 pos, Quaternion rot)
        {
            float 
                rotX = rot.eulerAngles.x, 
                rotY = rot.eulerAngles.y, 
                rotZ = rot.eulerAngles.z;

            // Move keyframe up or down if it is outside the default 360 degree range.
            //  (if difference from last frame is over 180 degrees)
            if(RotCurveX.keys.Length >= 1)
            {
                rotX = Mathf.Abs(RotCurveX.keys[RotCurveX.length - 1].value - rotX) > 180 ? 
                    ((RotCurveX.keys[RotCurveX.length - 1].value > rotX) ? rotX + 360 : rotX - 360) : rotX;
                rotY = Mathf.Abs(RotCurveY.keys[RotCurveY.length - 1].value - rotY) > 180 ?
                    ((RotCurveY.keys[RotCurveY.length - 1].value > rotY) ? rotY + 360 : rotY - 360) : rotY;
                rotZ = Mathf.Abs(RotCurveZ.keys[RotCurveZ.length - 1].value - rotZ) > 180 ?
                    ((RotCurveZ.keys[RotCurveZ.length - 1].value > rotZ) ? rotZ + 360 : rotZ - 360) : rotZ;
            }
            
            PosCurveX.AddKey(time, pos.x);  PosCurveY.AddKey(time, pos.y);
            PosCurveZ.AddKey(time, pos.z);  RotCurveX.AddKey(time, rotX);
            RotCurveY.AddKey(time, rotY);   RotCurveZ.AddKey(time, rotZ);
        }

        /// <summary>
        /// Clears AnimationCurves
        /// </summary>
        public void Clear()
        {
            PosCurveX = new AnimationCurve();
            PosCurveY = new AnimationCurve();
            PosCurveZ = new AnimationCurve();
            RotCurveX = new AnimationCurve();
            RotCurveY = new AnimationCurve();
            RotCurveZ = new AnimationCurve();
        }
    }

    [Header("Calibration Pose")]
    public MotionCaptureDataObject LeftLeg;     public MotionCaptureDataObject RightLeg;
    public MotionCaptureDataObject LeftHand;    public MotionCaptureDataObject RightHand;
    public MotionCaptureDataObject Head;        public MotionCaptureDataObject Chest;
    public MotionCaptureDataObject Pelvis;

    /// <summary>
    /// Clears every MotionCaptureDataObjects
    /// </summary>
    public void Clear()
    {
        LeftLeg.Clear();
        RightLeg.Clear();
        LeftHand.Clear();
        RightHand.Clear();
        Head.Clear();
        Chest.Clear();
        Pelvis.Clear();
    }

    /// <summary>
    /// Converts MotionCaptureData to AnimationClip
    /// </summary>
    public void ConvertToAnimationClip(AnimationClip clip)
    {
        clip.legacy = true;
        clip.ClearCurves();
        SetAnimationCurveToClip("RightHand", RightHand, clip);
        SetAnimationCurveToClip("LeftHand", LeftHand, clip);
        SetAnimationCurveToClip("RightLeg", RightLeg, clip);
        SetAnimationCurveToClip("LeftLeg", LeftLeg, clip);
        SetAnimationCurveToClip("Pelvis", Pelvis, clip);
        SetAnimationCurveToClip("Shoulders", Chest, clip);
        SetAnimationCurveToClip("Head", Head, clip);
    }

    /// <summary>
    /// Adds position and rotation curves from MotionCaptureDataObject to AnimationClip
    /// </summary>
    static void SetAnimationCurveToClip(string name, MotionCaptureDataObject captureDataObject, AnimationClip clip)
    {
        clip.SetCurve(name, typeof(Transform), "localPosition.x", captureDataObject.PosCurveX);
        clip.SetCurve(name, typeof(Transform), "localPosition.y", captureDataObject.PosCurveY);
        clip.SetCurve(name, typeof(Transform), "localPosition.z", captureDataObject.PosCurveZ);
        clip.SetCurve(name, typeof(Transform), "localEulerAnglesRaw.x", captureDataObject.RotCurveX);
        clip.SetCurve(name, typeof(Transform), "localEulerAnglesRaw.y", captureDataObject.RotCurveY);
        clip.SetCurve(name, typeof(Transform), "localEulerAnglesRaw.z", captureDataObject.RotCurveZ);
    }
}
