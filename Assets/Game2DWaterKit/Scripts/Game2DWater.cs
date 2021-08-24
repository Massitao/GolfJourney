using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game2DWaterKit
{
    [
        RequireComponent(typeof(MeshRenderer)),
        RequireComponent(typeof(MeshFilter)),
        RequireComponent(typeof(BoxCollider2D)),
        RequireComponent(typeof(BuoyancyEffector2D))
    ]

    [ExecuteInEditMode]
    public class Game2DWater : MonoBehaviour
    {

        // MODIFIED
        Ball theBall;

        private void Start()
        {
          theBall = FindObjectOfType<Ball>();
        }
        

        public void PlayerIsInsideWater()
        {
            theBall.InsideWater();
        }
        public void PlayerIsOutsideWater()
        {
            theBall.OutsideWater();
        }

        // MODIFIED

        #region Custom Types
        private class ParticleEffectPool
        {
            private int instanceID;
            private ParticleSystem particleSystem;
            private UnityEvent stopAction;
            private bool expandIfNecessary;

            private Transform poolRoot;
            private List<ParticleSystem> pool;

            private int firstActiveParticleSystemIndex;
            private int nextParticleSystemToActivateIndex;
            private int activeParticleSystemsCount;

            public bool ExpandIfNecessary
            {
                get
                {
                    return expandIfNecessary;
                }
                set
                {
                    expandIfNecessary = value;
                }
            }

            public ParticleEffectPool(int instanceID, ParticleSystem particleSystem, int poolSize, UnityEvent stopAction, bool expandIfNecessary)
            {
                this.instanceID = instanceID;
                this.particleSystem = particleSystem;
                this.stopAction = stopAction;
                this.ExpandIfNecessary = expandIfNecessary;
                ReconstructPool(particleSystem, poolSize);
            }

            public void DestroyPool()
            {
                pool = null;
                if (poolRoot != null)
                {
                    Destroy(poolRoot.gameObject);
                    poolRoot = null;
                }
            }

            public void ReconstructPool(ParticleSystem particleSystem, int poolSize)
            {
                if (particleSystem == null || poolSize < 1)
                    return;

                if (pool == null)
                    pool = new List<ParticleSystem>(poolSize);

                if (particleSystem != this.particleSystem || poolSize < pool.Capacity)
                {
                    DestroyPool();
                    pool = new List<ParticleSystem>(poolSize);
                    this.particleSystem = particleSystem;
                    firstActiveParticleSystemIndex = 0;
                    nextParticleSystemToActivateIndex = 0;
                    activeParticleSystemsCount = 0;
                }

                if (poolSize > pool.Capacity)
                    pool.Capacity = poolSize;

                if (poolRoot == null)
                {
                    poolRoot = new GameObject(string.Format("Game2DWaterKit_ParticleEffectPool_For_{0}", instanceID)).transform;
                    poolRoot.hideFlags = HideFlags.HideInHierarchy;
                }

                GameObject particleSystemGameObject = particleSystem.gameObject;
                Vector3 position = Vector3.zero;
                Quaternion rotation = particleSystemGameObject.transform.rotation;

                for (int i = pool.Count; i < poolSize; i++)
                {
                    GameObject particleSystemInstanceGameObject = Instantiate(particleSystemGameObject, position, rotation, poolRoot);
                    particleSystemInstanceGameObject.SetActive(false);
                    pool.Add(particleSystemInstanceGameObject.GetComponent<ParticleSystem>());
                }
            }

            public void UpdatePool()
            {
                if (pool != null && activeParticleSystemsCount > 0)
                {
                    ParticleSystem firstActiveParticleSystem = pool[firstActiveParticleSystemIndex];
                    if (!firstActiveParticleSystem.IsAlive(true))
                    {
                        firstActiveParticleSystem.gameObject.SetActive(false);
                        if (stopAction != null)
                            stopAction.Invoke();

                        activeParticleSystemsCount--;
                        firstActiveParticleSystemIndex++;
                        if (firstActiveParticleSystemIndex == pool.Count)
                            firstActiveParticleSystemIndex = 0;
                    }
                }
            }

            public void PlayParticleEffect(Vector3 position)
            {
                if (pool == null)
                    return;

                if (activeParticleSystemsCount == pool.Count)
                {
                    if (expandIfNecessary)
                    {
                        nextParticleSystemToActivateIndex = pool.Count;
                        ReconstructPool(particleSystem, pool.Count * 2);
                    }
                    else
                        return;
                }

                ParticleSystem newlyActivatedParticleSystem = pool[nextParticleSystemToActivateIndex];
                newlyActivatedParticleSystem.transform.position = position;
                newlyActivatedParticleSystem.gameObject.SetActive(true);
                newlyActivatedParticleSystem.Play(true);

                activeParticleSystemsCount++;
                nextParticleSystemToActivateIndex++;
                if (nextParticleSystemToActivateIndex == pool.Count)
                    nextParticleSystemToActivateIndex = 0;
            }
        }

        private class SoundEffectPool
        {
            private int instanceID;
            private AudioClip audioClip;
            private bool expandIfNecessary;

            private Transform poolRoot;
            private List<AudioSource> pool;

            private int firstActiveAudioSourceIndex;
            private int nextAudioSourceToActivateIndex;
            private int activeAudioSourcesCount;

            public bool ExpandIfNecessary
            {
                get
                {
                    return expandIfNecessary;
                }
                set
                {
                    expandIfNecessary = value;
                }
            }

            public SoundEffectPool(int instanceID, AudioClip audioClip, int poolSize, bool expandIfNecessary)
            {
                this.instanceID = instanceID;
                this.audioClip = audioClip;
                this.expandIfNecessary = expandIfNecessary;
                ReconstructPool(audioClip, poolSize);
            }

            public void DestroyPool()
            {
                pool = null;
                if (poolRoot != null)
                {
                    Destroy(poolRoot.gameObject);
                    poolRoot = null;
                }
            }

            public void ReconstructPool(AudioClip audioClip, int poolSize)
            {
                if (audioClip == null || poolSize < 1)
                    return;

                if (pool == null)
                    pool = new List<AudioSource>(poolSize);

                if (audioClip != this.audioClip || poolSize < pool.Capacity)
                {
                    DestroyPool();
                    pool = new List<AudioSource>(poolSize);
                    this.audioClip = audioClip;
                    firstActiveAudioSourceIndex = 0;
                    nextAudioSourceToActivateIndex = 0;
                    activeAudioSourcesCount = 0;
                }

                if (poolSize > pool.Capacity)
                    pool.Capacity = poolSize;

                if (poolRoot == null)
                {
                    poolRoot = new GameObject(string.Format("Game2DWaterKit_SoundEffectPool_For_{0}", instanceID)).transform;
                    poolRoot.hideFlags = HideFlags.HideInHierarchy;
                }

                for (int i = pool.Count; i < poolSize; i++)
                {
                    GameObject audioSourceGameObject = new GameObject("Sound Effect");
                    audioSourceGameObject.transform.parent = poolRoot;
                    audioSourceGameObject.SetActive(false);
                    AudioSource attachedAudioSource = audioSourceGameObject.AddComponent<AudioSource>();
                    attachedAudioSource.clip = audioClip;
                    pool.Add(attachedAudioSource);
                }
            }

            public void UpdatePool()
            {
                if (pool != null && activeAudioSourcesCount > 0)
                {
                    AudioSource firstActiveAudioSource = pool[firstActiveAudioSourceIndex];
                    if (!firstActiveAudioSource.isPlaying)
                    {
                        firstActiveAudioSource.gameObject.SetActive(false);

                        activeAudioSourcesCount--;
                        firstActiveAudioSourceIndex++;
                        if (firstActiveAudioSourceIndex == pool.Count)
                            firstActiveAudioSourceIndex = 0;
                    }
                }
            }

            public void PlaySoundEffect(Vector3 position, float pitch, float volume)
            {
                if (pool == null)
                    return;

                if (activeAudioSourcesCount == pool.Count)
                {
                    if (ExpandIfNecessary)
                    {
                        nextAudioSourceToActivateIndex = pool.Count;
                        ReconstructPool(audioClip, pool.Count * 2);
                    }
                    else
                        return;
                }

                AudioSource newlyActivatedAudioSource = pool[nextAudioSourceToActivateIndex];
                newlyActivatedAudioSource.transform.position = position;
                newlyActivatedAudioSource.gameObject.SetActive(true);
                newlyActivatedAudioSource.volume = volume;
                newlyActivatedAudioSource.pitch = pitch;
                newlyActivatedAudioSource.Play();

                activeAudioSourcesCount++;
                nextAudioSourceToActivateIndex++;
                if (nextAudioSourceToActivateIndex == pool.Count)
                    nextAudioSourceToActivateIndex = 0;
            }
        }
        #endregion

        #region variables

        #region Water Variables
        //Mesh Properties
        [SerializeField] Vector2 waterSize = Vector2.one;
        [SerializeField] Vector2 lastWaterSize = Vector2.one; //stores the last frame water size, used when animating the water size
        [SerializeField] int subdivisionsCountPerUnit = 3;
        Mesh mesh;
        int surfaceVerticesCount;
        List<Vector3> vertices;
        List<Vector2> uvs;
        List<int> triangles;
        //Wave Properties
        [SerializeField] float damping = 0.05f;
        [SerializeField] float stiffness = 60f;
        [SerializeField] float stiffnessSquareRoot = Mathf.Sqrt(60f);
        [SerializeField] float spread = 60f;
        [SerializeField] bool useCustomBoundaries = false;
        [SerializeField] float firstCustomBoundary = 0.5f;
        [SerializeField] float secondCustomBoundary = -0.5f;
        [SerializeField] float lastFirstCustomBoundary = 0.5f; //stores the last frame first boundary, used when animating the water custom boundaries
        [SerializeField] float lastSecondCustomBoundary = -0.5f; //stores the last frame second boundary, used when animating the water custom boundaries
        float[] velocities;
        bool updateWaterSimulation;
        float waterPositionOfRest;
        //Water Events
        [SerializeField] UnityEvent onWaterEnter = new UnityEvent();
        [SerializeField] UnityEvent onWaterExit = new UnityEvent();
        //Misc Properties
        [SerializeField] float buoyancyEffectorSurfaceLevel = 0.02f;
        Material waterMaterial;
        MaterialPropertyBlock materialPropertyBlock;
        bool waterIsVisible = false;
        Matrix4x4 waterLocalToWorldMatrix;
        Matrix4x4 waterWorldToLocalMatrix;
        Vector3 waterPosition;
        #endregion

        #region On-Collision Ripples Variables
        [SerializeField] bool activateOnCollisionOnWaterEnterRipples = true;
        [SerializeField] bool activateOnCollisionOnWaterExitRipples = true;
        //Disturbance Properties
        [FormerlySerializedAs("minimumDisturbance"), SerializeField] float onCollisionRipplesMinimumDisturbance = 0.1f;
        [FormerlySerializedAs("maximumDisturbance"), SerializeField] float onCollisionRipplesMaximumDisturbance = 0.75f;
        [FormerlySerializedAs("velocityMultiplier"), SerializeField] float onCollisionRipplesVelocityMultiplier = 0.12f;
        //Collision Properties
        [FormerlySerializedAs("collisionMask"), SerializeField] LayerMask onCollisionRipplesCollisionMask = ~(1 << 4);
        [FormerlySerializedAs("collisionMinimumDepth"), SerializeField] float onCollisionRipplesCollisionMinimumDepth = -10f;
        [FormerlySerializedAs("collisionMaximumDepth"), SerializeField] float onCollisionRipplesCollisionMaximumDepth = 10f;
        [FormerlySerializedAs("collisionRaycastMaxDistance"), SerializeField] float onCollisionRipplesCollisionRaycastMaxDistance = 0.5f;
        Vector3 onCollisionRipplesCollisionRaycastDirection;
        //Particle Effect Properties (On Water Enter)
        [FormerlySerializedAs("activateOnCollisionSplashParticleEffect"), SerializeField] bool onCollisionRipplesActivateOnWaterEnterParticleEffect = false;
        [FormerlySerializedAs("onCollisionSplashParticleEffect"), SerializeField] ParticleSystem onCollisionRipplesOnWaterEnterParticleEffect = null;
        [FormerlySerializedAs("onCollisionSplashParticleEffectSpawnOffset"), SerializeField] Vector3 onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] UnityEvent onCollisionRipplesOnWaterEnterParticleEffectStopAction = new UnityEvent();
        ParticleEffectPool onCollisionRipplesOnWaterEnterParticleEffectPool;
        [FormerlySerializedAs("onCollisionSplashParticleEffectPoolSize"), SerializeField] int onCollisionRipplesOnWaterEnterParticleEffectPoolSize = 10;
        [SerializeField] bool onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary;
        [SerializeField] bool onCollisionRipplesReconstructOnWaterEnterParticleEffectPool;
        //Sound Effect Properties (On Water Enter)
        [SerializeField] bool onCollisionRipplesActivateOnWaterEnterSoundEffect = true;
        [FormerlySerializedAs("splashAudioClip"), SerializeField] AudioClip onCollisionRipplesOnWaterEnterAudioClip = null;
        [FormerlySerializedAs("useConstantAudioPitch"), SerializeField] bool onCollisionRipplesUseConstantOnWaterEnterAudioPitch = false;
        [FormerlySerializedAs("audioPitch"), SerializeField] float onCollisionRipplesOnWaterEnterAudioPitch = 1f;
        [FormerlySerializedAs("minimumAudioPitch"), SerializeField] float onCollisionRipplesOnWaterEnterMinimumAudioPitch = 0.75f;
        [FormerlySerializedAs("maximumAudioPitch"), SerializeField] float onCollisionRipplesOnWaterEnterMaximumAudioPitch = 1.25f;
        [SerializeField] float onCollisionRipplesOnWaterEnterAudioVolume = 1.0f;
        SoundEffectPool onCollisionRipplesOnWaterEnterSoundEffectPool;
        [SerializeField] int onCollisionRipplesOnWaterEnterSoundEffectPoolSize = 10;
        [SerializeField] bool onCollisionRipplesReconstructOnWaterEnterSoundEffectPool;
        [SerializeField] bool onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary;
        //Particle Effect Properties (On Water Exit)
        [SerializeField] bool onCollisionRipplesActivateOnWaterExitParticleEffect = false;
        [SerializeField] ParticleSystem onCollisionRipplesOnWaterExitParticleEffect = null;
        [SerializeField] Vector3 onCollisionRipplesOnWaterExitParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] UnityEvent onCollisionRipplesOnWaterExitParticleEffectStopAction = new UnityEvent();
        ParticleEffectPool onCollisionRipplesOnWaterExitParticleEffectPool;
        [SerializeField] int onCollisionRipplesOnWaterExitParticleEffectPoolSize = 10;
        [SerializeField] bool onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary;
        [SerializeField] bool onCollisionRipplesReconstructOnWaterExitParticleEffectPool;
        //Sound Effect Properties (On Water Exit)
        [SerializeField] bool onCollisionRipplesActivateOnWaterExitSoundEffect = false;
        [SerializeField] AudioClip onCollisionRipplesOnWaterExitAudioClip = null;
        [SerializeField] bool onCollisionRipplesUseConstantOnWaterExitAudioPitch = false;
        [SerializeField] float onCollisionRipplesOnWaterExitAudioPitch = 1f;
        [SerializeField] float onCollisionRipplesOnWaterExitMinimumAudioPitch = 0.75f;
        [SerializeField] float onCollisionRipplesOnWaterExitMaximumAudioPitch = 1.25f;
        [SerializeField] float onCollisionRipplesOnWaterExitAudioVolume = 1.0f;
        SoundEffectPool onCollisionRipplesOnWaterExitSoundEffectPool;
        [SerializeField] int onCollisionRipplesOnWaterExitSoundEffectPoolSize = 10;
        [SerializeField] bool onCollisionRipplesReconstructOnWaterExitSoundEffectPool;
        [SerializeField] bool onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary;
        #endregion

        #region Constant Ripples Variables
        [SerializeField] bool activateConstantRipples = false;
        [SerializeField] bool constantRipplesUpdateWhenOffscreen = false;
        //Disturbance Properties
        [SerializeField] bool constantRipplesRandomizeDisturbance = false;
        [SerializeField] bool constantRipplesSmoothDisturbance = false;
        [SerializeField] float constantRipplesSmoothFactor = 0.5f;
        [SerializeField] float constantRipplesDisturbance = 0.10f;
        [SerializeField] float constantRipplesMinimumDisturbance = 0.08f;
        [SerializeField] float constantRipplesMaximumDisturbance = 0.12f;
        //Interval Properties
        [SerializeField] bool constantRipplesRandomizeInterval = false;
        [SerializeField] float constantRipplesInterval = 1f;
        [SerializeField] float constantRipplesMinimumInterval = 0.75f;
        [SerializeField] float constantRipplesMaximumInterval = 1.25f;
        float constantRipplesDeltaTime;
        float constantRipplesCurrentInterval;
        //Ripple Source Positions Properties
        [SerializeField] bool constantRipplesRandomizeRipplesSourcesPositions = false;
        [SerializeField] int constantRipplesRandomizeRipplesSourcesCount = 3;
        [SerializeField] bool constantRipplesAllowDuplicateRipplesSourcesPositions = false;
        [SerializeField] List<float> constantRipplesSourcePositions = new List<float>();
        List<int> constantRipplesSourcesIndices;
        bool constantRipplesUpdateSourcesIndices = true;
        //Sound Effect Properties
        [SerializeField] bool constantRipplesActivateSoundEffect = false;
        [SerializeField] bool constantRipplesUseConstantAudioPitch = false;
        [SerializeField] AudioClip constantRipplesAudioClip = null;
        [SerializeField] float constantRipplesAudioPitch = 1f;
        [SerializeField] float constantRipplesMinimumAudioPitch = 0.75f;
        [SerializeField] float constantRipplesMaximumAudioPitch = 1.25f;
        SoundEffectPool constantRipplesSoundEffectPool;
        [SerializeField] int constantRipplesSoundEffectPoolSize = 10;
        [SerializeField] bool constantRipplesReconstructSoundEffectPool;
        [SerializeField] bool constantRipplesSoundEffectPoolExpandIfNecessary;
        [SerializeField] float constantRipplesAudioVolume = 1.0f;
        //Particle Effect Properties
        [FormerlySerializedAs("activateConstantSplashParticleEffect"), SerializeField] bool constantRipplesActivateParticleEffect = false;
        [FormerlySerializedAs("constantSplashParticleEffect"), SerializeField] ParticleSystem constantRipplesParticleEffect = null;
        [FormerlySerializedAs("constantSplashParticleEffectSpawnOffset"), SerializeField] Vector3 constantRipplesParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] UnityEvent constantRipplesParticleEffectStopAction = new UnityEvent();
        ParticleEffectPool constantRipplesParticleEffectPool;
        [FormerlySerializedAs("constantSplashParticleEffectPoolSize"), SerializeField] int constantRipplesParticleEffectPoolSize = 10;
        [SerializeField] bool constantRipplesParticleEffectPoolExpandIfNecessary;
        [SerializeField] bool constantRipplesReconstructParticleEffectPool;
        #endregion

        #region Script-Generated Ripples Variables
        //Disturbance Properties
        [SerializeField] float scriptGeneratedRipplesMinimumDisturbance = 0.1f;
        [SerializeField] float scriptGeneratedRipplesMaximumDisturbance = 0.75f;
        //Sound Effect Properties
        [SerializeField] bool scriptGeneratedRipplesActivateSoundEffect = false;
        [SerializeField] AudioClip scriptGeneratedRipplesAudioClip = null;
        [SerializeField] bool scriptGeneratedRipplesUseConstantAudioPitch = false;
        [SerializeField] float scriptGeneratedRipplesAudioPitch = 1f;
        [SerializeField] float scriptGeneratedRipplesMinimumAudioPitch = 0.75f;
        [SerializeField] float scriptGeneratedRipplesMaximumAudioPitch = 1.25f;
        [SerializeField] float scriptGeneratedRipplesAudioVolume = 1.0f;
        SoundEffectPool scriptGeneratedRipplesSoundEffectPool;
        [SerializeField] int scriptGeneratedRipplesSoundEffectPoolSize = 10;
        [SerializeField] bool scriptGeneratedRipplesReconstructSoundEffectPool;
        [SerializeField] bool scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary;
        //Particle Effect Properties
        [SerializeField] bool scriptGeneratedRipplesActivateParticleEffect = false;
        [SerializeField] ParticleSystem scriptGeneratedRipplesParticleEffect = null;
        [SerializeField] Vector3 scriptGeneratedRipplesParticleEffectSpawnOffset = Vector3.zero;
        [SerializeField] UnityEvent scriptGeneratedRipplesParticleEffectStopAction = new UnityEvent();
        ParticleEffectPool scriptGeneratedRipplesParticleEffectPool;
        [SerializeField] int scriptGeneratedRipplesParticleEffectPoolSize = 10;
        [SerializeField] bool scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary;
        [SerializeField] bool scriptGeneratedRipplesReconstructParticleEffectPool;
        #endregion

        #region Refraction & Reflection Rendering Variables
        //Refraction Properties
        [SerializeField] float refractionRenderTextureResizeFactor = 1f;
        [SerializeField] LayerMask refractionCullingMask = ~(1 << 4);
        RenderTexture refractionRenderTexture;
        [SerializeField] FilterMode refractionRenderTextureFilterMode = FilterMode.Bilinear;
        bool renderRefraction;
        Vector3 waterCameraPositionForRefractionRendering = Vector3.zero;
        Quaternion waterCameraRotationForRefractionRendering = Quaternion.identity;
