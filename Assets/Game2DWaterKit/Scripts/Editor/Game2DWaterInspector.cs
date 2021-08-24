using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace Game2DWaterKit
{
    [CanEditMultipleObjects, CustomEditor(typeof(Game2DWater))]
    class Game2DWaterInspector : Editor
    {
        #region variables

        #region Serialized Properties

        #region Water Properties
        //Mesh Properties
        private SerializedProperty subdivisionsCountPerUnit;
        private SerializedProperty waterSize;
        //Wave Properties
        private SerializedProperty damping;
        private SerializedProperty stiffness;
        private SerializedProperty spread;
        private SerializedProperty useCustomBoundaries;
        private SerializedProperty firstCustomBoundary;
        private SerializedProperty secondCustomBoundary;
        //Misc Properties
        private SerializedProperty buoyancyEffectorSurfaceLevel;
        //Water Events Properties
        private SerializedProperty onWaterEnter;
        private SerializedProperty onWaterExit;
        #endregion

        #region On-Collision Ripples Properties
        private SerializedProperty activateOnCollisionOnWaterEnterRipples;
        private SerializedProperty activateOnCollisionOnWaterExitRipples;
        //Disturbance Properties
        private SerializedProperty onCollisionRipplesMinimumDisturbance;
        private SerializedProperty onCollisionRipplesMaximumDisturbance;
        private SerializedProperty onCollisionRipplesVelocityMultiplier;
        //Collision Properties
        private SerializedProperty onCollisionRipplesCollisionMask;
        private SerializedProperty onCollisionRipplesCollisionMinimumDepth;
        private SerializedProperty onCollisionRipplesCollisionMaximumDepth;
        private SerializedProperty onCollisionRipplesCollisionRaycastMaxDistance;
        //Sound Effect Properties (On Water Enter)
        private SerializedProperty onCollisionRipplesActivateOnWaterEnterSoundEffect;
        private SerializedProperty onCollisionRipplesOnWaterEnterAudioClip;
        private SerializedProperty onCollisionRipplesOnWaterEnterMinimumAudioPitch;
        private SerializedProperty onCollisionRipplesOnWaterEnterMaximumAudioPitch;
        private SerializedProperty onCollisionRipplesUseConstantOnWaterEnterAudioPitch;
        private SerializedProperty onCollisionRipplesOnWaterEnterAudioPitch;
        private SerializedProperty onCollisionRipplesOnWaterEnterAudioVolume;
        private SerializedProperty onCollisionRipplesOnWaterEnterSoundEffectPoolSize;
        private SerializedProperty onCollisionRipplesReconstructOnWaterEnterSoundEffectPool;
        private SerializedProperty onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary;
        //Sound Effect Properties (On Water Exit)
        private SerializedProperty onCollisionRipplesActivateOnWaterExitSoundEffect;
        private SerializedProperty onCollisionRipplesOnWaterExitAudioClip;
        private SerializedProperty onCollisionRipplesOnWaterExitMinimumAudioPitch;
        private SerializedProperty onCollisionRipplesOnWaterExitMaximumAudioPitch;
        private SerializedProperty onCollisionRipplesUseConstantOnWaterExitAudioPitch;
        private SerializedProperty onCollisionRipplesOnWaterExitAudioPitch;
        private SerializedProperty onCollisionRipplesOnWaterExitAudioVolume;
        private SerializedProperty onCollisionRipplesOnWaterExitSoundEffectPoolSize;
        private SerializedProperty onCollisionRipplesReconstructOnWaterExitSoundEffectPool;
        private SerializedProperty onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary;
        //Particle Effect Properties (On Water Enter)
        private SerializedProperty onCollisionRipplesActivateOnWaterEnterParticleEffect;
        private SerializedProperty onCollisionRipplesOnWaterEnterParticleEffect;
        private SerializedProperty onCollisionRipplesOnWaterEnterParticleEffectPoolSize;
        private SerializedProperty onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset;
        private SerializedProperty onCollisionRipplesOnWaterEnterParticleEffectStopAction;
        private SerializedProperty onCollisionRipplesReconstructOnWaterEnterParticleEffectPool;
        private SerializedProperty onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary;
        //Particle Effect Properties (On Water Exit)
        private SerializedProperty onCollisionRipplesActivateOnWaterExitParticleEffect;
        private SerializedProperty onCollisionRipplesOnWaterExitParticleEffect;
        private SerializedProperty onCollisionRipplesOnWaterExitParticleEffectPoolSize;
        private SerializedProperty onCollisionRipplesOnWaterExitParticleEffectSpawnOffset;
        private SerializedProperty onCollisionRipplesOnWaterExitParticleEffectStopAction;
        private SerializedProperty onCollisionRipplesReconstructOnWaterExitParticleEffectPool;
        private SerializedProperty onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary;
        #endregion

        #region Constant Ripples Properties
        private SerializedProperty activateConstantRipples;
        private SerializedProperty constantRipplesUpdateWhenOffscreen;
        //Disturbance Properties
        private SerializedProperty constantRipplesDisturbance;
        private SerializedProperty constantRipplesRandomizeDisturbance;
        private SerializedProperty constantRipplesMinimumDisturbance;
        private SerializedProperty constantRipplesMaximumDisturbance;
        private SerializedProperty constantRipplesSmoothDisturbance;
        private SerializedProperty constantRipplesSmoothFactor;
        //Interval Properties
        private SerializedProperty constantRipplesRandomizeInterval;
        private SerializedProperty constantRipplesInterval;
        private SerializedProperty constantRipplesMinimumInterval;
        private SerializedProperty constantRipplesMaximumInterval;
        //Ripple Source Positions Properties
        private SerializedProperty constantRipplesRandomizeRipplesSourcesPositions;
        private SerializedProperty constantRipplesRandomizeRipplesSourcesCount;
        private SerializedProperty constantRipplesAllowDuplicateRipplesSourcesPositions;
        private SerializedProperty constantRipplesSourcePositions;
        //Sound Effect Properties
        private SerializedProperty constantRipplesActivateSoundEffect;
        private SerializedProperty constantRipplesAudioClip;
        private SerializedProperty constantRipplesUseConstantAudioPitch;
        private SerializedProperty constantRipplesAudioPitch;
        private SerializedProperty constantRipplesMinimumAudioPitch;
        private SerializedProperty constantRipplesMaximumAudioPitch;
        private SerializedProperty constantRipplesAudioVolume;
        private SerializedProperty constantRipplesSoundEffectPoolSize;
        private SerializedProperty constantRipplesReconstructSoundEffectPool;
        private SerializedProperty constantRipplesSoundEffectPoolExpandIfNecessary;
        //Particle Effect Properties
        private SerializedProperty constantRipplesActivateParticleEffect;
        private SerializedProperty constantRipplesParticleEffect;
        private SerializedProperty constantRipplesParticleEffectPoolSize;
        private SerializedProperty constantRipplesParticleEffectSpawnOffset;
        private SerializedProperty constantRipplesParticleEffectStopAction;
        private SerializedProperty constantRipplesReconstructParticleEffectPool;
        private SerializedProperty constantRipplesParticleEffectPoolExpandIfNecessary;
        #endregion

        #region Script-Generated Ripples
        //Disturbance Properties
        private SerializedProperty scriptGeneratedRipplesMinimumDisturbance;
        private SerializedProperty scriptGeneratedRipplesMaximumDisturbance;
        //Sound Effect Properties
        private SerializedProperty scriptGeneratedRipplesActivateSoundEffect;
        private SerializedProperty scriptGeneratedRipplesAudioClip;
        private SerializedProperty scriptGeneratedRipplesUseConstantAudioPitch;
        private SerializedProperty scriptGeneratedRipplesAudioPitch;
        private SerializedProperty scriptGeneratedRipplesMinimumAudioPitch;
        private SerializedProperty scriptGeneratedRipplesAudioVolume;
        private SerializedProperty scriptGeneratedRipplesMaximumAudioPitch;
        private SerializedProperty scriptGeneratedRipplesSoundEffectPoolSize;
        private SerializedProperty scriptGeneratedRipplesReconstructSoundEffectPool;
        private SerializedProperty scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary;
        //Particle Effect Properties
        private SerializedProperty scriptGeneratedRipplesActivateParticleEffect;
        private SerializedProperty scriptGeneratedRipplesParticleEffect;
        private SerializedProperty scriptGeneratedRipplesParticleEffectPoolSize;
        private SerializedProperty scriptGeneratedRipplesParticleEffectSpawnOffset;
        private SerializedProperty scriptGeneratedRipplesParticleEffectStopAction;
        private SerializedProperty scriptGeneratedRipplesReconstructParticleEffectPool;
        private SerializedProperty scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary;
        #endregion

        #region Refraction & Reflection Rendering Properties
        //Refraction Properties
        private SerializedProperty refractionRenderTextureResizeFactor;
        private SerializedProperty refractionCullingMask;
        private SerializedProperty refractionRenderTextureFilterMode;
        //Reflection Properties
        private SerializedProperty reflectionRenderTextureResizeFactor;
        private SerializedProperty reflectionCullingMask;
        private SerializedProperty reflectionZOffset;
        private SerializedProperty reflectionRenderTextureFilterMode;
        //Shared Properties
        private SerializedProperty renderPixelLights;
        private SerializedProperty sortingLayerID;
        private SerializedProperty sortingOrder;
        private SerializedProperty allowMSAA;
        private SerializedProperty allowHDR;
        private SerializedProperty farClipPlane;
        #endregion

        #endregion

        #region Serialized Properties Labels

        #region WaterProperties

        //Mesh Properties
        private static readonly string meshPropertiesLabel = "Mesh Properties";
        private static readonly GUIContent waterSizeLabel = new GUIContent("Water Size", "Sets the water size. X represents the width and Y represents the height.");
        private static readonly GUIContent subdivisionsCountPerUnitLabel = new GUIContent("Subdivisions Per Unit", "Sets the number of water’s surface vertices within one unit.");
        private static readonly GUIContent waterPropertiesFoldoutLabel = new GUIContent("Water Properties");
        //Wave Properties
        private static readonly string wavePropertiesLabel = "Wave Properties";
        private static readonly GUIContent dampingLabel = new GUIContent("Damping", "Controls how fast the waves decay. A low value will make waves oscillate for a long time, while a high value will make waves oscillate for a short time.");
        private static readonly GUIContent spreadLabel = new GUIContent("Spread", "Controls how fast the waves spread.");
        private static readonly GUIContent stiffnessLabel = new GUIContent("Stiffness", "Controls the frequency of wave vibration. A low value will make waves oscillate slowly, while a high value will make waves oscillate quickly.");
        private static readonly GUIContent useCustomBoundariesLabel = new GUIContent("Use Custom Boundaries", "Enable/Disable using custom wave boundaries. When waves reach a boundary, they bounce back.");
        private static readonly GUIContent firstCustomBoundaryLabel = new GUIContent("First Boundary", "The location of the first boundary.");
        private static readonly GUIContent secondCustomBoundaryLabel = new GUIContent("Second Boundary", "The location of the second boundary.");
        //Water Events Properties
        private static readonly GUIContent onWaterEnterLabel = new GUIContent("OnWaterEnter", "UnityEvent that is triggered when a GameObject enters the water.");
        private static readonly GUIContent onWaterExitLabel = new GUIContent("OnWaterExit", "UnityEvent that is triggered when a GameObject exits the water.");
        //Misc Properties
        private static readonly string miscLabel = "Misc";
        private static readonly GUIContent buoyancyEffectorSurfaceLevelLabel = new GUIContent("Surface Level", "Sets the surface location of the buoyancy fluid. When a GameObject is above this line, no buoyancy forces are applied. When a GameObject is intersecting or completely below this line, buoyancy forces are applied.");
        private static readonly GUIContent useEdgeCollider2DLabel = new GUIContent("Use Edge Collider 2D", "Adds/Removes an EdgeCollider2D component. The points of the edge collider are automatically updated whenever the water size changes.");
        private static readonly GUIContent fixScalingButtonLabel = new GUIContent("Fix Scaling");
        private static readonly string nonUniformScaleWarning = "Unexpected water simulation results may occur when using non-uniform scaling.";

        #endregion

        #region On-Collision Ripples Properties
        private static readonly string onCollisionOnWaterEnterRipplesLabel = "On Water Enter Ripples";
        private static readonly string onCollisionOnWaterExitRipplesLabel = "On Water Exit Ripples";
        private static readonly GUIContent activateOnCollisionOnWaterEnterRipplesLabel = new GUIContent("Activate", "Activates/Deactivates Generating ripple.s");
        private static readonly GUIContent activateOnCollisionOnWaterExitRipplesLabel = new GUIContent("Activate", "Activates/Deactivates Generating ripples.");
        //Disturbance Properties
        private static readonly GUIContent onCollisionRipplesMinimumDisturbanceLabel = new GUIContent("Minimum", "The minimum displacement of water’s surface.");
        private static readonly GUIContent onCollisionRipplesMaximumDisturbanceLabel = new GUIContent("Maximum", "The maximum displacement of water’s surface.");
        private static readonly GUIContent onCollisionRipplesVelocityMultiplierLabel = new GUIContent("Velocity Multiplier", "When a rigidbody falls into water or leaves the water, the amount of water’s surface displacement is determined by multiplying the rigidbody velocity by this factor.");
        //Collision Properties
        private static readonly GUIContent onCollisionRipplesCollisionMinimumDepthLabel = new GUIContent("Minimum Depth", "Only GameObjects with Z coordinate (depth) greater than or equal to this value will disturb the water’s surface.");
        private static readonly GUIContent onCollisionRipplesCollisionMaximumDepthLabel = new GUIContent("Maximum Depth", "Only GameObjects with Z coordinate (depth) less than or equal to this value will disturb the water’s surface.");
        private static readonly GUIContent onCollisionRipplesCollisionRaycastMaxDistanceLabel = new GUIContent("Maximum Distance", "The maximum distance from the water's surface over which to check for collisions (Default: 0.5)");
        private static readonly GUIContent onCollisionRipplesCollisionMaskLabel = new GUIContent("Collision Mask", "Only GameObjects on these layers will disturb the water’s surface.");
        private static readonly string collisionPropertiesLabel = "Collision Properties";
        //Sound Effect Properties (On Water Enter)
        private static readonly GUIContent onCollisionRipplesActivateOnWaterEnterSoundEffectLabel = new GUIContent("Sound Effect", "Activates/Deactivates playing the sound effect when a GameObject falls into water.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterAudioClipLabel = new GUIContent("Audio Clip", "The AudioClip asset to play when a GameObject falls into water.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterMinimumAudioPitchLabel = new GUIContent("Minimum Pitch", "Sets the audio clip’s minimum playback speed.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterMaximumAudioPitchLabel = new GUIContent("Maximum Pitch", "Sets the audio clip’s maximum playback speed.");
        private static readonly GUIContent onCollisionRipplesUseConstantOnWaterEnterAudioPitchLabel = new GUIContent("Constant Pitch", "Apply constant audio clip playback speed.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterAudioPitchLabel = new GUIContent("Pitch", "Sets the audio clip’s playback speed.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterAudioVolumeLabel = new GUIContent("Volume", "Sets the audio clip’s volume.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterSoundEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of audio sources objects that will be created and pooled when the game starts");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        private static readonly string onCollisionRipplesOnWaterEnterAudioPitchMessage = "The AudioSource pitch (playback speed) is linearly interpolated between the minimum pitch and the maximum pitch. When a GameObject falls into water, the higher its velocity, the lower the pitch value is.";
        //Particle Effect Properties (On Water Enter)
        private static readonly GUIContent onCollisionRipplesActivateOnWaterEnterParticleEffectLabel = new GUIContent("Particle Effect", "Activates/Deactivates playing the particle effect when a GameObject falls into water.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterParticleEffectLabel = new GUIContent("Particle System", "Sets the particle effect system to play when a GameObject falls into water.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterParticleEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of particle systems objects that will be created and pooled when the game starts.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterParticleEffectSpawnOffsetLabel = new GUIContent("Spawn Offset", "Shifts the particle effect spawn position.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterParticleEffectStopActionLabel = new GUIContent("Stop Action", "UnityEvent that is triggered when the particle effect stops playing.");
        private static readonly GUIContent onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        //Sound Effect Properties (On Water Exit)
        private static readonly GUIContent onCollisionRipplesActivateOnWaterExitSoundEffectLabel = new GUIContent("Sound Effect", "Activates/Deactivates playing the sound effect when a GameObject leaves the water.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitAudioClipLabel = new GUIContent("Audio Clip", "The AudioClip asset to play when a GameObject leaves the water.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitMinimumAudioPitchLabel = new GUIContent("Minimum Pitch", "Sets the audio clip’s minimum playback speed.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitMaximumAudioPitchLabel = new GUIContent("Maximum Pitch", "Sets the audio clip’s maximum playback speed.");
        private static readonly GUIContent onCollisionRipplesUseConstantOnWaterExitAudioPitchLabel = new GUIContent("Constant Pitch", "Apply constant audio clip playback speed.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitAudioPitchLabel = new GUIContent("Pitch", "Sets the audio clip’s playback speed.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitAudioVolumeLabel = new GUIContent("Volume", "Sets the audio clip’s volume.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitSoundEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of audio sources objects that will be created and pooled when the game starts");
        private static readonly GUIContent onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        private static readonly string onCollisionRipplesOnWaterExitAudioPitchMessage = "The AudioSource pitch (playback speed) is linearly interpolated between the minimum pitch and the maximum pitch. When a GameObject leaves the water, the higher its velocity, the lower the pitch value is.";
        //Particle Effect Properties (On Water Exit)
        private static readonly GUIContent onCollisionRipplesActivateOnWaterExitParticleEffectLabel = new GUIContent("Particle Effect", "Activates/Deactivates playing the particle effect when a GameObject leaves the water.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitParticleEffectLabel = new GUIContent("Particle System", "Sets the particle effect system to play when a GameObject leaves the water.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitParticleEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of particle systems objects that will be created and pooled when the game starts.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitParticleEffectSpawnOffsetLabel = new GUIContent("Spawn Offset", "Shifts the particle effect spawn position.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitParticleEffectStopActionLabel = new GUIContent("Stop Action", "UnityEvent that is triggered when the particle effect stops playing.");
        private static readonly GUIContent onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");

        //Misc
        private static readonly GUIContent onCollisionRipplesPropertiesFoldoutLabel = new GUIContent("On Collision Ripples Properties");
        #endregion

        #region Constant Ripples Properties
        private static readonly GUIContent constantRipplesPropertiesFoldoutLabel = new GUIContent("Constant Ripples Properties");
        private static readonly GUIContent activateConstantRipplesLabel = new GUIContent("Activate", "Activates/Deactivates generating constant ripples.");
        private static readonly GUIContent constantRipplesUpdateWhenOffscreenLabel = new GUIContent("Update When Off-screen", "Generate constant ripples even when the water is invisible to the camera.");
        //Disturbance Properties
        private static readonly GUIContent constantRipplesDisturbanceLabel = new GUIContent("Disturbance", "Sets the displacement of water’s surface.");
        private static readonly GUIContent constantRipplesRandomizeDisturbanceLabel = new GUIContent("Randomize", "Randomize the disturbance (displacement) of water's surface.");
        private static readonly GUIContent constantRipplesMinimumDisturbanceLabel = new GUIContent("Minimum", "Sets the minimum displacement of water’s surface.");
        private static readonly GUIContent constantRipplesMaximumDisturbanceLabel = new GUIContent("Maximum", "Sets the maximum displacement of water’s surface.");
        //Interval Properties
        private static readonly string intervalPropertiesLabel = "Interval Properties";
        private static readonly GUIContent randomizePersistnetWaveIntervalLabel = new GUIContent("Randomize", "Randomize the interval.");
        private static readonly GUIContent constantRipplesIntervalLabel = new GUIContent("Interval", "Apply constant ripples at regular intervals (in seconds).");
        private static readonly GUIContent constantRipplesMinimumIntervalLabel = new GUIContent("Minimum", "Minimum Interval.");
        private static readonly GUIContent constantRipplesMaximumIntervalLabel = new GUIContent("Maximum", "Maximum Interval.");
        //Ripple Source Positions Properties
        private static readonly string constantRipplesSourcesPropertiesLabel = "Ripple Source Positions Properties";
        private static readonly GUIContent constantRipplesRandomizeRipplesSourcesCountLabel = new GUIContent("Sources Count", "Sets the number of constant ripples sources.");
        private static readonly GUIContent constantRipplesSmoothDisturbanceLabel = new GUIContent("Smooth Ripples", "Disturb neighbor vertices to create a smoother ripple (wave).");
        private static readonly GUIContent constantRipplesSmoothFactorLabel = new GUIContent("Smoothing Factor", "The amount of disturbance to apply to neighbor vertices.");
        private static readonly GUIContent constantRipplesRandomizeRipplesSourcesPositionsLabel = new GUIContent("Randomize", "Randomize constant ripples sources positions.");
        private static readonly GUIContent constantRipplesSourcePositionsLabel = new GUIContent("Ripples Sources Positions (X-axis)", "Sets the constant ripples sources positions.");
        private static readonly GUIContent constantRipplesAllowDuplicateRipplesSourcesPositionsLabel = new GUIContent("Allow Duplicate Positions", "Allow generating on the same frame and in the same position multiple constant ripples.");
        private static readonly GUIContent constantRipplesEditSourcesPositionsLabel = new GUIContent("Edit Positions", "Edit constant ripples sources positions.");
        //Sound Effect Properties
        private static readonly string constantRipplesAudioPitchMessage = "The AudioSource pitch (playback speed) is linearly interpolated between the minimum pitch and the maximum pitch. When a ripple is generated, the higher its disturbance, the lower the pitch value is.";
        private static readonly GUIContent constantRipplesActivateSoundEffectLabel = new GUIContent("Activate", "Activate/Deactivate playing the sound effect when generating constant ripples. ");
        private static readonly GUIContent constantRipplesAudioClipLabel = new GUIContent("Audio Clip", "The AudioClip asset to play  when generating constant ripples.");
        private static readonly GUIContent constantRipplesMinimumAudioPitchLabel = new GUIContent("Minimum Pitch", "Sets the audio clip’s minimum playback speed.");
        private static readonly GUIContent constantRipplesMaximumAudioPitchLabel = new GUIContent("Maximum Pitch", "Sets the audio clip’s maximum playback speed.");
        private static readonly GUIContent constantRipplesUseConstantAudioPitchLabel = new GUIContent("Constant Pitch", "Apply constant audio clip playback speed.");
        private static readonly GUIContent constantRipplesAudioPitchLabel = new GUIContent("Pitch", "Sets the audio clip’s playback speed.");
        private static readonly GUIContent constantRipplesAudioVolumeLabel = new GUIContent("Volume", "Sets the audio clip’s volume.");
        private static readonly GUIContent constantRipplesSoundEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of audio sources objects that will be created and pooled when the game starts");
        private static readonly GUIContent constantRipplesSoundEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        //Particle Effect Properties
        private static readonly GUIContent constantRipplesActivateParticleEffectLabel = new GUIContent("Activate", "Activate/Deactivate playing the particle effect when generating constant ripples.");
        private static readonly GUIContent constantRipplesParticleEffectLabel = new GUIContent("Particle System", "Sets the particle effect system to play when generating constant ripples");
        private static readonly GUIContent constantRipplesParticleEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of particle effect objects that will be created and pooled when the game starts");
        private static readonly GUIContent constantRipplesParticleEffectSpawnOffsetLabel = new GUIContent("Spawn Offset", "Shift the particle effect spawn position.");
        private static readonly GUIContent constantRipplesParticleEffectStopActionLabel = new GUIContent("Stop Action", "UnityEvent that is triggered when the particle effect stops playing.");
        private static readonly GUIContent constantRipplesParticleEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        #endregion

        #region Script-Generated Ripples Properties
        private static readonly string scriptGeneratedRipplesMessage = "Script-Generated ripples are ripples created through script using the method GenerateRipple(..).";
        //Disturbance Properties
        private static readonly GUIContent scriptGeneratedRipplesPropertiesFoldoutLabel = new GUIContent("Script-Generated Ripples Properties");
        private static readonly GUIContent scriptGeneratedRipplesMaximumDisturbanceLabel = new GUIContent("Maximum", "Sets the maximum displacement of water’s surface.");
        private static readonly GUIContent scriptGeneratedRipplesMinimumDisturbanceLabel = new GUIContent("Minimum", "Sets the minimum displacement of water’s surface.");
        //Sound Effect Properties
        private static readonly string scriptGeneratedRipplesAudioPitchMessage = "The AudioSource pitch (playback speed) is linearly interpolated between the minimum pitch and the maximum pitch. When a ripple is generated, the higher its disturbance, the lower the pitch value is.";
        private static readonly GUIContent scriptGeneratedRipplesActivateSoundEffectLabel = new GUIContent("Activate", "Activate/Deactivate playing the sound effect when generating ripples through script.");
        private static readonly GUIContent scriptGeneratedRipplesAudioClipLabel = new GUIContent("Audio Clip", "The AudioClip asset to play when generating ripples through script.");
        private static readonly GUIContent scriptGeneratedRipplesMinimumAudioPitchLabel = new GUIContent("Minimum Pitch", "Sets the audio clip’s minimum playback speed.");
        private static readonly GUIContent scriptGeneratedRipplesMaximumAudioPitchLabel = new GUIContent("Maximum Pitch", "Sets the audio clip’s maximum playback speed.");
        private static readonly GUIContent scriptGeneratedRipplesUseConstantAudioPitchLabel = new GUIContent("Constant Pitch", "Apply constant audio clip playback speed.");
        private static readonly GUIContent scriptGeneratedRipplesAudioPitchLabel = new GUIContent("Pitch", "Sets the audio clip’s playback speed.");
        private static readonly GUIContent scriptGeneratedRipplesAudioVolumeLabel = new GUIContent("Volume", "Sets the audio clip’s volume.");
        private static readonly GUIContent scriptGeneratedRipplesSoundEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of audio sources objects that will be created and pooled when the game starts");
        private static readonly GUIContent scriptGeneratedRipplesSoundEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        //Particle Effect Properties
        private static readonly GUIContent scriptGeneratedRipplesActivateParticleEffectLabel = new GUIContent("Activate", "Activate/Deactivate playing the particle effect when generating ripples through script.");
        private static readonly GUIContent scriptGeneratedRipplesParticleEffectLabel = new GUIContent("Particle System", "Sets the particle effect system to play when generating ripples through script.");
        private static readonly GUIContent scriptGeneratedRipplesParticleEffectPoolSizeLabel = new GUIContent("Pool Size", "Sets the number of particle effect objects that will be created and pooled when the game starts.");
        private static readonly GUIContent scriptGeneratedRipplesParticleEffectSpawnOffsetLabel = new GUIContent("Spawn Offset", "Shift the particle effect spawn position.");
        private static readonly GUIContent scriptGeneratedRipplesParticleEffectStopActionLabel = new GUIContent("Stop Action", "UnityEvent that is triggered when the particle effect stops playing.");
        private static readonly GUIContent scriptGeneratedRipplesParticleEffectPoolExpandIfNecessaryLabel = new GUIContent("Expand If Necessary", "Enables/Disables increasing the number of pooled objects at runtime if needed.");
        #endregion

        #region Refraction & Reflection Rendering Properties
        //Refraction Properties
        private static readonly GUIContent refractionPropertiesFoldoutLabel = new GUIContent("Refraction Properties");
        private static readonly GUIContent refractionRenderTextureResizeFactorLabel = new GUIContent("Resize Factor", "Specifies how much the RenderTexture used to render refraction is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality.");
        private static readonly GUIContent refractionRenderTextureFilterModeLabel = new GUIContent("Filter Mode", "Sets the RenderTexture's filter mode");
        private static readonly string refractionMessage = "Refraction properties are disabled. \"Refraction\" can be activated in the material editor.";
        //Reflection Properties
        private static readonly GUIContent reflectionPropertiesFoldoutLabel = new GUIContent("Reflection Properties");
        private static readonly GUIContent reflectionRenderTextureResizeFactorLabel = new GUIContent("Resize Factor", "Specifies how much the RenderTexture used to render reflection is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality.");
        private static readonly GUIContent reflectionZOffsetLabel = new GUIContent("Z Offset", "Controls where to start rendering reflection relative to the water GameObject position.");
        private static readonly GUIContent reflectionRenderTextureFilterModeLabel = new GUIContent("Filter Mode", "Sets the RenderTexture's filter mode");
        private static readonly string reflectionMessage = "Reflection properties are disabled. \"Reflection\" can be activated in the material editor.";
        //Shared Properties
        private static readonly GUIContent renderingSettingsFoldoutLabel = new GUIContent("Rendering Settings");
        private static readonly GUIContent refractionReflectionCullingMaskLabel = new GUIContent("Culling Mask", "Only GameObjects on these layers will be rendered.");
        private static readonly GUIContent farClipPlaneLabel = new GUIContent("Far Clip Plane", "Sets the furthest point relative to the water that will be drawn when rendering refraction and/or reflection.");
        private static readonly GUIContent renderPixelLightsLabel = new GUIContent("Render Pixel Lights", "Controls whether the rendered objects will be affected by pixel lights. Disabling this could increase performance at the expense of visual fidelity.");
        private static readonly GUIContent sortingLayerLabel = new GUIContent("Sorting Layer", "The name of the water mesh renderer sorting layer.");
        private static readonly GUIContent orderInLayerLabel = new GUIContent("Order In Layer", "The water mesh renderer order within a sorting layer.");
        private static readonly GUIContent allowMSAALabel = new GUIContent("Allow MSAA", "Allow multisample antialiasing rendering.");
        private static readonly GUIContent allowHDRLabel = new GUIContent("Allow HDR", "Allow high dynamic range rendering.");
        #endregion

        #region Misc
        private static readonly GUIContent prefabUtilityFoldoutLabel = new GUIContent("Prefab Utility");
        private static readonly string particleSystemLoopMessage = "Please make sure the particle system is not looping!";
        private static readonly string particleEffectPropertiesLabel = "Particle Effect Properties";
        private static readonly string soundEffectPropertiesLabel = "Sound Effect Properties";
        private static readonly string disturbancePropertiesLabel = "Disturbance Properties";
        private static readonly string noiseTextureShaderPropertyName = "_NoiseTexture";
#if UNITY_2018_3_OR_NEWER
        private static readonly string newPrefabWorkflowMessage = "As of Unity 2018.3, disconnecting (unlinking) and relinking a Prefab instance are no longer supported. Alternatively, you can now unpack a Prefab instance if you want to entirely remove its link to its Prefab asset and thus be able to restructure the resulting plain GameObject as you please.";
#endif

        #endregion

        #endregion

        #region Misc
        private AnimBool waterPropertiesExpanded = new AnimBool();
        private AnimBool onCollisionRipplesPropertiesExpanded = new AnimBool();
        private AnimBool constantRipplesPropertiesExpanded = new AnimBool();
        private AnimBool scriptGeneratedRipplesPropertiesExpanded = new AnimBool();
        private AnimBool refractionPropertiesExpanded = new AnimBool();
        private AnimBool reflectionPropertiesExpanded = new AnimBool();
        private AnimBool renderingSettingsExpanded = new AnimBool();
        private AnimBool prefabUtilityExpanded = new AnimBool();
        private AnimBool activateOnCollisionOnWaterEnterParticleEffectExpanded = new AnimBool();
        private AnimBool activateOnCollisionOnWaterExitParticleEffectExpanded = new AnimBool();
        private AnimBool activateConstantSplashParticleEffectExpanded = new AnimBool();
        private AnimBool activateScriptGeneratedSplashParticleEffectExpanded = new AnimBool();
        private AnimBool activateOnCollisionOnWaterEnterSoundEffectExpanded = new AnimBool();
        private AnimBool activateOnCollisionOnWaterExitSoundEffectExpanded = new AnimBool();
        private AnimBool activateConstantSplashSoundEffectExpanded = new AnimBool();
        private AnimBool activateScriptGeneratedSplashSoundEffectExpanded = new AnimBool();
        private AnimBool activateOnCollisionOnWaterEnterRipplesExpanded = new AnimBool();
        private AnimBool activateOnCollisionOnWaterExitRipplesExpanded = new AnimBool();

        private static readonly Color wireframeColor = new Color(0.89f, 0.259f, 0.204f, 0.375f);
        private static readonly Color constantRipplesSourcesColorAdd = Color.green;
        private static readonly Color constantRipplesSourcesColorRemove = Color.red;
        private static readonly Color buoyancyEffectorSurfaceLevelGuidelineColor = Color.cyan;

        private bool isMultiEditing = false;
        private bool constantRipplesEditSourcesPositions = false;
        private string prefabsPath;
        private UnityAction repaint;
        #endregion

        #endregion

        #region Methods

        private void OnEnable()
        {
            repaint = new UnityAction(Repaint);
            isMultiEditing = targets.Length > 1;

            //Water Properties
            //Mesh Properties
            waterSize = serializedObject.FindProperty("waterSize");
            subdivisionsCountPerUnit = serializedObject.FindProperty("subdivisionsCountPerUnit");
            waterPropertiesExpanded.valueChanged.AddListener(repaint);
            waterPropertiesExpanded.target = EditorPrefs.GetBool("Water2D_WaterPropertiesExpanded", false);
            //Water Wave Properties
            damping = serializedObject.FindProperty("damping");
            stiffness = serializedObject.FindProperty("stiffness");
            spread = serializedObject.FindProperty("spread");
            useCustomBoundaries = serializedObject.FindProperty("useCustomBoundaries");
            firstCustomBoundary = serializedObject.FindProperty("firstCustomBoundary");
            secondCustomBoundary = serializedObject.FindProperty("secondCustomBoundary");
            //Water Events Properties
            onWaterEnter = serializedObject.FindProperty("onWaterEnter");
            onWaterExit = serializedObject.FindProperty("onWaterExit");
            //Misc Properties
            buoyancyEffectorSurfaceLevel = serializedObject.FindProperty("buoyancyEffectorSurfaceLevel");

            //On-Collision Ripples Properties
            onCollisionRipplesPropertiesExpanded.valueChanged.AddListener(repaint);
            onCollisionRipplesPropertiesExpanded.target = EditorPrefs.GetBool("Water2D_OnCollisionRipplesPropertiesExpanded", false);
            //Disturbance Properties
            onCollisionRipplesMinimumDisturbance = serializedObject.FindProperty("onCollisionRipplesMinimumDisturbance");
            onCollisionRipplesMaximumDisturbance = serializedObject.FindProperty("onCollisionRipplesMaximumDisturbance");
            onCollisionRipplesVelocityMultiplier = serializedObject.FindProperty("onCollisionRipplesVelocityMultiplier");
            //Collision Properties
            onCollisionRipplesCollisionMask = serializedObject.FindProperty("onCollisionRipplesCollisionMask");
            onCollisionRipplesCollisionMinimumDepth = serializedObject.FindProperty("onCollisionRipplesCollisionMinimumDepth");
            onCollisionRipplesCollisionMaximumDepth = serializedObject.FindProperty("onCollisionRipplesCollisionMaximumDepth");
            onCollisionRipplesCollisionRaycastMaxDistance = serializedObject.FindProperty("onCollisionRipplesCollisionRaycastMaxDistance");
            //On Water Enter Ripples Properties
            activateOnCollisionOnWaterEnterRipples = serializedObject.FindProperty("activateOnCollisionOnWaterEnterRipples");
            activateOnCollisionOnWaterEnterRipplesExpanded.valueChanged.AddListener(repaint);
            activateOnCollisionOnWaterEnterRipplesExpanded.target = activateOnCollisionOnWaterEnterRipples.boolValue;
            //Sound Effect Properies (On Water Enter)
            onCollisionRipplesActivateOnWaterEnterSoundEffect = serializedObject.FindProperty("onCollisionRipplesActivateOnWaterEnterSoundEffect");
            onCollisionRipplesOnWaterEnterAudioClip = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterAudioClip");
            onCollisionRipplesOnWaterEnterMinimumAudioPitch = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterMinimumAudioPitch");
            onCollisionRipplesOnWaterEnterMaximumAudioPitch = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterMaximumAudioPitch");
            onCollisionRipplesUseConstantOnWaterEnterAudioPitch = serializedObject.FindProperty("onCollisionRipplesUseConstantOnWaterEnterAudioPitch");
            onCollisionRipplesOnWaterEnterAudioPitch = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterAudioPitch");
            onCollisionRipplesOnWaterEnterAudioVolume = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterAudioVolume");
            onCollisionRipplesOnWaterEnterSoundEffectPoolSize = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterSoundEffectPoolSize");
            onCollisionRipplesReconstructOnWaterEnterSoundEffectPool = serializedObject.FindProperty("onCollisionRipplesReconstructOnWaterEnterSoundEffectPool");
            onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary");
            activateOnCollisionOnWaterEnterSoundEffectExpanded.valueChanged.AddListener(repaint);
            activateOnCollisionOnWaterEnterSoundEffectExpanded.target = onCollisionRipplesActivateOnWaterEnterSoundEffect.boolValue;
            //Particle Effect Properties (OnWaterEnter)
            onCollisionRipplesActivateOnWaterEnterParticleEffect = serializedObject.FindProperty("onCollisionRipplesActivateOnWaterEnterParticleEffect");
            onCollisionRipplesOnWaterEnterParticleEffect = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterParticleEffect");
            onCollisionRipplesOnWaterEnterParticleEffectPoolSize = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterParticleEffectPoolSize");
            onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset");
            onCollisionRipplesOnWaterEnterParticleEffectStopAction = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterParticleEffectStopAction");
            onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary = serializedObject.FindProperty("onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary");
            onCollisionRipplesReconstructOnWaterEnterParticleEffectPool = serializedObject.FindProperty("onCollisionRipplesReconstructOnWaterEnterParticleEffectPool");
            activateOnCollisionOnWaterEnterParticleEffectExpanded.valueChanged.AddListener(repaint);
            activateOnCollisionOnWaterEnterParticleEffectExpanded.target = onCollisionRipplesActivateOnWaterEnterParticleEffect.boolValue;
            //On Water Exit Ripples Properties
            activateOnCollisionOnWaterExitRipples = serializedObject.FindProperty("activateOnCollisionOnWaterExitRipples");
            activateOnCollisionOnWaterExitRipplesExpanded.valueChanged.AddListener(repaint);
            activateOnCollisionOnWaterExitRipplesExpanded.target = activateOnCollisionOnWaterExitRipples.boolValue;
            //Sound Effect Properies (On Water Exit)
            onCollisionRipplesActivateOnWaterExitSoundEffect = serializedObject.FindProperty("onCollisionRipplesActivateOnWaterExitSoundEffect");
            onCollisionRipplesOnWaterExitAudioClip = serializedObject.FindProperty("onCollisionRipplesOnWaterExitAudioClip");
            onCollisionRipplesOnWaterExitMinimumAudioPitch = serializedObject.FindProperty("onCollisionRipplesOnWaterExitMinimumAudioPitch");
            onCollisionRipplesOnWaterExitMaximumAudioPitch = serializedObject.FindProperty("onCollisionRipplesOnWaterExitMaximumAudioPitch");
            onCollisionRipplesUseConstantOnWaterExitAudioPitch = serializedObject.FindProperty("onCollisionRipplesUseConstantOnWaterExitAudioPitch");
            onCollisionRipplesOnWaterExitAudioPitch = serializedObject.FindProperty("onCollisionRipplesOnWaterExitAudioPitch");
            onCollisionRipplesOnWaterExitAudioVolume = serializedObject.FindProperty("onCollisionRipplesOnWaterExitAudioVolume");
            onCollisionRipplesOnWaterExitSoundEffectPoolSize = serializedObject.FindProperty("onCollisionRipplesOnWaterExitSoundEffectPoolSize");
            onCollisionRipplesReconstructOnWaterExitSoundEffectPool = serializedObject.FindProperty("onCollisionRipplesReconstructOnWaterExitSoundEffectPool");
            onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary = serializedObject.FindProperty("onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary");
            activateOnCollisionOnWaterExitSoundEffectExpanded.valueChanged.AddListener(repaint);
            activateOnCollisionOnWaterExitSoundEffectExpanded.target = onCollisionRipplesActivateOnWaterExitSoundEffect.boolValue;
            //Particle Effect Properties (On Water Exit)
            onCollisionRipplesActivateOnWaterExitParticleEffect = serializedObject.FindProperty("onCollisionRipplesActivateOnWaterExitParticleEffect");
            onCollisionRipplesOnWaterExitParticleEffect = serializedObject.FindProperty("onCollisionRipplesOnWaterExitParticleEffect");
            onCollisionRipplesOnWaterExitParticleEffectPoolSize = serializedObject.FindProperty("onCollisionRipplesOnWaterExitParticleEffectPoolSize");
            onCollisionRipplesOnWaterExitParticleEffectSpawnOffset = serializedObject.FindProperty("onCollisionRipplesOnWaterExitParticleEffectSpawnOffset");
            onCollisionRipplesOnWaterExitParticleEffectStopAction = serializedObject.FindProperty("onCollisionRipplesOnWaterExitParticleEffectStopAction");
            onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary = serializedObject.FindProperty("onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary");
            onCollisionRipplesReconstructOnWaterExitParticleEffectPool = serializedObject.FindProperty("onCollisionRipplesReconstructOnWaterExitParticleEffectPool");
            activateOnCollisionOnWaterExitParticleEffectExpanded.valueChanged.AddListener(repaint);
            activateOnCollisionOnWaterExitParticleEffectExpanded.target = onCollisionRipplesActivateOnWaterExitParticleEffect.boolValue;

            //Constant Ripples Properties
            activateConstantRipples = serializedObject.FindProperty("activateConstantRipples");
            constantRipplesPropertiesExpanded.valueChanged.AddListener(repaint);
            constantRipplesPropertiesExpanded.target = EditorPrefs.GetBool("Water2D_ConstantRipplesPropertiesExpanded", false);
            //Disturbance Properties
            constantRipplesDisturbance = serializedObject.FindProperty("constantRipplesDisturbance");
            constantRipplesUpdateWhenOffscreen = serializedObject.FindProperty("constantRipplesUpdateWhenOffscreen");
            constantRipplesRandomizeDisturbance = serializedObject.FindProperty("constantRipplesRandomizeDisturbance");
            constantRipplesMinimumDisturbance = serializedObject.FindProperty("constantRipplesMinimumDisturbance");
            constantRipplesMaximumDisturbance = serializedObject.FindProperty("constantRipplesMaximumDisturbance");
            constantRipplesSmoothDisturbance = serializedObject.FindProperty("constantRipplesSmoothDisturbance");
            constantRipplesSmoothFactor = serializedObject.FindProperty("constantRipplesSmoothFactor");
            //Interval Proeprties
            constantRipplesRandomizeInterval = serializedObject.FindProperty("constantRipplesRandomizeInterval");
            constantRipplesInterval = serializedObject.FindProperty("constantRipplesInterval");
            constantRipplesMinimumInterval = serializedObject.FindProperty("constantRipplesMinimumInterval");
            constantRipplesMaximumInterval = serializedObject.FindProperty("constantRipplesMaximumInterval");
            //Ripple Source Position
            constantRipplesRandomizeRipplesSourcesPositions = serializedObject.FindProperty("constantRipplesRandomizeRipplesSourcesPositions");
            constantRipplesRandomizeRipplesSourcesCount = serializedObject.FindProperty("constantRipplesRandomizeRipplesSourcesCount");
            constantRipplesSourcePositions = serializedObject.FindProperty("constantRipplesSourcePositions");
            constantRipplesAllowDuplicateRipplesSourcesPositions = serializedObject.FindProperty("constantRipplesAllowDuplicateRipplesSourcesPositions");
            //Sound Effect Properties
            constantRipplesActivateSoundEffect = serializedObject.FindProperty("constantRipplesActivateSoundEffect");
            constantRipplesUseConstantAudioPitch = serializedObject.FindProperty("constantRipplesUseConstantAudioPitch");
            constantRipplesAudioPitch = serializedObject.FindProperty("constantRipplesAudioPitch");
            constantRipplesAudioVolume = serializedObject.FindProperty("constantRipplesAudioVolume");
            constantRipplesMinimumAudioPitch = serializedObject.FindProperty("constantRipplesMinimumAudioPitch");
            constantRipplesMaximumAudioPitch = serializedObject.FindProperty("constantRipplesMaximumAudioPitch");
            constantRipplesAudioClip = serializedObject.FindProperty("constantRipplesAudioClip");
            constantRipplesSoundEffectPoolSize = serializedObject.FindProperty("constantRipplesSoundEffectPoolSize");
            constantRipplesReconstructSoundEffectPool = serializedObject.FindProperty("constantRipplesReconstructSoundEffectPool");
            constantRipplesSoundEffectPoolExpandIfNecessary = serializedObject.FindProperty("constantRipplesSoundEffectPoolExpandIfNecessary");
            activateConstantSplashSoundEffectExpanded.valueChanged.AddListener(repaint);
            activateConstantSplashSoundEffectExpanded.target = constantRipplesActivateSoundEffect.boolValue;
            //Particle Effect Proeprties
            constantRipplesActivateParticleEffect = serializedObject.FindProperty("constantRipplesActivateParticleEffect");
            constantRipplesParticleEffect = serializedObject.FindProperty("constantRipplesParticleEffect");
            constantRipplesParticleEffectPoolSize = serializedObject.FindProperty("constantRipplesParticleEffectPoolSize");
            constantRipplesParticleEffectSpawnOffset = serializedObject.FindProperty("constantRipplesParticleEffectSpawnOffset");
            constantRipplesParticleEffectStopAction = serializedObject.FindProperty("constantRipplesParticleEffectStopAction");
            constantRipplesReconstructParticleEffectPool = serializedObject.FindProperty("constantRipplesReconstructParticleEffectPool");
            constantRipplesParticleEffectPoolExpandIfNecessary = serializedObject.FindProperty("constantRipplesParticleEffectPoolExpandIfNecessary");
            activateConstantSplashParticleEffectExpanded.valueChanged.AddListener(repaint);
            activateConstantSplashParticleEffectExpanded.target = constantRipplesActivateParticleEffect.boolValue;

            //Script-Generated Ripples Properties
            scriptGeneratedRipplesPropertiesExpanded.valueChanged.AddListener(repaint);
            scriptGeneratedRipplesPropertiesExpanded.target = EditorPrefs.GetBool("Water2D_ScriptGeneratedRipplesPropertiesExpanded", false);
            //Disturbance Properties
            scriptGeneratedRipplesMinimumDisturbance = serializedObject.FindProperty("scriptGeneratedRipplesMinimumDisturbance");
            scriptGeneratedRipplesMaximumDisturbance = serializedObject.FindProperty("scriptGeneratedRipplesMaximumDisturbance");
            //Sound Effect Properties
            scriptGeneratedRipplesActivateSoundEffect = serializedObject.FindProperty("scriptGeneratedRipplesActivateSoundEffect");
            scriptGeneratedRipplesUseConstantAudioPitch = serializedObject.FindProperty("scriptGeneratedRipplesUseConstantAudioPitch");
            scriptGeneratedRipplesAudioPitch = serializedObject.FindProperty("scriptGeneratedRipplesAudioPitch");
            scriptGeneratedRipplesAudioVolume = serializedObject.FindProperty("scriptGeneratedRipplesAudioVolume");
            scriptGeneratedRipplesMinimumAudioPitch = serializedObject.FindProperty("scriptGeneratedRipplesMinimumAudioPitch");
            scriptGeneratedRipplesMaximumAudioPitch = serializedObject.FindProperty("scriptGeneratedRipplesMaximumAudioPitch");
            scriptGeneratedRipplesAudioClip = serializedObject.FindProperty("scriptGeneratedRipplesAudioClip");
            scriptGeneratedRipplesSoundEffectPoolSize = serializedObject.FindProperty("scriptGeneratedRipplesSoundEffectPoolSize");
            scriptGeneratedRipplesReconstructSoundEffectPool = serializedObject.FindProperty("scriptGeneratedRipplesReconstructSoundEffectPool");
            scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary = serializedObject.FindProperty("scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary");
            activateScriptGeneratedSplashSoundEffectExpanded.valueChanged.AddListener(repaint);
            activateScriptGeneratedSplashSoundEffectExpanded.target = scriptGeneratedRipplesActivateSoundEffect.boolValue;
            //Particle Effect Properties
            scriptGeneratedRipplesActivateParticleEffect = serializedObject.FindProperty("scriptGeneratedRipplesActivateParticleEffect");
            scriptGeneratedRipplesParticleEffect = serializedObject.FindProperty("scriptGeneratedRipplesParticleEffect");
            scriptGeneratedRipplesParticleEffectPoolSize = serializedObject.FindProperty("scriptGeneratedRipplesParticleEffectPoolSize");
            scriptGeneratedRipplesParticleEffectSpawnOffset = serializedObject.FindProperty("scriptGeneratedRipplesParticleEffectSpawnOffset");
            scriptGeneratedRipplesParticleEffectStopAction = serializedObject.FindProperty("scriptGeneratedRipplesParticleEffectStopAction");
            scriptGeneratedRipplesReconstructParticleEffectPool = serializedObject.FindProperty("scriptGeneratedRipplesReconstructParticleEffectPool");
            scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary = serializedObject.FindProperty("scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary");
            activateScriptGeneratedSplashParticleEffectExpanded.valueChanged.AddListener(repaint);
            activateScriptGeneratedSplashParticleEffectExpanded.target = scriptGeneratedRipplesActivateParticleEffect.boolValue;

            //Reflection & Refraction Rendering Proeprties
            //Refraction Properties
            refractionRenderTextureResizeFactor = serializedObject.FindProperty("refractionRenderTextureResizeFactor");
            refractionCullingMask = serializedObject.FindProperty("refractionCullingMask");
            refractionRenderTextureFilterMode = serializedObject.FindProperty("refractionRenderTextureFilterMode");
            refractionPropertiesExpanded.valueChanged.AddListener(repaint);
            refractionPropertiesExpanded.target = EditorPrefs.GetBool("Water2D_RefractionPropertiesExpanded", false);
            //Reflection Properties
            reflectionRenderTextureResizeFactor = serializedObject.FindProperty("reflectionRenderTextureResizeFactor");
            reflectionCullingMask = serializedObject.FindProperty("reflectionCullingMask");
            reflectionZOffset = serializedObject.FindProperty("reflectionZOffset");
            reflectionRenderTextureFilterMode = serializedObject.FindProperty("reflectionRenderTextureFilterMode");
            reflectionPropertiesExpanded.valueChanged.AddListener(repaint);
            reflectionPropertiesExpanded.target = EditorPrefs.GetBool("Water2D_ReflectionPropertiesExpanded", false);
            //Rendering Properties
            renderPixelLights = serializedObject.FindProperty("renderPixelLights");
            sortingLayerID = serializedObject.FindProperty("sortingLayerID");
            sortingOrder = serializedObject.FindProperty("sortingOrder");
            allowMSAA = serializedObject.FindProperty("allowMSAA");
            allowHDR = serializedObject.FindProperty("allowHDR");
            farClipPlane = serializedObject.FindProperty("farClipPlane");
            renderingSettingsExpanded.valueChanged.AddListener(repaint);
            renderingSettingsExpanded.target = EditorPrefs.GetBool("Water2D_RenderingSettingsExpanded", false);

            //Prefabs Utility
            prefabUtilityExpanded.valueChanged.AddListener(repaint);
            prefabUtilityExpanded.target = EditorPrefs.GetBool("Water2D_PrefabUtilityExpanded", false);
            prefabsPath = EditorPrefs.GetString("Water2D_Paths_PrefabUtility_Path", "Assets/");
        }

        private void OnDisable()
        {
            //Water Proeperties
            waterPropertiesExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_WaterPropertiesExpanded", waterPropertiesExpanded.target);

            //On-Collision Ripples Properties
            onCollisionRipplesPropertiesExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_OnCollisionRipplesPropertiesExpanded", onCollisionRipplesPropertiesExpanded.target);

            //Constant Ripples Properties
            constantRipplesPropertiesExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_ConstantRipplesPropertiesExpanded", constantRipplesPropertiesExpanded.target);

            //Script-Generated Ripples Properties
            scriptGeneratedRipplesPropertiesExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_ScriptGeneratedRipplesPropertiesExpanded", scriptGeneratedRipplesPropertiesExpanded.target);

            //Refraction & Reflection Rendering Properties
            refractionPropertiesExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_RefractionPropertiesExpanded", refractionPropertiesExpanded.target);

            reflectionPropertiesExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_ReflectionPropertiesExpanded", reflectionPropertiesExpanded.target);

            renderingSettingsExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_RenderingSettingsExpanded", renderingSettingsExpanded.target);

            //Prefabs Utility
            prefabUtilityExpanded.valueChanged.RemoveListener(repaint);
            EditorPrefs.SetBool("Water2D_PrefabUtilityExpanded", prefabUtilityExpanded.target);

            EditorPrefs.SetString("Water2D_Paths_PrefabUtility_Path", prefabsPath);
        }

        #region Inspector

        public override void OnInspectorGUI()
        {
            Game2DWater water2D = target as Game2DWater;
            Material water2DMaterial = water2D.GetComponent<MeshRenderer>().sharedMaterial;
            bool hasRefraction = false;
            bool hasReflection = false;
            if (water2DMaterial)
            {
                hasRefraction = water2DMaterial.IsKeywordEnabled("Water2D_Refraction");
                hasReflection = water2DMaterial.IsKeywordEnabled("Water2D_Reflection");
            }

            serializedObject.Update();

            if (!isMultiEditing)
                DrawFixScalingField(water2D);

            DrawWaterProperties(water2D);
            DrawOnCollisionRipplesProperties(water2D);
            DrawConstantRipplesProperties(water2D);
            DrawScriptGeneratedRipplesProperties(water2D);
            DrawRefractionProperties(hasRefraction);
            DrawReflectionProperties(hasReflection);
            DrawRenderingSettingsProperties(hasRefraction, hasReflection);

#if UNITY_2018_3_OR_NEWER
            //Editing the Prefab GameObjects in the Project Browser is no longer supported as of Unity 2018.3 due to the technical changes in the Prefabs back-end.
            bool isPrefabSelectedInProjectBrowser = Application.isPlaying || UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(water2D.gameObject);
#else
            bool isPrefabSelectedInProjectBrowser = PrefabUtility.GetPrefabType(water2D) == PrefabType.Prefab;
#endif

            if (!isMultiEditing && !isPrefabSelectedInProjectBrowser)
                DrawPrefabUtility(water2D, water2DMaterial);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onWaterEnter, onWaterEnterLabel);
            EditorGUILayout.PropertyField(onWaterExit, onWaterExitLabel);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWaterProperties(Game2DWater water2D)
        {
            waterPropertiesExpanded.target = EditorGUILayout.Foldout(waterPropertiesExpanded.target, waterPropertiesFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(waterPropertiesExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField(meshPropertiesLabel, EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(waterSize, waterSizeLabel);
                    EditorGUILayout.PropertyField(subdivisionsCountPerUnit, subdivisionsCountPerUnitLabel);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(wavePropertiesLabel, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(stiffness, stiffnessLabel);
                    EditorGUILayout.PropertyField(spread, spreadLabel);
                    EditorGUILayout.Slider(damping, 0f, 1f, dampingLabel);
                    EditorGUILayout.PropertyField(useCustomBoundaries, useCustomBoundariesLabel);
                    if (useCustomBoundaries.boolValue)
                    {
                        EditorGUILayout.PropertyField(firstCustomBoundary, firstCustomBoundaryLabel);
                        EditorGUILayout.PropertyField(secondCustomBoundary, secondCustomBoundaryLabel);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(miscLabel, EditorStyles.boldLabel);
                    EditorGUILayout.Slider(buoyancyEffectorSurfaceLevel, 0f, 1f, buoyancyEffectorSurfaceLevelLabel);
                    if (!isMultiEditing)
                        DrawEdgeColliderPropertyField(water2D);

                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawFixScalingField(Game2DWater water2D)
        {
            Vector2 scale = water2D.transform.localScale;
            if (!Mathf.Approximately(scale.x, 1f) || !Mathf.Approximately(scale.y, 1f))
            {
                EditorGUILayout.HelpBox(nonUniformScaleWarning, MessageType.Warning, true);
                if (GUILayout.Button(fixScalingButtonLabel))
                {
                    waterSize.vector2Value = Vector2.Scale(waterSize.vector2Value, scale);
                    water2D.transform.localScale = Vector3.one;
                }
            }
        }

        private void DrawEdgeColliderPropertyField(Game2DWater water2D)
        {
            EditorGUI.BeginChangeCheck();
            bool useEdgeCollider = EditorGUILayout.Toggle(useEdgeCollider2DLabel, water2D.GetComponent<EdgeCollider2D>() != null);
            if (EditorGUI.EndChangeCheck())
            {
                if (useEdgeCollider)
                {
                    water2D.gameObject.AddComponent<EdgeCollider2D>();
                    water2D.OnValidate();
                }
                else
                    DestroyImmediate(water2D.GetComponent<EdgeCollider2D>());
            }
        }

        private void DrawOnCollisionRipplesProperties(Game2DWater water2D)
        {
            onCollisionRipplesPropertiesExpanded.target = EditorGUILayout.Foldout(onCollisionRipplesPropertiesExpanded.target, onCollisionRipplesPropertiesFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(onCollisionRipplesPropertiesExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(disturbancePropertiesLabel, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesMinimumDisturbance, onCollisionRipplesMinimumDisturbanceLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesMaximumDisturbance, onCollisionRipplesMaximumDisturbanceLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesVelocityMultiplier, onCollisionRipplesVelocityMultiplierLabel);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(collisionPropertiesLabel, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesCollisionMask, onCollisionRipplesCollisionMaskLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesCollisionMinimumDepth, onCollisionRipplesCollisionMinimumDepthLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesCollisionMaximumDepth, onCollisionRipplesCollisionMaximumDepthLabel);
                    EditorGUILayout.PropertyField(onCollisionRipplesCollisionRaycastMaxDistance, onCollisionRipplesCollisionRaycastMaxDistanceLabel);

                    EditorGUILayout.Space();
                    //On Water Enter Ripples Properties
                    EditorGUILayout.LabelField(onCollisionOnWaterEnterRipplesLabel, EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = activateOnCollisionOnWaterEnterRipples.hasMultipleDifferentValues;
                    bool onWaterEnterRipplesState = EditorGUILayout.ToggleLeft(activateOnCollisionOnWaterEnterRipplesLabel, activateOnCollisionOnWaterEnterRipples.boolValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        activateOnCollisionOnWaterEnterRipples.boolValue = onWaterEnterRipplesState;
                        activateOnCollisionOnWaterEnterRipplesExpanded.target = onWaterEnterRipplesState;
                    }
                    EditorGUI.showMixedValue = false;

                    using (var onWaterEnterSubGroup = new EditorGUILayout.FadeGroupScope(activateOnCollisionOnWaterEnterRipplesExpanded.faded))
                    {
                        if (onWaterEnterSubGroup.visible)
                        {
                            EditorGUI.indentLevel++;

                            //Sound Effect
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = onCollisionRipplesActivateOnWaterEnterSoundEffect.hasMultipleDifferentValues;
                            bool onWaterEnterRipplesSoundEffectState = EditorGUILayout.ToggleLeft(onCollisionRipplesActivateOnWaterEnterSoundEffectLabel, onCollisionRipplesActivateOnWaterEnterSoundEffect.boolValue);
                            if (EditorGUI.EndChangeCheck())
                            {
                                onCollisionRipplesActivateOnWaterEnterSoundEffect.boolValue = onWaterEnterRipplesSoundEffectState;
                                activateOnCollisionOnWaterEnterSoundEffectExpanded.target = onWaterEnterRipplesSoundEffectState;
                            }
                            EditorGUI.showMixedValue = false;

                            using (var subGroup = new EditorGUILayout.FadeGroupScope(activateOnCollisionOnWaterEnterSoundEffectExpanded.faded))
                            {
                                if (subGroup.visible)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterAudioClip, onCollisionRipplesOnWaterEnterAudioClipLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterSoundEffectPoolSize, onCollisionRipplesOnWaterEnterSoundEffectPoolSizeLabel);
                                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                        onCollisionRipplesReconstructOnWaterEnterSoundEffectPool.boolValue = true;
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary, onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessaryLabel);
                                    EditorGUILayout.Slider(onCollisionRipplesOnWaterEnterAudioVolume, 0f, 1f, onCollisionRipplesOnWaterEnterAudioVolumeLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesUseConstantOnWaterEnterAudioPitch, onCollisionRipplesUseConstantOnWaterEnterAudioPitchLabel);
                                    if (onCollisionRipplesUseConstantOnWaterEnterAudioPitch.boolValue)
                                    {
                                        EditorGUILayout.Slider(onCollisionRipplesOnWaterEnterAudioPitch, -3f, 3f, onCollisionRipplesOnWaterEnterAudioPitchLabel);
                                    }
                                    else
                                    {
                                        EditorGUILayout.Slider(onCollisionRipplesOnWaterEnterMinimumAudioPitch, -3f, 3f, onCollisionRipplesOnWaterEnterMinimumAudioPitchLabel);
                                        EditorGUILayout.Slider(onCollisionRipplesOnWaterEnterMaximumAudioPitch, -3f, 3f, onCollisionRipplesOnWaterEnterMaximumAudioPitchLabel);
                                        EditorGUILayout.HelpBox(onCollisionRipplesOnWaterEnterAudioPitchMessage, MessageType.None, true);
                                    }
                                    EditorGUI.indentLevel--;
                                }
                            }

                            //Particle Effect

                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = onCollisionRipplesActivateOnWaterEnterParticleEffect.hasMultipleDifferentValues;
                            bool onWaterEnterRipplesParticleEffectState = EditorGUILayout.ToggleLeft(onCollisionRipplesActivateOnWaterEnterParticleEffectLabel, onCollisionRipplesActivateOnWaterEnterParticleEffect.boolValue);
                            if (EditorGUI.EndChangeCheck())
                            {
                                onCollisionRipplesActivateOnWaterEnterParticleEffect.boolValue = onWaterEnterRipplesParticleEffectState;
                                activateOnCollisionOnWaterEnterParticleEffectExpanded.target = onWaterEnterRipplesParticleEffectState;
                            }
                            EditorGUI.showMixedValue = false;

                            using (var subGroup = new EditorGUILayout.FadeGroupScope(activateOnCollisionOnWaterEnterParticleEffectExpanded.faded))
                            {
                                if (subGroup.visible)
                                {
                                    EditorGUI.indentLevel++;
                                    ParticleSystem particleSystem = onCollisionRipplesOnWaterEnterParticleEffect.objectReferenceValue as ParticleSystem;
                                    if (particleSystem != null && particleSystem.main.loop)
                                        EditorGUILayout.HelpBox(particleSystemLoopMessage, MessageType.Warning);
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterParticleEffect, onCollisionRipplesOnWaterEnterParticleEffectLabel);
                                    EditorGUILayout.DelayedIntField(onCollisionRipplesOnWaterEnterParticleEffectPoolSize, onCollisionRipplesOnWaterEnterParticleEffectPoolSizeLabel);
                                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                        onCollisionRipplesReconstructOnWaterEnterParticleEffectPool.boolValue = true;
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary, onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessaryLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset, onCollisionRipplesOnWaterEnterParticleEffectSpawnOffsetLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterEnterParticleEffectStopAction, onCollisionRipplesOnWaterEnterParticleEffectStopActionLabel);
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUILayout.Space();

                    //On Water Exit Ripples Properties
                    EditorGUILayout.LabelField(onCollisionOnWaterExitRipplesLabel, EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = activateOnCollisionOnWaterExitRipples.hasMultipleDifferentValues;
                    bool onWaterExitRipplesState = EditorGUILayout.ToggleLeft(activateOnCollisionOnWaterExitRipplesLabel, activateOnCollisionOnWaterExitRipples.boolValue, EditorStyles.boldLabel);
                    if (EditorGUI.EndChangeCheck())
                    {
                        activateOnCollisionOnWaterExitRipples.boolValue = onWaterExitRipplesState;
                        activateOnCollisionOnWaterExitRipplesExpanded.target = onWaterExitRipplesState;
                    }
                    EditorGUI.showMixedValue = false;

                    using (var onWaterExitSubGroup = new EditorGUILayout.FadeGroupScope(activateOnCollisionOnWaterExitRipplesExpanded.faded))
                    {
                        if (onWaterExitSubGroup.visible)
                        {
                            EditorGUI.indentLevel++;

                            //Sound Effect
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = onCollisionRipplesActivateOnWaterExitSoundEffect.hasMultipleDifferentValues;
                            bool onWaterExitRipplesSoundEffectState = EditorGUILayout.ToggleLeft(onCollisionRipplesActivateOnWaterExitSoundEffectLabel, onCollisionRipplesActivateOnWaterExitSoundEffect.boolValue);
                            if (EditorGUI.EndChangeCheck())
                            {
                                onCollisionRipplesActivateOnWaterExitSoundEffect.boolValue = onWaterExitRipplesSoundEffectState;
                                activateOnCollisionOnWaterExitSoundEffectExpanded.target = onWaterExitRipplesSoundEffectState;
                            }
                            EditorGUI.showMixedValue = false;

                            using (var subGroup = new EditorGUILayout.FadeGroupScope(activateOnCollisionOnWaterExitSoundEffectExpanded.faded))
                            {
                                if (subGroup.visible)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitAudioClip, onCollisionRipplesOnWaterExitAudioClipLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitSoundEffectPoolSize, onCollisionRipplesOnWaterExitSoundEffectPoolSizeLabel);
                                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                        onCollisionRipplesReconstructOnWaterExitSoundEffectPool.boolValue = true;
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary, onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessaryLabel);
                                    EditorGUILayout.Slider(onCollisionRipplesOnWaterExitAudioVolume, 0f, 1f, onCollisionRipplesOnWaterExitAudioVolumeLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesUseConstantOnWaterExitAudioPitch, onCollisionRipplesUseConstantOnWaterExitAudioPitchLabel);
                                    if (onCollisionRipplesUseConstantOnWaterExitAudioPitch.boolValue)
                                    {
                                        EditorGUILayout.Slider(onCollisionRipplesOnWaterExitAudioPitch, -3f, 3f, onCollisionRipplesOnWaterExitAudioPitchLabel);
                                    }
                                    else
                                    {
                                        EditorGUILayout.Slider(onCollisionRipplesOnWaterExitMinimumAudioPitch, -3f, 3f, onCollisionRipplesOnWaterExitMinimumAudioPitchLabel);
                                        EditorGUILayout.Slider(onCollisionRipplesOnWaterExitMaximumAudioPitch, -3f, 3f, onCollisionRipplesOnWaterExitMaximumAudioPitchLabel);
                                        EditorGUILayout.HelpBox(onCollisionRipplesOnWaterExitAudioPitchMessage, MessageType.None, true);
                                    }
                                    EditorGUI.indentLevel--;
                                }
                            }

                            //Particle Effect

                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = onCollisionRipplesActivateOnWaterExitParticleEffect.hasMultipleDifferentValues;
                            bool onWaterExitRipplesParticleEffectState = EditorGUILayout.ToggleLeft(onCollisionRipplesActivateOnWaterExitParticleEffectLabel, onCollisionRipplesActivateOnWaterExitParticleEffect.boolValue);
                            if (EditorGUI.EndChangeCheck())
                            {
                                onCollisionRipplesActivateOnWaterExitParticleEffect.boolValue = onWaterExitRipplesParticleEffectState;
                                activateOnCollisionOnWaterExitParticleEffectExpanded.target = onWaterExitRipplesParticleEffectState;
                            }
                            EditorGUI.showMixedValue = false;

                            using (var subGroup = new EditorGUILayout.FadeGroupScope(activateOnCollisionOnWaterExitParticleEffectExpanded.faded))
                            {
                                if (subGroup.visible)
                                {
                                    EditorGUI.indentLevel++;
                                    ParticleSystem particleSystem = onCollisionRipplesOnWaterExitParticleEffect.objectReferenceValue as ParticleSystem;
                                    if (particleSystem != null && particleSystem.main.loop)
                                        EditorGUILayout.HelpBox(particleSystemLoopMessage, MessageType.Warning);
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitParticleEffect, onCollisionRipplesOnWaterExitParticleEffectLabel);
                                    EditorGUILayout.DelayedIntField(onCollisionRipplesOnWaterExitParticleEffectPoolSize, onCollisionRipplesOnWaterExitParticleEffectPoolSizeLabel);
                                    if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                        onCollisionRipplesReconstructOnWaterExitParticleEffectPool.boolValue = true;
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary, onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessaryLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitParticleEffectSpawnOffset, onCollisionRipplesOnWaterExitParticleEffectSpawnOffsetLabel);
                                    EditorGUILayout.PropertyField(onCollisionRipplesOnWaterExitParticleEffectStopAction, onCollisionRipplesOnWaterExitParticleEffectStopActionLabel);
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }


                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawConstantRipplesProperties(Game2DWater water2D)
        {
            constantRipplesPropertiesExpanded.target = EditorGUILayout.Foldout(constantRipplesPropertiesExpanded.target, constantRipplesPropertiesFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(constantRipplesPropertiesExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(activateConstantRipples, activateConstantRipplesLabel);

                    EditorGUI.BeginDisabledGroup(!activateConstantRipples.boolValue);
                    EditorGUILayout.PropertyField(constantRipplesUpdateWhenOffscreen, constantRipplesUpdateWhenOffscreenLabel);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField(disturbancePropertiesLabel, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(constantRipplesRandomizeDisturbance, constantRipplesRandomizeDisturbanceLabel);
                    bool randomizeDisturbance = constantRipplesRandomizeDisturbance.boolValue;
                    if (randomizeDisturbance)
                    {
                        EditorGUILayout.PropertyField(constantRipplesMinimumDisturbance, constantRipplesMinimumDisturbanceLabel);
                        EditorGUILayout.PropertyField(constantRipplesMaximumDisturbance, constantRipplesMaximumDisturbanceLabel);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(constantRipplesDisturbance, constantRipplesDisturbanceLabel);
                    }
                    EditorGUILayout.PropertyField(constantRipplesSmoothDisturbance, constantRipplesSmoothDisturbanceLabel);
                    bool smoothWave = constantRipplesSmoothDisturbance.boolValue;
                    if (smoothWave)
                        EditorGUILayout.Slider(constantRipplesSmoothFactor, 0f, 1f, constantRipplesSmoothFactorLabel);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(intervalPropertiesLabel, EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(constantRipplesRandomizeInterval, randomizePersistnetWaveIntervalLabel);
                    bool randomizeInterval = constantRipplesRandomizeInterval.boolValue;
                    if (randomizeInterval)
                    {
                        EditorGUILayout.PropertyField(constantRipplesMinimumInterval, constantRipplesMinimumIntervalLabel);
                        EditorGUILayout.PropertyField(constantRipplesMaximumInterval, constantRipplesMaximumIntervalLabel);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(constantRipplesInterval, constantRipplesIntervalLabel);
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(constantRipplesSourcesPropertiesLabel, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(constantRipplesRandomizeRipplesSourcesPositions, constantRipplesRandomizeRipplesSourcesPositionsLabel);
                    bool randomizeRipplesSources = constantRipplesRandomizeRipplesSourcesPositions.boolValue;
                    if (!randomizeRipplesSources)
                    {
                        EditorGUILayout.PropertyField(constantRipplesAllowDuplicateRipplesSourcesPositions, constantRipplesAllowDuplicateRipplesSourcesPositionsLabel);
                        EditorGUI.BeginDisabledGroup(isMultiEditing);
                        constantRipplesEditSourcesPositions = GUILayout.Toggle(constantRipplesEditSourcesPositions, constantRipplesEditSourcesPositionsLabel, "Button");
                        constantRipplesSourcePositions.isExpanded |= constantRipplesEditSourcesPositions;
                        EditorGUILayout.PropertyField(constantRipplesSourcePositions, constantRipplesSourcePositionsLabel, true);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(constantRipplesRandomizeRipplesSourcesCount, constantRipplesRandomizeRipplesSourcesCountLabel);
                        constantRipplesEditSourcesPositions = false;
                    }

                    //Sound Effect
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(soundEffectPropertiesLabel, EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(constantRipplesActivateSoundEffect, constantRipplesActivateSoundEffectLabel);
                    activateConstantSplashSoundEffectExpanded.target = constantRipplesActivateSoundEffect.boolValue;

                    using (var subGroup = new EditorGUILayout.FadeGroupScope(activateConstantSplashSoundEffectExpanded.faded))
                    {
                        if (subGroup.visible)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(constantRipplesAudioClip, constantRipplesAudioClipLabel);
                            EditorGUILayout.PropertyField(constantRipplesSoundEffectPoolSize, constantRipplesSoundEffectPoolSizeLabel);
                            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                constantRipplesReconstructSoundEffectPool.boolValue = true;
                            EditorGUILayout.PropertyField(constantRipplesSoundEffectPoolExpandIfNecessary, constantRipplesSoundEffectPoolExpandIfNecessaryLabel);
                            EditorGUILayout.Slider(constantRipplesAudioVolume, 0f, 1f, constantRipplesAudioVolumeLabel);
                            EditorGUILayout.PropertyField(constantRipplesUseConstantAudioPitch, constantRipplesUseConstantAudioPitchLabel);
                            if (constantRipplesUseConstantAudioPitch.boolValue)
                            {
                                EditorGUILayout.Slider(constantRipplesAudioPitch, -3f, 3f, constantRipplesAudioPitchLabel);
                            }
                            else
                            {
                                EditorGUILayout.Slider(constantRipplesMinimumAudioPitch, -3f, 3f, constantRipplesMinimumAudioPitchLabel);
                                EditorGUILayout.Slider(constantRipplesMaximumAudioPitch, -3f, 3f, constantRipplesMaximumAudioPitchLabel);
                                EditorGUILayout.HelpBox(constantRipplesAudioPitchMessage, MessageType.None, true);
                            }
                            EditorGUI.indentLevel--;
                        }
                    }

                    //Particle Effect
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(particleEffectPropertiesLabel, EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(constantRipplesActivateParticleEffect, constantRipplesActivateParticleEffectLabel);
                    activateConstantSplashParticleEffectExpanded.target = constantRipplesActivateParticleEffect.boolValue;

                    using (var subGroup = new EditorGUILayout.FadeGroupScope(activateConstantSplashParticleEffectExpanded.faded))
                    {
                        if (subGroup.visible)
                        {
                            EditorGUI.indentLevel++;
                            ParticleSystem particleSystem = constantRipplesParticleEffect.objectReferenceValue as ParticleSystem;
                            if (particleSystem != null && particleSystem.main.loop)
                                EditorGUILayout.HelpBox(particleSystemLoopMessage, MessageType.Warning);
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(constantRipplesParticleEffect, constantRipplesParticleEffectLabel);
                            EditorGUILayout.DelayedIntField(constantRipplesParticleEffectPoolSize, constantRipplesParticleEffectPoolSizeLabel);
                            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                constantRipplesReconstructParticleEffectPool.boolValue = true;
                            EditorGUILayout.PropertyField(constantRipplesParticleEffectPoolExpandIfNecessary, constantRipplesParticleEffectPoolExpandIfNecessaryLabel);
                            EditorGUILayout.PropertyField(constantRipplesParticleEffectSpawnOffset, constantRipplesParticleEffectSpawnOffsetLabel);
                            EditorGUILayout.PropertyField(constantRipplesParticleEffectStopAction, constantRipplesParticleEffectStopActionLabel);
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUI.indentLevel--;

                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void DrawScriptGeneratedRipplesProperties(Game2DWater water2D)
        {
            scriptGeneratedRipplesPropertiesExpanded.target = EditorGUILayout.Foldout(scriptGeneratedRipplesPropertiesExpanded.target, scriptGeneratedRipplesPropertiesFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(scriptGeneratedRipplesPropertiesExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.HelpBox(scriptGeneratedRipplesMessage, MessageType.None);
                    EditorGUILayout.LabelField(disturbancePropertiesLabel, EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(scriptGeneratedRipplesMinimumDisturbance, scriptGeneratedRipplesMinimumDisturbanceLabel);
                    EditorGUILayout.PropertyField(scriptGeneratedRipplesMaximumDisturbance, scriptGeneratedRipplesMaximumDisturbanceLabel);

                    //Sound Effect
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(soundEffectPropertiesLabel, EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(scriptGeneratedRipplesActivateSoundEffect, scriptGeneratedRipplesActivateSoundEffectLabel);
                    activateScriptGeneratedSplashSoundEffectExpanded.target = scriptGeneratedRipplesActivateSoundEffect.boolValue;

                    using (var subGroup = new EditorGUILayout.FadeGroupScope(activateScriptGeneratedSplashSoundEffectExpanded.faded))
                    {
                        if (subGroup.visible)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesAudioClip, scriptGeneratedRipplesAudioClipLabel);
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesSoundEffectPoolSize, scriptGeneratedRipplesSoundEffectPoolSizeLabel);
                            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                scriptGeneratedRipplesReconstructSoundEffectPool.boolValue = true;
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary, scriptGeneratedRipplesSoundEffectPoolExpandIfNecessaryLabel);
                            EditorGUILayout.Slider(scriptGeneratedRipplesAudioVolume, 0f, 1f, scriptGeneratedRipplesAudioVolumeLabel);
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesUseConstantAudioPitch, scriptGeneratedRipplesUseConstantAudioPitchLabel);
                            if (scriptGeneratedRipplesUseConstantAudioPitch.boolValue)
                            {
                                EditorGUILayout.Slider(scriptGeneratedRipplesAudioPitch, -3f, 3f, scriptGeneratedRipplesAudioPitchLabel);
                            }
                            else
                            {
                                EditorGUILayout.Slider(scriptGeneratedRipplesMinimumAudioPitch, -3f, 3f, scriptGeneratedRipplesMinimumAudioPitchLabel);
                                EditorGUILayout.Slider(scriptGeneratedRipplesMaximumAudioPitch, -3f, 3f, scriptGeneratedRipplesMaximumAudioPitchLabel);
                                EditorGUILayout.HelpBox(scriptGeneratedRipplesAudioPitchMessage, MessageType.None, true);
                            }
                            EditorGUI.indentLevel--;
                        }
                    }

                    //Particle Effect
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(particleEffectPropertiesLabel, EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(scriptGeneratedRipplesActivateParticleEffect, scriptGeneratedRipplesActivateParticleEffectLabel);
                    activateScriptGeneratedSplashParticleEffectExpanded.target = scriptGeneratedRipplesActivateParticleEffect.boolValue;

                    using (var subGroup = new EditorGUILayout.FadeGroupScope(activateScriptGeneratedSplashParticleEffectExpanded.faded))
                    {
                        if (subGroup.visible)
                        {
                            EditorGUI.indentLevel++;
                            ParticleSystem particleSystem = scriptGeneratedRipplesParticleEffect.objectReferenceValue as ParticleSystem;
                            if (particleSystem != null && particleSystem.main.loop)
                                EditorGUILayout.HelpBox(particleSystemLoopMessage, MessageType.Warning);
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesParticleEffect, scriptGeneratedRipplesParticleEffectLabel);
                            EditorGUILayout.DelayedIntField(scriptGeneratedRipplesParticleEffectPoolSize, scriptGeneratedRipplesParticleEffectPoolSizeLabel);
                            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                                scriptGeneratedRipplesReconstructParticleEffectPool.boolValue = true;
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary, scriptGeneratedRipplesParticleEffectPoolExpandIfNecessaryLabel);
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesParticleEffectSpawnOffset, scriptGeneratedRipplesParticleEffectSpawnOffsetLabel);
                            EditorGUILayout.PropertyField(scriptGeneratedRipplesParticleEffectStopAction, scriptGeneratedRipplesParticleEffectStopActionLabel);
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUI.indentLevel--;

                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void DrawRefractionProperties(bool hasRefraction)
        {
            refractionPropertiesExpanded.target = EditorGUILayout.Foldout(refractionPropertiesExpanded.target, refractionPropertiesFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(refractionPropertiesExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;

                    if (!hasRefraction)
                        EditorGUILayout.HelpBox(refractionMessage, MessageType.None, true);
                    EditorGUI.BeginDisabledGroup(!hasRefraction);
                    EditorGUILayout.PropertyField(refractionCullingMask, refractionReflectionCullingMaskLabel);
                    EditorGUILayout.Slider(refractionRenderTextureResizeFactor, 0f, 1f, refractionRenderTextureResizeFactorLabel);
                    EditorGUILayout.PropertyField(refractionRenderTextureFilterMode, refractionRenderTextureFilterModeLabel);
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawReflectionProperties(bool hasReflection)
        {
            reflectionPropertiesExpanded.target = EditorGUILayout.Foldout(reflectionPropertiesExpanded.target, reflectionPropertiesFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(reflectionPropertiesExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;

                    if (!hasReflection)
                        EditorGUILayout.HelpBox(reflectionMessage, MessageType.None, true);
                    EditorGUI.BeginDisabledGroup(!hasReflection);
                    EditorGUILayout.PropertyField(reflectionCullingMask, refractionReflectionCullingMaskLabel);
                    EditorGUILayout.Slider(reflectionRenderTextureResizeFactor, 0f, 1f, reflectionRenderTextureResizeFactorLabel);
                    EditorGUILayout.PropertyField(reflectionZOffset, reflectionZOffsetLabel);
                    EditorGUILayout.PropertyField(reflectionRenderTextureFilterMode, reflectionRenderTextureFilterModeLabel);
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawRenderingSettingsProperties(bool hasRefraction, bool hasReflection)
        {
            renderingSettingsExpanded.target = EditorGUILayout.Foldout(renderingSettingsExpanded.target, renderingSettingsFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(renderingSettingsExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;

                    EditorGUI.BeginDisabledGroup(!(hasReflection || hasRefraction));
                    EditorGUILayout.PropertyField(farClipPlane, farClipPlaneLabel);
                    EditorGUILayout.PropertyField(renderPixelLights, renderPixelLightsLabel);
                    EditorGUILayout.PropertyField(allowMSAA, allowMSAALabel);
                    EditorGUILayout.PropertyField(allowHDR, allowHDRLabel);
                    EditorGUI.EndDisabledGroup();
                    DrawSortingLayerField(sortingLayerID, sortingOrder);

                    EditorGUI.indentLevel--;
                }
            }
        }

        static void DrawSortingLayerField(SerializedProperty layerID, SerializedProperty orderInLayer)
        {
            MethodInfo methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new[] {
                typeof( GUIContent ),
                typeof( SerializedProperty ),
                typeof( GUIStyle ),
                typeof( GUIStyle )
            }, null);

            if (methodInfo != null)
            {
                object[] parameters = { sortingLayerLabel, layerID, EditorStyles.popup, EditorStyles.label };
                methodInfo.Invoke(null, parameters);
                EditorGUILayout.PropertyField(orderInLayer, orderInLayerLabel);
            }
        }

        private void DrawPrefabUtility(Game2DWater water2D, Material water2DMaterial)
        {
            prefabUtilityExpanded.target = EditorGUILayout.Foldout(prefabUtilityExpanded.target, prefabUtilityFoldoutLabel, true);
            using (var group = new EditorGUILayout.FadeGroupScope(prefabUtilityExpanded.faded))
            {
                if (group.visible)
                {
                    EditorGUI.indentLevel++;

                    GameObject water2DGameObject = water2D.gameObject;
                    Texture waterNoiseTexture = water2DMaterial != null && water2DMaterial.HasProperty(noiseTextureShaderPropertyName) ? water2DMaterial.GetTexture(noiseTextureShaderPropertyName) : null;

#if UNITY_2018_3_OR_NEWER
                    bool isPrefabInstance = PrefabUtility.GetPrefabInstanceStatus(water2DGameObject) == PrefabInstanceStatus.Connected;
#else
                    PrefabType prefabType = PrefabUtility.GetPrefabType(water2DGameObject);
                    bool isPrefabInstance = prefabType == PrefabType.PrefabInstance;
                    bool isPrefabInstanceDisconnected = prefabType == PrefabType.DisconnectedPrefabInstance;
#endif

                    bool materialAssetAlreadyExist = water2DMaterial != null && AssetDatabase.Contains(water2DMaterial);
                    bool textureAssetAlreadyExist = waterNoiseTexture != null && AssetDatabase.Contains(waterNoiseTexture);

                    EditorGUI.BeginDisabledGroup(true);
#if UNITY_2018_2_OR_NEWER
                    Object prefabObjct = isPrefabInstance ? PrefabUtility.GetCorrespondingObjectFromSource(water2DGameObject) : null;
#else
                    Object prefabObjct = isPrefabInstance ? PrefabUtility.GetPrefabParent(water2DGameObject) : null;
#endif
                    EditorGUILayout.ObjectField(prefabObjct, typeof(Object), false);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(prefabsPath, string.Format("Prefab Path: {0}", prefabsPath)), EditorStyles.textField);
                    if (GUILayout.Button(".", EditorStyles.miniButton, GUILayout.MaxWidth(14f)))
                    {
                        string newPrefabsPath = EditorUtility.OpenFolderPanel("Select prefabs path", "Assets", "");
                        if (!string.IsNullOrEmpty(newPrefabsPath))
                        {
                            newPrefabsPath = newPrefabsPath.Substring(Application.dataPath.Length);
                            prefabsPath = "Assets" + newPrefabsPath + "/";
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (!isPrefabInstance)
                    {
                        if (GUILayout.Button("Create Prefab"))
                        {
                            string fileName = GetValidAssetFileName(water2DGameObject.name, ".prefab", typeof(GameObject));

                            if (!textureAssetAlreadyExist && waterNoiseTexture != null)
                            {
                                string noiseTexturePath = prefabsPath + fileName + "_noiseTexture.asset";
                                AssetDatabase.CreateAsset(waterNoiseTexture, noiseTexturePath);
                            }

                            if (!materialAssetAlreadyExist && water2DMaterial != null)
                            {
                                string materialPath = prefabsPath + fileName + ".mat";
                                AssetDatabase.CreateAsset(water2DMaterial, materialPath);
                            }

                            string prefabPath = prefabsPath + fileName + ".prefab";
#if UNITY_2018_3_OR_NEWER
                            PrefabUtility.SaveAsPrefabAssetAndConnect(water2DGameObject, prefabPath, InteractionMode.AutomatedAction);
#else
                            PrefabUtility.CreatePrefab(prefabPath, water2DGameObject, ReplacePrefabOptions.ConnectToPrefab);
#endif
                        }
                    }
#if UNITY_2018_3_OR_NEWER
                    /*
                    As of Unity 2018.3, disconnecting (unlinking) and relinking a Prefab instance are no longer supported.
                    Alternatively, we can now unpack a Prefab instance if we want to entirely remove its link to its Prefab asset 
                    and thus be able to restructure the resulting plain GameObject as we please.
                    */
                    if (isPrefabInstance)
                    {
                        EditorGUI.indentLevel--;
                        EditorGUILayout.HelpBox(newPrefabWorkflowMessage, MessageType.Info);
                        EditorGUI.indentLevel++;
                    }
#else
                    if (isPrefabInstance)
                    {
                        if (GUILayout.Button("Unlink Prefab"))
                        {
#if UNITY_2018_2_OR_NEWER
                            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(water2DGameObject) as GameObject;
#else
                            GameObject prefab = PrefabUtility.GetPrefabParent(water2DGameObject) as GameObject;
#endif
                            PrefabUtility.DisconnectPrefabInstance(water2DGameObject);
                            Material prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
                            if (water2DMaterial != null && water2DMaterial == prefabMaterial)
                            {
                                bool usePrefabMaterial = EditorUtility.DisplayDialog("Use same prefab's material?",
                            "Do you still want to use the prefab's material?",
                            "Yes",
                            "No, create water's own material");

                                if (!usePrefabMaterial)
                                {
                                    Material duplicateMaterial = new Material(water2DMaterial);
                                    if (waterNoiseTexture != null)
                                    {
                                        Texture duplicateWaterNoiseTexture = Instantiate<Texture>(waterNoiseTexture);
                                        duplicateMaterial.SetTexture("_NoiseTexture", duplicateWaterNoiseTexture);
                                    }
                                    water2DGameObject.GetComponent<MeshRenderer>().sharedMaterial = duplicateMaterial;
                                }
                            }
                        }
                    }

                    if (isPrefabInstanceDisconnected)
                    {
                        if (GUILayout.Button("Relink Prefab"))
                        {
                            PrefabUtility.ReconnectToLastPrefab(water2DGameObject);
#if UNITY_2018_2_OR_NEWER
                            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(water2DGameObject) as GameObject;
#else
                            GameObject prefab = PrefabUtility.GetPrefabParent(water2DGameObject) as GameObject;
#endif
                            Material prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;

                            if (prefabMaterial != null && water2DMaterial != prefabMaterial)
                            {
                                bool usePrefabMaterial = EditorUtility.DisplayDialog("Use prefab's material?",
                                "Do you want to use the prefab's material?",
                                "Yes",
                                "No, continue to use the current water material");

                                if (usePrefabMaterial)
                                {
                                    water2DGameObject.GetComponent<MeshRenderer>().sharedMaterial = prefabMaterial;
                                }
                                else
                                {
                                    if (!materialAssetAlreadyExist)
                                    {
                                        string fileName = GetValidAssetFileName(water2DGameObject.name, ".mat", typeof(Material));

                                        if (!textureAssetAlreadyExist)
                                        {
                                            string noiseTexturePath = prefabsPath + fileName + "_noiseTexture.asset";
                                            AssetDatabase.CreateAsset(waterNoiseTexture, noiseTexturePath);
                                        }

                                        string materialPath = prefabsPath + fileName + ".mat";
                                        AssetDatabase.CreateAsset(water2DMaterial, materialPath);
                                    }
                                }
                            }
                        }
                    }
#endif
                    EditorGUI.indentLevel--;
                }
            }
        }

        private string GetValidAssetFileName(string assetName, string assetExtension, System.Type assetType)
        {
            string fileName = assetName;

            string path = prefabsPath + fileName + assetExtension;
            bool prefabWithSameNameExist = AssetDatabase.LoadAssetAtPath(path, assetType) != null;
            if (prefabWithSameNameExist)
            {
                int i = 1;
                while (prefabWithSameNameExist)
                {
                    fileName = assetName + " " + i;
                    path = prefabsPath + fileName + assetExtension;
                    prefabWithSameNameExist = AssetDatabase.LoadAssetAtPath(path, assetType) != null;
                    i++;
                }
            }

            return fileName;
        }

        #endregion

        #region SceneView

        private void OnSceneGUI()
        {
            Game2DWater water2D = target as Game2DWater;

            if (!isMultiEditing)
            {
                DrawWaterResizer(water2D);
                if (constantRipplesEditSourcesPositions)
                    DrawConstantRipplesSourcesPositions(water2D);
            }
            DrawWaterWireframe(water2D);
            DrawBuoyancyEffectorSurfaceLevelGuideline(water2D);

            if (GUI.changed)
                SceneView.RepaintAll();
        }

        private void DrawWaterWireframe(Game2DWater water2D)
        {
            List<Vector3> vertices = new List<Vector3>();
            water2D.GetComponent<MeshFilter>().sharedMesh.GetVertices(vertices);
            int start, end;
            if (water2D.UseCustomBoundaries)
            {
                start = 2;
                end = vertices.Count - 4;
            }
            else
            {
                start = 0;
                end = vertices.Count - 2;
            }
            Matrix4x4 localToWorldMatrix = water2D.transform.localToWorldMatrix;
            using (new Handles.DrawingScope(wireframeColor, localToWorldMatrix))
            {
                for (int i = start; i <= end; i += 2)
                {
                    Handles.DrawLine(vertices[i], vertices[i + 1]);
                }
            }
        }

        private void DrawBuoyancyEffectorSurfaceLevelGuideline(Game2DWater water2D)
        {
            Vector2 halfWaterSize = water2D.WaterSize / 2f;
            float y = halfWaterSize.y * (1f - 2f * water2D.BuoyancyEffectorSurfaceLevel);
            Vector3 lineStart = water2D.transform.TransformPoint(-halfWaterSize.x, y, 0f);
            Vector3 lineEnd = water2D.transform.TransformPoint(halfWaterSize.x, y, 0f);
            Handles.color = buoyancyEffectorSurfaceLevelGuidelineColor;
            Handles.DrawLine(lineStart, lineEnd);
            Handles.color = Color.white;
        }

        private void DrawWaterResizer(Game2DWater water2D)
        {
            Matrix4x4 localToWorldMatrix = water2D.transform.localToWorldMatrix;
            using (new Handles.DrawingScope(localToWorldMatrix))
            {
                Vector2 halfWaterSize = water2D.WaterSize / 2f;
                float handlesSize = HandleUtility.GetHandleSize(Vector3.zero) * 0.5f;
                Handles.CapFunction handlesCap = Handles.ArrowHandleCap;
                const float handlesSnap = 1f;

                EditorGUI.BeginChangeCheck();
                Vector3 upPos = Handles.Slider(new Vector3(0f, halfWaterSize.y, 0f), Vector3.up, handlesSize, handlesCap, handlesSnap);
                Vector3 downPos = Handles.Slider(new Vector3(0f, -halfWaterSize.y, 0f), Vector3.down, handlesSize, handlesCap, handlesSnap);
                Vector3 rightPos = Handles.Slider(new Vector3(halfWaterSize.x, 0f, 0f), Vector3.right, handlesSize, handlesCap, handlesSnap);
                Vector3 leftPos = Handles.Slider(new Vector3(-halfWaterSize.x, 0f, 0f), Vector3.left, handlesSize, handlesCap, handlesSnap);
                if (EditorGUI.EndChangeCheck())
                {
                    Vector2 newWaterSize;
                    newWaterSize.x = Mathf.Clamp(rightPos.x - leftPos.x, 0f, float.MaxValue);
                    newWaterSize.y = Mathf.Clamp(upPos.y - downPos.y, 0f, float.MaxValue);

                    if (newWaterSize.x > 0f && newWaterSize.y > 0f)
                    {
                        Undo.RecordObjects(new Object[] { water2D, water2D.transform }, "changing water size");

                        float deltaX = (rightPos.x + leftPos.x) / 2f;
                        if (water2D.UseCustomBoundaries)
                        {
                            float halfNewWaterSize = newWaterSize.x / 2f;
                            water2D.FirstCustomBoundary = Mathf.Clamp(water2D.FirstCustomBoundary - deltaX, -halfNewWaterSize, halfNewWaterSize);
                            water2D.SecondCustomBoundary = Mathf.Clamp(water2D.SecondCustomBoundary - deltaX, -halfNewWaterSize, halfNewWaterSize);
                        }

                        if (water2D.ActivateConstantRipples)
                        {
                            List<float> ripplesSourcesPositions = water2D.ConstantRipplesSourcePositions;
                            for (int i = 0, max = ripplesSourcesPositions.Count; i < max; i++)
                            {
                                ripplesSourcesPositions[i] = ripplesSourcesPositions[i] - deltaX;
                            }
                        }

                        water2D.WaterSize = newWaterSize;
                        water2D.transform.position = localToWorldMatrix.MultiplyPoint3x4(new Vector3((rightPos.x + leftPos.x) / 2f, (upPos.y + downPos.y) / 2f, 0f));
                        EditorUtility.SetDirty(water2D);
                    }
                }
            }
        }

        private void DrawConstantRipplesSourcesPositions(Game2DWater water2D)
        {
            List<float> ripplesSourcesPositions = water2D.ConstantRipplesSourcePositions;
            List<int> ripplesSourcesIndices = new List<int>(ripplesSourcesPositions.Count);
            List<Vector3> meshVerticesPositions = new List<Vector3>();
            water2D.GetComponent<MeshFilter>().sharedMesh.GetVertices(meshVerticesPositions);
            int surfaceVerticesCount = meshVerticesPositions.Count / 2;

            Vector2 halfWaterSize = water2D.WaterSize / 2f;

            float xStep, leftmostBoundary, rightmostBoundary;
            int indexOffset;
            int start, end;

            bool changeMade = false;
            bool addNewSource = false;
            int index = -1;

            if (water2D.UseCustomBoundaries)
            {
                float firstCustomBoundary = water2D.FirstCustomBoundary;
                float secondCustomBoundary = water2D.SecondCustomBoundary;
                if (firstCustomBoundary < secondCustomBoundary)
                {
                    leftmostBoundary = firstCustomBoundary;
                    rightmostBoundary = secondCustomBoundary;
                }
                else
                {
                    leftmostBoundary = secondCustomBoundary;
                    rightmostBoundary = firstCustomBoundary;
                }
                xStep = (rightmostBoundary - leftmostBoundary) / (surfaceVerticesCount - 3);
                indexOffset = 1;
                start = 2;
                end = meshVerticesPositions.Count - 4;
            }
            else
            {
                xStep = halfWaterSize.x * 2f / (surfaceVerticesCount - 1);
                leftmostBoundary = -halfWaterSize.x;
                rightmostBoundary = halfWaterSize.x;
                indexOffset = 0;
                start = 0;
                end = meshVerticesPositions.Count - 2;
            }

            Quaternion handlesRotation = Quaternion.identity;
            float handlesSize = HandleUtility.GetHandleSize(water2D.transform.position) * 0.05f;
            Handles.CapFunction handlesCap = Handles.DotHandleCap;
            Color handlesColor = Handles.color;

            using (new Handles.DrawingScope(water2D.transform.localToWorldMatrix))
            {
                for (int i = 0, maxi = ripplesSourcesPositions.Count; i < maxi; i++)
                {
                    float xPosition = ripplesSourcesPositions[i];
                    if (xPosition < leftmostBoundary || xPosition > rightmostBoundary)
                    {
                        Handles.color = constantRipplesSourcesColorRemove;
                        if (Handles.Button(new Vector3(xPosition, halfWaterSize.y), handlesRotation, handlesSize, handlesSize, handlesCap))
                        {
                            changeMade = true;
                            index = i;
                            addNewSource = false;
                        }
                        ripplesSourcesIndices.Add(-1);
                    }
                    else
                    {
                        int nearestIndex = Mathf.RoundToInt((xPosition - leftmostBoundary) / xStep) + indexOffset;
                        ripplesSourcesIndices.Add(nearestIndex * 2);
                    }
                }

                for (int i = start; i <= end; i += 2)
                {
                    Vector3 pos = meshVerticesPositions[i];

                    bool foundMatch = false;
                    int foundMatchIndex = -1;
                    for (int j = 0, maxj = ripplesSourcesIndices.Count; j < maxj; j++)
                    {
                        if (ripplesSourcesIndices[j] == i)
                        {
                            foundMatch = true;
                            foundMatchIndex = j;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        Handles.color = constantRipplesSourcesColorRemove;
                        if (Handles.Button(pos, handlesRotation, handlesSize, handlesSize, handlesCap))
                        {
                            changeMade = true;
                            index = foundMatchIndex;
                            addNewSource = false;
                        }
                    }
                    else
                    {
                        Handles.color = constantRipplesSourcesColorAdd;
                        if (Handles.Button(pos, handlesRotation, handlesSize, handlesSize, handlesCap))
                        {
                            changeMade = true;
                            index = i;
                            addNewSource = true;
                        }
                    }
                }
            }

            Handles.color = handlesColor;

            if (changeMade)
            {
                Undo.RecordObject(water2D, "editing water ripple source position");
                if (addNewSource)
                    ripplesSourcesPositions.Add(meshVerticesPositions[index].x);
                else
                    ripplesSourcesPositions.RemoveAt(index);
                EditorUtility.SetDirty(water2D);

                if (Application.isPlaying)
                    water2D.ConstantRipplesSourcePositions = ripplesSourcesPositions;
            }
        }

        #endregion

        #endregion
    }
}