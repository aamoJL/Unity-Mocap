# Unity-Mocap
SteamVR motion capture for Unity using SteamVR trackers.

Dependencies:
- Unity 2020.3.20f1
- [SteamVR Unity Plugin v2.7.3](https://github.com/ValveSoftware/steamvr_unity_plugin/releases/tag/2.7.3)
- [OpenVR XR Plugin 1.1.4](https://github.com/ValveSoftware/unity-xr-plugin/releases/tag/v1.1.4)

## Scene initialization:
1. Install all the dependencies and import the `Mocap.unitypackage` to the project.
2. Add SteamVR `[CameraRig]` and Tracker objects.
3. Add SteamVR boolean inputs: _"CalibratePose"_ and _"Record"_. Remember to map the buttons to the controllers using the SteamVR input manager.
4. Add the `Mocap` prefab with `Motion Capture`, `Animation` and `Mecanim Recorder` components.
	- Set the Trackers to the corresponding tracker objects.
	- Disable `Mecanim Recorder` component.
5. Add humanoid character to the scene and attach `IK` component to it.
	- Disable `Animator` and `IK` components.
	- Set the `Animator` component's **Culling Mode** to **Always Animate**.
	- Set the `IK` component's IKs to the `Mocap` object's corresponding children.
	- Set the `IK` and `Animator` components to the `Mecanim Recorder` component.

## How to use:
1. Play the scene.
2. Match the pose with the character and press the _"CalibratePose"_ button.
3. To record an IK animation, press the _"Record"_ button, act the animation and press the button again to stop the recording.
	- The recording will be played after the recording has been stopped.
4. Stop the scene.
	- You can view the recording again by enabling the `IK` component and setting the `Animation`'s **Play Automatically** to **true**.
4. To convert the IK recording to a **mecanim animation**, enable the `Mecanim Recorder` component and play the scene.
	- "Mecanim Recording ENDED" will be logged to the console when the animation has been converted.
	- The animation will be saved to the component's **AnimationFile** file.

##

[mocap-video.webm](https://github.com/user-attachments/assets/e8a6d130-1aa0-4e17-95c8-a0201a840bdf)