#if UNITY_2017_1_OR_NEWER
        static int refractionTextureID = Shader.PropertyToID("_RefractionTexture");
#else
        int refractionTextureID;
#endif
        //Reflection Properties
        [SerializeField] float reflectionRenderTextureResizeFactor = 1f;
        [SerializeField] LayerMask reflectionCullingMask = ~(1 << 4);
        [SerializeField] float reflectionZOffset = 0f;
        RenderTexture reflectionRenderTexture;
        [SerializeField] FilterMode reflectionRenderTextureFilterMode = FilterMode.Bilinear;
        bool renderReflection;
        Vector3 waterCameraPositionForReflectionRendering = Vector3.zero;
        Quaternion waterCameraRotationForReflectionRendering = Quaternion.identity;
#if UNITY_2017_1_OR_NEWER
        static int reflectionTextureID = Shader.PropertyToID("_ReflectionTexture");
        static int waterReflectionLowerLimitID = Shader.PropertyToID("_ReflectionLowerLimit");
#else
        int reflectionTextureID;
        int waterReflectionLowerLimitID;
#endif
        //Shared Properties
        [SerializeField] int sortingLayerID = 0;
        [SerializeField] int sortingOrder = 0;
        [SerializeField] float farClipPlane = 100f;
        [SerializeField] bool renderPixelLights = true;
        [SerializeField] bool allowMSAA = false;
        [SerializeField] bool allowHDR = false;
        Camera waterCamera;
        bool updateWaterCameraRenderingSettings = true;
        Quaternion defaultWaterCameraRotation = Quaternion.identity;
        Vector2 lastWaterMeshBoundsScreenSpaceMin = Vector2.zero;
        Vector2 lastWaterMeshBoundsScreenSpaceMax = Vector2.zero;
        Vector2[] currentRenderingCameraWorldSpaceBounds = new Vector2[4];
        List<Vector2> waterWorldspaceBounds = new List<Vector2>(4);
        List<Vector2>[] clippingInputOutput = new List<Vector2>[] { new List<Vector2>(), new List<Vector2>() };
#if UNITY_2017_1_OR_NEWER
        static int waterMatrixID = Shader.PropertyToID("_WaterMVP");
#else
        int waterMatrixID;
#endif
        #endregion

        #region Attached Components Variables
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        BoxCollider2D boxCollider;
        BuoyancyEffector2D buoyancyEffector;
        EdgeCollider2D edgeCollider;
        Vector2[] edgeColliderPoints = new Vector2[4];
        #endregion

        #endregion

        #region Properties

        #region Water Properties
        //MeshProperties

        /// <summary>
        /// Sets the water size. X represents the width and Y represents the height.
        /// </summary>
        public Vector2 WaterSize
        {
            get
            {
                return waterSize;
            }
            set
            {
                Vector2 newWaterSize;
                newWaterSize.x = Mathf.Clamp(value.x, 0f, float.MaxValue);
                newWaterSize.y = Mathf.Clamp(value.y, 0f, float.MaxValue);
                if (waterSize == newWaterSize)
                    return;
                waterSize = newWaterSize;
                RecomputeMesh();
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        /// <summary>
        /// Sets the number of water’s surface vertices within one unit.
        /// </summary>
        public int SubdivisionsCountPerUnit
        {
            get
            {
                return subdivisionsCountPerUnit;
            }
            set
            {
                int newSubdivisionsCountPerUnit = Mathf.Clamp(value, 0, int.MaxValue);
                if (subdivisionsCountPerUnit == newSubdivisionsCountPerUnit)
                    return;
                subdivisionsCountPerUnit = newSubdivisionsCountPerUnit;
                RecomputeMesh();
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        //Wave Properties

        /// <summary>
        /// Controls how fast the waves decay. A low value will make waves oscillate for a long time, while a high value will make waves oscillate for a short time.
        /// </summary>
        public float Damping
        {
            get
            {
                return damping;
            }
            set
            {
                damping = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Controls how fast the waves spread.
        /// </summary>
        public float Spread
        {
            get
            {
                return spread;
            }
            set
            {
                spread = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// Controls the frequency of wave vibration. A low value will make waves oscillate slowly, while a high value will make waves oscillate quickly.
        /// </summary>
        public float Stiffness
        {
            get
            {
                return stiffness;
            }
            set
            {
                stiffness = Mathf.Clamp(value, 0f, float.MaxValue);
                stiffnessSquareRoot = Mathf.Sqrt(stiffness);
            }
        }

        /// <summary>
        /// Enables/Disables using custom wave boundaries. When waves reach a boundary, they bounce back.
        /// </summary>
        public bool UseCustomBoundaries
        {
            get
            {
                return useCustomBoundaries;
            }
            set
            {
                if (useCustomBoundaries == value)
                    return;
                useCustomBoundaries = value;
                RecomputeMesh();
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        /// <summary>
        /// The location of the first boundary.
        /// </summary>
        public float FirstCustomBoundary
        {
            get
            {
                return firstCustomBoundary;
            }
            set
            {
                float halfWidth = waterSize.x / 2f;
                float newFirstCustomBoundary = Mathf.Clamp(value, -halfWidth, halfWidth);
                if (Mathf.Approximately(firstCustomBoundary, newFirstCustomBoundary))
                    return;
                firstCustomBoundary = newFirstCustomBoundary;
                if (useCustomBoundaries)
                    RecomputeMesh();
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        /// <summary>
        /// The location of the second boundary.
        /// </summary>
        public float SecondCustomBoundary
        {
            get
            {
                return secondCustomBoundary;
            }
            set
            {
                float halfWidth = waterSize.x / 2f;
                float newSecondCustomBoundary = Mathf.Clamp(value, -halfWidth, halfWidth);
                if (Mathf.Approximately(secondCustomBoundary, newSecondCustomBoundary))
                    return;
                secondCustomBoundary = newSecondCustomBoundary;
                if (useCustomBoundaries)
                    RecomputeMesh();
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        //Misc Properties

        /// <summary>
        /// Sets the surface location of the buoyancy fluid. When a GameObject is above this line, no buoyancy forces are applied. When a GameObject is intersecting or completely below this line, buoyancy forces are applied.
        /// </summary>
        public float BuoyancyEffectorSurfaceLevel
        {
            get
            {
                return buoyancyEffectorSurfaceLevel;
            }
            set
            {
                float newBuoyancyEffectorSurfaceLevel = Mathf.Clamp01(value);
                if (Mathf.Approximately(buoyancyEffectorSurfaceLevel, newBuoyancyEffectorSurfaceLevel))
                    return;
                buoyancyEffectorSurfaceLevel = newBuoyancyEffectorSurfaceLevel;
                if (buoyancyEffector)
                    buoyancyEffector.surfaceLevel = waterSize.y * (0.5f - buoyancyEffectorSurfaceLevel);
            }
        }

        //Water Events

        /// <summary>
        /// UnityEvent that is triggered when a GameObject enters the water.
        /// </summary>
        public UnityEvent OnWaterEnter
        {
            get
            {
                return onWaterEnter;
            }
            set
            {
                onWaterEnter = value;
            }
        }

        /// <summary>
        /// UnityEvent that is triggered when a GameObject exits the water.
        /// </summary>
        public UnityEvent OnWaterExit
        {
            get
            {
                return onWaterExit;
            }
            set
            {
                onWaterExit = value;
            }
        }

        #endregion

        #region On-Collision Ripples Properties

        /// <summary>
        /// Activates/Deactivates generating ripples when a GameObject falls into water.
        /// </summary>
        public bool ActivateOnCollisionOnWaterEnterRipples
        {
            get
            {
                return activateOnCollisionOnWaterEnterRipples;
            }
            set
            {
                activateOnCollisionOnWaterEnterRipples = true;
            }
        }

        /// <summary>
        /// Activates/Deactivates generating ripples when a GameObject leaves the water.
        /// </summary>
        public bool ActivateOnCollisionOnWaterExitRipples
        {
            get
            {
                return activateOnCollisionOnWaterExitRipples;
            }
            set
            {
                activateOnCollisionOnWaterExitRipples = true;
            }
        }

        //Disturbance Properties

        /// <summary>
        /// The minimum displacement of water’s surface when an GameObject falls into water.
        /// </summary>
        public float OnCollisionRipplesMinimumDisturbance
        {
            get
            {
                return onCollisionRipplesMinimumDisturbance;
            }
            set
            {
                onCollisionRipplesMinimumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesMinimumDisturbance instead")]
        public float MinimumDisturbance
        {
            get
            {
                return OnCollisionRipplesMinimumDisturbance;
            }
            set
            {
                OnCollisionRipplesMinimumDisturbance = value;
            }
        }

        /// <summary>
        /// The maximum displacement of water’s surface when an GameObject falls into water.
        /// </summary>
        public float OnCollisionRipplesMaximumDisturbance
        {
            get
            {
                return onCollisionRipplesMaximumDisturbance;
            }
            set
            {
                onCollisionRipplesMaximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesMaximumDisturbance instead")]
        public float MaximumDisturbance
        {
            get
            {
                return OnCollisionRipplesMaximumDisturbance;
            }
            set
            {
                OnCollisionRipplesMaximumDisturbance = value;
            }
        }

        /// <summary>
        /// When a rigidbody falls into water, the amount of water’s surface displacement is determined by multiplying the rigidbody velocity by this factor.
        /// </summary>
        public float OnCollisionRipplesVelocityMultiplier
        {
            get
            {
                return onCollisionRipplesVelocityMultiplier;
            }
            set
            {
                onCollisionRipplesVelocityMultiplier = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesVelocityMultiplier instead")]
        public float VelocityMultiplier
        {
            get
            {
                return OnCollisionRipplesVelocityMultiplier;
            }
            set
            {
                OnCollisionRipplesVelocityMultiplier = value;
            }
        }

        //Collision Properties

        /// <summary>
        /// Only GameObjects on these layers will disturb the water’s surface (produce ripples) when they fall into water.
        /// </summary>
        public LayerMask OnCollisionRipplesCollisionMask
        {
            get
            {
                return onCollisionRipplesCollisionMask;
            }
            set
            {
                onCollisionRipplesCollisionMask = value & ~(1 << 4);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesCollisionMask instead")]
        public LayerMask CollisionMask
        {
            get
            {
                return OnCollisionRipplesCollisionMask;
            }
            set
            {
                OnCollisionRipplesCollisionMask = value;
            }
        }

        /// <summary>
        /// The maximum distance from the water's surface over which to check for collisions (Default: 0.5)
        /// </summary>
        public float OnCollisionRipplesCollisionRaycastMaxDistance
        {
            get
            {
                return onCollisionRipplesCollisionRaycastMaxDistance;
            }
            set
            {
                onCollisionRipplesCollisionRaycastMaxDistance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesCollisionRaycastMaxDistance instead")]
        public float CollisionRaycastMaxDistance
        {
            get
            {
                return OnCollisionRipplesCollisionRaycastMaxDistance;
            }
            set
            {
                OnCollisionRipplesCollisionRaycastMaxDistance = value;
            }
        }

        /// <summary>
        /// Only GameObjects with Z coordinate (depth) greater than or equal to this value will disturb the water’s surface when they fall into water.
        /// </summary>
        public float OnCollisionRipplesCollisionMinimumDepth
        {
            get
            {
                return onCollisionRipplesCollisionMinimumDepth;
            }
            set
            {
                onCollisionRipplesCollisionMinimumDepth = value;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesCollisionMinimumDepth instead")]
        public float MinimumCollisionDepth
        {
            get
            {
                return OnCollisionRipplesCollisionMinimumDepth;
            }
            set
            {
                OnCollisionRipplesCollisionMinimumDepth = value;
            }
        }

        /// <summary>
        /// Only GameObjects with Z coordinate (depth) less than or equal to this value will disturb the water’s surface when they fall into water.
        /// </summary>
        public float OnCollisionRipplesCollisionMaximumDepth
        {
            get
            {
                return onCollisionRipplesCollisionMaximumDepth;
            }
            set
            {
                onCollisionRipplesCollisionMaximumDepth = value;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesCollisionMaximumDepth instead")]
        public float MaximumCollisionDepth
        {
            get
            {
                return OnCollisionRipplesCollisionMaximumDepth;
            }
            set
            {
                OnCollisionRipplesCollisionMaximumDepth = value;
            }
        }

        //Sound Effect Properties (On Water Enter)

        /// <summary>
        /// Activates/Deactivates playing the sound effect when a GameObject falls into water.
        /// </summary>
        public bool OnCollisionRipplesActivateOnWaterEnterSoundEffect
        {
            get
            {
                return onCollisionRipplesActivateOnWaterEnterSoundEffect;
            }
            set
            {
                if (onCollisionRipplesActivateOnWaterEnterSoundEffect == value)
                    return;

                onCollisionRipplesActivateOnWaterEnterSoundEffect = value;
                onCollisionRipplesReconstructOnWaterEnterSoundEffectPool = true;
            }
        }

        /// <summary>
        /// The AudioClip asset to play when a GameObject falls into water.
        /// </summary>
        public AudioClip OnCollisionRipplesOnWaterEnterAudioClip
        {
            get
            {
                return onCollisionRipplesOnWaterEnterAudioClip;
            }
            set
            {
                if (onCollisionRipplesOnWaterEnterAudioClip == value)
                    return;

                onCollisionRipplesOnWaterEnterAudioClip = value;
                onCollisionRipplesReconstructOnWaterEnterSoundEffectPool = true;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterAudioClip instead")]
        public AudioClip SplashAudioClip
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterAudioClip;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterAudioClip = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s minimum playback speed.
        /// </summary>
        public float OnCollisionRipplesOnWaterEnterMinimumAudioPitch
        {
            get
            {
                return onCollisionRipplesOnWaterEnterMinimumAudioPitch;
            }
            set
            {
                onCollisionRipplesOnWaterEnterMinimumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterMinimumAudioPitch instead")]
        public float MinimumAudioPitch
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterMinimumAudioPitch;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterMinimumAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s maximum playback speed.
        /// </summary>
        public float OnCollisionRipplesOnWaterEnterMaximumAudioPitch
        {
            get
            {
                return onCollisionRipplesOnWaterEnterMaximumAudioPitch;
            }
            set
            {
                onCollisionRipplesOnWaterEnterMaximumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterMaximumAudioPitch instead")]
        public float MaximumAudioPitch
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterMaximumAudioPitch;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterMaximumAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s volume (Range: 0..1).
        /// </summary>
        public float OnCollisionRipplesOnWaterEnterAudioVolume
        {
            get
            {
                return onCollisionRipplesOnWaterEnterAudioVolume;
            }
            set
            {
                onCollisionRipplesOnWaterEnterAudioVolume = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Apply constant audio clip playback speed.
        /// </summary>
        public bool OnCollisionRipplesUseConstantOnWaterEnterAudioPitch
        {
            get
            {
                return onCollisionRipplesUseConstantOnWaterEnterAudioPitch;
            }
            set
            {
                onCollisionRipplesUseConstantOnWaterEnterAudioPitch = value;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesUseConstantOnWaterEnterAudioPitch instead")]
        public bool UseConstantAudioPitch
        {
            get
            {
                return OnCollisionRipplesUseConstantOnWaterEnterAudioPitch;
            }
            set
            {
                OnCollisionRipplesUseConstantOnWaterEnterAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s playback speed.
        /// </summary>
        public float OnCollisionRipplesOnWaterEnterAudioPitch
        {
            get
            {
                return onCollisionRipplesOnWaterEnterAudioPitch;
            }
            set
            {
                onCollisionRipplesOnWaterEnterAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterAudioPitch instead")]
        public float AudioPitch
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterAudioPitch;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the number of audio sources objects that will be created and pooled when the game starts.
        /// </summary>
        public int OnCollisionRipplesOnWaterEnterSoundEffectPoolSize
        {
            get
            {
                return onCollisionRipplesOnWaterEnterSoundEffectPoolSize;
            }
            set
            {
                if (onCollisionRipplesOnWaterEnterSoundEffectPoolSize == value)
                    return;

                onCollisionRipplesOnWaterEnterSoundEffectPoolSize = value;
                onCollisionRipplesReconstructOnWaterEnterSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool OnCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary
        {
            get
            {
                return onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary;
            }
            set
            {
                onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary = value;
                if (onCollisionRipplesOnWaterEnterSoundEffectPool != null)
                    onCollisionRipplesOnWaterEnterSoundEffectPool.ExpandIfNecessary = value;
            }
        }

        //Particle Effect Proeprties (On Water Enter)

        /// <summary>
        /// Activates/Deactivates playing the particle effect when a GameObject falls into water.
        /// </summary>
        public bool OnCollisionRipplesActivateOnWaterEnterParticleEffect
        {
            get
            {
                return onCollisionRipplesActivateOnWaterEnterParticleEffect;
            }
            set
            {
                if (onCollisionRipplesActivateOnWaterEnterParticleEffect == value)
                    return;

                onCollisionRipplesActivateOnWaterEnterParticleEffect = value;
                onCollisionRipplesReconstructOnWaterEnterParticleEffectPool = true;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesActivateOnWaterEnterParticleEffect instead")]
        public bool ActivateOnCollisionSplashParticleEffect
        {
            get
            {
                return OnCollisionRipplesActivateOnWaterEnterParticleEffect;
            }
            set
            {
                OnCollisionRipplesActivateOnWaterEnterParticleEffect = value;
            }
        }

        /// <summary>
        /// Sets the particle system to play when a GameObject falls into water.
        /// </summary>
        public ParticleSystem OnCollisionRipplesOnWaterEnterParticleEffect
        {
            get
            {
                return onCollisionRipplesOnWaterEnterParticleEffect;
            }
            set
            {
                if (onCollisionRipplesOnWaterEnterParticleEffect == value)
                    return;

                onCollisionRipplesOnWaterEnterParticleEffect = value;
                onCollisionRipplesReconstructOnWaterEnterParticleEffectPool = true;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterParticleEffect instead")]
        public ParticleSystem OnCollisionSplashParticleEffect
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterParticleEffect;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterParticleEffect = value;
            }
        }

        /// <summary>
        /// Sets the number of particle effect objects that will be created and pooled when the game starts
        /// </summary>
        public int OnCollisionRipplesOnWaterEnterParticleEffectPoolSize
        {
            get
            {
                return onCollisionRipplesOnWaterEnterParticleEffectPoolSize;
            }
            set
            {
                int newValue = Mathf.Clamp(value, 0, int.MaxValue);
                if (onCollisionRipplesOnWaterEnterParticleEffectPoolSize == newValue)
                    return;

                onCollisionRipplesOnWaterEnterParticleEffectPoolSize = newValue;
                onCollisionRipplesReconstructOnWaterEnterParticleEffectPool = true;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterParticleEffectPoolSize instead")]
        public int OnCollisionSplashParticleEffectPoolSize
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterParticleEffectPoolSize;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterParticleEffectPoolSize = value;
            }
        }

        /// <summary>
        /// Shift the particle effect spawn position.
        /// </summary>
        public Vector3 OnCollisionRipplesOnWaterEnterParticleEffectSpawnOffset
        {
            get
            {
                return onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset;
            }
            set
            {
                onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = value;
            }
        }

        [System.Obsolete("Use OnCollisionRipplesOnWaterEnterParticleEffectSpawnOffset instead")]
        public Vector3 OnCollisionSplashParticleEffectSpawnOffset
        {
            get
            {
                return OnCollisionRipplesOnWaterEnterParticleEffectSpawnOffset;
            }
            set
            {
                OnCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = value;
            }
        }

        /// <summary>
        /// UnityEvent that is triggered when the particle effect stops playing.
        /// </summary>
        public UnityEvent OnCollisionRipplesOnWaterEnterParticleEffectStopAction
        {
            get
            {
                return onCollisionRipplesOnWaterEnterParticleEffectStopAction;
            }
            set
            {
                onCollisionRipplesOnWaterEnterParticleEffectStopAction = value;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool OnCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary
        {
            get
            {
                return onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary;
            }
            set
            {
                onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary = value;
                if (onCollisionRipplesOnWaterEnterParticleEffectPool != null)
                    onCollisionRipplesOnWaterEnterParticleEffectPool.ExpandIfNecessary = value;
            }
        }

        //Sound Effect Properties (On Water Exit)

        /// <summary>
        /// Activates/Deactivates playing the sound effect when a GameObject leaves the water.
        /// </summary>
        public bool OnCollisionRipplesActivateOnWaterExitSoundEffect
        {
            get
            {
                return onCollisionRipplesActivateOnWaterExitSoundEffect;
            }
            set
            {
                if (onCollisionRipplesActivateOnWaterExitSoundEffect == value)
                    return;

                onCollisionRipplesActivateOnWaterExitSoundEffect = value;
                onCollisionRipplesReconstructOnWaterExitSoundEffectPool = true;
            }
        }

        /// <summary>
        /// The AudioClip asset to play when a GameObject leaves the water.
        /// </summary>
        public AudioClip OnCollisionRipplesOnWaterExitAudioClip
        {
            get
            {
                return onCollisionRipplesOnWaterExitAudioClip;
            }
            set
            {
                if (onCollisionRipplesOnWaterExitAudioClip == value)
                    return;

                onCollisionRipplesOnWaterExitAudioClip = value;
                onCollisionRipplesReconstructOnWaterExitSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the audio clip’s minimum playback speed.
        /// </summary>
        public float OnCollisionRipplesOnWaterExitMinimumAudioPitch
        {
            get
            {
                return onCollisionRipplesOnWaterExitMinimumAudioPitch;
            }
            set
            {
                onCollisionRipplesOnWaterExitMinimumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the audio clip’s maximum playback speed.
        /// </summary>
        public float OnCollisionRipplesOnWaterExitMaximumAudioPitch
        {
            get
            {
                return onCollisionRipplesOnWaterExitMaximumAudioPitch;
            }
            set
            {
                onCollisionRipplesOnWaterExitMaximumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the audio clip’s volume (Range: 0..1).
        /// </summary>
        public float OnCollisionRipplesOnWaterExitAudioVolume
        {
            get
            {
                return onCollisionRipplesOnWaterExitAudioVolume;
            }
            set
            {
                onCollisionRipplesOnWaterExitAudioVolume = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Apply constant audio clip playback speed.
        /// </summary>
        public bool OnCollisionRipplesUseConstantOnWaterExitAudioPitch
        {
            get
            {
                return onCollisionRipplesUseConstantOnWaterExitAudioPitch;
            }
            set
            {
                onCollisionRipplesUseConstantOnWaterExitAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s playback speed.
        /// </summary>
        public float OnCollisionRipplesOnWaterExitAudioPitch
        {
            get
            {
                return onCollisionRipplesOnWaterExitAudioPitch;
            }
            set
            {
                onCollisionRipplesOnWaterExitAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the number of audio sources objects that will be created and pooled when the game starts.
        /// </summary>
        public int OnCollisionRipplesOnWaterExitSoundEffectPoolSize
        {
            get
            {
                return onCollisionRipplesOnWaterExitSoundEffectPoolSize;
            }
            set
            {
                if (onCollisionRipplesOnWaterExitSoundEffectPoolSize == value)
                    return;

                onCollisionRipplesOnWaterExitSoundEffectPoolSize = value;
                onCollisionRipplesReconstructOnWaterExitSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool OnCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary
        {
            get
            {
                return onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary;
            }
            set
            {
                onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary = value;
                if (onCollisionRipplesOnWaterExitSoundEffectPool != null)
                    onCollisionRipplesOnWaterExitSoundEffectPool.ExpandIfNecessary = value;
            }
        }

        //Particle Effect Proeprties (On Water Exit)

        /// <summary>
        /// Activates/Deactivates playing the particle effect when a GameObject leaves the water.
        /// </summary>
        public bool OnCollisionRipplesActivateOnWaterExitParticleEffect
        {
            get
            {
                return onCollisionRipplesActivateOnWaterExitParticleEffect;
            }
            set
            {
                if (onCollisionRipplesActivateOnWaterExitParticleEffect == value)
                    return;

                onCollisionRipplesActivateOnWaterExitParticleEffect = value;
                onCollisionRipplesReconstructOnWaterExitParticleEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the particle system to play when a GameObject leaves the water.
        /// </summary>
        public ParticleSystem OnCollisionRipplesOnWaterExitParticleEffect
        {
            get
            {
                return onCollisionRipplesOnWaterExitParticleEffect;
            }
            set
            {
                if (onCollisionRipplesOnWaterExitParticleEffect == value)
                    return;

                onCollisionRipplesOnWaterExitParticleEffect = value;
                onCollisionRipplesReconstructOnWaterExitParticleEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the number of particle effect objects that will be created and pooled when the game starts
        /// </summary>
        public int OnCollisionRipplesOnWaterExitParticleEffectPoolSize
        {
            get
            {
                return onCollisionRipplesOnWaterExitParticleEffectPoolSize;
            }
            set
            {
                int newValue = Mathf.Clamp(value, 0, int.MaxValue);
                if (onCollisionRipplesOnWaterExitParticleEffectPoolSize == newValue)
                    return;

                onCollisionRipplesOnWaterExitParticleEffectPoolSize = newValue;
                onCollisionRipplesReconstructOnWaterExitParticleEffectPool = true;
            }
        }

        /// <summary>
        /// Shift the particle effect spawn position.
        /// </summary>
        public Vector3 OnCollisionRipplesOnWaterExitParticleEffectSpawnOffset
        {
            get
            {
                return onCollisionRipplesOnWaterExitParticleEffectSpawnOffset;
            }
            set
            {
                onCollisionRipplesOnWaterExitParticleEffectSpawnOffset = value;
            }
        }

        /// <summary>
        /// UnityEvent that is triggered when the particle effect stops playing.
        /// </summary>
        public UnityEvent OnCollisionRipplesOnWaterExitParticleEffectStopAction
        {
            get
            {
                return onCollisionRipplesOnWaterExitParticleEffectStopAction;
            }
            set
            {
                onCollisionRipplesOnWaterExitParticleEffectStopAction = value;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool OnCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary
        {
            get
            {
                return onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary;
            }
            set
            {
                onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary = value;
                if (onCollisionRipplesOnWaterExitParticleEffectPool != null)
                    onCollisionRipplesOnWaterExitParticleEffectPool.ExpandIfNecessary = value;
            }
        }

        #endregion

        #region Constant Ripples Properties

        /// <summary>
        /// Activate/Deactivate generating constant ripples.
        /// </summary>
        public bool ActivateConstantRipples
        {
            get
            {
                return activateConstantRipples;
            }
            set
            {
                activateConstantRipples = value;
                if (value)
                    constantRipplesUpdateSourcesIndices = true;
            }
        }

        /// <summary>
        /// Generate constant ripples even when the water is invisible to the camera.
        /// </summary>
        public bool ConstantRipplesUpdateWhenOffscreen
        {
            get
            {
                return constantRipplesUpdateWhenOffscreen;
            }
            set
            {
                constantRipplesUpdateWhenOffscreen = value;
            }
        }

        //Disturbance Properties

        /// <summary>
        /// Randomize the disturbance (displacement) of the water's surface.
        /// </summary>
        public bool ConstantRipplesRandomizeDisturbance
        {
            get
            {
                return constantRipplesRandomizeDisturbance;
            }
            set
            {
                constantRipplesRandomizeDisturbance = value;
            }
        }

        /// <summary>
        /// Sets the displacement of water’s surface.
        /// </summary>
        public float ConstantRipplesDisturbance
        {
            get
            {
                return constantRipplesDisturbance;
            }
            set
            {
                constantRipplesDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// Sets the minimum displacement of water’s surface.
        /// </summary>
        public float ConstantRipplesMinimumDisturbance
        {
            get
            {
                return constantRipplesMinimumDisturbance;
            }
            set
            {
                constantRipplesMinimumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// Sets the maximum displacement of water’s surface.
        /// </summary>
        public float ConstantRipplesMaximumDisturbance
        {
            get
            {
                return constantRipplesMaximumDisturbance;
            }
            set
            {
                constantRipplesMaximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// Disturb neighbor vertices to create a smoother ripple (wave).
        /// </summary>
        public bool ConstantRipplesSmoothDisturbance
        {
            get
            {
                return constantRipplesSmoothDisturbance;
            }
            set
            {
                constantRipplesSmoothDisturbance = value;
            }
        }

        /// <summary>
        /// The amount of disturbance to apply to neighbor vertices.
        /// </summary>
        public float ConstantRipplesSmoothFactor
        {
            get
            {
                return constantRipplesSmoothFactor;
            }
            set
            {
                constantRipplesSmoothFactor = Mathf.Clamp01(value);
            }
        }

        //Interval Properties

        /// <summary>
        /// Randomize the interval.
        /// </summary>
        public bool ConstantRipplesRandomizeInterval
        {
            get
            {
                return constantRipplesRandomizeInterval;
            }
            set
            {
                constantRipplesRandomizeInterval = value;
            }
        }

        /// <summary>
        /// Apply constant ripples at regular intervals (second).
        /// </summary>
        public float ConstantRipplesInterval
        {
            get
            {
                return constantRipplesInterval;
            }
            set
            {
                constantRipplesInterval = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// Minimum Interval.
        /// </summary>
        public float ConstantRipplesMinimumInterval
        {
            get
            {
                return constantRipplesMinimumInterval;
            }
            set
            {
                constantRipplesMinimumInterval = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// Maximum Interval.
        /// </summary>
        public float ConstantRipplesMaximumInterval
        {
            get
            {
                return constantRipplesMaximumInterval;
            }
            set
            {
                constantRipplesMaximumInterval = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        //Ripple Source Positions Properties

        /// <summary>
        /// Randomize constant ripples sources positions.
        /// </summary>
        public bool ConstantRipplesRandomizeRipplesSourcesPositions
        {
            get
            {
                return constantRipplesRandomizeRipplesSourcesPositions;
            }
            set
            {
                constantRipplesRandomizeRipplesSourcesPositions = value;
            }
        }

        /// <summary>
        /// Sets the constant ripples sources positions. (Positions are in water gameobject local-space coordinates)
        /// </summary>
        public List<float> ConstantRipplesSourcePositions
        {
            get
            {
                return constantRipplesSourcePositions;
            }
            set
            {
                constantRipplesSourcePositions = value;
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        /// <summary>
        /// Sets the number of constant ripples sources.
        /// </summary>
        public int ConstantRipplesRandomizeRipplesSourcesCount
        {
            get
            {
                return constantRipplesRandomizeRipplesSourcesCount;
            }
            set
            {
                constantRipplesRandomizeRipplesSourcesCount = Mathf.Clamp(value, 0, int.MaxValue);
            }
        }

        /// <summary>
        /// Allow generating on the same frame and in the same position multiple constant ripples.
        /// </summary>
        public bool ConstantRipplesAllowDuplicateRipplesSourcesPositions
        {
            get
            {
                return constantRipplesAllowDuplicateRipplesSourcesPositions;
            }
            set
            {
                constantRipplesAllowDuplicateRipplesSourcesPositions = value;
                constantRipplesUpdateSourcesIndices = true;
            }
        }

        //Sound Effect Properties

        /// <summary>
        /// Activate/Deactivate playing the sound effect when generating constant ripples.
        /// </summary>
        public bool ConstantRipplesActivateSoundEffect
        {
            get
            {
                return constantRipplesActivateSoundEffect;
            }
            set
            {
                if (constantRipplesActivateSoundEffect == value)
                    return;

                constantRipplesActivateSoundEffect = value;
                constantRipplesReconstructSoundEffectPool = true;
            }
        }

        /// <summary>
        /// The AudioClip asset to play when generating constant ripples.
        /// </summary>
        public AudioClip ConstantRipplesAudioClip
        {
            get
            {
                return constantRipplesAudioClip;
            }
            set
            {
                if (constantRipplesAudioClip == value)
                    return;

                constantRipplesAudioClip = value;
                constantRipplesReconstructSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the audio clip’s minimum playback speed.
        /// </summary>
        public float ConstantRipplesMinimumAudioPitch
        {
            get
            {
                return constantRipplesMinimumAudioPitch;
            }
            set
            {
                constantRipplesMinimumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the audio clip’s maximum playback speed.
        /// </summary>
        public float ConstantRipplesMaximumAudioPitch
        {
            get
            {
                return constantRipplesMaximumAudioPitch;
            }
            set
            {
                constantRipplesMaximumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the audio clip’s volume (Range: 0..1).
        /// </summary>
        public float ConstantRipplesAudioVolume
        {
            get
            {
                return constantRipplesAudioVolume;
            }
            set
            {
                constantRipplesAudioVolume = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Apply constant audio clip playback speed.
        /// </summary>
        public bool ConstantRipplesUseConstantAudioPitch
        {
            get
            {
                return constantRipplesUseConstantAudioPitch;
            }
            set
            {
                constantRipplesUseConstantAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s playback speed.
        /// </summary>
        public float ConstantRipplesAudioPitch
        {
            get
            {
                return constantRipplesAudioPitch;
            }
            set
            {
                constantRipplesAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the number of audio sources objects that will be created and pooled when the game starts.
        /// </summary>
        public int ConstantRipplesSoundEffectPoolSize
        {
            get
            {
                return constantRipplesSoundEffectPoolSize;
            }
            set
            {
                if (constantRipplesSoundEffectPoolSize == value)
                    return;

                constantRipplesSoundEffectPoolSize = value;
                constantRipplesReconstructSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool ConstantRipplesSoundEffectPoolExpandIfNecessary
        {
            get
            {
                return constantRipplesSoundEffectPoolExpandIfNecessary;
            }
            set
            {
                constantRipplesSoundEffectPoolExpandIfNecessary = value;
                if (constantRipplesSoundEffectPool != null)
                    constantRipplesSoundEffectPool.ExpandIfNecessary = value;
            }
        }

        //Particle Effect Properties

        /// <summary>
        /// Activate/Deactivate Playing the particle effect when generating constant ripples.
        /// </summary>
        public bool ConstantRipplesActivateParticleEffect
        {
            get
            {
                return constantRipplesActivateParticleEffect;
            }
            set
            {
                if (constantRipplesActivateParticleEffect == value)
                    return;

                constantRipplesActivateParticleEffect = value;
                constantRipplesReconstructParticleEffectPool = true;
            }
        }

        [System.Obsolete("Use ConstantRipplesActivateParticleEffect instead")]
        public bool ActivateConstantSplashParticleEffect
        {
            get
            {
                return ConstantRipplesActivateParticleEffect;
            }
            set
            {
                ConstantRipplesActivateParticleEffect = value;
            }
        }

        /// <summary>
        /// Sets the particle effect prefab.
        /// </summary>
        public ParticleSystem ConstantRipplesParticleEffect
        {
            get
            {
                return constantRipplesParticleEffect;
            }
            set
            {
                if (constantRipplesParticleEffect == value)
                    return;

                constantRipplesParticleEffect = value;
                constantRipplesReconstructParticleEffectPool = true;
            }
        }

        [System.Obsolete("Use ConstantRipplesParticleEffect instead")]
        public ParticleSystem ConstantSplashParticleEffect
        {
            get
            {
                return ConstantRipplesParticleEffect;
            }
            set
            {
                ConstantRipplesParticleEffect = value;
            }
        }

        /// <summary>
        /// Sets the number of particle effect objects that will be created and pooled when the game starts
        /// </summary>
        public int ConstantRipplesParticleEffectPoolSize
        {
            get
            {
                return constantRipplesParticleEffectPoolSize;
            }
            set
            {
                int newValue = Mathf.Clamp(value, 0, int.MaxValue);
                if (constantRipplesParticleEffectPoolSize == newValue)
                    return;

                constantRipplesParticleEffectPoolSize = newValue;
                constantRipplesReconstructParticleEffectPool = true;
            }
        }

        [System.Obsolete("Use ConstantRipplesParticleEffectPoolSize instead")]
        public int ConstantSplashParticleEffectPoolSize
        {
            get
            {
                return ConstantRipplesParticleEffectPoolSize;
            }
            set
            {
                ConstantRipplesParticleEffectPoolSize = value;
            }
        }

        /// <summary>
        /// UnityEvent that is triggered when the particle effect stops playing.
        /// </summary>
        public UnityEvent ConstantRipplesParticleEffectStopAction
        {
            get
            {
                return constantRipplesParticleEffectStopAction;
            }
            set
            {
                constantRipplesParticleEffectStopAction = value;
            }
        }

        /// <summary>
        /// Shift the particle effect spawn position.
        /// </summary>
        public Vector3 ConstantRipplesParticleEffectSpawnOffset
        {
            get
            {
                return constantRipplesParticleEffectSpawnOffset;
            }
            set
            {
                constantRipplesParticleEffectSpawnOffset = value;
            }
        }

        [System.Obsolete("Use ConstantRipplesParticleEffectSpawnOffset instead")]
        public Vector3 ConstantSplashParticleEffectSpawnOffset
        {
            get
            {
                return ConstantRipplesParticleEffectSpawnOffset;
            }
            set
            {
                ConstantRipplesParticleEffectSpawnOffset = value;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool ConstantRipplesParticleEffectPoolExpandIfNecessary
        {
            get
            {
                return constantRipplesParticleEffectPoolExpandIfNecessary;
            }
            set
            {
                constantRipplesParticleEffectPoolExpandIfNecessary = value;
                if (constantRipplesParticleEffectPool != null)
                    constantRipplesParticleEffectPool.ExpandIfNecessary = value;
            }
        }
        #endregion

        #region Script-Generated Ripples Properties

        /// <summary>
        /// The minimum displacement of water’s surface when generating ripples through script.
        /// </summary>
        public float ScriptGeneratedRipplesMinimumDisturbance
        {
            get
            {
                return scriptGeneratedRipplesMinimumDisturbance;
            }
            set
            {
                scriptGeneratedRipplesMinimumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        /// <summary>
        /// The maximum displacement of water’s surface when generating ripples through script.
        /// </summary>
        public float ScriptGeneratedRipplesMaximumDisturbance
        {
            get
            {
                return scriptGeneratedRipplesMaximumDisturbance;
            }
            set
            {
                scriptGeneratedRipplesMaximumDisturbance = Mathf.Clamp(value, 0f, float.MaxValue);
            }
        }

        //Sound Effect Properties

        /// <summary>
        /// Activate/Deactivate playing the sound effect when generating ripples through script.
        /// </summary>
        public bool ScriptGeneratedRipplesActivateSoundEffect
        {
            get
            {
                return scriptGeneratedRipplesActivateSoundEffect;
            }
            set
            {
                if (scriptGeneratedRipplesActivateSoundEffect == value)
                    return;

                scriptGeneratedRipplesActivateSoundEffect = value;
                scriptGeneratedRipplesReconstructSoundEffectPool = true;
            }
        }

        /// <summary>
        /// The AudioClip asset to play when generating ripples through script.
        /// </summary>
        public AudioClip ScriptGeneratedRipplesAudioClip
        {
            get
            {
                return scriptGeneratedRipplesAudioClip;
            }
            set
            {
                if (scriptGeneratedRipplesAudioClip == value)
                    return;

                scriptGeneratedRipplesAudioClip = value;
                scriptGeneratedRipplesReconstructSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the audio clip’s minimum playback speed.
        /// </summary>
        public float ScriptGeneratedRipplesMinimumAudioPitch
        {
            get
            {
                return scriptGeneratedRipplesMinimumAudioPitch;
            }
            set
            {
                scriptGeneratedRipplesMinimumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the audio clip’s maximum playback speed.
        /// </summary>
        public float ScriptGeneratedRipplesMaximumAudioPitch
        {
            get
            {
                return scriptGeneratedRipplesMaximumAudioPitch;
            }
            set
            {
                scriptGeneratedRipplesMaximumAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Apply constant audio clip playback speed.
        /// </summary>
        public bool ScriptGeneratedRipplesUseConstantAudioPitch
        {
            get
            {
                return scriptGeneratedRipplesUseConstantAudioPitch;
            }
            set
            {
                scriptGeneratedRipplesUseConstantAudioPitch = value;
            }
        }

        /// <summary>
        /// Sets the audio clip’s playback speed.
        /// </summary>
        public float ScriptGeneratedRipplesAudioPitch
        {
            get
            {
                return scriptGeneratedRipplesAudioPitch;
            }
            set
            {
                scriptGeneratedRipplesAudioPitch = Mathf.Clamp(value, -3f, 3f);
            }
        }

        /// <summary>
        /// Sets the audio clip’s volume (Range: 0..1).
        /// </summary>
        public float ScriptGeneratedRipplesAudioVolume
        {
            get
            {
                return scriptGeneratedRipplesAudioVolume;
            }
            set
            {
                scriptGeneratedRipplesAudioVolume = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Sets the number of audio sources objects that will be created and pooled when the game starts.
        /// </summary>
        public int ScriptGeneratedRipplesSoundEffectPoolSize
        {
            get
            {
                return scriptGeneratedRipplesSoundEffectPoolSize;
            }
            set
            {
                if (scriptGeneratedRipplesSoundEffectPoolSize == value)
                    return;

                scriptGeneratedRipplesSoundEffectPoolSize = value;
                scriptGeneratedRipplesReconstructSoundEffectPool = true;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool ScriptGeneratedRipplesSoundEffectPoolExpandIfNecessary
        {
            get
            {
                return scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary;
            }
            set
            {
                scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary = value;
                if (scriptGeneratedRipplesSoundEffectPool != null)
                    scriptGeneratedRipplesSoundEffectPool.ExpandIfNecessary = value;
            }
        }
        //Particle Effect Properties

        /// <summary>
        /// Play the particle effect when applying script-generated ripples through script.
        /// </summary>
        public bool ScriptGeneratedRipplesActivateParticleEffect
        {
            get
            {
                return scriptGeneratedRipplesActivateParticleEffect;
            }
            set
            {
                if (scriptGeneratedRipplesActivateParticleEffect == value)
                    return;

                scriptGeneratedRipplesActivateParticleEffect = value;
                scriptGeneratedRipplesReconstructParticleEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the particle effect prefab to play when generating ripples through script.
        /// </summary>
        public ParticleSystem ScriptGeneratedRipplesParticleEffect
        {
            get
            {
                return scriptGeneratedRipplesParticleEffect;
            }
            set
            {
                scriptGeneratedRipplesParticleEffect = value;
                scriptGeneratedRipplesReconstructParticleEffectPool = true;
            }
        }

        /// <summary>
        /// Sets the number of particle effect objects that will be created and pooled when the game starts.
        /// </summary>
        public int ScriptGeneratedRipplesParticleEffectPoolSize
        {
            get
            {
                return scriptGeneratedRipplesParticleEffectPoolSize;
            }
            set
            {
                scriptGeneratedRipplesParticleEffectPoolSize = Mathf.Clamp(value, 0, int.MaxValue);
                scriptGeneratedRipplesReconstructParticleEffectPool = true;
            }
        }

        /// <summary>
        /// Shift the particle effect spawn position.
        /// </summary>
        public Vector3 ScriptGeneratedRipplesParticleEffectSpawnOffset
        {
            get
            {
                return scriptGeneratedRipplesParticleEffectSpawnOffset;
            }
            set
            {
                scriptGeneratedRipplesParticleEffectSpawnOffset = value;
            }
        }

        /// <summary>
        /// UnityEvent that is triggered when the particle effect stops playing.
        /// </summary>
        public UnityEvent ScriptGeneratedRipplesParticleEffectStopAction
        {
            get
            {
                return scriptGeneratedRipplesParticleEffectStopAction;
            }
            set
            {
                scriptGeneratedRipplesParticleEffectStopAction = value;
            }
        }

        /// <summary>
        /// Enables/Disables increasing the number of pooled objects at runtime if needed.
        /// </summary>
        public bool ScriptGeneratedRipplesParticleEffectPoolExpandIfNecessary
        {
            get
            {
                return scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary;
            }
            set
            {
                scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary = value;
                if (scriptGeneratedRipplesParticleEffectPool != null)
                    scriptGeneratedRipplesParticleEffectPool.ExpandIfNecessary = value;
            }
        }
        #endregion

        #region Refraction & Reflection Rendering Properties
        //Refraction Properties

        /// <summary>
        /// Specifies how much the RenderTexture used to render refraction is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality.
        /// </summary>
        public float RefractionRenderTextureResizeFactor
        {
            get
            {
                return refractionRenderTextureResizeFactor;
            }
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(refractionRenderTextureResizeFactor, value))
                    return;
                refractionRenderTextureResizeFactor = value;
                updateWaterCameraRenderingSettings = true;
            }
        }

        /// <summary>
        /// Only GameObjects on these layers will be rendered.
        /// </summary>
        public LayerMask RefractionCullingMask
        {
            get
            {
                return refractionCullingMask;
            }
            set
            {
                refractionCullingMask = value & ~(1 << 4);
            }
        }

        /// <summary>
        /// Sets refraction render texture's filter mode.
        /// </summary>
        public FilterMode RefractionRenderTextureFilterMode
        {
            get
            {
                return refractionRenderTextureFilterMode;
            }
            set
            {
                refractionRenderTextureFilterMode = value;
            }
        }

        //Reflection Properties

        /// <summary>
        /// Specifies how much the RenderTexture used to render reflection is resized. Decreasing this value lowers the RenderTexture resolution and thus improves performance at the expense of visual quality.
        /// </summary>
        public float ReflectionRenderTextureResizeFactor
        {
            get
            {
                return reflectionRenderTextureResizeFactor;
            }
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(reflectionRenderTextureResizeFactor, value))
                    return;
                reflectionRenderTextureResizeFactor = value;
                updateWaterCameraRenderingSettings = true;
            }
        }

        /// <summary>
        /// Only GameObjects on these layers will be rendered.
        /// </summary>
        public LayerMask ReflectionCullingMask
        {
            get
            {
                return reflectionCullingMask;
            }
            set
            {
                reflectionCullingMask = value & ~(1 << 4);
            }
        }

        /// <summary>
        /// Controls where to start rendering reflection relative to the water GameObject position.
        /// </summary>
        public float ReflectionZOffset
        {
            get
            {
                return reflectionZOffset;
            }
            set
            {
                reflectionZOffset = value;
                waterCameraPositionForReflectionRendering.z = waterPosition.z + reflectionZOffset;
            }
        }

        /// <summary>
        /// Sets reflection render texture's filter mode.
        /// </summary>
        public FilterMode ReflectionRenderTextureFilterMode
        {
            get
            {
                return reflectionRenderTextureFilterMode;
            }
            set
            {
                reflectionRenderTextureFilterMode = value;
            }
        }

        //Shared Properties

        /// <summary>
        /// The name of the water mesh renderer sorting layer.
        /// </summary>
        public int SortingLayerID
        {
            get
            {
                return sortingLayerID;
            }
            set
            {
                if (sortingLayerID == value)
                    return;
                sortingLayerID = value;
                if (meshRenderer)
                    meshRenderer.sortingLayerID = sortingLayerID;
            }
        }

        /// <summary>
        /// The water mesh renderer order within a sorting layer.
        /// </summary>
        public int SortingOrder
        {
            get
            {
                return sortingOrder;
            }
            set
            {
                if (sortingOrder == value)
                    return;
                sortingOrder = value;
                if (meshRenderer)
                    meshRenderer.sortingOrder = sortingOrder;
            }
        }

        /// <summary>
        /// Controls whether the rendered objects will be affected by pixel lights. Disabling this could increase performance at the expense of visual fidelity.
        /// </summary>
        public bool RenderPixelLights
        {
            get
            {
                return renderPixelLights;
            }
            set
            {
                renderPixelLights = value;
            }
        }

        /// <summary>
        /// Sets the furthest point relative to the water to draw when rendering refraction and/or reflection.
        /// </summary>
        public float FarClipPlane
        {
            get
            {
                return farClipPlane;
            }
            set
            {
                if (Mathf.Approximately(farClipPlane, value))
                    return;
                farClipPlane = value;
                updateWaterCameraRenderingSettings = true;
                if (waterCamera)
                    waterCamera.farClipPlane = farClipPlane;
            }
        }

        /// <summary>
        /// Allow multisample antialiasing rendering.
        /// </summary>
        public bool AllowMSAA
        {
            get
            {
                return allowMSAA;
            }
            set
            {
                if (allowMSAA == value)
                    return;
                allowMSAA = value;
                if (waterCamera)
                    waterCamera.allowMSAA = allowMSAA;
            }
        }

        /// <summary>
        /// Allow high dynamic range rendering.
        /// </summary>
        public bool AllowHDR
        {
            get
            {
                return allowHDR;
            }
            set
            {
                allowHDR = value;
                if (waterCamera)
                    waterCamera.allowHDR = allowHDR;
            }
        }

        #endregion

        #endregion

        #region Methods

#if UNITY_EDITOR
        // Add menu item to create Game2D Water GameObject.
        // Priority 10 ensures it is grouped with the other menu items of the same kind and propagated to the hierarchy dropdown and hierarchy context menus.
        [MenuItem("GameObject/2D Object/Game2D Water", false, 10)]
        static private void CreateCustomGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Game2D Water");
            go.AddComponent<Game2DWater>();
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create" + go.name);
            Selection.activeObject = go;
        }

        private void Reset()
        {
            //Water Properties
            //Mesh Properties
            waterSize = Vector2.one;
            lastWaterSize = Vector2.one;
            subdivisionsCountPerUnit = 3;
            //Wave Properties
            damping = 0.05f;
            stiffness = 60f;
            stiffnessSquareRoot = Mathf.Sqrt(60f);
            spread = 60f;
            useCustomBoundaries = false;
            firstCustomBoundary = 0.5f;
            secondCustomBoundary = -0.5f;
            lastSecondCustomBoundary = -0.5f;
            lastFirstCustomBoundary = 0.5f;
            //Events
            onWaterEnter = new UnityEvent();
            onWaterExit = new UnityEvent();
            //Misc
            buoyancyEffectorSurfaceLevel = 0.02f;

            //On-Collision Ripples Properties
            //Disturbance
            onCollisionRipplesMinimumDisturbance = 0.1f;
            onCollisionRipplesMaximumDisturbance = 0.75f;
            onCollisionRipplesVelocityMultiplier = 0.12f;
            //Collision
            onCollisionRipplesCollisionMask = ~(1 << 4);
            onCollisionRipplesCollisionMinimumDepth = -10f;
            onCollisionRipplesCollisionMaximumDepth = 10f;
            onCollisionRipplesCollisionRaycastMaxDistance = 0.5f;
            //Sound Effect (On Water Enter)
            onCollisionRipplesActivateOnWaterEnterSoundEffect = false;
            onCollisionRipplesOnWaterEnterAudioClip = null;
            onCollisionRipplesUseConstantOnWaterEnterAudioPitch = false;
            onCollisionRipplesOnWaterEnterMinimumAudioPitch = 0.75f;
            onCollisionRipplesOnWaterEnterMaximumAudioPitch = 1.25f;
            onCollisionRipplesOnWaterEnterAudioPitch = 1f;
            onCollisionRipplesOnWaterEnterSoundEffectPoolSize = 10;
            onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary = false;
            if (onCollisionRipplesOnWaterEnterSoundEffectPool != null)
            {
                onCollisionRipplesOnWaterEnterSoundEffectPool.DestroyPool();
                onCollisionRipplesOnWaterEnterSoundEffectPool = null;
            }
            //Sound Effect (On Water Exit)
            onCollisionRipplesActivateOnWaterExitSoundEffect = false;
            onCollisionRipplesOnWaterExitAudioClip = null;
            onCollisionRipplesUseConstantOnWaterExitAudioPitch = false;
            onCollisionRipplesOnWaterExitMinimumAudioPitch = 0.75f;
            onCollisionRipplesOnWaterExitMaximumAudioPitch = 1.25f;
            onCollisionRipplesOnWaterExitAudioPitch = 1f;
            onCollisionRipplesOnWaterExitSoundEffectPoolSize = 10;
            onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary = false;
            if (onCollisionRipplesOnWaterExitSoundEffectPool != null)
            {
                onCollisionRipplesOnWaterExitSoundEffectPool.DestroyPool();
                onCollisionRipplesOnWaterExitSoundEffectPool = null;
            }
            //Particle Effect (On Water Enter)
            onCollisionRipplesActivateOnWaterEnterParticleEffect = false;
            onCollisionRipplesOnWaterEnterParticleEffect = null;
            onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary = false;
            onCollisionRipplesOnWaterEnterParticleEffectPoolSize = 10;
            onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset = Vector3.zero;
            onCollisionRipplesOnWaterEnterParticleEffectStopAction = new UnityEvent();
            if (onCollisionRipplesOnWaterEnterParticleEffectPool != null)
            {
                onCollisionRipplesOnWaterEnterParticleEffectPool.DestroyPool();
                onCollisionRipplesOnWaterEnterParticleEffectPool = null;
            }
            //Particle Effect (On Water Exit)
            onCollisionRipplesActivateOnWaterExitParticleEffect = false;
            onCollisionRipplesOnWaterExitParticleEffect = null;
            onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary = false;
            onCollisionRipplesOnWaterExitParticleEffectPoolSize = 10;
            onCollisionRipplesOnWaterExitParticleEffectSpawnOffset = Vector3.zero;
            onCollisionRipplesOnWaterExitParticleEffectStopAction = new UnityEvent();
            if (onCollisionRipplesOnWaterExitParticleEffectPool != null)
            {
                onCollisionRipplesOnWaterExitParticleEffectPool.DestroyPool();
                onCollisionRipplesOnWaterExitParticleEffectPool = null;
            }

            //Constant Ripples Properties
            activateConstantRipples = false;
            constantRipplesUpdateWhenOffscreen = false;
            //Disturbance
            constantRipplesRandomizeDisturbance = false;
            constantRipplesDisturbance = 0.10f;
            constantRipplesMinimumDisturbance = 0.08f;
            constantRipplesMaximumDisturbance = 0.12f;
            constantRipplesSmoothDisturbance = false;
            constantRipplesSmoothFactor = 0.5f;
            //Interval
            constantRipplesRandomizeInterval = false;
            constantRipplesInterval = 1f;
            constantRipplesMinimumInterval = 0.75f;
            constantRipplesMaximumInterval = 1.25f;
            constantRipplesDeltaTime = 0f;
            constantRipplesCurrentInterval = 0f;
            //Sources Positions
            constantRipplesRandomizeRipplesSourcesPositions = false;
            constantRipplesRandomizeRipplesSourcesCount = 3;
            constantRipplesAllowDuplicateRipplesSourcesPositions = false;
            constantRipplesSourcePositions.Clear();
            if (constantRipplesSourcesIndices != null)
                constantRipplesSourcesIndices.Clear();
            //Sound Effect
            constantRipplesActivateSoundEffect = false;
            constantRipplesUseConstantAudioPitch = false;
            constantRipplesAudioClip = null;
            constantRipplesAudioPitch = 1f;
            constantRipplesMinimumAudioPitch = 0.75f;
            constantRipplesMaximumAudioPitch = 1.25f;
            constantRipplesSoundEffectPoolExpandIfNecessary = false;
            constantRipplesSoundEffectPoolSize = 10;
            if (constantRipplesSoundEffectPool != null)
            {
                constantRipplesSoundEffectPool.DestroyPool();
                constantRipplesSoundEffectPool = null;
            }
            //Particle Effect
            constantRipplesActivateParticleEffect = false;
            constantRipplesParticleEffect = null;
            constantRipplesParticleEffectPoolExpandIfNecessary = false;
            constantRipplesParticleEffectPoolSize = 10;
            constantRipplesParticleEffectSpawnOffset = Vector3.zero;
            constantRipplesParticleEffectStopAction = new UnityEvent();
            if (constantRipplesParticleEffectPool != null)
            {
                constantRipplesParticleEffectPool.DestroyPool();
                constantRipplesParticleEffectPool = null;
            }

            //Script-Generated Ripples Properties
            //Disturbance
            scriptGeneratedRipplesMinimumDisturbance = 0.1f;
            scriptGeneratedRipplesMaximumDisturbance = 0.75f;
            //Sound Effect
            scriptGeneratedRipplesActivateSoundEffect = false;
            scriptGeneratedRipplesAudioClip = null;
            scriptGeneratedRipplesUseConstantAudioPitch = false;
            scriptGeneratedRipplesAudioPitch = 1f;
            scriptGeneratedRipplesMinimumAudioPitch = 0.75f;
            scriptGeneratedRipplesMaximumAudioPitch = 1.25f;
            scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary = false;
            scriptGeneratedRipplesSoundEffectPoolSize = 10;
            if (scriptGeneratedRipplesSoundEffectPool != null)
            {
                scriptGeneratedRipplesSoundEffectPool.DestroyPool();
                scriptGeneratedRipplesSoundEffectPool = null;
            }
            //Particle Effect
            scriptGeneratedRipplesActivateParticleEffect = false;
            scriptGeneratedRipplesParticleEffect = null;
            scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary = false;
            scriptGeneratedRipplesParticleEffectPoolSize = 10;
            scriptGeneratedRipplesParticleEffectSpawnOffset = Vector3.zero;
            scriptGeneratedRipplesParticleEffectStopAction = new UnityEvent();
            if (scriptGeneratedRipplesParticleEffectPool != null)
            {
                scriptGeneratedRipplesParticleEffectPool.DestroyPool();
                scriptGeneratedRipplesParticleEffectPool = null;
            }

            //Refraction & Reflection Rendering Properties
            //Refraction
            refractionRenderTextureResizeFactor = 1f;
            refractionCullingMask = ~(1 << 4);
            //Reflection
            reflectionRenderTextureResizeFactor = 1f;
            reflectionCullingMask = ~(1 << 4);
            reflectionZOffset = 0f;
            //Shared rendering Properties
            sortingLayerID = 0;
            sortingOrder = 0;
            farClipPlane = 100f;
            renderPixelLights = true;
            allowMSAA = false;
            allowHDR = false;
            updateWaterCameraRenderingSettings = true;
            if (waterCamera)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(waterCamera.gameObject);
                else
                    Destroy(waterCamera.gameObject);
            }

            RecomputeMesh();
        }

        /// <summary>
        /// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        /// </summary>
        public void OnValidate()
        {
            //Water Properties
            //Mesh
            waterSize.x = Mathf.Clamp(waterSize.x, 0f, float.MaxValue);
            waterSize.y = Mathf.Clamp(waterSize.y, 0f, float.MaxValue);
            subdivisionsCountPerUnit = Mathf.Clamp(subdivisionsCountPerUnit, 0, int.MaxValue);
            //Waves
            damping = Mathf.Clamp01(damping);
            stiffness = Mathf.Clamp(stiffness, 0f, float.MaxValue);
            stiffnessSquareRoot = Mathf.Sqrt(stiffness);
            spread = Mathf.Clamp(spread, 0f, float.MaxValue);
            float halfWidth = waterSize.x / 2f;
            firstCustomBoundary = Mathf.Clamp(firstCustomBoundary, -halfWidth, halfWidth);
            secondCustomBoundary = Mathf.Clamp(secondCustomBoundary, -halfWidth, halfWidth);
            //Misc
            buoyancyEffectorSurfaceLevel = Mathf.Clamp01(buoyancyEffectorSurfaceLevel);

            //On-Collision Ripples
            //Disturbance
            onCollisionRipplesMinimumDisturbance = Mathf.Clamp(onCollisionRipplesMinimumDisturbance, 0f, float.MaxValue);
            onCollisionRipplesMaximumDisturbance = Mathf.Clamp(onCollisionRipplesMaximumDisturbance, 0f, float.MaxValue);
            onCollisionRipplesVelocityMultiplier = Mathf.Clamp(onCollisionRipplesVelocityMultiplier, 0f, float.MaxValue);
            //Collision
            onCollisionRipplesCollisionMask &= ~(1 << 4);
            onCollisionRipplesCollisionRaycastMaxDistance = Mathf.Clamp(onCollisionRipplesCollisionRaycastMaxDistance, 0f, float.MaxValue);
            //Sound Effect (On Water Enter)
            onCollisionRipplesOnWaterEnterSoundEffectPoolSize = Mathf.Clamp(onCollisionRipplesOnWaterEnterSoundEffectPoolSize, 0, int.MaxValue);
            if (onCollisionRipplesOnWaterEnterSoundEffectPool != null)
                onCollisionRipplesOnWaterEnterSoundEffectPool.ExpandIfNecessary = onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary;
            //Sound Effect (On Water Exit)
            onCollisionRipplesOnWaterExitSoundEffectPoolSize = Mathf.Clamp(onCollisionRipplesOnWaterExitSoundEffectPoolSize, 0, int.MaxValue);
            if (onCollisionRipplesOnWaterExitSoundEffectPool != null)
                onCollisionRipplesOnWaterExitSoundEffectPool.ExpandIfNecessary = onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary;
            //Particle Effect (On Water Enter)
            onCollisionRipplesOnWaterEnterParticleEffectPoolSize = Mathf.Clamp(onCollisionRipplesOnWaterEnterParticleEffectPoolSize, 0, int.MaxValue);
            if (onCollisionRipplesOnWaterEnterParticleEffectPool != null)
                onCollisionRipplesOnWaterEnterParticleEffectPool.ExpandIfNecessary = onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary;
            //Particle Effect (On Water Exit)
            onCollisionRipplesOnWaterExitParticleEffectPoolSize = Mathf.Clamp(onCollisionRipplesOnWaterExitParticleEffectPoolSize, 0, int.MaxValue);
            if (onCollisionRipplesOnWaterExitParticleEffectPool != null)
                onCollisionRipplesOnWaterExitParticleEffectPool.ExpandIfNecessary = onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary;

            //Constant Ripples Properties
            //Disturbance
            constantRipplesDisturbance = Mathf.Clamp(constantRipplesDisturbance, 0f, float.MaxValue);
            constantRipplesMinimumDisturbance = Mathf.Clamp(constantRipplesMinimumDisturbance, 0f, float.MaxValue);
            constantRipplesMaximumDisturbance = Mathf.Clamp(constantRipplesMaximumDisturbance, 0f, float.MaxValue);
            //Interval
            constantRipplesInterval = Mathf.Clamp(constantRipplesInterval, 0f, float.MaxValue);
            constantRipplesMinimumInterval = Mathf.Clamp(constantRipplesMinimumInterval, 0f, float.MaxValue);
            constantRipplesMaximumInterval = Mathf.Clamp(constantRipplesMaximumInterval, 0f, float.MaxValue);
            //Sources Positions
            constantRipplesRandomizeRipplesSourcesCount = Mathf.Clamp(constantRipplesRandomizeRipplesSourcesCount, 0, int.MaxValue);
            //Sound Effect
            constantRipplesSoundEffectPoolSize = Mathf.Clamp(constantRipplesSoundEffectPoolSize, 0, int.MaxValue);
            if (constantRipplesSoundEffectPool != null)
                constantRipplesSoundEffectPool.ExpandIfNecessary = constantRipplesSoundEffectPoolExpandIfNecessary;
            //Particle Effect
            constantRipplesParticleEffectPoolSize = Mathf.Clamp(constantRipplesParticleEffectPoolSize, 0, int.MaxValue);
            if (constantRipplesParticleEffectPool != null)
                constantRipplesParticleEffectPool.ExpandIfNecessary = constantRipplesParticleEffectPoolExpandIfNecessary;

            //Script-Generated Ripples
            //Disturbance
            scriptGeneratedRipplesMinimumDisturbance = Mathf.Clamp(scriptGeneratedRipplesMinimumDisturbance, 0f, float.MaxValue);
            scriptGeneratedRipplesMaximumDisturbance = Mathf.Clamp(scriptGeneratedRipplesMaximumDisturbance, 0f, float.MaxValue);
            //Sound Effect
            scriptGeneratedRipplesSoundEffectPoolSize = Mathf.Clamp(scriptGeneratedRipplesSoundEffectPoolSize, 0, int.MaxValue);
            if (scriptGeneratedRipplesSoundEffectPool != null)
                scriptGeneratedRipplesSoundEffectPool.ExpandIfNecessary = scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary;
            //Particle Effect
            scriptGeneratedRipplesParticleEffectPoolSize = Mathf.Clamp(scriptGeneratedRipplesParticleEffectPoolSize, 0, int.MaxValue);
            if (scriptGeneratedRipplesParticleEffectPool != null)
                scriptGeneratedRipplesParticleEffectPool.ExpandIfNecessary = scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary;

            //Refraction & Reflection Rendering Properties
            //Refracion
            refractionRenderTextureResizeFactor = Mathf.Clamp01(refractionRenderTextureResizeFactor);
            refractionCullingMask &= ~(1 << 4);
            //Reflection
            reflectionRenderTextureResizeFactor = Mathf.Clamp01(reflectionRenderTextureResizeFactor);
            reflectionCullingMask &= ~(1 << 4);
            //Shared Rendering Properties
            if (waterCamera)
            {
                waterCamera.allowMSAA = allowMSAA;
                waterCamera.allowHDR = allowHDR;
                waterCamera.farClipPlane = farClipPlane;
            }
            if (meshRenderer)
            {
                meshRenderer.sortingLayerID = sortingLayerID;
                meshRenderer.sortingOrder = sortingOrder;
            }

            if (meshFilter && meshFilter.sharedMesh)
            {
                Vector3 meshSize = meshFilter.sharedMesh.bounds.size;
                int meshVerticesCount = meshFilter.sharedMesh.vertexCount;
                int vertexCount;
                Vector2 size;
                if (useCustomBoundaries)
                    size = new Vector2(Mathf.Abs(secondCustomBoundary - firstCustomBoundary), waterSize.y);
                else
                    size = waterSize;
                vertexCount = (Mathf.RoundToInt(size.x * subdivisionsCountPerUnit) + 2) * 2;

                if (meshVerticesCount != vertexCount || !Mathf.Approximately(meshSize.x, size.x) || !Mathf.Approximately(meshSize.y, size.y))
                {
                    RecomputeMesh();
                    constantRipplesUpdateSourcesIndices = true;
                }
            }

            if (edgeCollider == null)
                edgeCollider = GetComponent<EdgeCollider2D>();

            UpdateComponents();
        }
#endif

        private void OnEnable()
        {
            if (!mesh || !meshFilter.sharedMesh)
                RecomputeMesh();

#if !UNITY_2017_1_OR_NEWER
                refractionTextureID = Shader.PropertyToID("_RefractionTexture");
                reflectionTextureID = Shader.PropertyToID("_ReflectionTexture");
                waterMatrixID = Shader.PropertyToID("_WaterMVP");
                waterReflectionLowerLimitID = Shader.PropertyToID("_ReflectionLowerLimit");
#endif
        }

        private void OnDisable()
        {
            if (waterCamera)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(waterCamera.gameObject);
                else
#endif
                    Destroy(waterCamera.gameObject);
            }
        }

        private void OnDestroy()
        {
            //Particle Effect Pools
            if (onCollisionRipplesOnWaterEnterParticleEffectPool != null)
                onCollisionRipplesOnWaterEnterParticleEffectPool.DestroyPool();

            if (onCollisionRipplesOnWaterExitParticleEffectPool != null)
                onCollisionRipplesOnWaterExitParticleEffectPool.DestroyPool();

            if (constantRipplesParticleEffectPool != null)
                constantRipplesParticleEffectPool.DestroyPool();

            if (scriptGeneratedRipplesParticleEffectPool != null)
                scriptGeneratedRipplesParticleEffectPool.DestroyPool();

            //Sound Effect Pools
            if (onCollisionRipplesOnWaterEnterSoundEffectPool != null)
                onCollisionRipplesOnWaterEnterSoundEffectPool.DestroyPool();

            if (onCollisionRipplesOnWaterExitSoundEffectPool != null)
                onCollisionRipplesOnWaterExitSoundEffectPool.DestroyPool();

            if (constantRipplesSoundEffectPool != null)
                constantRipplesSoundEffectPool.DestroyPool();

            if (scriptGeneratedRipplesSoundEffectPool != null)
                scriptGeneratedRipplesSoundEffectPool.DestroyPool();
        }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            edgeCollider = GetComponent<EdgeCollider2D>();
            buoyancyEffector = GetComponent<BuoyancyEffector2D>();

            waterMaterial = meshRenderer.sharedMaterial;
            if (!waterMaterial)
            {
                waterMaterial = new Material(Shader.Find("Game2DWaterKit/Unlit"));
                waterMaterial.name = gameObject.name + " Material";
                meshRenderer.sharedMaterial = waterMaterial;
            }
            meshRenderer.sortingLayerID = sortingLayerID;
            meshRenderer.sortingOrder = sortingOrder;

            //BuoyancyEffector only works when an attached collider is marked as a trigger and used by effector 
            boxCollider.isTrigger = true;
            boxCollider.usedByEffector = true;

            gameObject.layer = LayerMask.NameToLayer("Water");

            renderRefraction = waterMaterial.IsKeywordEnabled("Water2D_Refraction");
            renderReflection = waterMaterial.IsKeywordEnabled("Water2D_Reflection");
        }

        private void OnBecameVisible()
        {
            waterIsVisible = true;
        }

        private void OnBecameInvisible()
        {
            waterIsVisible = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (onCollisionRipplesCollisionMask != (onCollisionRipplesCollisionMask | (1 << other.gameObject.layer)))
                return;

            if (activateOnCollisionOnWaterEnterRipples)
            {
                Vector2 sumOfHitPointsPositions = new Vector2();
                float sumOfHitPointsVelocities = 0f;
                int hitsCount = 0;

                int vertexIndex;
                int endIndex;
                int startIndex;
                if (useCustomBoundaries)
                {
                    startIndex = 1;
                    endIndex = surfaceVerticesCount - 1;
                    vertexIndex = 2;
                }
                else
                {
                    startIndex = 0;
                    endIndex = surfaceVerticesCount;
                    vertexIndex = 0;
                }

                for (int i = startIndex; i < endIndex; i++, vertexIndex += 2)
                {
                    Vector2 surfaceVertexLocalSpacePosition = vertices[vertexIndex];
                    Vector2 surfaceVertexWorldSpacePosition;
                    surfaceVertexWorldSpacePosition.x = waterLocalToWorldMatrix.m00 * surfaceVertexLocalSpacePosition.x + waterLocalToWorldMatrix.m01 * surfaceVertexLocalSpacePosition.y + waterLocalToWorldMatrix.m03;
                    surfaceVertexWorldSpacePosition.y = waterLocalToWorldMatrix.m10 * surfaceVertexLocalSpacePosition.x + waterLocalToWorldMatrix.m11 * surfaceVertexLocalSpacePosition.y + waterLocalToWorldMatrix.m13;
                    RaycastHit2D hit = Physics2D.Raycast(surfaceVertexWorldSpacePosition, onCollisionRipplesCollisionRaycastDirection, onCollisionRipplesCollisionRaycastMaxDistance, onCollisionRipplesCollisionMask, onCollisionRipplesCollisionMinimumDepth, onCollisionRipplesCollisionMaximumDepth);
                    if (hit.collider == other && hit.rigidbody)
                    {
                        float velocity = -hit.rigidbody.GetPointVelocity(surfaceVertexWorldSpacePosition).y * onCollisionRipplesVelocityMultiplier;
                        velocity = Mathf.Clamp(velocity, onCollisionRipplesMinimumDisturbance, onCollisionRipplesMaximumDisturbance);
                        velocities[i] -= velocity * stiffnessSquareRoot;
                        sumOfHitPointsVelocities += velocity;
                        sumOfHitPointsPositions += hit.point;
                        hitsCount++;
                    }
                }

                if (hitsCount > 0)
                {
                    updateWaterSimulation = true;
                    float meanVelocity = sumOfHitPointsVelocities / hitsCount;
                    Vector2 meanPosition = sumOfHitPointsPositions / hitsCount;
                    Vector3 spawnPosition = new Vector3(meanPosition.x, meanPosition.y, waterPosition.z);

                    if (onCollisionRipplesActivateOnWaterEnterParticleEffect)
                        onCollisionRipplesOnWaterEnterParticleEffectPool.PlayParticleEffect(spawnPosition + onCollisionRipplesOnWaterEnterParticleEffectSpawnOffset);

                    if (onCollisionRipplesActivateOnWaterEnterSoundEffect)
                    {
                        float pitch;
                        if (onCollisionRipplesUseConstantOnWaterEnterAudioPitch)
                        {
                            pitch = onCollisionRipplesOnWaterEnterAudioPitch;
                        }
                        else
                        {
                            float interpolationValue = 1f - Mathf.InverseLerp(onCollisionRipplesMinimumDisturbance, onCollisionRipplesMaximumDisturbance, meanVelocity);
                            pitch = Mathf.Lerp(onCollisionRipplesOnWaterEnterMinimumAudioPitch, onCollisionRipplesOnWaterEnterMaximumAudioPitch, interpolationValue);
                        }
                        onCollisionRipplesOnWaterEnterSoundEffectPool.PlaySoundEffect(spawnPosition, pitch, onCollisionRipplesOnWaterEnterAudioVolume);
                    }
                }
            }

            if (onWaterEnter != null)
                onWaterEnter.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (onCollisionRipplesCollisionMask != (onCollisionRipplesCollisionMask | (1 << other.gameObject.layer)))
                return;

            if (activateOnCollisionOnWaterExitRipples)
            {
                Vector2 sumOfHitPointsPositions = new Vector2();
                float sumOfHitPointsVelocities = 0f;
                int hitsCount = 0;

                int vertexIndex;
                int endIndex;
                int startIndex;
                if (useCustomBoundaries)
                {
                    startIndex = 1;
                    endIndex = surfaceVerticesCount - 1;
                    vertexIndex = 2;
                }
                else
                {
                    startIndex = 0;
                    endIndex = surfaceVerticesCount;
                    vertexIndex = 0;
                }

                for (int i = startIndex; i < endIndex; i++, vertexIndex += 2)
                {
                    Vector2 surfaceVertexLocalSpacePosition = vertices[vertexIndex];
                    Vector2 surfaceVertexWorldSpacePosition;
                    surfaceVertexWorldSpacePosition.x = waterLocalToWorldMatrix.m00 * surfaceVertexLocalSpacePosition.x + waterLocalToWorldMatrix.m01 * surfaceVertexLocalSpacePosition.y + waterLocalToWorldMatrix.m03;
                    surfaceVertexWorldSpacePosition.y = waterLocalToWorldMatrix.m10 * surfaceVertexLocalSpacePosition.x + waterLocalToWorldMatrix.m11 * surfaceVertexLocalSpacePosition.y + waterLocalToWorldMatrix.m13;
                    RaycastHit2D hit = Physics2D.Raycast(surfaceVertexWorldSpacePosition, onCollisionRipplesCollisionRaycastDirection, onCollisionRipplesCollisionRaycastMaxDistance, onCollisionRipplesCollisionMask, onCollisionRipplesCollisionMinimumDepth, onCollisionRipplesCollisionMaximumDepth);
                    if (hit.collider == other && hit.rigidbody)
                    {
                        float velocity = hit.rigidbody.GetPointVelocity(surfaceVertexWorldSpacePosition).y * onCollisionRipplesVelocityMultiplier;
                        velocity = Mathf.Clamp(velocity, onCollisionRipplesMinimumDisturbance, onCollisionRipplesMaximumDisturbance);
                        velocities[i] += velocity * stiffnessSquareRoot;
                        sumOfHitPointsVelocities += velocity;
                        sumOfHitPointsPositions += hit.point;
                        hitsCount++;
                    }
                }

                if (hitsCount > 0)
                {
                    updateWaterSimulation = true;
                    float meanVelocity = sumOfHitPointsVelocities / hitsCount;
                    Vector2 meanPosition = sumOfHitPointsPositions / hitsCount;
                    Vector3 spawnPosition = new Vector3(meanPosition.x, meanPosition.y, waterPosition.z);

                    if (onCollisionRipplesActivateOnWaterExitParticleEffect)
                        onCollisionRipplesOnWaterExitParticleEffectPool.PlayParticleEffect(spawnPosition + onCollisionRipplesOnWaterExitParticleEffectSpawnOffset);

                    if (onCollisionRipplesActivateOnWaterExitSoundEffect)
                    {
                        float pitch;
                        if (onCollisionRipplesUseConstantOnWaterExitAudioPitch)
                        {
                            pitch = onCollisionRipplesOnWaterExitAudioPitch;
                        }
                        else
                        {
                            float interpolationValue = 1f - Mathf.InverseLerp(onCollisionRipplesMinimumDisturbance, onCollisionRipplesMaximumDisturbance, meanVelocity);
                            pitch = Mathf.Lerp(onCollisionRipplesOnWaterExitMinimumAudioPitch, onCollisionRipplesOnWaterExitMaximumAudioPitch, interpolationValue);
                        }
                        onCollisionRipplesOnWaterExitSoundEffectPool.PlaySoundEffect(spawnPosition, pitch, onCollisionRipplesOnWaterExitAudioVolume);
                    }
                }
            }

            if (OnWaterExit != null)
                OnWaterExit.Invoke();
        }

        private void FixedUpdate()
        {
            if (updateWaterSimulation)
            {
                updateWaterSimulation = false;

                float deltaTime = Time.fixedDeltaTime;
                float dampingFactor = damping * 2f * stiffnessSquareRoot;
                float spreadFactor = spread * subdivisionsCountPerUnit;

                int vertexIndex;
                int startIndex;
                int endIndex;

                if (useCustomBoundaries)
                {
                    startIndex = 1;
                    endIndex = surfaceVerticesCount - 1;
                    vertexIndex = 2;
                }
                else
                {
                    startIndex = 0;
                    endIndex = surfaceVerticesCount;
                    vertexIndex = 0;
                }

                Vector3 currentVertexPosition = vertices[vertexIndex];
                Vector3 previousVertexPosition = currentVertexPosition;
                Vector3 nextVertexPosition;

                for (int i = startIndex; i < endIndex; i++, vertexIndex += 2)
                {
                    nextVertexPosition = i < endIndex - 1 ? vertices[vertexIndex + 2] : currentVertexPosition;

                    float velocity = velocities[i];
                    float restoringForce = stiffness * (waterPositionOfRest - currentVertexPosition.y);
                    float dampingForce = -dampingFactor * velocity;
                    float spreadForce = spreadFactor * (previousVertexPosition.y - currentVertexPosition.y + nextVertexPosition.y - currentVertexPosition.y);

                    previousVertexPosition = currentVertexPosition;

                    velocity += (restoringForce + dampingForce + spreadForce) * deltaTime;
                    currentVertexPosition.y += velocity * deltaTime;

                    velocities[i] = velocity;
                    vertices[vertexIndex] = currentVertexPosition;

                    currentVertexPosition = nextVertexPosition;

                    if (!updateWaterSimulation)
                    {
                        //if all the velocities are in the [-UpdateWaterSimulationThreshold,UpdateWaterSimulationThreshold] range then, we don't need to continue updating the 
                        //water simulation and thus improving performance
                        const float UpdateWaterSimulationThreshold = 0.0001f;
                        updateWaterSimulation |= velocity > UpdateWaterSimulationThreshold || velocity < -UpdateWaterSimulationThreshold;
                    }
                }

                //We apply the changes to the mesh only if the water is visible to a camera
                if (waterIsVisible)
                {
                    mesh.SetVertices(vertices);
                    mesh.UploadMeshData(false);
                    mesh.RecalculateBounds();
                }
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                if (activateConstantRipples)
                    UpdateConstantRipples();

                UpdateParticleEffectPools();
                UpdateSoundEffectPools();
#if UNITY_EDITOR
            }
#endif
            AnimateWaterSize();

            if (transform.hasChanged)
            {
                waterWorldToLocalMatrix = transform.worldToLocalMatrix;
                waterLocalToWorldMatrix = transform.localToWorldMatrix;
                waterPosition = transform.position;
                onCollisionRipplesCollisionRaycastDirection = transform.up;
                transform.hasChanged = false;
            }
        }

        //OnWillRenderObject is called for each camera if the object is visible.
        private void OnWillRenderObject()
        {
#if UNITY_EDITOR
            waterMaterial = meshRenderer.sharedMaterial;
            if (!waterMaterial)
                return;

            renderRefraction = waterMaterial.IsKeywordEnabled("Water2D_Refraction");
            renderReflection = waterMaterial.IsKeywordEnabled("Water2D_Reflection");
#endif

            Camera currentRenderingCamera = Camera.current;

            if (!currentRenderingCamera || !(renderReflection || renderRefraction) || Mathf.Approximately(waterSize.x, 0f) || Mathf.Approximately(waterSize.y, 0f))
                return;

            if (!waterCamera)
            {
                GameObject waterCameraGameObject = new GameObject("Water (Refraction/Reflection) Camera For " + GetInstanceID());
                //we will take care of creating and destroying this camera and we will render this camera manually
                waterCameraGameObject.SetActive(false);
                waterCameraGameObject.hideFlags = HideFlags.HideAndDontSave;
                waterCamera = waterCameraGameObject.AddComponent<Camera>();
                waterCamera.enabled = false;
                waterCamera.clearFlags = CameraClearFlags.SolidColor;
                waterCamera.orthographic = true;
                waterCamera.nearClipPlane = 0.03f;
                waterCamera.farClipPlane = farClipPlane;
                waterCamera.allowHDR = allowHDR;
                waterCamera.allowMSAA = allowMSAA;
            }

            Bounds waterMeshBoundsWorldSpace = meshRenderer.bounds;
            Vector2 waterMeshBoundsWorldSpaceMin, waterMeshBoundsWorldSpaceMax;
            waterMeshBoundsWorldSpaceMin = waterMeshBoundsWorldSpace.min;
            waterMeshBoundsWorldSpaceMax = waterMeshBoundsWorldSpace.max;

            //when the size or the position of the water changes, or the current camera moves , we update the waterCamera rendering settings
            Vector2 waterMeshBoundsScreenSpaceMin = currentRenderingCamera.WorldToScreenPoint(waterMeshBoundsWorldSpaceMin);
            Vector2 waterMeshBoundsScreenSpaceMax = currentRenderingCamera.WorldToScreenPoint(waterMeshBoundsWorldSpaceMax);
            updateWaterCameraRenderingSettings = waterMeshBoundsScreenSpaceMin != lastWaterMeshBoundsScreenSpaceMin || waterMeshBoundsScreenSpaceMax != lastWaterMeshBoundsScreenSpaceMax;

#if UNITY_EDITOR
            //We will always update the waterCamera rendering settings inside the editor
            updateWaterCameraRenderingSettings = true;
#endif

            if (updateWaterCameraRenderingSettings)
            {
                lastWaterMeshBoundsScreenSpaceMin = waterMeshBoundsScreenSpaceMin;
                lastWaterMeshBoundsScreenSpaceMax = waterMeshBoundsScreenSpaceMax;
                updateWaterCameraRenderingSettings = false;

                //Find water/camera bounds intersection polygon
                waterWorldspaceBounds.Clear();
                waterWorldspaceBounds.Add(waterMeshBoundsWorldSpaceMax);
                waterWorldspaceBounds.Add(new Vector2(waterMeshBoundsWorldSpaceMax.x, waterMeshBoundsWorldSpaceMin.y));
                waterWorldspaceBounds.Add(waterMeshBoundsWorldSpaceMin);
                waterWorldspaceBounds.Add(new Vector2(waterMeshBoundsWorldSpaceMin.x, waterMeshBoundsWorldSpaceMax.y));

                currentRenderingCameraWorldSpaceBounds[0] = currentRenderingCamera.ViewportToWorldPoint(new Vector3(1f, 1f));
                currentRenderingCameraWorldSpaceBounds[1] = currentRenderingCamera.ViewportToWorldPoint(new Vector3(1f, 0f));
                currentRenderingCameraWorldSpaceBounds[2] = currentRenderingCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
                currentRenderingCameraWorldSpaceBounds[3] = currentRenderingCamera.ViewportToWorldPoint(new Vector3(0f, 1f));

                List<Vector2> waterMeshBoundsAndCameraBoundsIntersection = FindIntersectedPolygon();

                //compute the water visible area bounds (water mesh bounds/current rendering camera bounds intersection)
                Vector2 intersectedPolygonBoundsMin = waterMeshBoundsWorldSpaceMax;
                Vector2 intersectedPolygonBoundsMax = waterMeshBoundsWorldSpaceMin;

                for (int i = 0, length = waterMeshBoundsAndCameraBoundsIntersection.Count; i < length; i++)
                {
                    Vector2 vertexPosition = waterMeshBoundsAndCameraBoundsIntersection[i];
                    if (vertexPosition.x < intersectedPolygonBoundsMin.x)
                        intersectedPolygonBoundsMin.x = vertexPosition.x;

                    if (vertexPosition.x > intersectedPolygonBoundsMax.x)
                        intersectedPolygonBoundsMax.x = vertexPosition.x;

                    if (vertexPosition.y < intersectedPolygonBoundsMin.y)
                        intersectedPolygonBoundsMin.y = vertexPosition.y;

                    if (vertexPosition.y > intersectedPolygonBoundsMax.y)
                        intersectedPolygonBoundsMax.y = vertexPosition.y;
                }

                float intersectedPolygonBoundsHeight, intersectedPolygonBoundsWidth;
                intersectedPolygonBoundsHeight = intersectedPolygonBoundsMax.y - intersectedPolygonBoundsMin.y;
                intersectedPolygonBoundsWidth = intersectedPolygonBoundsMax.x - intersectedPolygonBoundsMin.x;
                float currentRenderingCameraHeight, currentRenderingCameraWidth;
                currentRenderingCameraHeight = currentRenderingCameraWorldSpaceBounds[0].y - currentRenderingCameraWorldSpaceBounds[1].y;
                currentRenderingCameraWidth = currentRenderingCameraWorldSpaceBounds[1].x - currentRenderingCameraWorldSpaceBounds[2].x;

                //We compute waterCamera's projection matrix, position, rotation and renderTexture size
                float waterCameraFrustrumLeft, waterCameraFrustrumRight, waterCameraFrustrumBottom, waterCameraFrustrumTop;
                Vector3 waterCameraPosition;
                Quaternion waterCameraRotation;
                int renderTextureWidth, renderTextureHeight;

                bool useCurrentRenderingCameraSettings = intersectedPolygonBoundsHeight * intersectedPolygonBoundsWidth > currentRenderingCameraHeight * currentRenderingCameraWidth;

                if (useCurrentRenderingCameraSettings)
                {
                    Matrix4x4 currentRenderingCameraProjectionMatrix = currentRenderingCamera.projectionMatrix;
                    waterCameraFrustrumRight = 1f / currentRenderingCameraProjectionMatrix.m00;
                    waterCameraFrustrumLeft = -waterCameraFrustrumRight;
                    waterCameraFrustrumTop = 1f / currentRenderingCameraProjectionMatrix.m11;
                    waterCameraFrustrumBottom = -waterCameraFrustrumTop;

                    Vector3 currentRenderingCameraPosition = currentRenderingCamera.transform.position;
                    waterCameraPosition = new Vector3(currentRenderingCameraPosition.x, currentRenderingCameraPosition.y, waterPosition.z);
                    waterCameraRotation = currentRenderingCamera.transform.rotation;

                    renderTextureWidth = currentRenderingCamera.pixelWidth;
                    renderTextureHeight = currentRenderingCamera.pixelHeight;
                }
                else
                {
                    waterCameraFrustrumRight = intersectedPolygonBoundsWidth / 2f;
                    waterCameraFrustrumLeft = -waterCameraFrustrumRight;
                    waterCameraFrustrumTop = intersectedPolygonBoundsHeight / 2f;
                    waterCameraFrustrumBottom = -waterCameraFrustrumTop;

                    Vector2 intersectedPolygonBoundsCenterPosition = (intersectedPolygonBoundsMin + intersectedPolygonBoundsMax) / 2f;
                    waterCameraPosition = new Vector3(intersectedPolygonBoundsCenterPosition.x, intersectedPolygonBoundsCenterPosition.y, waterPosition.z);
                    waterCameraRotation = defaultWaterCameraRotation;

                    float screenPixelsPerUnit = currentRenderingCamera.pixelHeight / currentRenderingCameraHeight;
                    renderTextureWidth = Mathf.RoundToInt(intersectedPolygonBoundsWidth * screenPixelsPerUnit);
                    renderTextureHeight = Mathf.RoundToInt(intersectedPolygonBoundsHeight * screenPixelsPerUnit);
                }

                Matrix4x4 waterMatrix = Matrix4x4.Ortho(waterCameraFrustrumLeft, waterCameraFrustrumRight, waterCameraFrustrumBottom, waterCameraFrustrumTop, 0.03f, farClipPlane);
                waterCamera.projectionMatrix = waterMatrix;
                waterCamera.transform.SetPositionAndRotation(waterCameraPosition, waterCameraRotation);
                waterMatrix = waterMatrix * waterCamera.worldToCameraMatrix * waterLocalToWorldMatrix;

                if (materialPropertyBlock == null)
                    materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetMatrix(waterMatrixID, waterMatrix);

                if (renderRefraction)
                {
                    waterCameraPositionForRefractionRendering = waterCameraPosition;
                    waterCameraRotationForRefractionRendering = waterCameraRotation;

                    int refractionTextureWidth = Mathf.RoundToInt(renderTextureWidth * refractionRenderTextureResizeFactor);
                    int refractionTextureHeight = Mathf.RoundToInt(renderTextureHeight * refractionRenderTextureResizeFactor);

                    if (refractionTextureWidth < 1 || refractionTextureHeight < 1)
                        return;

                    if (refractionRenderTexture)
                    {
                        RenderTexture.ReleaseTemporary(refractionRenderTexture);
                    }
                    refractionRenderTexture = RenderTexture.GetTemporary(refractionTextureWidth, refractionTextureHeight, 16);
                    refractionRenderTexture.filterMode = refractionRenderTextureFilterMode;

                    materialPropertyBlock.SetTexture(refractionTextureID, refractionRenderTexture);
                }

                if (renderReflection)
                {
                    waterCameraPositionForReflectionRendering.x = waterCameraPosition.x;
                    waterCameraPositionForReflectionRendering.y = waterLocalToWorldMatrix.m11 * waterSize.y + 2f * waterLocalToWorldMatrix.m13 - waterCameraPosition.y;
                    waterCameraPositionForReflectionRendering.z = waterPosition.z + reflectionZOffset;
                    Vector3 waterCameraRotationEulerAngles = waterCameraRotation.eulerAngles;
                    waterCameraRotationEulerAngles.z *= -1f;
                    waterCameraRotationForReflectionRendering.eulerAngles = waterCameraRotationEulerAngles;

                    int reflectionTextureWidth = Mathf.RoundToInt(renderTextureWidth * reflectionRenderTextureResizeFactor);
                    int reflectionTextureHeight = Mathf.RoundToInt(renderTextureHeight * reflectionRenderTextureResizeFactor);

                    if (reflectionTextureWidth < 1 || reflectionTextureHeight < 1)
                        return;

                    if (reflectionRenderTexture)
                    {
                        RenderTexture.ReleaseTemporary(reflectionRenderTexture);
                    }
                    reflectionRenderTexture = RenderTexture.GetTemporary(reflectionTextureWidth, reflectionTextureHeight, 16);
                    reflectionRenderTexture.filterMode = reflectionRenderTextureFilterMode;

                    materialPropertyBlock.SetTexture(reflectionTextureID, reflectionRenderTexture);
                    materialPropertyBlock.SetFloat(waterReflectionLowerLimitID, waterSize.y / 2f);
                }

                meshRenderer.SetPropertyBlock(materialPropertyBlock);
            }

            int pixelLightsCount = QualitySettings.pixelLightCount;
            if (!renderPixelLights)
            {
                QualitySettings.pixelLightCount = 0;
            }

            Color waterCameraBackgroundColor = currentRenderingCamera.backgroundColor;
            //We make sure the waterCamera's backgroundColor alpha value is set to 0 to correctly blend the reflectionRenderTexture and the refractionRenderTexture when rendering both the refraction and the reflection.
            waterCameraBackgroundColor.a = 0f;
            waterCamera.backgroundColor = waterCameraBackgroundColor;

            if (renderRefraction)
            {
                waterCamera.transform.SetPositionAndRotation(waterCameraPositionForRefractionRendering, waterCameraRotationForRefractionRendering);
                waterCamera.cullingMask = refractionCullingMask;
                waterCamera.targetTexture = refractionRenderTexture;
                waterCamera.Render();
            }

            if (renderReflection)
            {
                waterCamera.transform.SetPositionAndRotation(waterCameraPositionForReflectionRendering, waterCameraRotationForReflectionRendering);
                waterCamera.cullingMask = reflectionCullingMask;
                waterCamera.targetTexture = reflectionRenderTexture;
                waterCamera.Render();
            }

            QualitySettings.pixelLightCount = pixelLightsCount;
        }

        private void RecomputeMesh()
        {
            if (!mesh || !meshFilter.sharedMesh)
            {
                mesh = new Mesh();
                mesh.MarkDynamic();
                mesh.hideFlags = HideFlags.HideAndDontSave;
                mesh.name = "Water2D Mesh";
                meshFilter.sharedMesh = mesh;
            }

            float halfWidth = waterSize.x / 2f;
            float halfHeight = waterSize.y / 2f;

            float activeWaterSurfaceWidth;
            float xStep;
            float leftmostBoundary;
            float uStep;
            float leftmostBoundaryU;

            if (useCustomBoundaries)
            {
                activeWaterSurfaceWidth = Mathf.Abs(secondCustomBoundary - firstCustomBoundary);
                surfaceVerticesCount = Mathf.RoundToInt(activeWaterSurfaceWidth * subdivisionsCountPerUnit) + 4;
                xStep = activeWaterSurfaceWidth / (surfaceVerticesCount - 3);
                leftmostBoundary = Mathf.Min(secondCustomBoundary, firstCustomBoundary);
                uStep = (activeWaterSurfaceWidth / waterSize.x) / (surfaceVerticesCount - 3);
                leftmostBoundaryU = (leftmostBoundary + halfWidth) / waterSize.x;
            }
            else
            {
                activeWaterSurfaceWidth = 2f * halfWidth;
                surfaceVerticesCount = Mathf.RoundToInt(waterSize.x * subdivisionsCountPerUnit) + 2;
                xStep = activeWaterSurfaceWidth / (surfaceVerticesCount - 1);
                leftmostBoundary = -halfWidth + xStep;
                uStep = 1f / (surfaceVerticesCount - 1);
                leftmostBoundaryU = uStep;
            }

            vertices = new List<Vector3>(surfaceVerticesCount * 2);
            uvs = new List<Vector2>(surfaceVerticesCount * 2);
            triangles = new List<int>((surfaceVerticesCount - 1) * 6);
            velocities = new float[surfaceVerticesCount];

            vertices.Add(new Vector3(-halfWidth, halfHeight));
            vertices.Add(new Vector3(-halfWidth, -halfHeight));

            uvs.Add(new Vector2(0f, 1f));
            uvs.Add(new Vector2(0f, 0f));

            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(3);
            triangles.Add(0);
            triangles.Add(3);
            triangles.Add(1);

            float xPosition = 0f;
            float uPosition = 0f;
            for (int i = 1, index = 2, max = surfaceVerticesCount - 1; i < max; i++, index += 2)
            {
                float x = xPosition + leftmostBoundary;
                xPosition += xStep;
                vertices.Add(new Vector3(x, halfHeight));
                vertices.Add(new Vector3(x, -halfHeight));

                float u = uPosition + leftmostBoundaryU;
                uPosition += uStep;
                uvs.Add(new Vector2(u, 1f));
                uvs.Add(new Vector2(u, 0f));

                triangles.Add(index);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
                triangles.Add(index);
                triangles.Add(index + 3);
                triangles.Add(index + 1);
            }

            vertices.Add(new Vector3(halfWidth, halfHeight));
            vertices.Add(new Vector3(halfWidth, -halfHeight));

            uvs.Add(new Vector2(1f, 1f));
            uvs.Add(new Vector2(1f, 0f));

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            UpdateComponents();
            waterPositionOfRest = halfHeight;
            lastWaterSize = waterSize;
            if (useCustomBoundaries)
            {
                lastFirstCustomBoundary = firstCustomBoundary;
                lastSecondCustomBoundary = secondCustomBoundary;
            }
        }

        private void UpdateComponents()
        {
            float halfWidth = waterSize.x / 2f;
            float halfHeight = waterSize.y / 2f;
            if (boxCollider != null)
                boxCollider.size = waterSize;
            if (edgeCollider != null)
            {
                edgeColliderPoints[0].x = edgeColliderPoints[1].x = -halfWidth;
                edgeColliderPoints[2].x = edgeColliderPoints[3].x = halfWidth;

                edgeColliderPoints[0].y = edgeColliderPoints[3].y = halfHeight;
                edgeColliderPoints[1].y = edgeColliderPoints[2].y = -halfHeight;

                edgeCollider.points = edgeColliderPoints;
            }
            if (buoyancyEffector != null)
                buoyancyEffector.surfaceLevel = waterSize.y * (0.5f - buoyancyEffectorSurfaceLevel);
        }

        private void UpdateParticleEffectPools()
        {
            if (activateOnCollisionOnWaterEnterRipples && onCollisionRipplesActivateOnWaterEnterParticleEffect)
            {
                if (onCollisionRipplesOnWaterEnterParticleEffectPool == null)
                    onCollisionRipplesOnWaterEnterParticleEffectPool = new ParticleEffectPool(GetInstanceID(), onCollisionRipplesOnWaterEnterParticleEffect, onCollisionRipplesOnWaterEnterParticleEffectPoolSize, onCollisionRipplesOnWaterEnterParticleEffectStopAction, onCollisionRipplesOnWaterEnterParticleEffectPoolExpandIfNecessary);

                if (onCollisionRipplesReconstructOnWaterEnterParticleEffectPool)
                {
                    onCollisionRipplesOnWaterEnterParticleEffectPool.ReconstructPool(onCollisionRipplesOnWaterEnterParticleEffect, onCollisionRipplesOnWaterEnterParticleEffectPoolSize);
                    onCollisionRipplesReconstructOnWaterEnterParticleEffectPool = false;
                }

                onCollisionRipplesOnWaterEnterParticleEffectPool.UpdatePool();
            }

            if (activateOnCollisionOnWaterExitRipples && onCollisionRipplesActivateOnWaterExitParticleEffect)
            {
                if (onCollisionRipplesOnWaterExitParticleEffectPool == null)
                    onCollisionRipplesOnWaterExitParticleEffectPool = new ParticleEffectPool(GetInstanceID(), onCollisionRipplesOnWaterExitParticleEffect, onCollisionRipplesOnWaterExitParticleEffectPoolSize, onCollisionRipplesOnWaterExitParticleEffectStopAction, onCollisionRipplesOnWaterExitParticleEffectPoolExpandIfNecessary);

                if (onCollisionRipplesReconstructOnWaterExitParticleEffectPool)
                {
                    onCollisionRipplesOnWaterExitParticleEffectPool.ReconstructPool(onCollisionRipplesOnWaterExitParticleEffect, onCollisionRipplesOnWaterExitParticleEffectPoolSize);
                    onCollisionRipplesReconstructOnWaterExitParticleEffectPool = false;
                }

                onCollisionRipplesOnWaterExitParticleEffectPool.UpdatePool();
            }

            if (constantRipplesActivateParticleEffect)
            {
                if (constantRipplesParticleEffectPool == null)
                    constantRipplesParticleEffectPool = new ParticleEffectPool(GetInstanceID(), constantRipplesParticleEffect, constantRipplesParticleEffectPoolSize, constantRipplesParticleEffectStopAction, constantRipplesParticleEffectPoolExpandIfNecessary);

                if (constantRipplesReconstructParticleEffectPool)
                {
                    constantRipplesParticleEffectPool.ReconstructPool(constantRipplesParticleEffect, constantRipplesParticleEffectPoolSize);
                    constantRipplesReconstructParticleEffectPool = false;
                }

                constantRipplesParticleEffectPool.UpdatePool();
            }

            if (scriptGeneratedRipplesActivateParticleEffect)
            {
                if (scriptGeneratedRipplesParticleEffectPool == null)
                    scriptGeneratedRipplesParticleEffectPool = new ParticleEffectPool(GetInstanceID(), scriptGeneratedRipplesParticleEffect, scriptGeneratedRipplesParticleEffectPoolSize, scriptGeneratedRipplesParticleEffectStopAction, scriptGeneratedRipplesParticleEffectPoolExpandIfNecessary);

                if (scriptGeneratedRipplesReconstructParticleEffectPool)
                {
                    scriptGeneratedRipplesParticleEffectPool.ReconstructPool(scriptGeneratedRipplesParticleEffect, scriptGeneratedRipplesParticleEffectPoolSize);
                    scriptGeneratedRipplesReconstructParticleEffectPool = false;
                }

                scriptGeneratedRipplesParticleEffectPool.UpdatePool();
            }
        }

        private void UpdateSoundEffectPools()
        {
            if (activateOnCollisionOnWaterEnterRipples && onCollisionRipplesActivateOnWaterEnterSoundEffect)
            {
                if (onCollisionRipplesOnWaterEnterSoundEffectPool == null)
                    onCollisionRipplesOnWaterEnterSoundEffectPool = new SoundEffectPool(GetInstanceID(), onCollisionRipplesOnWaterEnterAudioClip, onCollisionRipplesOnWaterEnterSoundEffectPoolSize, onCollisionRipplesOnWaterEnterSoundEffectPoolExpandIfNecessary);

                if (onCollisionRipplesReconstructOnWaterEnterSoundEffectPool)
                {
                    onCollisionRipplesOnWaterEnterSoundEffectPool.ReconstructPool(onCollisionRipplesOnWaterEnterAudioClip, onCollisionRipplesOnWaterEnterSoundEffectPoolSize);
                    onCollisionRipplesReconstructOnWaterEnterSoundEffectPool = false;
                }

                onCollisionRipplesOnWaterEnterSoundEffectPool.UpdatePool();
            }

            if (activateOnCollisionOnWaterExitRipples && onCollisionRipplesActivateOnWaterExitSoundEffect)
            {
                if (onCollisionRipplesOnWaterExitSoundEffectPool == null)
                    onCollisionRipplesOnWaterExitSoundEffectPool = new SoundEffectPool(GetInstanceID(), onCollisionRipplesOnWaterExitAudioClip, onCollisionRipplesOnWaterExitSoundEffectPoolSize, onCollisionRipplesOnWaterExitSoundEffectPoolExpandIfNecessary);

                if (onCollisionRipplesReconstructOnWaterExitSoundEffectPool)
                {
                    onCollisionRipplesOnWaterExitSoundEffectPool.ReconstructPool(onCollisionRipplesOnWaterExitAudioClip, onCollisionRipplesOnWaterExitSoundEffectPoolSize);
                    onCollisionRipplesReconstructOnWaterExitSoundEffectPool = false;
                }

                onCollisionRipplesOnWaterExitSoundEffectPool.UpdatePool();
            }

            if (constantRipplesActivateSoundEffect)
            {
                if (constantRipplesSoundEffectPool == null)
                    constantRipplesSoundEffectPool = new SoundEffectPool(GetInstanceID(), constantRipplesAudioClip, constantRipplesSoundEffectPoolSize, constantRipplesSoundEffectPoolExpandIfNecessary);

                if (constantRipplesReconstructSoundEffectPool)
                {
                    constantRipplesSoundEffectPool.ReconstructPool(constantRipplesAudioClip, constantRipplesSoundEffectPoolSize);
                    constantRipplesReconstructSoundEffectPool = false;
                }

                constantRipplesSoundEffectPool.UpdatePool();
            }

            if (scriptGeneratedRipplesActivateSoundEffect)
            {
                if (scriptGeneratedRipplesSoundEffectPool == null)
                    scriptGeneratedRipplesSoundEffectPool = new SoundEffectPool(GetInstanceID(), scriptGeneratedRipplesAudioClip, scriptGeneratedRipplesSoundEffectPoolSize, scriptGeneratedRipplesSoundEffectPoolExpandIfNecessary);

                if (scriptGeneratedRipplesReconstructSoundEffectPool)
                {
                    scriptGeneratedRipplesSoundEffectPool.ReconstructPool(scriptGeneratedRipplesAudioClip, scriptGeneratedRipplesSoundEffectPoolSize);
                    scriptGeneratedRipplesReconstructSoundEffectPool = false;
                }

                scriptGeneratedRipplesSoundEffectPool.UpdatePool();
            }
        }

        private void UpdateConstantRipplesSourceIndices()
        {
            if (constantRipplesSourcesIndices != null)
                constantRipplesSourcesIndices.Clear();
            else
                constantRipplesSourcesIndices = new List<int>();

            if (constantRipplesRandomizeRipplesSourcesPositions)
            {
                int startIndex, endIndex;
                if (useCustomBoundaries)
                {
                    startIndex = 1;
                    endIndex = surfaceVerticesCount - 1;
                }
                else
                {
                    startIndex = 0;
                    endIndex = surfaceVerticesCount;
                }
                for (int i = 0; i < constantRipplesRandomizeRipplesSourcesCount; i++)
                {
                    constantRipplesSourcesIndices.Add(Random.Range(startIndex, endIndex));
                }
            }
            else
            {
                float halfWidth = waterSize.x / 2f;

                float xStep, leftmostBoundary, rightmostBoundary;
                int indexOffset;

                if (useCustomBoundaries)
                {
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
                }
                else
                {
                    xStep = 2f * halfWidth / (surfaceVerticesCount - 1);
                    leftmostBoundary = -halfWidth;
                    rightmostBoundary = halfWidth;
                    indexOffset = 0;
                }

                for (int i = 0, maxi = constantRipplesSourcePositions.Count; i < maxi; i++)
                {
                    float xPosition = constantRipplesSourcePositions[i];
                    if (xPosition < leftmostBoundary || xPosition > rightmostBoundary)
                        continue;
                    int nearestIndex = Mathf.RoundToInt((xPosition - leftmostBoundary) / xStep) + indexOffset;
                    if (!constantRipplesAllowDuplicateRipplesSourcesPositions)
                    {
                        bool isDuplicate = false;
                        for (int j = 0, maxj = constantRipplesSourcesIndices.Count; j < maxj; j++)
                        {
                            if (constantRipplesSourcesIndices[j] == nearestIndex)
                            {
                                isDuplicate = true;
                                break;
                            }
                        }
                        if (isDuplicate)
                            continue;
                    }
                    constantRipplesSourcesIndices.Add(nearestIndex);
                }
                constantRipplesUpdateSourcesIndices = false;
            }
        }

        private void UpdateConstantRipples()
        {
            if (!constantRipplesUpdateWhenOffscreen && !waterIsVisible)
                return;

            constantRipplesDeltaTime += Time.deltaTime;

            if (constantRipplesDeltaTime >= constantRipplesCurrentInterval)
            {
                if (constantRipplesRandomizeRipplesSourcesPositions || (!constantRipplesRandomizeRipplesSourcesPositions && constantRipplesUpdateSourcesIndices))
                    UpdateConstantRipplesSourceIndices();

                int startIndex, endIndex;
                if (useCustomBoundaries)
                {
                    startIndex = 1;
                    endIndex = surfaceVerticesCount - 1;
                }
                else
                {
                    startIndex = 0;
                    endIndex = surfaceVerticesCount;
                }

                for (int i = 0, max = constantRipplesSourcesIndices.Count; i < max; i++)
                {
                    int index = constantRipplesSourcesIndices[i];

                    float disturbance = constantRipplesRandomizeDisturbance ?
                Random.Range(constantRipplesMinimumDisturbance, constantRipplesMaximumDisturbance) :
                constantRipplesDisturbance;
                    disturbance *= stiffnessSquareRoot;

                    velocities[index] -= disturbance;

                    if (constantRipplesSmoothDisturbance)
                    {
                        float smoothedDisturbance = disturbance * constantRipplesSmoothFactor;
                        int previousIndex, nextIndex;
                        previousIndex = index - 1;
                        nextIndex = index + 1;

                        if (previousIndex >= startIndex)
                            velocities[previousIndex] -= smoothedDisturbance;
                        if (nextIndex < endIndex)
                            velocities[nextIndex] -= smoothedDisturbance;
                    }

                    Vector3 vertexPosition = vertices[index * 2];
                    Vector3 spawnPosition;
                    spawnPosition.x = waterLocalToWorldMatrix.m00 * vertexPosition.x + waterLocalToWorldMatrix.m01 * vertexPosition.y + waterLocalToWorldMatrix.m03;
                    spawnPosition.y = waterLocalToWorldMatrix.m10 * vertexPosition.x + waterLocalToWorldMatrix.m11 * vertexPosition.y + waterLocalToWorldMatrix.m13;
                    spawnPosition.z = waterPosition.z;

                    if (constantRipplesActivateParticleEffect)
                        constantRipplesParticleEffectPool.PlayParticleEffect(spawnPosition + constantRipplesParticleEffectSpawnOffset);

                    if (constantRipplesActivateSoundEffect)
                    {
                        float pitch;
                        if (constantRipplesUseConstantAudioPitch)
                            pitch = constantRipplesAudioPitch;
                        else
                        {
                            float disturbanceFactor = Mathf.InverseLerp(constantRipplesMinimumDisturbance, constantRipplesMaximumDisturbance, disturbance);
                            pitch = Mathf.Lerp(constantRipplesMinimumAudioPitch, constantRipplesMaximumAudioPitch, 1f - disturbanceFactor);
                        }

                        constantRipplesSoundEffectPool.PlaySoundEffect(spawnPosition, pitch, constantRipplesAudioVolume);
                    }
                }

                constantRipplesCurrentInterval = constantRipplesRandomizeInterval ?
                 Random.Range(constantRipplesMinimumInterval, constantRipplesMaximumInterval) :
                 constantRipplesInterval;

                constantRipplesDeltaTime = 0f;
                updateWaterSimulation = true;
            }
        }

        private void AnimateWaterSize()
        {
            if (!waterIsVisible)
                return;

            if (waterSize != lastWaterSize
             || (useCustomBoundaries
             && (!Mathf.Approximately(firstCustomBoundary, lastFirstCustomBoundary)
             || !Mathf.Approximately(secondCustomBoundary, lastSecondCustomBoundary))))
            {
                float halfWidth = waterSize.x / 2f;
                float halfDeltaHeight = (waterSize.y - lastWaterSize.y) / 2f;
                float xStep;
                float leftmostBoundary;

                if (useCustomBoundaries)
                {
                    xStep = Mathf.Abs(firstCustomBoundary - secondCustomBoundary) / (surfaceVerticesCount - 3);
                    leftmostBoundary = Mathf.Min(firstCustomBoundary, secondCustomBoundary);

                    lastFirstCustomBoundary = firstCustomBoundary;
                    lastSecondCustomBoundary = secondCustomBoundary;
                }
                else
                {
                    xStep = waterSize.x / (surfaceVerticesCount - 1);
                    leftmostBoundary = -waterSize.x / 2f + xStep;
                }

                Vector3 vertexTop = vertices[0];
                Vector3 vertexBottom = vertices[1];

                vertexTop.x = vertexBottom.x = -halfWidth;
                vertexTop.y += halfDeltaHeight;
                vertexBottom.y -= halfDeltaHeight;
                vertices[0] = vertexTop;
                vertices[1] = vertexBottom;

                float xPosition = 0f;
                for (int i = 1, vertexIndex = 2; i < surfaceVerticesCount - 1; i++, vertexIndex += 2)
                {
                    vertexTop = vertices[vertexIndex];
                    vertexBottom = vertices[vertexIndex + 1];

                    float x = xPosition + leftmostBoundary;
                    xPosition += xStep;

                    vertexTop.x = vertexBottom.x = x;
                    vertexTop.y += halfDeltaHeight;
                    vertexBottom.y -= halfDeltaHeight;
                    vertices[vertexIndex] = vertexTop;
                    vertices[vertexIndex + 1] = vertexBottom;
                }

                int lastVertexIndex = (surfaceVerticesCount - 1) * 2;
                vertexTop = vertices[lastVertexIndex];
                vertexBottom = vertices[lastVertexIndex + 1];

                vertexTop.x = vertexBottom.x = halfWidth;
                vertexTop.y += halfDeltaHeight;
                vertexBottom.y -= halfDeltaHeight;
                vertices[lastVertexIndex] = vertexTop;
                vertices[lastVertexIndex + 1] = vertexBottom;

                mesh.SetVertices(vertices);
                mesh.UploadMeshData(false);
                mesh.RecalculateBounds();

                UpdateComponents();
                lastWaterSize = waterSize;
                waterPositionOfRest = halfWidth;
            }
        }

        private List<Vector2> FindIntersectedPolygon()
        {
            //We use Sutherland–Hodgman algorithm to find the intersected polygon

            clippingInputOutput[0] = waterWorldspaceBounds;
            List<Vector2> inputList = clippingInputOutput[0];
            List<Vector2> outputList = clippingInputOutput[1];

            Vector2 previousClippingPolygonVertex = currentRenderingCameraWorldSpaceBounds[3];
            for (int i = 0; i < 4; i++)
            {
                outputList.Clear();
                Vector2 currentClippingPolygonVertex = currentRenderingCameraWorldSpaceBounds[i];
                int inputListCount = inputList.Count;
                if (inputListCount == 0)
                    break;
                Vector2 previousSubjectPolygonVertex = inputList[inputListCount - 1];
                bool isPreviousSubjectPolygonVertexInsideClipEdge = IsVertexInsideClipEdge(previousSubjectPolygonVertex, previousClippingPolygonVertex, currentClippingPolygonVertex);
                for (int j = 0; j < inputListCount; j++)
                {
                    Vector2 currentSubjectPoygonVertex = inputList[j];
                    bool isCurrentSubjectPolygonVertexInsideClipEdge = IsVertexInsideClipEdge(currentSubjectPoygonVertex, previousClippingPolygonVertex, currentClippingPolygonVertex);
                    if (isCurrentSubjectPolygonVertexInsideClipEdge)
                    {
                        if (!isPreviousSubjectPolygonVertexInsideClipEdge)
                        {
                            Vector2? intersectionPoint = FindIntersectionPoint(previousSubjectPolygonVertex, currentSubjectPoygonVertex, previousClippingPolygonVertex, currentClippingPolygonVertex);
                            if (intersectionPoint != null)
                            {
                                outputList.Add(intersectionPoint.Value);
                            }
                        }
                        outputList.Add(currentSubjectPoygonVertex);
                    }
                    else
                    {
                        if (isPreviousSubjectPolygonVertexInsideClipEdge)
                        {
                            Vector2? intersectionPoint = FindIntersectionPoint(previousSubjectPolygonVertex, currentSubjectPoygonVertex, previousClippingPolygonVertex, currentClippingPolygonVertex);
                            if (intersectionPoint != null)
                            {
                                outputList.Add(intersectionPoint.Value);
                            }
                        }
                    }
                    previousSubjectPolygonVertex = currentSubjectPoygonVertex;
                    isPreviousSubjectPolygonVertexInsideClipEdge = isCurrentSubjectPolygonVertexInsideClipEdge;
                }
                previousClippingPolygonVertex = currentClippingPolygonVertex;
                //Swap input output lists
                List<Vector2> temp = inputList;
                inputList = outputList;
                outputList = temp;
            }

            return clippingInputOutput[0];
        }

        private static Vector2? FindIntersectionPoint(Vector2 subjectEdgeStart, Vector2 subjectEdgeEnd, Vector2 clipEdgeStart, Vector2 clipEdgeEnd)
        {
            Vector2 subjectEdgeEndStart = subjectEdgeEnd - subjectEdgeStart;
            Vector2 clipEdgeEndStart = clipEdgeEnd - clipEdgeStart;
            float dotProduct = (subjectEdgeEndStart.x * clipEdgeEndStart.y) - (subjectEdgeEndStart.y * clipEdgeEndStart.x);
            //There is no intersection point if the dot product is (approximately) 0
            if (System.Math.Abs(dotProduct) <= .000000001f)
                return null;
            Vector2 subjectEdgeStartClipEdgeStart = clipEdgeStart - subjectEdgeStart;
            float delta = (subjectEdgeStartClipEdgeStart.x * clipEdgeEndStart.y - subjectEdgeStartClipEdgeStart.y * clipEdgeEndStart.x) / dotProduct;
            Vector2 intersectionPoint = subjectEdgeStart + delta * subjectEdgeEndStart;
            return intersectionPoint;
        }

        private static bool IsVertexInsideClipEdge(Vector2 vertex, Vector2 clipEdgeStart, Vector2 clipEdgeEnd)
        {
            return (clipEdgeEnd.x - clipEdgeStart.x) * (vertex.y - clipEdgeStart.y) - (vertex.x - clipEdgeStart.x) * (clipEdgeEnd.y - clipEdgeStart.y) <= 0f;
        }

        /// <summary>
        /// Generate a ripple at a particular position.
        /// </summary>
        /// <param name="position">Ripple position.</param>
        /// <param name="disturbanceFactor">Range: [-1..1]: The disturbance is linearly interpolated between the minimum disturbance and the maximum disturbance by the absolute value of this factor.</param>
        /// <param name="playSoundEffect">Play the sound effect.</param>
        /// <param name="playParticleEffect">Play the particle effect.</param>
        /// <param name="smooth">Disturb neighbor vertices to create a smoother ripple (wave).</param>
        /// <param name="smoothingFactor">Range: [0..1]: The amount of disturbance to apply to neighbor vertices.</param>
        public void GenerateRipple(Vector2 position, float disturbanceFactor, bool playSoundEffect, bool playParticleEffect, bool smooth, float smoothingFactor = 0.5f)
        {
            float xPosition = waterWorldToLocalMatrix.m00 * position.x + waterWorldToLocalMatrix.m01 * position.y + waterWorldToLocalMatrix.m03;

            Vector2 halfWaterSize = waterSize / 2f;

            float leftMostWaterVertexX, rightMostWaterVertexX;
            int startIndex, endIndex;
            if (!useCustomBoundaries)
            {
                leftMostWaterVertexX = -halfWaterSize.x;
                rightMostWaterVertexX = halfWaterSize.x;
                startIndex = 0;
                endIndex = velocities.Length - 1;
            }
            else
            {
                if (firstCustomBoundary < secondCustomBoundary)
                {
                    leftMostWaterVertexX = firstCustomBoundary;
                    rightMostWaterVertexX = secondCustomBoundary;
                }
                else
                {
                    leftMostWaterVertexX = secondCustomBoundary;
                    rightMostWaterVertexX = firstCustomBoundary;
                }
                startIndex = 1;
                endIndex = velocities.Length - 2;
            }

            if (xPosition < leftMostWaterVertexX || xPosition > rightMostWaterVertexX)
                return;

            float disturbanceFactorSign = disturbanceFactor > 0f ? 1f : -1f;
            disturbanceFactor = Mathf.Clamp01(Mathf.Abs(disturbanceFactor));
            float velocity = disturbanceFactorSign * stiffnessSquareRoot * Mathf.Lerp(scriptGeneratedRipplesMinimumDisturbance, scriptGeneratedRipplesMaximumDisturbance, disturbanceFactor);
            int indexOffset = useCustomBoundaries ? 1 : 0;
            float delta = (xPosition - leftMostWaterVertexX) * subdivisionsCountPerUnit;

            int index = indexOffset + Mathf.RoundToInt(delta);
            velocities[index] -= velocity;
            if (smooth)
            {
                smoothingFactor = Mathf.Clamp01(smoothingFactor);
                float smoothedVelocity = velocity * smoothingFactor;

                int previousNearestIndex = index - 1;
                if (previousNearestIndex >= startIndex)
                    velocities[previousNearestIndex] -= smoothedVelocity;

                int nextNearestIndex = index + 1;
                if (nextNearestIndex <= endIndex)
                    velocities[nextNearestIndex] -= smoothedVelocity;
            }

            Vector3 spawnPosition;
            spawnPosition.x = waterLocalToWorldMatrix.m00 * xPosition + waterLocalToWorldMatrix.m01 * halfWaterSize.y + waterLocalToWorldMatrix.m03;
            spawnPosition.y = waterLocalToWorldMatrix.m10 * xPosition + waterLocalToWorldMatrix.m11 * halfWaterSize.y + waterLocalToWorldMatrix.m13;
            spawnPosition.z = waterPosition.z;

            if (playParticleEffect && scriptGeneratedRipplesActivateParticleEffect)
                scriptGeneratedRipplesParticleEffectPool.PlayParticleEffect(spawnPosition + scriptGeneratedRipplesParticleEffectSpawnOffset);

            if (playSoundEffect && scriptGeneratedRipplesActivateSoundEffect)
            {
                float pitch;
                if (scriptGeneratedRipplesUseConstantAudioPitch)
                    pitch = scriptGeneratedRipplesAudioPitch;
                else
                    pitch = Mathf.Lerp(scriptGeneratedRipplesMinimumAudioPitch, scriptGeneratedRipplesMaximumAudioPitch, 1f - disturbanceFactor);

                scriptGeneratedRipplesSoundEffectPool.PlaySoundEffect(spawnPosition, pitch, scriptGeneratedRipplesAudioVolume);
            }

            updateWaterSimulation = true;
        }

        [System.Obsolete("Use GenerateRipple instead")]
        public void CreateSplash(float xPosition, float disturbanceFactor, bool playSound, bool playParticleEffect, bool smooth, float smoothingFactor = 0.5f)
        {
            GenerateRipple(new Vector2(xPosition, 0f), disturbanceFactor, playSound, playParticleEffect, smooth, smoothingFactor);
        }
        #endregion
    }
}
