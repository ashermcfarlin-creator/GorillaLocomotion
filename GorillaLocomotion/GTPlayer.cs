// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.GTPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using AA;
using BoingKit;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

#nullable disable
namespace GorillaLocomotion;

public class GTPlayer : MonoBehaviour
{
  private static GTPlayer _instance;
  public static bool hasInstance;
  public Camera mainCamera;
  public SphereCollider headCollider;
  public CapsuleCollider bodyCollider;
  private float bodyInitialRadius;
  private float bodyInitialHeight;
  private UnityEngine.RaycastHit bodyHitInfo;
  private UnityEngine.RaycastHit lastHitInfoHand;
  public GorillaVelocityTracker bodyVelocityTracker;
  public PlayerAudioManager audioManager;
  [SerializeField]
  private GTPlayer.HandState leftHand;
  [SerializeField]
  private GTPlayer.HandState rightHand;
  private GTPlayer.HandState[] stiltStates = new GTPlayer.HandState[12];
  private bool anyHandIsColliding;
  private bool anyHandWasColliding;
  private bool anyHandIsSliding;
  private bool anyHandWasSliding;
  private bool anyHandIsSticking;
  private bool anyHandWasSticking;
  private bool forceRBSync;
  public Vector3 lastHeadPosition;
  private Vector3 lastRigidbodyPosition;
  private Rigidbody playerRigidBody;
  private RigidbodyInterpolation playerRigidbodyInterpolationDefault;
  public int velocityHistorySize;
  public float maxArmLength = 1f;
  public float unStickDistance = 1f;
  public float velocityLimit;
  public float slideVelocityLimit;
  public float maxJumpSpeed;
  private float _jumpMultiplier;
  public float minimumRaycastDistance = 0.05f;
  public float defaultSlideFactor = 0.03f;
  public float slidingMinimum = 0.9f;
  public float defaultPrecision = 0.995f;
  public float teleportThresholdNoVel = 1f;
  public float frictionConstant = 1f;
  public float slideControl = 0.00425f;
  public float stickDepth = 0.01f;
  private Vector3[] velocityHistory;
  private Vector3[] slideAverageHistory;
  private int velocityIndex;
  private Vector3 currentVelocity;
  private Vector3 averagedVelocity;
  private Vector3 lastPosition;
  public Vector3 bodyOffset;
  public LayerMask locomotionEnabledLayers;
  public LayerMask waterLayer;
  public bool wasHeadTouching;
  public int currentMaterialIndex;
  public Vector3 headSlideNormal;
  public float headSlipPercentage;
  [SerializeField]
  private Transform cosmeticsHeadTarget;
  [SerializeField]
  private float nativeScale = 1f;
  [SerializeField]
  private float scaleMultiplier = 1f;
  private NativeSizeChangerSettings activeSizeChangerSettings;
  public bool debugMovement;
  public bool disableMovement;
  [NonSerialized]
  public bool inOverlay;
  [NonSerialized]
  public bool isUserPresent;
  public GameObject turnParent;
  public GorillaSurfaceOverride currentOverride;
  public MaterialDatasSO materialDatasSO;
  private float degreesTurnedThisFrame;
  private Vector3 bodyOffsetVector;
  private Vector3 movementToProjectedAboveCollisionPlane;
  private MeshCollider meshCollider;
  private UnityEngine.Mesh collidedMesh;
  private GTPlayer.MaterialData foundMatData;
  private string findMatName;
  private int vertex1;
  private int vertex2;
  private int vertex3;
  private List<int> trianglesList = new List<int>(1000000);
  private Dictionary<UnityEngine.Mesh, int[]> meshTrianglesDict = new Dictionary<UnityEngine.Mesh, int[]>(128 /*0x80*/);
  private int[] sharedMeshTris;
  private float lastRealTime;
  private float calcDeltaTime;
  private float tempRealTime;
  private Vector3 slideVelocity;
  private Vector3 slideAverageNormal;
  private UnityEngine.RaycastHit tempHitInfo;
  private UnityEngine.RaycastHit junkHit;
  private Vector3 firstPosition;
  private UnityEngine.RaycastHit tempIterativeHit;
  private float maxSphereSize1;
  private float maxSphereSize2;
  private Collider[] overlapColliders = new Collider[10];
  private int overlapAttempts;
  private float averageSlipPercentage;
  private Vector3 surfaceDirection;
  public float iceThreshold = 0.9f;
  private float bodyMaxRadius;
  public float bodyLerp = 0.17f;
  private bool areBothTouching;
  private float slideFactor;
  [DebugOption]
  public bool didAJump;
  private bool updateRB;
  private Renderer slideRenderer;
  private UnityEngine.RaycastHit[] rayCastNonAllocColliders;
  private Vector3[] crazyCheckVectors;
  private UnityEngine.RaycastHit emptyHit;
  private int bufferCount;
  private Vector3 lastOpenHeadPosition;
  private List<Material> tempMaterialArray = new List<Material>(16 /*0x10*/);
  private Vector3? antiDriftLastPosition;
  private const float CameraFarClipDefault = 500f;
  private const float CameraNearClipDefault = 0.01f;
  private const float CameraNearClipTiny = 0.002f;
  private Dictionary<GameObject, PhysicsMaterial> bodyTouchedSurfaces;
  private bool primaryButtonPressed = true;
  [Header("Swimming")]
  public PlayerSwimmingParameters swimmingParams;
  public WaterParameters waterParams;
  public List<GTPlayer.LiquidProperties> liquidPropertiesList = new List<GTPlayer.LiquidProperties>(16 /*0x10*/);
  public bool debugDrawSwimming;
  [Header("Slam/Hit effects")]
  public GameObject wizardStaffSlamEffects;
  public GameObject geodeHitEffects;
  [Header("Freeze Tag")]
  public float freezeTagHandSlidePercent = 0.88f;
  public bool debugFreezeTag;
  public float frozenBodyBuoyancyFactor = 1.5f;
  [Space]
  private WaterVolume leftHandWaterVolume;
  private WaterVolume rightHandWaterVolume;
  private WaterVolume.SurfaceQuery leftHandWaterSurface;
  private WaterVolume.SurfaceQuery rightHandWaterSurface;
  private Vector3 swimmingVelocity = Vector3.zero;
  private WaterVolume.SurfaceQuery waterSurfaceForHead;
  private bool bodyInWater;
  private bool headInWater;
  private bool audioSetToUnderwater;
  private float buoyancyExtension;
  private float lastWaterSurfaceJumpTimeLeft = -1f;
  private float lastWaterSurfaceJumpTimeRight = -1f;
  private float waterSurfaceJumpCooldown = 0.1f;
  private float leftHandNonDiveHapticsAmount;
  private float rightHandNonDiveHapticsAmount;
  private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16 /*0x10*/);
  private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16 /*0x10*/);
  private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16 /*0x10*/);
  private Quaternion playerRotationOverride;
  private int playerRotationOverrideFrame = -1;
  private float playerRotationOverrideDecayRate = Mathf.Exp(1.5f);
  private ContactPoint[] bodyCollisionContacts = new ContactPoint[8];
  private int bodyCollisionContactsCount;
  private ContactPoint bodyGroundContact;
  private float bodyGroundContactTime;
  private const float movingSurfaceVelocityLimit = 40f;
  private bool exitMovingSurface;
  private float exitMovingSurfaceThreshold = 6f;
  private bool isClimbableMoving;
  private Quaternion lastClimbableRotation;
  private int lastAttachedToMovingSurfaceFrame;
  private const int MIN_FRAMES_OFF_SURFACE_TO_DETACH = 3;
  private bool isHandHoldMoving;
  private Quaternion lastHandHoldRotation;
  private Vector3 movingHandHoldReleaseVelocity;
  private GTPlayer.MovingSurfaceContactPoint lastMovingSurfaceContact;
  private int lastMovingSurfaceID = -1;
  private BuilderPiece lastMonkeBlock;
  private Quaternion lastMovingSurfaceRot;
  private UnityEngine.RaycastHit lastMovingSurfaceHit;
  private Vector3 lastMovingSurfaceTouchLocal;
  private Vector3 lastMovingSurfaceTouchWorld;
  private Vector3 movingSurfaceOffset;
  private bool wasMovingSurfaceMonkeBlock;
  private Vector3 lastMovingSurfaceVelocity;
  private bool wasBodyOnGround;
  private BasePlatform currentPlatform;
  private BasePlatform lastPlatformTouched;
  private Vector3 lastFrameTouchPosLocal;
  private Vector3 lastFrameTouchPosWorld;
  private bool lastFrameHasValidTouchPos;
  private Vector3 refMovement = Vector3.zero;
  private Vector3 platformTouchOffset;
  private Vector3 debugLastRightHandPosition;
  private Vector3 debugPlatformDeltaPosition;
  public double tempFreezeRightHandEnableTime;
  public double tempFreezeLeftHandEnableTime;
  private const float climbingMaxThrowSpeed = 5.5f;
  private const float climbHelperSmoothSnapSpeed = 12f;
  [NonSerialized]
  public bool isClimbing;
  private GorillaClimbable currentClimbable;
  private GorillaHandClimber currentClimber;
  private Vector3 climbHelperTargetPos = Vector3.zero;
  private Transform climbHelper;
  private GorillaRopeSwing currentSwing;
  private GorillaZipline currentZipline;
  [SerializeField]
  private ConnectedControllerHandler controllerState;
  public int sizeLayerMask;
  public bool InReportMenu;
  private LayerChanger layerChanger;
  private bool hasCorrectedForTracking;
  private float halloweenLevitationStrength;
  private float halloweenLevitationFullStrengthDuration;
  private float halloweenLevitationTotalDuration = 1f;
  private float halloweenLevitationBonusStrength;
  private float halloweenLevitateBonusOffAtYSpeed;
  private float halloweenLevitateBonusFullAtYSpeed = 1f;
  private float lastTouchedGroundTimestamp;
  private bool teleportToTrain;
  public bool isAttachedToTrain;
  private bool stuckLeft;
  private bool stuckRight;
  private float lastScale;
  private Vector3 currentSlopDirection;
  private Vector3 lastSlopeDirection = Vector3.zero;
  private readonly Dictionary<UnityEngine.Object, Action<GTPlayer>> gravityOverrides = new Dictionary<UnityEngine.Object, Action<GTPlayer>>();
  private int hoverAllowedCount;
  [Header("Hoverboard")]
  [SerializeField]
  private float hoverIdealHeight = 0.5f;
  [SerializeField]
  private float hoverCarveSidewaysSpeedLossFactor = 1f;
  [SerializeField]
  private AnimationCurve hoverCarveAngleResponsiveness;
  [SerializeField]
  private HoverboardVisual hoverboardVisual;
  [SerializeField]
  private float sidewaysDrag = 0.1f;
  [SerializeField]
  private float hoveringSlowSpeed = 0.1f;
  [SerializeField]
  private float hoveringSlowStoppingFactor = 0.95f;
  [SerializeField]
  private float hoverboardPaddleBoostMultiplier = 0.1f;
  [SerializeField]
  private float hoverboardPaddleBoostMax = 10f;
  [SerializeField]
  private float hoverboardBoostGracePeriod = 1f;
  [SerializeField]
  private float hoverBodyHasCollisionsOutsideRadius = 0.5f;
  [SerializeField]
  private float hoverBodyCollisionRadiusUpOffset = 0.2f;
  [SerializeField]
  private float hoverGeneralUpwardForce = 8f;
  [SerializeField]
  private float hoverTiltAdjustsForwardFactor = 0.2f;
  [SerializeField]
  private float hoverMinGrindSpeed = 1f;
  [SerializeField]
  private float hoverSlamJumpStrengthFactor = 25f;
  [SerializeField]
  private float hoverMaxPaddleSpeed = 35f;
  [SerializeField]
  private HoverboardAudio hoverboardAudio;
  private bool hasHoverPoint;
  private float boostEnabledUntilTimestamp;
  private GTPlayer.HoverBoardCast[] hoverboardCasts = new GTPlayer.HoverBoardCast[3]
  {
    new GTPlayer.HoverBoardCast()
    {
      localOrigin = new Vector3(0.0f, 1f, 0.36f),
      localDirection = Vector3.down,
      distance = 1f,
      sphereRadius = 0.2f,
      intersectToVelocityCap = 0.1f
    },
    new GTPlayer.HoverBoardCast()
    {
      localOrigin = new Vector3(0.0f, 0.05f, 0.36f),
      localDirection = Vector3.forward,
      distance = 0.25f,
      sphereRadius = 0.01f,
      intersectToVelocityCap = 0.0f,
      isSolid = true
    },
    new GTPlayer.HoverBoardCast()
    {
      localOrigin = new Vector3(0.0f, 0.05f, -0.1f),
      localDirection = -Vector3.forward,
      distance = 0.24f,
      sphereRadius = 0.01f,
      intersectToVelocityCap = 0.0f,
      isSolid = true
    }
  };
  private Vector3 hoverboardPlayerLocalPos;
  private Quaternion hoverboardPlayerLocalRot;
  private bool didHoverLastFrame;
  private bool hasLeftHandTentacleMove;
  private bool hasRightHandTentacleMove;
  private Vector3 leftHandTentacleMove;
  private Vector3 rightHandTentacleMove;
  private GTPlayer.HandHoldState activeHandHold;
  private GTPlayer.HandHoldState secondaryHandHold;
  public PhysicsMaterial slipperyMaterial;
  private bool wasHoldingHandhold;
  private Vector3 secondLastPreHandholdVelocity;
  private Vector3 lastPreHandholdVelocity;
  [Header("Native Scale Adjustment")]
  [SerializeField]
  private AnimationCurve nativeScaleMagnitudeAdjustmentFactor;

  public static GTPlayer Instance => GTPlayer._instance;

  public GTPlayer.HandState LeftHand => this.leftHand;

  public GTPlayer.HandState RightHand => this.rightHand;

  public int GetMaterialTouchIndex(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).materialTouchIndex;
  }

  public GorillaSurfaceOverride GetSurfaceOverride(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).surfaceOverride;
  }

  public UnityEngine.RaycastHit GetTouchHitInfo(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).hitInfo;
  }

  public bool IsHandTouching(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).wasColliding;
  }

  public GorillaVelocityTracker GetHandVelocityTracker(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).velocityTracker;
  }

  public GorillaVelocityTracker GetInteractPointVelocityTracker(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).interactPointVelocityTracker;
  }

  public Transform GetControllerTransform(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).controllerTransform;
  }

  public Transform GetHandFollower(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).handFollower;
  }

  public Vector3 GetHandOffset(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).handOffset;
  }

  public Quaternion GetHandRotOffset(bool isLeftHand)
  {
    return (isLeftHand ? this.leftHand : this.rightHand).handRotOffset;
  }

  public Vector3 GetHandPosition(bool isLeftHand, StiltID stiltID = StiltID.None)
  {
    return (stiltID != StiltID.None ? this.stiltStates[(int) stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).lastPosition;
  }

  public void GetHandTapData(
    bool isLeftHand,
    StiltID stiltID,
    out bool wasHandTouching,
    out bool wasSliding,
    out int handMatIndex,
    out GorillaSurfaceOverride surfaceOverride,
    out UnityEngine.RaycastHit handHitInfo,
    out Vector3 handPosition,
    out GorillaVelocityTracker handVelocityTracker)
  {
    (stiltID != StiltID.None ? this.stiltStates[(int) stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).GetHandTapData(out wasHandTouching, out wasSliding, out handMatIndex, out surfaceOverride, out handHitInfo, out handPosition, out handVelocityTracker);
  }

  public void SetHandOffsets(bool isLeftHand, Vector3 handOffset, Quaternion handRotOffset)
  {
    if (isLeftHand)
    {
      this.leftHand.handOffset = handOffset;
      this.leftHand.handRotOffset = handRotOffset;
    }
    else
    {
      this.rightHand.handOffset = handOffset;
      this.rightHand.handRotOffset = handRotOffset;
    }
  }

  public Vector3 InstantaneousVelocity => this.currentVelocity;

  public Vector3 AveragedVelocity => this.averagedVelocity;

  public Transform CosmeticsHeadTarget => this.cosmeticsHeadTarget;

  public float scale => this.scaleMultiplier * this.nativeScale;

  public float NativeScale => this.nativeScale;

  public float ScaleMultiplier => this.scaleMultiplier;

  public void SetScaleMultiplier(float s) => this.scaleMultiplier = s;

  public void SetNativeScale(NativeSizeChangerSettings s)
  {
    double nativeScale1 = (double) this.nativeScale;
    this.activeSizeChangerSettings = s == null || (double) s.playerSizeScale <= 0.0 || (double) s.playerSizeScale == 1.0 ? (NativeSizeChangerSettings) null : s;
    this.nativeScale = this.activeSizeChangerSettings != null ? this.activeSizeChangerSettings.playerSizeScale : 1f;
    double nativeScale2 = (double) this.nativeScale;
    if (nativeScale1 == nativeScale2 || !NetworkSystem.Instance.InRoom)
      return;
    int num = (UnityEngine.Object) GorillaTagger.Instance.myVRRig != (UnityEngine.Object) null ? 1 : 0;
  }

  public bool IsDefaultScale => (double) Mathf.Abs(1f - this.scale) < 1.0 / 1000.0;

  public bool turnedThisFrame => (double) this.degreesTurnedThisFrame != 0.0;

  public List<GTPlayer.MaterialData> materialData => this.materialDatasSO.datas;

  protected bool IsFrozen { get; set; }

  public List<WaterVolume> HeadOverlappingWaterVolumes => this.headOverlappingWaterVolumes;

  public bool InWater => this.bodyInWater;

  public bool HeadInWater => this.headInWater;

  public WaterVolume CurrentWaterVolume
  {
    get
    {
      return this.bodyOverlappingWaterVolumes.Count <= 0 ? (WaterVolume) null : this.bodyOverlappingWaterVolumes[0];
    }
  }

  public WaterVolume.SurfaceQuery WaterSurfaceForHead => this.waterSurfaceForHead;

  public WaterVolume LeftHandWaterVolume => this.leftHandWaterVolume;

  public WaterVolume RightHandWaterVolume => this.rightHandWaterVolume;

  public WaterVolume.SurfaceQuery LeftHandWaterSurface => this.leftHandWaterSurface;

  public WaterVolume.SurfaceQuery RightHandWaterSurface => this.rightHandWaterSurface;

  public Vector3 LastLeftHandPosition => this.leftHand.lastPosition;

  public Vector3 LastRightHandPosition => this.rightHand.lastPosition;

  public Vector3 RigidbodyVelocity => this.playerRigidBody.linearVelocity;

  public Vector3 HeadCenterPosition
  {
    get
    {
      return this.headCollider.transform.position + this.headCollider.transform.rotation * new Vector3(0.0f, 0.0f, -0.11f);
    }
  }

  public bool HandContactingSurface => this.leftHand.isColliding || this.rightHand.isColliding;

  public bool BodyOnGround
  {
    get => (double) this.bodyGroundContactTime >= (double) Time.time - 0.05000000074505806;
  }

  public bool IsGroundedHand
  {
    get
    {
      return this.HandContactingSurface || this.isClimbing || this.leftHand.isHolding || this.rightHand.isHolding;
    }
  }

  public bool IsGroundedButt => this.BodyOnGround;

  public int ThrusterActiveAtFrame { get; set; }

  public bool IsThrusterActive => this.ThrusterActiveAtFrame == Time.frameCount;

  public Quaternion PlayerRotationOverride
  {
    set
    {
      this.playerRotationOverride = value;
      this.playerRotationOverrideFrame = Time.frameCount;
    }
  }

  public bool IsBodySliding { get; set; }

  public GorillaClimbable CurrentClimbable => this.currentClimbable;

  public GorillaHandClimber CurrentClimber => this.currentClimber;

  public float jumpMultiplier
  {
    get => this._jumpMultiplier;
    set => this._jumpMultiplier = value;
  }

  public float LastTouchedGroundAtNetworkTime { get; private set; }

  public float LastHandTouchedGroundAtNetworkTime { get; private set; }

  public void EnableStilt(
    StiltID stiltID,
    bool isLeftHand,
    Vector3 currentTipWorldPos,
    float maxArmLength,
    bool canTag,
    bool canStun,
    float customBoostFactor = 0.0f,
    GorillaVelocityTracker velocityTracker = null)
  {
    this.stiltStates[(int) stiltID] = new GTPlayer.HandState()
    {
      isActive = true,
      controllerTransform = (isLeftHand ? this.leftHand : this.rightHand).controllerTransform,
      velocityTracker = (UnityEngine.Object) velocityTracker != (UnityEngine.Object) null ? velocityTracker : (isLeftHand ? this.leftHand : this.rightHand).velocityTracker,
      handRotOffset = Quaternion.identity,
      canTag = canTag,
      canStun = canStun,
      customBoostFactor = customBoostFactor,
      hasCustomBoost = (double) customBoostFactor > 0.0
    };
    this.stiltStates[(int) stiltID].Init(this, isLeftHand, maxArmLength);
    this.UpdateStiltOffset(stiltID, currentTipWorldPos);
  }

  public void DisableStilt(StiltID stiltID) => this.stiltStates[(int) stiltID].isActive = false;

  public void UpdateStiltOffset(StiltID stiltID, Vector3 currentTipWorldPos)
  {
    this.stiltStates[(int) stiltID].handOffset = this.stiltStates[(int) stiltID].controllerTransform.InverseTransformPoint(currentTipWorldPos);
  }

  private void Awake()
  {
    if ((UnityEngine.Object) GTPlayer._instance != (UnityEngine.Object) null && (UnityEngine.Object) GTPlayer._instance != (UnityEngine.Object) this)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      GTPlayer._instance = this;
      GTPlayer.hasInstance = true;
    }
    this.InitializeValues();
    this.playerRigidbodyInterpolationDefault = this.playerRigidBody.interpolation;
    this.playerRigidBody.maxAngularVelocity = 0.0f;
    this.bodyOffsetVector = new Vector3(0.0f, (float) (-(double) this.bodyCollider.height / 2.0), 0.0f);
    this.bodyInitialHeight = this.bodyCollider.height;
    this.bodyInitialRadius = this.bodyCollider.radius;
    this.rayCastNonAllocColliders = new UnityEngine.RaycastHit[5];
    this.crazyCheckVectors = new Vector3[7];
    this.emptyHit = new UnityEngine.RaycastHit();
    this.crazyCheckVectors[0] = Vector3.up;
    this.crazyCheckVectors[1] = Vector3.down;
    this.crazyCheckVectors[2] = Vector3.left;
    this.crazyCheckVectors[3] = Vector3.right;
    this.crazyCheckVectors[4] = Vector3.forward;
    this.crazyCheckVectors[5] = Vector3.back;
    this.crazyCheckVectors[6] = Vector3.zero;
    if ((UnityEngine.Object) this.controllerState == (UnityEngine.Object) null)
      this.controllerState = this.GetComponent<ConnectedControllerHandler>();
    this.layerChanger = this.GetComponent<LayerChanger>();
    this.bodyTouchedSurfaces = new Dictionary<GameObject, PhysicsMaterial>();
    if (!Application.isPlaying)
      return;
    Application.onBeforeRender += new UnityAction(this.OnBeforeRenderInit);
  }

  protected void Start()
  {
    if ((UnityEngine.Object) this.mainCamera == (UnityEngine.Object) null)
      this.mainCamera = Camera.main;
    this.mainCamera.farClipPlane = 500f;
    this.lastScale = this.scale;
    this.layerChanger.InitializeLayers(this.transform);
    this.Turn(Quaternion.Angle(Quaternion.identity, GorillaTagger.Instance.offlineVRRig.transform.rotation) * Mathf.Sign(Vector3.Dot(Vector3.up, GorillaTagger.Instance.offlineVRRig.transform.right)));
  }

  protected void OnDestroy()
  {
    if ((UnityEngine.Object) GTPlayer._instance == (UnityEngine.Object) this)
    {
      GTPlayer._instance = (GTPlayer) null;
      GTPlayer.hasInstance = false;
    }
    if (!(bool) (UnityEngine.Object) this.climbHelper)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.climbHelper.gameObject);
  }

  public void InitializeValues()
  {
    Physics.SyncTransforms();
    this.playerRigidBody = this.GetComponent<Rigidbody>();
    this.velocityHistory = new Vector3[this.velocityHistorySize];
    this.slideAverageHistory = new Vector3[this.velocityHistorySize];
    for (int index = 0; index < this.velocityHistory.Length; ++index)
    {
      this.velocityHistory[index] = Vector3.zero;
      this.slideAverageHistory[index] = Vector3.zero;
    }
    this.leftHand.Init(this, true, this.maxArmLength);
    this.rightHand.Init(this, false, this.maxArmLength);
    this.lastHeadPosition = this.headCollider.transform.position;
    this.velocityIndex = 0;
    this.averagedVelocity = Vector3.zero;
    this.slideVelocity = Vector3.zero;
    this.lastPosition = this.transform.position;
    this.lastRealTime = Time.realtimeSinceStartup;
    this.lastOpenHeadPosition = this.headCollider.transform.position;
    this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
    this.bodyCollider.transform.eulerAngles = new Vector3(0.0f, this.headCollider.transform.eulerAngles.y, 0.0f);
    this.ForceRigidBodySync();
  }

  public void SetHalloweenLevitation(
    float levitateStrength,
    float levitateDuration,
    float levitateBlendOutDuration,
    float levitateBonusStrength,
    float levitateBonusOffAtYSpeed,
    float levitateBonusFullAtYSpeed)
  {
    this.halloweenLevitationStrength = levitateStrength;
    this.halloweenLevitationFullStrengthDuration = levitateDuration;
    this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
    this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
    this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
    this.halloweenLevitationBonusStrength = levitateBonusStrength;
  }

  public void TeleportToTrain(bool enable) => this.teleportToTrain = enable;

  public void TeleportTo(Vector3 position, Quaternion rotation, bool keepVelocity = false, bool center = false)
  {
    if (center)
    {
      Vector3 vector3 = this.mainCamera.transform.position - this.transform.position;
      position -= vector3;
    }
    this.ClearHandHolds();
    if ((UnityEngine.Object) this.playerRigidBody != (UnityEngine.Object) null)
    {
      this.playerRigidBody.isKinematic = true;
      this.playerRigidBody.position = position;
      this.playerRigidBody.rotation = rotation;
      this.playerRigidBody.isKinematic = false;
    }
    this.playerRigidBody.position = position;
    this.playerRigidBody.rotation = rotation;
    this.transform.position = position;
    this.transform.rotation = rotation;
    this.lastHeadPosition = this.headCollider.transform.position;
    this.lastPosition = position;
    this.lastOpenHeadPosition = this.headCollider.transform.position;
    this.leftHand.OnTeleport();
    this.rightHand.OnTeleport();
    if (!keepVelocity)
      this.playerRigidBody.linearVelocity = Vector3.zero;
    this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
    this.bodyCollider.transform.eulerAngles = new Vector3(0.0f, this.headCollider.transform.eulerAngles.y, 0.0f);
    Physics.SyncTransforms();
    GorillaTagger.Instance.offlineVRRig.transform.position = position;
    GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
    GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
    this.ForceRigidBodySync();
  }

  public void TeleportTo(
    Transform destination,
    bool matchDestinationRotation = true,
    bool maintainVelocity = true)
  {
    Vector3 vector3 = this.mainCamera.transform.position - this.transform.position;
    Vector3 position = destination.position - vector3;
    Quaternion rotation = destination.rotation;
    double y1 = (double) rotation.eulerAngles.y;
    rotation = this.mainCamera.transform.rotation;
    double y2 = (double) rotation.eulerAngles.y;
    float num = (float) (y1 - y2);
    Vector3 newVelocity = this.currentVelocity;
    if (!maintainVelocity)
      this.SetPlayerVelocity(Vector3.zero);
    else if (matchDestinationRotation)
    {
      newVelocity = Quaternion.AngleAxis(num, this.transform.up) * this.currentVelocity;
      this.SetPlayerVelocity(newVelocity);
    }
    if (matchDestinationRotation)
      this.Turn(num);
    this.TeleportTo(position, this.transform.rotation);
    if (maintainVelocity)
      this.SetPlayerVelocity(newVelocity);
    this.ForceRigidBodySync();
  }

  public void AddForce(Vector3 force, ForceMode mode)
  {
    if (mode != ForceMode.VelocityChange)
    {
      if (mode == ForceMode.Acceleration)
        this.playerRigidBody.AddForce(force * this.playerRigidBody.mass, ForceMode.Force);
      else
        this.playerRigidBody.AddForce(force, mode);
    }
    else
      this.playerRigidBody.AddForce(force * this.playerRigidBody.mass, ForceMode.Impulse);
  }

  public void SetPlayerVelocity(Vector3 newVelocity)
  {
    for (int index = 0; index < this.velocityHistory.Length; ++index)
      this.velocityHistory[index] = newVelocity;
    this.playerRigidBody.AddForce(newVelocity - this.playerRigidBody.linearVelocity, ForceMode.VelocityChange);
  }

  public void SetGravityOverride(UnityEngine.Object caller, Action<GTPlayer> gravityFunction)
  {
    this.gravityOverrides[caller] = gravityFunction;
  }

  public void UnsetGravityOverride(UnityEngine.Object caller)
  {
    this.gravityOverrides.Remove(caller);
  }

  private void ApplyGravityOverrides()
  {
    foreach (KeyValuePair<UnityEngine.Object, Action<GTPlayer>> gravityOverride in this.gravityOverrides)
      gravityOverride.Value(this);
  }

  public void ApplyKnockback(Vector3 direction, float speed, bool forceOffTheGround = false)
  {
    if (forceOffTheGround)
    {
      if (this.leftHand.wasColliding || this.rightHand.wasColliding)
      {
        this.leftHand.wasColliding = false;
        this.rightHand.wasColliding = false;
        this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
      }
      this.didAJump = true;
      this.SetMaximumSlipThisFrame();
    }
    if ((double) speed <= 0.0099999997764825821)
      return;
    float num = Mathf.InverseLerp(1.5f, 0.5f, Vector3.Dot(this.averagedVelocity, direction) / speed);
    Vector3 vector3 = this.averagedVelocity + direction * speed * num;
    this.playerRigidBody.linearVelocity = vector3;
    for (int index = 0; index < this.velocityHistory.Length; ++index)
      this.velocityHistory[index] = vector3;
  }

  public void FixedUpdate()
  {
    this.AntiTeleportTechnology();
    this.IsFrozen = GorillaTagger.Instance.offlineVRRig.IsFrozen || this.debugFreezeTag;
    bool isDefaultScale = this.IsDefaultScale;
    this.playerRigidBody.useGravity = false;
    if (this.gravityOverrides.Count > 0)
    {
      this.ApplyGravityOverrides();
    }
    else
    {
      if (!this.isClimbing)
        this.playerRigidBody.AddForce(Physics.gravity * this.scale * this.playerRigidBody.mass, ForceMode.Force);
      if ((double) this.halloweenLevitationBonusStrength > 0.0 || (double) this.halloweenLevitationStrength > 0.0)
      {
        float num1 = Time.time - this.lastTouchedGroundTimestamp;
        if ((double) num1 < (double) this.halloweenLevitationTotalDuration)
          this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num1) * this.playerRigidBody.mass, ForceMode.Force);
        float y = this.playerRigidBody.linearVelocity.y;
        if ((double) y <= (double) this.halloweenLevitateBonusFullAtYSpeed)
          this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * this.playerRigidBody.mass, ForceMode.Force);
        else if ((double) y <= (double) this.halloweenLevitateBonusOffAtYSpeed)
        {
          double num2 = (double) Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.linearVelocity.y);
          this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength * Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.linearVelocity.y) * this.playerRigidBody.mass, ForceMode.Force);
        }
      }
    }
    if (this.enableHoverMode)
      this.playerRigidBody.linearVelocity = this.HoverboardFixedUpdate(this.playerRigidBody.linearVelocity);
    else
      this.didHoverLastFrame = false;
    float fixedDeltaTime = Time.fixedDeltaTime;
    this.bodyInWater = false;
    Vector3 swimmingVelocity = this.swimmingVelocity;
    this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, this.swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
    this.leftHandNonDiveHapticsAmount = 0.0f;
    this.rightHandNonDiveHapticsAmount = 0.0f;
    if (this.bodyOverlappingWaterVolumes.Count > 0)
    {
      WaterVolume waterVolume = (WaterVolume) null;
      float num3 = float.MinValue;
      Vector3 point = this.headCollider.transform.position + Vector3.down * this.swimmingParams.floatingWaterLevelBelowHead * this.scale;
      this.activeWaterCurrents.Clear();
      for (int index = 0; index < this.bodyOverlappingWaterVolumes.Count; ++index)
      {
        WaterVolume.SurfaceQuery result;
        if (this.bodyOverlappingWaterVolumes[index].GetSurfaceQueryForPoint(point, out result))
        {
          float num4 = Vector3.Dot(result.surfacePoint - point, result.surfaceNormal);
          if ((double) num4 > (double) num3)
          {
            num3 = num4;
            waterVolume = this.bodyOverlappingWaterVolumes[index];
            this.waterSurfaceForHead = result;
          }
          WaterCurrent current = this.bodyOverlappingWaterVolumes[index].Current;
          if ((UnityEngine.Object) current != (UnityEngine.Object) null && (double) num4 > 0.0 && !this.activeWaterCurrents.Contains(current))
            this.activeWaterCurrents.Add(current);
        }
      }
      if ((UnityEngine.Object) waterVolume != (UnityEngine.Object) null)
      {
        Vector3 linearVelocity = this.playerRigidBody.linearVelocity;
        float magnitude1 = linearVelocity.magnitude;
        bool headInWater = this.headInWater;
        this.headInWater = (double) this.headCollider.transform.position.y < (double) this.waterSurfaceForHead.surfacePoint.y && (double) this.headCollider.transform.position.y > (double) this.waterSurfaceForHead.surfacePoint.y - (double) this.waterSurfaceForHead.maxDepth;
        if (this.headInWater && !headInWater)
        {
          this.audioSetToUnderwater = true;
          this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot);
        }
        else if (!this.headInWater & headInWater)
        {
          this.audioSetToUnderwater = false;
          this.audioManager.UnsetMixerSnapshot();
        }
        this.bodyInWater = (double) point.y < (double) this.waterSurfaceForHead.surfacePoint.y && (double) point.y > (double) this.waterSurfaceForHead.surfacePoint.y - (double) this.waterSurfaceForHead.maxDepth;
        if (this.bodyInWater)
        {
          GTPlayer.LiquidProperties liquidProperties = this.liquidPropertiesList[(int) waterVolume.LiquidType];
          if ((UnityEngine.Object) waterVolume != (UnityEngine.Object) null)
          {
            float num5;
            if (this.swimmingParams.extendBouyancyFromSpeed)
            {
              this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, this.swimmingParams.speedToBouyancyExtension.Evaluate(Mathf.Clamp(Vector3.Dot(linearVelocity / this.scale, this.waterSurfaceForHead.surfaceNormal), this.swimmingParams.speedToBouyancyExtensionMinMax.x, this.swimmingParams.speedToBouyancyExtensionMinMax.y)));
              double num6 = (double) Mathf.InverseLerp(0.0f, this.swimmingParams.buoyancyFadeDist + this.buoyancyExtension, num3 / this.scale + this.buoyancyExtension);
              this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, this.swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime);
              num5 = (float) num6;
            }
            else
              num5 = Mathf.InverseLerp(0.0f, this.swimmingParams.buoyancyFadeDist, num3 / this.scale);
            Vector3 vector3_1 = Physics.gravity * this.scale;
            Vector3 vector3_2 = liquidProperties.buoyancy * -vector3_1 * num5;
            if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
              vector3_2 *= this.frozenBodyBuoyancyFactor;
            this.playerRigidBody.AddForce(vector3_2 * this.playerRigidBody.mass, ForceMode.Force);
          }
          Vector3 zero1 = Vector3.zero;
          Vector3 zero2 = Vector3.zero;
          for (int index = 0; index < this.activeWaterCurrents.Count; ++index)
          {
            WaterCurrent activeWaterCurrent = this.activeWaterCurrents[index];
            Vector3 vector3_3 = linearVelocity + zero1;
            Vector3 position = this.bodyCollider.transform.position;
            Vector3 startingVelocity = vector3_3;
            double dt = (double) fixedDeltaTime;
            Vector3 vector3_4;
            ref Vector3 local1 = ref vector3_4;
            Vector3 vector3_5;
            ref Vector3 local2 = ref vector3_5;
            if (activeWaterCurrent.GetCurrentAtPoint(position, startingVelocity, (float) dt, out local1, out local2))
            {
              zero2 += vector3_4;
              zero1 += vector3_5;
            }
          }
          if ((double) magnitude1 > (double) Mathf.Epsilon)
          {
            float num7 = 0.01f;
            Vector3 vector3_6 = linearVelocity / magnitude1;
            Vector3 right = this.leftHand.handFollower.right;
            Vector3 dir = -this.rightHand.handFollower.right;
            Vector3 forward1 = this.leftHand.handFollower.forward;
            Vector3 forward2 = this.rightHand.handFollower.forward;
            Vector3 vector3_7 = vector3_6;
            float a1 = 0.0f;
            float a2 = 0.0f;
            float t1 = 0.0f;
            if (((!this.swimmingParams.applyDiveSteering ? 0 : (!this.disableMovement ? 1 : 0)) & (isDefaultScale ? 1 : 0)) != 0)
            {
              double num8 = (double) Vector3.Dot(linearVelocity - zero2, vector3_6);
              float b1 = this.swimmingParams.swimSpeedToRedirectAmount.Evaluate(Mathf.Clamp((float) num8, this.swimmingParams.swimSpeedToRedirectAmountMinMax.x, this.swimmingParams.swimSpeedToRedirectAmountMinMax.y));
              float num9 = this.swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(Mathf.Clamp((float) num8, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, this.swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y));
              float num10 = (float) ((double) Mathf.Acos(Vector3.Dot(vector3_6, forward1)) / 3.1415927410125732 * -2.0 + 1.0);
              double num11 = (double) Mathf.Acos(Vector3.Dot(vector3_6, forward2)) / 3.1415927410125732 * -2.0 + 1.0;
              float num12 = Mathf.Clamp(num10, this.swimmingParams.palmFacingToRedirectAmountMinMax.x, this.swimmingParams.palmFacingToRedirectAmountMinMax.y);
              double x1 = (double) this.swimmingParams.palmFacingToRedirectAmountMinMax.x;
              double y1 = (double) this.swimmingParams.palmFacingToRedirectAmountMinMax.y;
              float num13 = Mathf.Clamp((float) num11, (float) x1, (float) y1);
              double a3 = !float.IsNaN(num12) ? (double) this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num12) : 0.0;
              float a4 = !float.IsNaN(num13) ? this.swimmingParams.palmFacingToRedirectAmount.Evaluate(num13) : 0.0f;
              Vector3 vector3_8 = Vector3.ProjectOnPlane(vector3_6, right);
              Vector3 vector3_9 = Vector3.ProjectOnPlane(vector3_6, right);
              float num14 = Mathf.Min(vector3_8.magnitude, 1f);
              float num15 = Mathf.Min(vector3_9.magnitude, 1f);
              float magnitude2 = this.leftHand.velocityTracker.GetAverageVelocity(maxTimeFromPast: this.swimmingParams.diveVelocityAveragingWindow).magnitude;
              double magnitude3 = (double) this.rightHand.velocityTracker.GetAverageVelocity(maxTimeFromPast: this.swimmingParams.diveVelocityAveragingWindow).magnitude;
              float time1 = Mathf.Clamp(magnitude2, this.swimmingParams.handSpeedToRedirectAmountMinMax.x, this.swimmingParams.handSpeedToRedirectAmountMinMax.y);
              double x2 = (double) this.swimmingParams.handSpeedToRedirectAmountMinMax.x;
              double y2 = (double) this.swimmingParams.handSpeedToRedirectAmountMinMax.y;
              float time2 = Mathf.Clamp((float) magnitude3, (float) x2, (float) y2);
              float a5 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time1);
              float a6 = this.swimmingParams.handSpeedToRedirectAmount.Evaluate(time2);
              double magnitudeInDirection1 = (double) this.leftHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, maxTimeFromPast: this.swimmingParams.diveVelocityAveragingWindow);
              float magnitudeInDirection2 = this.rightHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(dir, maxTimeFromPast: this.swimmingParams.diveVelocityAveragingWindow);
              double x3 = (double) this.swimmingParams.handAccelToRedirectAmountMinMax.x;
              double y3 = (double) this.swimmingParams.handAccelToRedirectAmountMinMax.y;
              float time3 = Mathf.Clamp((float) magnitudeInDirection1, (float) x3, (float) y3);
              float time4 = Mathf.Clamp(magnitudeInDirection2, this.swimmingParams.handAccelToRedirectAmountMinMax.x, this.swimmingParams.handAccelToRedirectAmountMinMax.y);
              float b2 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time3);
              float b3 = this.swimmingParams.handAccelToRedirectAmount.Evaluate(time4);
              double b4 = (double) Mathf.Min(a5, b2);
              a1 = Mathf.Min((float) a3, (float) b4);
              float num16 = (double) Vector3.Dot(vector3_6, forward1) > 0.0 ? Mathf.Min(a1, b1) * num14 : 0.0f;
              a2 = Mathf.Min(a4, Mathf.Min(a6, b3));
              float num17 = (double) Vector3.Dot(vector3_6, forward2) > 0.0 ? Mathf.Min(a2, b1) * num15 : 0.0f;
              if (this.swimmingParams.reduceDiveSteeringBelowVelocityPlane)
              {
                Vector3 rhs = (double) Vector3.Dot(this.headCollider.transform.up, vector3_6) <= 0.949999988079071 ? Vector3.Cross(Vector3.Cross(vector3_6, this.headCollider.transform.up), vector3_6).normalized : -this.headCollider.transform.forward;
                Vector3 position = this.headCollider.transform.position;
                Vector3 lhs1 = position - this.leftHand.handFollower.position;
                Vector3 lhs2 = position - this.rightHand.handFollower.position;
                float planeFadeStartDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
                float planeFadeEndDist = this.swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
                float f1 = Vector3.Dot(lhs1, Vector3.up);
                float f2 = Vector3.Dot(lhs2, Vector3.up);
                float f3 = Vector3.Dot(lhs1, rhs);
                float f4 = Vector3.Dot(lhs2, rhs);
                float num18 = 1f - Mathf.InverseLerp(planeFadeStartDist, planeFadeEndDist, Mathf.Min(Mathf.Abs(f1), Mathf.Abs(f3)));
                float num19 = 1f - Mathf.InverseLerp(planeFadeStartDist, planeFadeEndDist, Mathf.Min(Mathf.Abs(f2), Mathf.Abs(f4)));
                num16 *= num18;
                num17 *= num19;
              }
              float t2 = num17 + num16;
              Vector3 zero3 = Vector3.zero;
              if (this.swimmingParams.applyDiveSteering && (double) t2 > (double) num7)
              {
                Vector3 normalized = ((num16 * vector3_8 + num17 * vector3_9) / t2).normalized;
                Vector3 target = Vector3.Lerp(vector3_6, normalized, t2);
                vector3_7 = Vector3.RotateTowards(vector3_6, target, (float) Math.PI / 180f * num9 * fixedDeltaTime, 0.0f);
              }
              else
                vector3_7 = vector3_6;
              t1 = Mathf.Clamp01((float) (((double) a1 + (double) a2) * 0.5));
            }
            float a7 = Mathf.Clamp(Vector3.Dot(swimmingVelocity, vector3_6), 0.0f, magnitude1);
            float num20 = magnitude1 - a7;
            if (this.swimmingParams.applyDiveSwimVelocityConversion && !this.disableMovement && (double) t1 > (double) num7 && (double) a7 < (double) this.swimmingParams.diveMaxSwimVelocityConversion)
            {
              float num21 = Mathf.Min(this.swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num20) * t1;
              a7 += num21;
              num20 -= num21;
            }
            float halflife1 = this.swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
            float halflife2 = this.swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
            float b5 = Spring.DamperDecayExact(a7 / this.scale, halflife1, fixedDeltaTime) * this.scale;
            float b6 = Spring.DamperDecayExact(num20 / this.scale, halflife2, fixedDeltaTime) * this.scale;
            if (this.swimmingParams.applyDiveDampingMultiplier && !this.disableMovement)
            {
              float t3 = Mathf.Lerp(1f, this.swimmingParams.diveDampingMultiplier, t1);
              b5 = Mathf.Lerp(a7, b5, t3);
              b6 = Mathf.Lerp(num20, b6, t3);
              float time5 = Mathf.Clamp((float) ((1.0 - (double) a1) * ((double) a7 + (double) num20)), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num7, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num7);
              float time6 = Mathf.Clamp((float) ((1.0 - (double) a2) * ((double) a7 + (double) num20)), this.swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num7, this.swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num7);
              this.leftHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time5);
              this.rightHandNonDiveHapticsAmount = this.swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time6);
            }
            this.swimmingVelocity = b5 * vector3_7 + zero1 * this.scale;
            this.playerRigidBody.linearVelocity = this.swimmingVelocity + b6 * vector3_7;
          }
        }
      }
    }
    else if (this.audioSetToUnderwater)
    {
      this.audioSetToUnderwater = false;
      this.audioManager.UnsetMixerSnapshot();
    }
    this.handleClimbing(Time.fixedDeltaTime);
    this.stuckHandsCheckFixedUpdate();
    this.FixedUpdate_HandHolds(Time.fixedDeltaTime);
  }

  public bool isHoverAllowed { get; private set; }

  public bool enableHoverMode { get; private set; }

  public void SetHoverboardPosRot(Vector3 worldPos, Quaternion worldRot)
  {
    this.hoverboardPlayerLocalPos = this.headCollider.transform.InverseTransformPoint(worldPos);
    this.hoverboardPlayerLocalRot = this.headCollider.transform.InverseTransformRotation(worldRot);
  }

  private void HoverboardLateUpdate()
  {
    Vector3 eulerAngles = this.headCollider.transform.eulerAngles;
    bool flag = false;
    for (int index = 0; index < this.hoverboardCasts.Length; ++index)
    {
      GTPlayer.HoverBoardCast hoverboardCast = this.hoverboardCasts[index];
      UnityEngine.RaycastHit hitInfo;
      hoverboardCast.didHit = Physics.SphereCast(new Ray(this.hoverboardVisual.transform.TransformPoint(hoverboardCast.localOrigin), this.hoverboardVisual.transform.rotation * hoverboardCast.localDirection), hoverboardCast.sphereRadius, out hitInfo, hoverboardCast.distance, (int) this.locomotionEnabledLayers);
      if (hoverboardCast.didHit)
      {
        if (hitInfo.collider.TryGetComponent<HoverboardCantHover>(out HoverboardCantHover _))
        {
          hoverboardCast.didHit = false;
        }
        else
        {
          hoverboardCast.pointHit = hitInfo.point;
          hoverboardCast.normalHit = hitInfo.normal;
        }
      }
      this.hoverboardCasts[index] = hoverboardCast;
      if (hoverboardCast.didHit)
        flag = true;
    }
    this.hasHoverPoint = flag;
    this.bodyCollider.enabled = (this.bodyCollider.transform.position - this.hoverboardVisual.transform.TransformPoint(Vector3.up * this.hoverBodyCollisionRadiusUpOffset)).IsLongerThan(this.hoverBodyHasCollisionsOutsideRadius);
  }

  private Vector3 HoverboardFixedUpdate(Vector3 velocity)
  {
    this.hoverboardVisual.transform.position = this.headCollider.transform.TransformPoint(this.hoverboardPlayerLocalPos);
    this.hoverboardVisual.transform.rotation = this.headCollider.transform.TransformRotation(this.hoverboardPlayerLocalRot);
    if (this.didHoverLastFrame)
      velocity += Vector3.up * this.hoverGeneralUpwardForce * Time.fixedDeltaTime;
    Vector3 position = this.hoverboardVisual.transform.position;
    Vector3 vector3_1 = position + velocity * Time.fixedDeltaTime;
    Vector3 forward = this.hoverboardVisual.transform.forward;
    Vector3 vector3_2 = this.hoverboardCasts[0].didHit ? this.hoverboardCasts[0].normalHit : Vector3.up;
    bool flag1 = false;
    for (int index = 0; index < this.hoverboardCasts.Length; ++index)
    {
      GTPlayer.HoverBoardCast hoverboardCast = this.hoverboardCasts[index];
      if (hoverboardCast.didHit)
      {
        Vector3 vector3_3 = position + Vector3.Project(hoverboardCast.pointHit - position, forward);
        Vector3 vector3_4 = vector3_1 + Vector3.Project(hoverboardCast.pointHit - position, forward);
        int num1 = hoverboardCast.isSolid ? 1 : ((double) Vector3.Dot(hoverboardCast.normalHit, hoverboardCast.pointHit - vector3_4) + (double) this.hoverIdealHeight > 0.0 ? 1 : 0);
        float num2 = hoverboardCast.isSolid ? Vector3.Dot(hoverboardCast.normalHit, hoverboardCast.pointHit - this.hoverboardVisual.transform.TransformPoint(hoverboardCast.localOrigin + hoverboardCast.localDirection * hoverboardCast.distance)) + hoverboardCast.sphereRadius : Vector3.Dot(hoverboardCast.normalHit, hoverboardCast.pointHit - vector3_3) + this.hoverIdealHeight;
        if (num1 != 0)
        {
          flag1 = true;
          this.boostEnabledUntilTimestamp = Time.time + this.hoverboardBoostGracePeriod;
          if ((double) Vector3.Dot(velocity, hoverboardCast.normalHit) < 0.0)
            velocity = Vector3.ProjectOnPlane(velocity, hoverboardCast.normalHit);
          this.playerRigidBody.transform.position += hoverboardCast.normalHit * num2;
          Vector3 vector3_5 = this.turnParent.transform.rotation * (this.hoverboardVisual.IsLeftHanded ? this.leftHand.velocityTracker : this.rightHand.velocityTracker).GetAverageVelocity();
          if ((double) Vector3.Dot(vector3_5, hoverboardCast.normalHit) < 0.0)
            velocity -= Vector3.Project(vector3_5, hoverboardCast.normalHit) * this.hoverSlamJumpStrengthFactor * Time.fixedDeltaTime;
          vector3_1 = position + velocity * Time.fixedDeltaTime;
        }
      }
    }
    Vector3 up = this.hoverboardVisual.transform.up;
    Vector3 vector3_6 = Vector3.ProjectOnPlane(vector3_2, forward);
    Vector3 normalized1 = vector3_6.normalized;
    float num3 = this.hoverCarveAngleResponsiveness.Evaluate(Mathf.Abs(Mathf.DeltaAngle(0.0f, (float) ((double) Mathf.Acos(Vector3.Dot(up, normalized1)) * 57.295780181884766))));
    vector3_6 = forward + Vector3.ProjectOnPlane(this.hoverboardVisual.transform.up, vector3_2) * this.hoverTiltAdjustsForwardFactor;
    Vector3 normalized2 = vector3_6.normalized;
    if (!flag1)
    {
      this.didHoverLastFrame = false;
      num3 = 0.0f;
    }
    Vector3 vector3_7 = velocity;
    if (this.enableHoverMode && this.hasHoverPoint)
    {
      Vector3 vector = Vector3.ProjectOnPlane(velocity, vector3_2);
      Vector3 vector3_8 = velocity - vector;
      Vector3 v2 = Vector3.Project(vector, normalized2);
      float magnitude1 = vector.magnitude;
      if ((double) magnitude1 <= (double) this.hoveringSlowSpeed)
        magnitude1 *= this.hoveringSlowStoppingFactor;
      Vector3 v = vector - v2;
      float num4 = 0.0f;
      bool flag2 = false;
      if ((double) num3 > 0.0)
      {
        if (v.IsLongerThan(v2))
        {
          num4 = Mathf.Min((v.magnitude - v2.magnitude) * this.hoverCarveSidewaysSpeedLossFactor * num3, magnitude1);
          if ((double) num4 > 0.0 && (double) magnitude1 > (double) this.hoverMinGrindSpeed)
          {
            flag2 = true;
            this.hoverboardVisual.PlayGrindHaptic();
          }
          magnitude1 -= num4;
        }
        Vector3 vector3_9 = v * (float) (1.0 - (double) num3 * (double) this.sidewaysDrag);
        if (!this.leftHand.isColliding && !this.rightHand.isColliding)
          velocity = (v2 + vector3_9).normalized * magnitude1 + vector3_8;
      }
      else
        velocity = vector.normalized * magnitude1 + vector3_8;
      vector3_6 = velocity - vector3_7;
      float magnitude2 = vector3_6.magnitude;
      HoverboardAudio hoverboardAudio = this.hoverboardAudio;
      double magnitude3 = (double) velocity.magnitude;
      vector3_6 = this.bodyVelocityTracker.GetAverageVelocity(true);
      double magnitude4 = (double) vector3_6.magnitude;
      double strainLevel = (double) magnitude2;
      double grindLevel = flag2 ? (double) num4 : 0.0;
      hoverboardAudio.UpdateAudioLoop((float) magnitude3, (float) magnitude4, (float) strainLevel, (float) grindLevel);
      if ((double) magnitude2 > 0.0 && !flag2)
        this.hoverboardVisual.PlayCarveHaptic(magnitude2);
    }
    else
    {
      HoverboardAudio hoverboardAudio = this.hoverboardAudio;
      vector3_6 = this.bodyVelocityTracker.GetAverageVelocity(true);
      double magnitude = (double) vector3_6.magnitude;
      hoverboardAudio.UpdateAudioLoop(0.0f, (float) magnitude, 0.0f, 0.0f);
    }
    return velocity;
  }

  public void GrabPersonalHoverboard(bool isLeftHand, Vector3 pos, Quaternion rot, Color col)
  {
    if (this.hoverboardVisual.IsHeld)
      this.hoverboardVisual.DropFreeBoard();
    this.hoverboardVisual.SetIsHeld(isLeftHand, pos, rot, col);
    this.hoverboardVisual.ProxyGrabHandle(isLeftHand);
    FreeHoverboardManager.instance.PreserveMaxHoverboardsConstraint(NetworkSystem.Instance.LocalPlayer.ActorNumber);
  }

  public void SetHoverAllowed(bool allowed, bool force = false)
  {
    if (allowed)
    {
      ++this.hoverAllowedCount;
      this.isHoverAllowed = true;
    }
    else
    {
      this.hoverAllowedCount = force || this.hoverAllowedCount == 0 ? 0 : this.hoverAllowedCount - 1;
      if (this.hoverAllowedCount != 0 || !this.isHoverAllowed)
        return;
      this.isHoverAllowed = false;
      if (!this.enableHoverMode)
        return;
      this.SetHoverActive(false);
      VRRig.LocalRig.hoverboardVisual.SetNotHeld();
    }
  }

  public void SetHoverActive(bool enable)
  {
    if (enable && !this.isHoverAllowed)
      return;
    this.enableHoverMode = enable;
    if (enable)
      return;
    this.bodyCollider.enabled = true;
    this.hasHoverPoint = false;
    this.didHoverLastFrame = false;
    for (int index = 0; index < this.hoverboardCasts.Length; ++index)
      this.hoverboardCasts[index].didHit = false;
    this.hoverboardAudio.Stop();
  }

  private void BodyCollider()
  {
    if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
    {
      if ((double) this.scale > 0.0)
        this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
      if (Physics.SphereCast(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, Vector3.down, out this.bodyHitInfo, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, (int) this.locomotionEnabledLayers, QueryTriggerInteraction.Ignore))
      {
        this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
      }
      else
      {
        this.bodyHitInfo = this.emptyHit;
        this.bodyCollider.height = this.bodyInitialHeight;
      }
      if (!this.bodyCollider.gameObject.activeSelf)
        this.bodyCollider.gameObject.SetActive(true);
    }
    else
      this.bodyCollider.gameObject.SetActive(false);
    this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
    this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
    this.bodyOffsetVector = Vector3.down * this.bodyCollider.height / 2f;
    this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
    this.bodyCollider.transform.eulerAngles = new Vector3(0.0f, this.headCollider.transform.eulerAngles.y, 0.0f);
  }

  private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
  {
    return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
  }

  public void ScaleAwayFromPoint(float oldScale, float newScale, Vector3 scaleCenter)
  {
    if ((double) oldScale >= (double) newScale)
      return;
    this.lastHeadPosition = GTPlayer.ScalePointAwayFromCenter(this.lastHeadPosition, this.headCollider.radius, oldScale, newScale, scaleCenter);
    this.leftHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.leftHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
    this.rightHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.rightHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
  }

  private static Vector3 ScalePointAwayFromCenter(
    Vector3 point,
    float baseRadius,
    float oldScale,
    float newScale,
    Vector3 scaleCenter)
  {
    float magnitude = (point - scaleCenter).magnitude;
    float num = (float) ((double) magnitude + (double) Mathf.Epsilon + (double) baseRadius * ((double) newScale - (double) oldScale));
    return scaleCenter + (point - scaleCenter) * num / magnitude;
  }

  private void OnBeforeRenderInit()
  {
    if (Application.isPlaying && !this.hasCorrectedForTracking && (UnityEngine.Object) this.mainCamera != (UnityEngine.Object) null && this.mainCamera.transform.localPosition != Vector3.zero)
    {
      this.ForceRigidBodySync();
      this.transform.position -= this.mainCamera.transform.localPosition;
      this.hasCorrectedForTracking = true;
    }
    Application.onBeforeRender -= new UnityAction(this.OnBeforeRenderInit);
  }

  private void LateUpdate()
  {
    this.antiDriftLastPosition.GetValueOrDefault();
    if (!this.antiDriftLastPosition.HasValue)
      this.antiDriftLastPosition = new Vector3?(this.transform.position);
    if ((double) (this.antiDriftLastPosition.Value - this.transform.position).sqrMagnitude < 1E-08)
      this.transform.position = this.antiDriftLastPosition.Value;
    else
      this.antiDriftLastPosition = new Vector3?(this.transform.position);
    if (!this.hasCorrectedForTracking && this.mainCamera.transform.localPosition != Vector3.zero)
    {
      this.transform.position -= this.mainCamera.transform.localPosition;
      this.hasCorrectedForTracking = true;
      Application.onBeforeRender -= new UnityAction(this.OnBeforeRenderInit);
    }
    if (this.playerRigidBody.isKinematic)
      return;
    float time1 = Time.time;
    Vector3 position1 = this.headCollider.transform.position;
    if (this.playerRotationOverrideFrame < Time.frameCount - 1)
      this.playerRotationOverride = Quaternion.Slerp(Quaternion.identity, this.playerRotationOverride, Mathf.Exp(-this.playerRotationOverrideDecayRate * Time.deltaTime));
    this.transform.rotation = this.playerRotationOverride;
    this.turnParent.transform.localScale = VRRig.LocalRig.transform.localScale;
    this.playerRigidBody.MovePosition(this.playerRigidBody.position + position1 - this.headCollider.transform.position);
    if ((double) Mathf.Abs(this.lastScale - this.scale) > 1.0 / 1000.0)
    {
      if (this.mainCamera == null)
        this.mainCamera = Camera.main;
      this.mainCamera.nearClipPlane = (double) this.scale > 0.5 ? 0.01f : 1f / 500f;
    }
    this.lastScale = this.scale;
    this.debugLastRightHandPosition = this.rightHand.lastPosition;
    this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
    if (this.debugMovement)
    {
      this.tempRealTime = Time.time;
      this.calcDeltaTime = Time.deltaTime;
      this.lastRealTime = this.tempRealTime;
    }
    else
    {
      this.tempRealTime = Time.realtimeSinceStartup;
      this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
      this.lastRealTime = this.tempRealTime;
      if ((double) this.calcDeltaTime > 0.10000000149011612)
        this.calcDeltaTime = 0.05f;
    }
    Vector3 worldHitPoint1;
    this.refMovement = !this.lastFrameHasValidTouchPos || !((UnityEngine.Object) this.lastPlatformTouched != (UnityEngine.Object) null) || !GTPlayer.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out worldHitPoint1) ? Vector3.zero : worldHitPoint1 - this.lastFrameTouchPosWorld;
    Vector3 vector3_1 = Vector3.zero;
    Quaternion rotationDelta = Quaternion.identity;
    Vector3 pivot = this.headCollider.transform.position;
    Vector3 worldHitPoint2;
    if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeWorldHitPoint(this.lastMovingSurfaceHit, this.lastMovingSurfaceTouchLocal, out worldHitPoint2))
    {
      if (this.wasMovingSurfaceMonkeBlock && ((UnityEngine.Object) this.lastMonkeBlock == (UnityEngine.Object) null || this.lastMonkeBlock.state != BuilderPiece.State.AttachedAndPlaced))
      {
        this.movingSurfaceOffset = Vector3.zero;
      }
      else
      {
        this.movingSurfaceOffset = worldHitPoint2 - this.lastMovingSurfaceTouchWorld;
        vector3_1 = this.movingSurfaceOffset / this.calcDeltaTime;
        rotationDelta = this.lastMovingSurfaceHit.collider.transform.rotation * Quaternion.Inverse(this.lastMovingSurfaceRot);
        pivot = worldHitPoint2;
      }
    }
    else
      this.movingSurfaceOffset = Vector3.zero;
    float num1 = 40f * this.scale;
    if ((double) vector3_1.sqrMagnitude >= (double) num1 * (double) num1)
    {
      this.movingSurfaceOffset = Vector3.zero;
      vector3_1 = Vector3.zero;
      rotationDelta = Quaternion.identity;
    }
    if (!this.didAJump && (this.leftHand.wasColliding || this.rightHand.wasColliding))
    {
      this.transform.position = this.transform.position + 4.9f * Vector3.down * this.calcDeltaTime * this.calcDeltaTime * this.scale;
      if ((double) Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) <= 0.0 && (double) Vector3.Dot(Vector3.up, this.slideAverageNormal) > 0.0)
        this.transform.position = this.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, Vector3.down);
    }
    if (!this.didAJump && this.anyHandWasSliding)
    {
      this.transform.position = this.transform.position + this.slideVelocity * this.calcDeltaTime;
      this.slideVelocity += 9.8f * Vector3.down * this.calcDeltaTime * this.scale;
    }
    float paddleBoostFactor = (double) Time.time > (double) this.boostEnabledUntilTimestamp ? 0.0f : Time.deltaTime * Mathf.Clamp(this.playerRigidBody.linearVelocity.magnitude * this.hoverboardPaddleBoostMultiplier, 0.0f, this.hoverboardPaddleBoostMax);
    int divisor = 0;
    Vector3 totalMove = Vector3.zero;
    this.anyHandIsColliding = false;
    this.anyHandIsSliding = false;
    this.anyHandIsSticking = false;
    this.leftHand.FirstIteration(ref totalMove, ref divisor, paddleBoostFactor);
    this.rightHand.FirstIteration(ref totalMove, ref divisor, paddleBoostFactor);
    for (int index = 0; index < 12; ++index)
    {
      if (this.stiltStates[index].isActive)
        this.stiltStates[index].FirstIteration(ref totalMove, ref divisor, 0.0f);
    }
    if (divisor != 0)
      totalMove /= (float) divisor;
    if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT || this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT)
      totalMove += this.movingSurfaceOffset;
    else if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.BODY)
    {
      Vector3 vector3_2 = this.lastHeadPosition + this.movingSurfaceOffset - this.headCollider.transform.position;
      totalMove += vector3_2;
    }
    if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2((float) ((double) this.headCollider.radius * 0.89999997615814209 * 0.75) * this.scale, this.lastHeadPosition))
      this.lastHeadPosition = this.lastOpenHeadPosition;
    Vector3 endPosition;
    if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + totalMove - this.lastHeadPosition, Vector3.zero, out endPosition, false, out float _, out this.junkHit, true))
      totalMove = endPosition - this.headCollider.transform.position;
    if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + totalMove, true, out this.maxSphereSize1) || !this.CrazyCheck2((float) ((double) this.headCollider.radius * 0.89999997615814209 * 0.75) * this.scale, this.lastHeadPosition + totalMove))
    {
      this.lastHeadPosition = this.lastOpenHeadPosition;
      totalMove = this.lastHeadPosition - this.headCollider.transform.position;
    }
    else if ((double) this.headCollider.radius * 0.89999997615814209 * 0.824999988079071 * (double) this.scale < (double) this.maxSphereSize1)
      this.lastOpenHeadPosition = this.headCollider.transform.position + totalMove;
    if (totalMove != Vector3.zero)
      this.transform.position += totalMove;
    if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && rotationDelta != Quaternion.identity && !this.isClimbing && !this.rightHand.isHolding && !this.leftHand.isHolding)
    {
      double num2 = (double) this.RotateWithSurface(rotationDelta, pivot);
    }
    this.lastHeadPosition = this.headCollider.transform.position;
    this.areBothTouching = !this.leftHand.isColliding && !this.leftHand.wasColliding || !this.rightHand.isColliding && !this.rightHand.wasColliding;
    this.HandleHandLink();
    this.HandleTentacleMovement();
    this.anyHandIsColliding = false;
    this.anyHandIsSliding = false;
    this.anyHandIsSticking = false;
    this.leftHand.FinalizeHandPosition();
    this.rightHand.FinalizeHandPosition();
    for (int index = 0; index < 12; ++index)
    {
      if (this.stiltStates[index].isActive)
      {
        this.stiltStates[index].FinalizeHandPosition();
        GTPlayer.HandState stiltState = this.stiltStates[index];
        GorillaTagger.Instance.SetExtraHandPosition((StiltID) index, stiltState.finalPositionThisFrame, stiltState.canTag, stiltState.canStun);
      }
    }
    Vector3 lastPosition = this.lastPosition;
    GTPlayer.MovingSurfaceContactPoint surfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
    int movingSurfaceId1 = -1;
    int movingSurfaceId2 = -1;
    bool sideTouch1 = false;
    bool isMonkeBlock1 = false;
    bool isMonkeBlock2 = false;
    bool flag1 = this.rightHand.isColliding && this.IsTouchingMovingSurface(this.rightHand.GetLastPosition(), this.rightHand.lastHitInfo, out movingSurfaceId1, out sideTouch1, out isMonkeBlock1);
    if (flag1 && !sideTouch1)
    {
      surfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
      this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
    }
    else
    {
      bool sideTouch2 = false;
      BuilderPiece lastMonkeBlock = flag1 ? this.lastMonkeBlock : (BuilderPiece) null;
      if ((!this.leftHand.isColliding ? 0 : (this.IsTouchingMovingSurface(this.leftHand.GetLastPosition(), this.leftHand.lastHitInfo, out movingSurfaceId2, out sideTouch2, out isMonkeBlock2) ? 1 : 0)) != 0)
      {
        if (sideTouch2 && isMonkeBlock1 == isMonkeBlock2)
        {
          if (sideTouch1 && movingSurfaceId2.Equals(movingSurfaceId1) && (double) Vector3.Dot(this.leftHand.lastHitInfo.point - this.leftHand.GetLastPosition(), this.rightHand.lastHitInfo.point - this.rightHand.GetLastPosition()) < 0.3)
          {
            surfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
            this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
            this.lastMonkeBlock = lastMonkeBlock;
          }
        }
        else
        {
          surfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.LEFT;
          this.lastMovingSurfaceHit = this.leftHand.lastHitInfo;
        }
      }
    }
    this.StoreVelocities();
    if (this.InWater)
      PlayerGameEvents.PlayerSwam((this.lastPosition - lastPosition).magnitude, this.currentVelocity.magnitude);
    else
      PlayerGameEvents.PlayerMoved((this.lastPosition - lastPosition).magnitude, this.currentVelocity.magnitude);
    this.didAJump = false;
    bool exitMovingSurface = this.exitMovingSurface;
    this.exitMovingSurface = false;
    if (this.leftHand.IsSlipOverriddenToMax() && this.rightHand.IsSlipOverriddenToMax())
    {
      this.didAJump = true;
      this.exitMovingSurface = true;
    }
    else if (this.anyHandIsSliding)
    {
      this.slideAverageNormal = Vector3.zero;
      int num3 = 0;
      this.averageSlipPercentage = 0.0f;
      bool flag2 = false;
      if (this.leftHand.isSliding)
      {
        this.slideAverageNormal += this.leftHand.slideNormal.normalized;
        this.averageSlipPercentage += this.leftHand.slipPercentage;
        ++num3;
      }
      if (this.rightHand.isSliding)
      {
        flag2 = true;
        this.slideAverageNormal += this.rightHand.slideNormal.normalized;
        this.averageSlipPercentage += this.rightHand.slipPercentage;
        ++num3;
      }
      for (int index = 0; index < this.stiltStates.Length; ++index)
      {
        if (this.stiltStates[index].isActive && this.stiltStates[index].isSliding)
        {
          if (!this.stiltStates[index].isLeftHand)
            flag2 = true;
          this.slideAverageNormal += this.stiltStates[index].slideNormal.normalized;
          this.averageSlipPercentage += this.stiltStates[index].slipPercentage;
          ++num3;
        }
      }
      this.slideAverageNormal = this.slideAverageNormal.normalized;
      this.averageSlipPercentage /= (float) num3;
      if (num3 == 1)
      {
        this.surfaceDirection = flag2 ? Vector3.ProjectOnPlane(this.rightHand.handFollower.forward, this.rightHand.slideNormal) : Vector3.ProjectOnPlane(this.leftHand.handFollower.forward, this.leftHand.slideNormal);
        this.slideVelocity = (double) Vector3.Dot(this.slideVelocity, this.surfaceDirection) <= 0.0 ? Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, -this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl)) : Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
      }
      this.slideVelocity = this.anyHandWasSliding ? ((double) Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0.0 ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity) : ((double) Vector3.Dot(this.playerRigidBody.linearVelocity, this.slideAverageNormal) <= 0.0 ? Vector3.ProjectOnPlane(this.playerRigidBody.linearVelocity, this.slideAverageNormal) : this.playerRigidBody.linearVelocity);
      this.slideVelocity = this.slideVelocity.normalized * Mathf.Min(this.slideVelocity.magnitude, Mathf.Max(0.5f, this.averagedVelocity.magnitude * 2f));
      this.playerRigidBody.linearVelocity = Vector3.zero;
    }
    else if (this.anyHandIsColliding)
      this.playerRigidBody.linearVelocity = this.turnedThisFrame ? this.playerRigidBody.linearVelocity.normalized * Mathf.Min(2f, this.playerRigidBody.linearVelocity.magnitude) : Vector3.zero;
    else if (this.anyHandWasSliding)
      this.playerRigidBody.linearVelocity = (double) Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0.0 ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity;
    if (this.anyHandIsColliding && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
    {
      if (this.anyHandIsSliding)
      {
        if ((double) Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > (double) this.slideVelocityLimit * (double) this.scale && (double) Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) > 0.0 && (double) Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > (double) Vector3.Project(this.slideVelocity, this.slideAverageNormal).magnitude)
        {
          this.leftHand.isSliding = false;
          this.rightHand.isSliding = false;
          for (int index = 0; index < this.stiltStates.Length; ++index)
            this.stiltStates[index].isSliding = false;
          this.anyHandIsSliding = false;
          this.didAJump = true;
          float num4 = this.ApplyNativeScaleAdjustment(Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude));
          this.playerRigidBody.linearVelocity = num4 * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal);
          if ((double) num4 > (double) this.slideVelocityLimit * (double) this.scale * (double) this.exitMovingSurfaceThreshold)
            this.exitMovingSurface = true;
        }
      }
      else if ((double) this.averagedVelocity.magnitude > (double) this.velocityLimit * (double) this.scale)
      {
        float num5 = !this.InWater || !((UnityEngine.Object) this.CurrentWaterVolume != (UnityEngine.Object) null) ? 1f : this.liquidPropertiesList[(int) this.CurrentWaterVolume.LiquidType].surfaceJumpFactor;
        double num6 = (double) this.ApplyNativeScaleAdjustment(this.enableHoverMode ? Mathf.Min(this.hoverMaxPaddleSpeed, this.averagedVelocity.magnitude) : Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num5 * this.averagedVelocity.magnitude));
        Vector3 vector3_3 = (float) num6 * this.averagedVelocity.normalized;
        this.didAJump = true;
        this.playerRigidBody.linearVelocity = vector3_3;
        if (this.InWater)
          this.swimmingVelocity += vector3_3 * this.swimmingParams.underwaterJumpsAsSwimVelocityFactor;
        if (num6 > (double) this.velocityLimit * (double) this.scale * (double) this.exitMovingSurfaceThreshold)
          this.exitMovingSurface = true;
      }
    }
    this.stuckHandsCheckLateUpdate(ref this.leftHand.finalPositionThisFrame, ref this.rightHand.finalPositionThisFrame);
    if ((UnityEngine.Object) this.lastPlatformTouched != (UnityEngine.Object) null && (UnityEngine.Object) this.currentPlatform == (UnityEngine.Object) null)
    {
      if (!this.playerRigidBody.isKinematic)
        this.playerRigidBody.linearVelocity += this.refMovement / this.calcDeltaTime;
      this.refMovement = Vector3.zero;
    }
    if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE)
    {
      if (!this.playerRigidBody.isKinematic)
        this.playerRigidBody.linearVelocity += this.lastMovingSurfaceVelocity;
      this.lastMovingSurfaceVelocity = Vector3.zero;
    }
    if (this.enableHoverMode)
      this.HoverboardLateUpdate();
    else
      this.hasHoverPoint = false;
    Vector3 zero1 = Vector3.zero;
    float a1 = 0.0f;
    float a2 = 0.0f;
    if (this.bodyInWater)
    {
      Vector3 swimmingVelocityChange1;
      if (this.GetSwimmingVelocityForHand(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out swimmingVelocityChange1) && !this.turnedThisFrame)
      {
        a1 = Mathf.InverseLerp(0.0f, 0.2f, swimmingVelocityChange1.magnitude) * this.swimmingParams.swimmingHapticsStrength;
        zero1 += swimmingVelocityChange1;
      }
      Vector3 swimmingVelocityChange2;
      if (this.GetSwimmingVelocityForHand(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out swimmingVelocityChange2) && !this.turnedThisFrame)
      {
        a2 = Mathf.InverseLerp(0.0f, 0.15f, swimmingVelocityChange2.magnitude) * this.swimmingParams.swimmingHapticsStrength;
        zero1 += swimmingVelocityChange2;
      }
    }
    Vector3 zero2 = Vector3.zero;
    Vector3 jumpVelocity1;
    if ((!this.swimmingParams.allowWaterSurfaceJumps ? 0 : ((double) time1 - (double) this.lastWaterSurfaceJumpTimeLeft > (double) this.waterSurfaceJumpCooldown ? 1 : 0)) != 0 && this.CheckWaterSurfaceJump(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.leftHand.velocityTracker.GetAverageVelocity(maxTimeFromPast: 0.1f) * this.scale, this.swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out jumpVelocity1))
    {
      if ((double) time1 - (double) this.lastWaterSurfaceJumpTimeRight > (double) this.waterSurfaceJumpCooldown)
        zero2 += jumpVelocity1;
      this.lastWaterSurfaceJumpTimeLeft = Time.time;
      GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
    }
    Vector3 jumpVelocity2;
    if ((!this.swimmingParams.allowWaterSurfaceJumps ? 0 : ((double) time1 - (double) this.lastWaterSurfaceJumpTimeRight > (double) this.waterSurfaceJumpCooldown ? 1 : 0)) != 0 && this.CheckWaterSurfaceJump(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.rightHand.velocityTracker.GetAverageVelocity(maxTimeFromPast: 0.1f) * this.scale, this.swimmingParams, this.rightHandWaterVolume, this.rightHandWaterSurface, out jumpVelocity2))
    {
      if ((double) time1 - (double) this.lastWaterSurfaceJumpTimeLeft > (double) this.waterSurfaceJumpCooldown)
        zero2 += jumpVelocity2;
      this.lastWaterSurfaceJumpTimeRight = Time.time;
      GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
    }
    Vector3 vector3_4 = Vector3.ClampMagnitude(zero2, this.swimmingParams.waterSurfaceJumpMaxSpeed * this.scale);
    float amplitude1 = Mathf.Max(a1, this.leftHandNonDiveHapticsAmount);
    if ((double) amplitude1 > 1.0 / 1000.0 && (double) time1 - (double) this.lastWaterSurfaceJumpTimeLeft > (double) GorillaTagger.Instance.tapHapticDuration)
      GorillaTagger.Instance.DoVibration(XRNode.LeftHand, amplitude1, this.calcDeltaTime);
    float amplitude2 = Mathf.Max(a2, this.rightHandNonDiveHapticsAmount);
    if ((double) amplitude2 > 1.0 / 1000.0 && (double) time1 - (double) this.lastWaterSurfaceJumpTimeRight > (double) GorillaTagger.Instance.tapHapticDuration)
      GorillaTagger.Instance.DoVibration(XRNode.RightHand, amplitude2, this.calcDeltaTime);
    if (!this.disableMovement)
    {
      this.swimmingVelocity += zero1;
      if (!this.playerRigidBody.isKinematic)
        this.playerRigidBody.linearVelocity += zero1 + vector3_4;
    }
    else
      this.swimmingVelocity = Vector3.zero;
    if (GorillaGameManager.instance is GorillaFreezeTagManager)
    {
      if (!this.IsFrozen || !this.primaryButtonPressed)
      {
        this.IsBodySliding = false;
        this.lastSlopeDirection = Vector3.zero;
        if (this.bodyTouchedSurfaces.Count > 0)
        {
          foreach (KeyValuePair<GameObject, PhysicsMaterial> bodyTouchedSurface in this.bodyTouchedSurfaces)
          {
            MeshCollider component;
            if (bodyTouchedSurface.Key.TryGetComponent<MeshCollider>(out component))
              component.material = bodyTouchedSurface.Value;
          }
          this.bodyTouchedSurfaces.Clear();
        }
      }
      else if (this.BodyOnGround && this.primaryButtonPressed)
      {
        UnityEngine.RaycastHit hitInfo;
        if (Physics.SphereCast(this.bodyCollider.transform.position - new Vector3(0.0f, this.bodyInitialHeight / 2f - this.bodyInitialRadius, 0.0f), (float) ((double) this.bodyInitialRadius - 0.0099999997764825821), Vector3.down, out hitInfo, 1f, ~LayerMask.GetMask("Gorilla Body Collider", "GorillaInteractable"), QueryTriggerInteraction.Ignore))
        {
          this.IsBodySliding = true;
          MeshCollider component;
          if (!this.bodyTouchedSurfaces.ContainsKey(hitInfo.transform.gameObject) && hitInfo.transform.gameObject.TryGetComponent<MeshCollider>(out component))
          {
            this.bodyTouchedSurfaces.Add(hitInfo.transform.gameObject, component.material);
            hitInfo.transform.gameObject.GetComponent<MeshCollider>().material = this.slipperyMaterial;
          }
        }
      }
      else
      {
        this.IsBodySliding = false;
        this.lastSlopeDirection = Vector3.zero;
      }
    }
    else
    {
      this.IsBodySliding = false;
      if (this.bodyTouchedSurfaces.Count > 0)
      {
        foreach (KeyValuePair<GameObject, PhysicsMaterial> bodyTouchedSurface in this.bodyTouchedSurfaces)
        {
          MeshCollider component;
          if (bodyTouchedSurface.Key.TryGetComponent<MeshCollider>(out component))
            component.material = bodyTouchedSurface.Value;
        }
        this.bodyTouchedSurfaces.Clear();
      }
    }
    this.leftHand.OnEndOfFrame();
    this.rightHand.OnEndOfFrame();
    for (int index = 0; index < 12; ++index)
    {
      if (this.stiltStates[index].isActive)
        this.stiltStates[index].OnEndOfFrame();
    }
    this.leftHand.PositionHandFollower();
    this.rightHand.PositionHandFollower();
    this.anyHandWasSliding = this.anyHandIsSliding;
    this.anyHandWasColliding = this.anyHandIsColliding;
    this.anyHandWasSticking = this.anyHandIsSticking;
    if (this.anyHandIsSticking)
      this.lastTouchedGroundTimestamp = Time.time;
    if (PhotonNetwork.InRoom)
    {
      if (this.IsGroundedHand || this.IsThrusterActive)
      {
        this.LastHandTouchedGroundAtNetworkTime = (float) PhotonNetwork.Time;
        this.LastTouchedGroundAtNetworkTime = (float) PhotonNetwork.Time;
      }
      else if (this.IsGroundedButt)
        this.LastTouchedGroundAtNetworkTime = (float) PhotonNetwork.Time;
    }
    else
    {
      this.LastHandTouchedGroundAtNetworkTime = 0.0f;
      this.LastTouchedGroundAtNetworkTime = 0.0f;
    }
    this.degreesTurnedThisFrame = 0.0f;
    this.lastPlatformTouched = this.currentPlatform;
    this.currentPlatform = (BasePlatform) null;
    this.lastMovingSurfaceVelocity = vector3_1;
    Vector3 localHitPoint1;
    if (GTPlayer.ComputeLocalHitPoint(this.lastHitInfoHand, out localHitPoint1))
    {
      this.lastFrameHasValidTouchPos = true;
      this.lastFrameTouchPosLocal = localHitPoint1;
      this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
    }
    else
    {
      this.lastFrameHasValidTouchPos = false;
      this.lastFrameTouchPosLocal = Vector3.zero;
      this.lastFrameTouchPosWorld = Vector3.zero;
    }
    this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
    UnityEngine.RaycastHit raycastHit = this.emptyHit;
    this.BodyCollider();
    if ((UnityEngine.Object) this.bodyHitInfo.collider != (UnityEngine.Object) null)
    {
      this.wasBodyOnGround = true;
      raycastHit = this.bodyHitInfo;
    }
    else if (surfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.bodyCollider.gameObject.activeSelf)
    {
      bool flag3 = false;
      this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
      this.bufferCount = Physics.SphereCastNonAlloc(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + (this.bodyInitialHeight * this.scale - this.bodyMaxRadius) * Vector3.down, this.bodyMaxRadius, Vector3.down, this.rayCastNonAllocColliders, this.minimumRaycastDistance * this.scale, this.locomotionEnabledLayers.value);
      if (this.bufferCount > 0)
      {
        this.tempHitInfo = this.rayCastNonAllocColliders[0];
        for (int index = 0; index < this.bufferCount; ++index)
        {
          if ((double) this.tempHitInfo.distance > 0.0 && (!flag3 || (double) this.rayCastNonAllocColliders[index].distance < (double) this.tempHitInfo.distance))
          {
            flag3 = true;
            raycastHit = this.rayCastNonAllocColliders[index];
          }
        }
      }
      this.wasBodyOnGround = flag3;
    }
    int movingSurfaceId3 = -1;
    bool isMonkeBlock3 = false;
    bool sideTouch3;
    if (this.wasBodyOnGround && surfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.IsTouchingMovingSurface(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), raycastHit, out movingSurfaceId3, out sideTouch3, out isMonkeBlock3) && !sideTouch3)
    {
      surfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.BODY;
      this.lastMovingSurfaceHit = raycastHit;
    }
    Vector3 localHitPoint2;
    if (surfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeLocalHitPoint(this.lastMovingSurfaceHit, out localHitPoint2))
    {
      this.lastMovingSurfaceTouchLocal = localHitPoint2;
      this.lastMovingSurfaceTouchWorld = this.lastMovingSurfaceHit.point;
      this.lastMovingSurfaceRot = this.lastMovingSurfaceHit.collider.transform.rotation;
      this.lastAttachedToMovingSurfaceFrame = Time.frameCount;
    }
    else
    {
      surfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
      this.lastMovingSurfaceTouchLocal = Vector3.zero;
      this.lastMovingSurfaceTouchWorld = Vector3.zero;
      this.lastMovingSurfaceRot = Quaternion.identity;
    }
    Vector3 position2 = this.lastMovingSurfaceTouchWorld;
    int num7 = -1;
    bool isMonkeBlock4 = false;
    switch (surfaceContactPoint)
    {
      case GTPlayer.MovingSurfaceContactPoint.NONE:
        if (exitMovingSurface)
          this.exitMovingSurface = true;
        num7 = -1;
        break;
      case GTPlayer.MovingSurfaceContactPoint.RIGHT:
        num7 = movingSurfaceId1;
        isMonkeBlock4 = isMonkeBlock1;
        position2 = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
        break;
      case GTPlayer.MovingSurfaceContactPoint.LEFT:
        num7 = movingSurfaceId2;
        isMonkeBlock4 = isMonkeBlock2;
        position2 = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
        break;
      case GTPlayer.MovingSurfaceContactPoint.BODY:
        num7 = movingSurfaceId3;
        isMonkeBlock4 = isMonkeBlock3;
        position2 = GorillaTagger.Instance.offlineVRRig.bodyTransform.position;
        break;
    }
    if (!isMonkeBlock4)
      this.lastMonkeBlock = (BuilderPiece) null;
    if (num7 != this.lastMovingSurfaceID || this.lastMovingSurfaceContact != surfaceContactPoint || isMonkeBlock4 != this.wasMovingSurfaceMonkeBlock)
    {
      if (num7 == -1)
      {
        if (Time.frameCount - this.lastAttachedToMovingSurfaceFrame > 3)
        {
          VRRig.DetachLocalPlayerFromMovingSurface();
          this.lastMovingSurfaceID = -1;
        }
      }
      else if (isMonkeBlock4)
      {
        if ((UnityEngine.Object) this.lastMonkeBlock != (UnityEngine.Object) null)
        {
          VRRig.AttachLocalPlayerToMovingSurface(num7, surfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, surfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, this.lastMonkeBlock.transform.InverseTransformPoint(position2), isMonkeBlock4);
          this.lastMovingSurfaceID = num7;
        }
        else
        {
          VRRig.DetachLocalPlayerFromMovingSurface();
          this.lastMovingSurfaceID = -1;
        }
      }
      else if ((UnityEngine.Object) MovingSurfaceManager.instance != (UnityEngine.Object) null)
      {
        MovingSurface result;
        if (MovingSurfaceManager.instance.TryGetMovingSurface(num7, out result))
        {
          VRRig.AttachLocalPlayerToMovingSurface(num7, surfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, surfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, result.transform.InverseTransformPoint(position2), isMonkeBlock4);
          this.lastMovingSurfaceID = num7;
        }
        else
        {
          VRRig.DetachLocalPlayerFromMovingSurface();
          this.lastMovingSurfaceID = -1;
        }
      }
      else
      {
        VRRig.DetachLocalPlayerFromMovingSurface();
        this.lastMovingSurfaceID = -1;
      }
    }
    if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE && surfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE)
      this.SetPlayerVelocity(Vector3.zero);
    this.lastMovingSurfaceContact = surfaceContactPoint;
    this.wasMovingSurfaceMonkeBlock = isMonkeBlock4;
    if (this.activeSizeChangerSettings != null)
    {
      if ((double) this.activeSizeChangerSettings.ExpireOnDistance > 0.0 && (double) Vector3.Distance(this.transform.position, this.activeSizeChangerSettings.WorldPosition) > (double) this.activeSizeChangerSettings.ExpireOnDistance)
        this.SetNativeScale((NativeSizeChangerSettings) null);
      if ((double) this.activeSizeChangerSettings.ExpireAfterSeconds > 0.0 && (double) Time.time - (double) this.activeSizeChangerSettings.ActivationTime > (double) this.activeSizeChangerSettings.ExpireAfterSeconds)
        this.SetNativeScale((NativeSizeChangerSettings) null);
    }
    HandLink grabbedLink = VRRig.LocalRig.leftHandLink.grabbedLink;
    if ((UnityEngine.Object) grabbedLink != (UnityEngine.Object) null)
    {
      double time2 = PhotonNetwork.Time;
      double groundAtNetworkTime1 = (double) this.LastHandTouchedGroundAtNetworkTime;
      double time3 = PhotonNetwork.Time;
      double groundAtNetworkTime2 = (double) grabbedLink.myRig.LastHandTouchedGroundAtNetworkTime;
    }
    if (!this.didAJump && !this.anyHandIsColliding && !this.anyHandIsSliding && !this.anyHandIsSticking && !this.IsGroundedHand && !this.forceRBSync)
      return;
    this.forceRBSync = false;
  }

  private float ApplyNativeScaleAdjustment(float adjustedMagnitude)
  {
    return (double) this.nativeScale > 0.0 && (double) this.nativeScale != 1.0 ? (adjustedMagnitude *= this.nativeScaleMagnitudeAdjustmentFactor.Evaluate(this.nativeScale)) : adjustedMagnitude;
  }

  private float RotateWithSurface(Quaternion rotationDelta, Vector3 pivot)
  {
    Quaternion twist;
    QuaternionUtil.DecomposeSwingTwist(rotationDelta, Vector3.up, out Quaternion _, out twist);
    float y = twist.eulerAngles.y;
    if ((double) y > 270.0)
      y -= 360f;
    else if ((double) y > 90.0)
      y -= 180f;
    if ((double) Mathf.Abs(y) >= 90.0 * (double) this.calcDeltaTime)
      return 0.0f;
    this.turnParent.transform.RotateAround(pivot, this.transform.up, y);
    return y;
  }

  private void stuckHandsCheckFixedUpdate()
  {
    Vector3 currentHandPosition1 = this.leftHand.GetCurrentHandPosition();
    this.stuckLeft = !this.controllerState.LeftValid || this.leftHand.isColliding && (double) (currentHandPosition1 - this.leftHand.GetLastPosition()).magnitude > (double) this.unStickDistance * (double) this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition1 - this.headCollider.transform.position).normalized, (currentHandPosition1 - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value);
    Vector3 currentHandPosition2 = this.rightHand.GetCurrentHandPosition();
    this.stuckRight = !this.controllerState.RightValid || this.rightHand.isColliding && (double) (currentHandPosition2 - this.rightHand.GetLastPosition()).magnitude > (double) this.unStickDistance * (double) this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition2 - this.headCollider.transform.position).normalized, (currentHandPosition2 - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value);
  }

  private void stuckHandsCheckLateUpdate(
    ref Vector3 finalLeftHandPosition,
    ref Vector3 finalRightHandPosition)
  {
    if (this.stuckLeft)
    {
      finalLeftHandPosition = this.leftHand.GetCurrentHandPosition();
      this.stuckLeft = this.leftHand.isColliding = false;
    }
    if (!this.stuckRight)
      return;
    finalRightHandPosition = this.rightHand.GetCurrentHandPosition();
    this.stuckRight = this.rightHand.isColliding = false;
  }

  private void handleClimbing(float deltaTime)
  {
    if (this.isClimbing && (this.inOverlay || (UnityEngine.Object) this.climbHelper == (UnityEngine.Object) null || (UnityEngine.Object) this.currentClimbable == (UnityEngine.Object) null || !this.currentClimbable.isActiveAndEnabled))
      this.EndClimbing(this.currentClimber, false);
    Vector3 zero = Vector3.zero;
    if (this.isClimbing && (double) (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1.0)
      this.EndClimbing(this.currentClimber, false);
    if (!this.isClimbing)
      return;
    this.playerRigidBody.linearVelocity = Vector3.zero;
    this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
    Vector3 vector3 = this.currentClimber.transform.position - this.climbHelper.position;
    vector3 = (double) vector3.sqrMagnitude > (double) this.maxArmLength * (double) this.maxArmLength ? vector3.normalized * this.maxArmLength : vector3;
    if (this.isClimbableMoving)
    {
      double num = (double) this.RotateWithSurface(this.currentClimbable.transform.rotation * Quaternion.Inverse(this.lastClimbableRotation), this.currentClimber.handRoot.position);
      this.lastClimbableRotation = this.currentClimbable.transform.rotation;
    }
    this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector3);
    if (!(bool) (UnityEngine.Object) this.currentSwing)
      return;
    this.currentSwing.lastGrabTime = Time.time;
  }

  public void RequestTentacleMove(bool isLeftHand, Vector3 move)
  {
    if (isLeftHand)
    {
      this.hasLeftHandTentacleMove = true;
      this.leftHandTentacleMove = move;
    }
    else
    {
      this.hasRightHandTentacleMove = true;
      this.rightHandTentacleMove = move;
    }
  }

  public void HandleTentacleMovement()
  {
    Vector3 vector3;
    if (this.hasLeftHandTentacleMove)
    {
      if (this.hasRightHandTentacleMove)
      {
        vector3 = (this.leftHandTentacleMove + this.rightHandTentacleMove) * 0.5f;
        this.hasRightHandTentacleMove = this.hasLeftHandTentacleMove = false;
      }
      else
      {
        vector3 = this.leftHandTentacleMove;
        this.hasLeftHandTentacleMove = false;
      }
    }
    else
    {
      if (!this.hasRightHandTentacleMove)
        return;
      vector3 = this.rightHandTentacleMove;
      this.hasRightHandTentacleMove = false;
    }
    this.playerRigidBody.transform.position += vector3;
    this.playerRigidBody.linearVelocity = Vector3.zero;
  }

  public HandLinkAuthorityStatus GetSelfHandLinkAuthority()
  {
    int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    if (this.IsGroundedHand)
      return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded);
    if ((double) this.LastHandTouchedGroundAtNetworkTime + 1.0 > PhotonNetwork.Time)
      return new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, this.LastHandTouchedGroundAtNetworkTime, actorNumber);
    return this.IsGroundedButt ? new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded) : new HandLinkAuthorityStatus(HandLinkAuthorityType.None, this.LastTouchedGroundAtNetworkTime, actorNumber);
  }

  private void HandleHandLink()
  {
    HandLink leftHandLink = VRRig.LocalRig.leftHandLink;
    HandLink rightHandLink = VRRig.LocalRig.rightHandLink;
    bool flag1 = (UnityEngine.Object) leftHandLink.grabbedLink != (UnityEngine.Object) null;
    bool flag2 = (UnityEngine.Object) rightHandLink.grabbedLink != (UnityEngine.Object) null;
    if (!flag1 && !flag2)
      return;
    HandLinkAuthorityStatus handLinkAuthority = this.GetSelfHandLinkAuthority();
    int stepsToAuth1 = -1;
    HandLinkAuthorityStatus b1 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
    if (flag1)
      b1 = leftHandLink.GetChainAuthority(out stepsToAuth1);
    int stepsToAuth2 = -1;
    HandLinkAuthorityStatus b2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
    if (flag2)
      b2 = rightHandLink.GetChainAuthority(out stepsToAuth2);
    if (flag1 & flag2)
    {
      if (leftHandLink.grabbedPlayer == rightHandLink.grabbedPlayer)
      {
        switch (handLinkAuthority.CompareTo(b1))
        {
          case -1:
            this.HandLink_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
            break;
          case 0:
            this.HandLink_PositionBoth_BothHands(leftHandLink, rightHandLink);
            break;
          case 1:
            this.HandLink_PositionChild_RemotePlayer_BothHands(leftHandLink, rightHandLink);
            break;
        }
      }
      else
      {
        switch (handLinkAuthority.CompareTo(b1) * 3 + handLinkAuthority.CompareTo(b2))
        {
          case -3:
          case -2:
            this.HandLink_PositionChild_LocalPlayer(leftHandLink);
            this.HandLink_PositionChild_RemotePlayer(rightHandLink);
            break;
          case -1:
          case 2:
            this.HandLink_PositionChild_LocalPlayer(rightHandLink);
            this.HandLink_PositionChild_RemotePlayer(leftHandLink);
            break;
          case 0:
            this.HandLink_PositionTriple(leftHandLink, rightHandLink);
            break;
          case 1:
            this.HandLink_PositionBoth(leftHandLink);
            this.HandLink_PositionChild_RemotePlayer(rightHandLink);
            break;
          case 3:
            this.HandLink_PositionBoth(rightHandLink);
            this.HandLink_PositionChild_RemotePlayer(leftHandLink);
            break;
          case 4:
            this.HandLink_PositionChild_RemotePlayer(leftHandLink);
            this.HandLink_PositionChild_RemotePlayer(rightHandLink);
            break;
          default:
            switch (b1.CompareTo(b2))
            {
              case -1:
                this.HandLink_PositionChild_LocalPlayer(rightHandLink);
                this.HandLink_PositionChild_RemotePlayer(leftHandLink);
                return;
              case 0:
                if (stepsToAuth1 > stepsToAuth2)
                {
                  this.HandLink_PositionChild_LocalPlayer(rightHandLink);
                  this.HandLink_PositionChild_RemotePlayer(leftHandLink);
                  return;
                }
                if (stepsToAuth1 < stepsToAuth2)
                {
                  this.HandLink_PositionChild_LocalPlayer(leftHandLink);
                  this.HandLink_PositionChild_RemotePlayer(rightHandLink);
                  return;
                }
                this.HandLink_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
                return;
              case 1:
                this.HandLink_PositionChild_LocalPlayer(leftHandLink);
                this.HandLink_PositionChild_RemotePlayer(rightHandLink);
                return;
              default:
                return;
            }
        }
      }
    }
    else if (flag1)
    {
      switch (handLinkAuthority.CompareTo(b1))
      {
        case -1:
          this.HandLink_PositionChild_LocalPlayer(leftHandLink);
          break;
        case 0:
          this.HandLink_PositionBoth(leftHandLink);
          break;
        case 1:
          this.HandLink_PositionChild_RemotePlayer(leftHandLink);
          break;
      }
    }
    else
    {
      switch (handLinkAuthority.CompareTo(b2))
      {
        case -1:
          this.HandLink_PositionChild_LocalPlayer(rightHandLink);
          break;
        case 0:
          this.HandLink_PositionBoth(rightHandLink);
          break;
        case 1:
          this.HandLink_PositionChild_RemotePlayer(rightHandLink);
          break;
      }
    }
  }

  private void HandLink_PositionTriple(HandLink linkA, HandLink linkB)
  {
    Vector3 vector3_1 = linkA.transform.position - linkA.grabbedLink.transform.position;
    Vector3 vector3_2 = linkB.transform.position - linkB.grabbedLink.transform.position;
    Vector3 vector3_3 = (vector3_1 + vector3_2) * 0.33f;
    linkA.grabbedLink.myRig.TrySweptOffsetMove(vector3_1 - vector3_3, out bool _, out bool _);
    linkB.grabbedLink.myRig.TrySweptOffsetMove(vector3_2 - vector3_3, out bool _, out bool _);
    this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector3_3);
    this.playerRigidBody.linearVelocity = Vector3.zero;
  }

  private void HandLink_PositionBoth(HandLink link)
  {
    Vector3 vector3 = (link.grabbedLink.transform.position - link.transform.position) * 0.5f;
    bool handCollided;
    bool buttCollided;
    link.grabbedLink.myRig.TrySweptOffsetMove(-vector3, out handCollided, out buttCollided);
    if (handCollided | buttCollided)
      this.HandLink_PositionChild_LocalPlayer(link);
    else
      this.playerRigidBody.transform.position += vector3;
    this.playerRigidBody.linearVelocity = Vector3.zero;
  }

  private void HandLink_PositionBoth_BothHands(HandLink link1, HandLink link2)
  {
    Vector3 vector3 = ((link1.grabbedLink.transform.position - link1.transform.position) * 0.5f + (link2.grabbedLink.transform.position - link2.transform.position) * 0.5f) * 0.5f;
    bool handCollided;
    bool buttCollided;
    link1.grabbedLink.myRig.TrySweptOffsetMove(-vector3, out handCollided, out buttCollided);
    if (handCollided | buttCollided)
      this.HandLink_PositionChild_LocalPlayer(link1, link2);
    else
      this.playerRigidBody.transform.position += vector3;
    this.playerRigidBody.linearVelocity = Vector3.zero;
  }

  private void HandLink_PositionChild_LocalPlayer(HandLink parentLink)
  {
    this.playerRigidBody.transform.position += parentLink.grabbedLink.transform.position - parentLink.transform.position;
    this.playerRigidBody.linearVelocity = Vector3.zero;
  }

  private void HandLink_PositionChild_LocalPlayer(HandLink linkA, HandLink linkB)
  {
    this.playerRigidBody.transform.position += (linkA.grabbedLink.transform.position - linkA.transform.position + (linkB.grabbedLink.transform.position - linkB.transform.position)) * 0.5f;
    this.playerRigidBody.linearVelocity = Vector3.zero;
  }

  private void HandLink_PositionChild_RemotePlayer(HandLink childLink)
  {
    Vector3 movement = childLink.transform.position - childLink.grabbedLink.transform.position;
    bool handCollided;
    bool buttCollided;
    childLink.grabbedLink.myRig.TrySweptOffsetMove(movement, out handCollided, out buttCollided);
    if (!(handCollided | buttCollided))
      return;
    this.HandLink_PositionChild_LocalPlayer(childLink);
  }

  private void HandLink_PositionChild_RemotePlayer_BothHands(
    HandLink childLink1,
    HandLink childLink2)
  {
    Vector3 movement = (childLink1.transform.position - childLink1.grabbedLink.transform.position + (childLink2.transform.position - childLink2.grabbedLink.transform.position)) * 0.5f;
    bool handCollided;
    bool buttCollided;
    childLink1.grabbedLink.myRig.TrySweptOffsetMove(movement, out handCollided, out buttCollided);
    if (!(handCollided | buttCollided))
      return;
    this.HandLink_PositionChild_LocalPlayer(childLink1, childLink2);
  }

  private bool IterativeCollisionSphereCast(
    Vector3 startPosition,
    float sphereRadius,
    Vector3 movementVector,
    Vector3 boostVector,
    out Vector3 endPosition,
    bool singleHand,
    out float slipPercentage,
    out UnityEngine.RaycastHit iterativeHitInfo,
    bool fullSlide)
  {
    slipPercentage = this.defaultSlideFactor;
    if (this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
    {
      this.firstPosition = endPosition;
      iterativeHitInfo = this.tempIterativeHit;
      this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
      slipPercentage = (double) this.slideFactor != (double) this.defaultSlideFactor ? this.slideFactor : (!singleHand ? this.defaultSlideFactor : 1f / 1000f);
      if (fullSlide)
        slipPercentage = 1f;
      this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
      Vector3 movementVector1 = Vector3.zero;
      if (boostVector.IsLongerThan(0.0f))
      {
        movementVector1 = Vector3.ProjectOnPlane(boostVector, iterativeHitInfo.normal);
        this.movementToProjectedAboveCollisionPlane += movementVector1;
        this.CollisionsSphereCast(this.firstPosition, sphereRadius, movementVector1, out endPosition, out this.tempIterativeHit);
        this.firstPosition = endPosition;
      }
      if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
      {
        iterativeHitInfo = this.tempIterativeHit;
        return true;
      }
      if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector + movementVector1 - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
      {
        iterativeHitInfo = this.tempIterativeHit;
        return true;
      }
      endPosition = Vector3.zero;
      return false;
    }
    iterativeHitInfo = this.tempIterativeHit;
    endPosition = Vector3.zero;
    return false;
  }

  private bool CollisionsSphereCast(
    Vector3 startPosition,
    float sphereRadius,
    Vector3 movementVector,
    out Vector3 finalPosition,
    out UnityEngine.RaycastHit collisionsHitInfo)
  {
    this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
    bool flag = false;
    this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
    this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
    if (this.bufferCount > 0)
    {
      this.tempHitInfo = this.rayCastNonAllocColliders[0];
      for (int index = 0; index < this.bufferCount; ++index)
      {
        if ((double) this.tempHitInfo.distance > 0.0 && (!flag || (double) this.rayCastNonAllocColliders[index].distance < (double) this.tempHitInfo.distance))
        {
          flag = true;
          this.tempHitInfo = this.rayCastNonAllocColliders[index];
        }
      }
    }
    if (flag)
    {
      collisionsHitInfo = this.tempHitInfo;
      finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
      this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
      this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
      if (this.bufferCount > 0)
      {
        this.tempHitInfo = this.rayCastNonAllocColliders[0];
        for (int index = 0; index < this.bufferCount; ++index)
        {
          if ((bool) (UnityEngine.Object) this.rayCastNonAllocColliders[index].collider && (double) this.rayCastNonAllocColliders[index].distance < (double) this.tempHitInfo.distance)
            this.tempHitInfo = this.rayCastNonAllocColliders[index];
        }
        finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
      }
      this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
      this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
      this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
      if (this.bufferCount > 0)
      {
        this.tempHitInfo = this.rayCastNonAllocColliders[0];
        for (int index = 0; index < this.bufferCount; ++index)
        {
          if ((UnityEngine.Object) this.rayCastNonAllocColliders[index].collider != (UnityEngine.Object) null && (double) this.rayCastNonAllocColliders[index].distance < (double) this.tempHitInfo.distance)
            this.tempHitInfo = this.rayCastNonAllocColliders[index];
        }
        finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
        collisionsHitInfo = this.tempHitInfo;
      }
      return true;
    }
    this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
    this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
    if (this.bufferCount > 0)
    {
      this.tempHitInfo = this.rayCastNonAllocColliders[0];
      for (int index = 0; index < this.bufferCount; ++index)
      {
        if ((UnityEngine.Object) this.rayCastNonAllocColliders[index].collider != (UnityEngine.Object) null && (double) this.rayCastNonAllocColliders[index].distance < (double) this.tempHitInfo.distance)
          this.tempHitInfo = this.rayCastNonAllocColliders[index];
      }
      collisionsHitInfo = this.tempHitInfo;
      finalPosition = startPosition;
      return true;
    }
    finalPosition = startPosition + movementVector;
    collisionsHitInfo = new UnityEngine.RaycastHit();
    return false;
  }

  public float GetSlidePercentage(UnityEngine.RaycastHit raycastHit)
  {
    if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
      return this.FreezeTagSlidePercentage();
    this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
    BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      this.currentPlatform = component;
    if ((UnityEngine.Object) this.currentOverride != (UnityEngine.Object) null)
    {
      if ((double) this.currentOverride.slidePercentageOverride >= 0.0)
        return this.currentOverride.slidePercentageOverride;
      this.currentMaterialIndex = this.currentOverride.overrideIndex;
      return this.currentMaterialIndex >= 0 && this.currentMaterialIndex < this.materialData.Count && this.materialData[this.currentMaterialIndex].overrideSlidePercent ? this.materialData[this.currentMaterialIndex].slidePercent : this.defaultSlideFactor;
    }
    this.meshCollider = raycastHit.collider as MeshCollider;
    if ((UnityEngine.Object) this.meshCollider == (UnityEngine.Object) null || (UnityEngine.Object) this.meshCollider.sharedMesh == (UnityEngine.Object) null || this.meshCollider.convex)
      return this.defaultSlideFactor;
    this.collidedMesh = this.meshCollider.sharedMesh;
    if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, out this.sharedMeshTris))
    {
      this.sharedMeshTris = this.collidedMesh.triangles;
      this.meshTrianglesDict.Add(this.collidedMesh, (int[]) this.sharedMeshTris.Clone());
    }
    this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
    this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
    this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
    this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
    if ((UnityEngine.Object) this.slideRenderer != (UnityEngine.Object) null)
      this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
    else
      this.tempMaterialArray.Clear();
    if (this.tempMaterialArray.Count > 1)
    {
      for (int index1 = 0; index1 < this.tempMaterialArray.Count; ++index1)
      {
        this.collidedMesh.GetTriangles(this.trianglesList, index1);
        for (int index2 = 0; index2 < this.trianglesList.Count; index2 += 3)
        {
          if (this.trianglesList[index2] == this.vertex1 && this.trianglesList[index2 + 1] == this.vertex2 && this.trianglesList[index2 + 2] == this.vertex3)
          {
            this.findMatName = this.tempMaterialArray[index1].name;
            if (this.findMatName.EndsWith("Uber"))
            {
              string findMatName = this.findMatName;
              this.findMatName = findMatName.Substring(0, findMatName.Length - 4);
            }
            this.foundMatData = this.materialData.Find((Predicate<GTPlayer.MaterialData>) (matData => matData.matName == this.findMatName));
            this.currentMaterialIndex = this.materialData.FindIndex((Predicate<GTPlayer.MaterialData>) (matData => matData.matName == this.findMatName));
            if (this.currentMaterialIndex == -1)
              this.currentMaterialIndex = 0;
            return !this.foundMatData.overrideSlidePercent ? this.defaultSlideFactor : this.foundMatData.slidePercent;
          }
        }
      }
    }
    else if (this.tempMaterialArray.Count > 0)
      return this.defaultSlideFactor;
    this.currentMaterialIndex = 0;
    return this.defaultSlideFactor;
  }

  public bool IsTouchingMovingSurface(
    Vector3 rayOrigin,
    UnityEngine.RaycastHit raycastHit,
    out int movingSurfaceId,
    out bool sideTouch,
    out bool isMonkeBlock)
  {
    movingSurfaceId = -1;
    sideTouch = false;
    isMonkeBlock = false;
    float num = Vector3.Dot(rayOrigin - raycastHit.point, Vector3.up);
    if ((double) num < -0.30000001192092896)
      return false;
    if ((double) num < 0.0)
      sideTouch = true;
    if ((UnityEngine.Object) raycastHit.collider == (UnityEngine.Object) null)
      return false;
    MovingSurface component = raycastHit.collider.GetComponent<MovingSurface>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      isMonkeBlock = false;
      movingSurfaceId = component.GetID();
      return true;
    }
    if (!BuilderTable.IsLocalPlayerInBuilderZone())
      return false;
    BuilderPiece pieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
    if ((UnityEngine.Object) pieceFromCollider != (UnityEngine.Object) null && pieceFromCollider.IsPieceMoving())
    {
      isMonkeBlock = true;
      movingSurfaceId = pieceFromCollider.pieceId;
      this.lastMonkeBlock = pieceFromCollider;
      return true;
    }
    sideTouch = false;
    return false;
  }

  public void Turn(float degrees)
  {
    Vector3 position = this.headCollider.transform.position;
    bool flag1 = this.rightHand.isColliding || this.rightHand.isHolding;
    bool flag2 = this.leftHand.isColliding || this.leftHand.isHolding;
    if (flag1 != flag2 & flag1)
      position = this.rightHand.controllerTransform.position;
    if (flag1 != flag2 & flag2)
      position = this.leftHand.controllerTransform.position;
    this.turnParent.transform.RotateAround(position, this.transform.up, degrees);
    this.degreesTurnedThisFrame = degrees;
    this.averagedVelocity = Vector3.zero;
    for (int index = 0; index < this.velocityHistory.Length; ++index)
    {
      this.velocityHistory[index] = Quaternion.Euler(0.0f, degrees, 0.0f) * this.velocityHistory[index];
      this.averagedVelocity += this.velocityHistory[index];
    }
    this.averagedVelocity /= (float) this.velocityHistorySize;
  }

  public void BeginClimbing(
    GorillaClimbable climbable,
    GorillaHandClimber hand,
    GorillaClimbableRef climbableRef = null)
  {
    if ((UnityEngine.Object) this.currentClimber != (UnityEngine.Object) null)
      this.EndClimbing(this.currentClimber, true);
    try
    {
      Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
      if (onBeforeClimb != null)
        onBeforeClimb(hand, climbableRef);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ex);
    }
    climbable.TryGetComponent<Rigidbody>(out Rigidbody _);
    this.VerifyClimbHelper();
    this.climbHelper.SetParent(climbable.transform);
    this.climbHelper.position = hand.transform.position;
    Vector3 localPosition = this.climbHelper.localPosition;
    if (climbable.snapX)
      SnapAxis(ref localPosition.x, climbable.maxDistanceSnap);
    if (climbable.snapY)
      SnapAxis(ref localPosition.y, climbable.maxDistanceSnap);
    if (climbable.snapZ)
      SnapAxis(ref localPosition.z, climbable.maxDistanceSnap);
    this.climbHelperTargetPos = localPosition;
    climbable.isBeingClimbed = true;
    hand.isClimbing = true;
    this.currentClimbable = climbable;
    this.currentClimber = hand;
    this.isClimbing = true;
    if (climbable.climbOnlyWhileSmall)
    {
      BuilderPiece componentInParent = climbable.GetComponentInParent<BuilderPiece>();
      if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null && componentInParent.IsPieceMoving())
      {
        this.isClimbableMoving = true;
        this.lastClimbableRotation = climbable.transform.rotation;
      }
      else
        this.isClimbableMoving = false;
    }
    else
      this.isClimbableMoving = false;
    GorillaRopeSegment component1;
    if (climbable.TryGetComponent<GorillaRopeSegment>(out component1) && (bool) (UnityEngine.Object) component1.swing)
    {
      this.currentSwing = component1.swing;
      this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.averagedVelocity);
    }
    else
    {
      GorillaZipline component2;
      if ((bool) (UnityEngine.Object) climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out component2))
      {
        this.currentZipline = component2;
      }
      else
      {
        PhotonView component3;
        if (climbable.TryGetComponent<PhotonView>(out component3))
        {
          VRRig.AttachLocalPlayerToPhotonView(component3, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
        }
        else
        {
          PhotonViewXSceneRef component4;
          if (climbable.TryGetComponent<PhotonViewXSceneRef>(out component4))
            VRRig.AttachLocalPlayerToPhotonView(component4.photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
        }
      }
    }
    GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
    if (!(bool) (UnityEngine.Object) climbable.clip)
      return;
    GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);

    static void SnapAxis(ref float val, float maxDist)
    {
      if ((double) val > (double) maxDist)
      {
        val = maxDist;
      }
      else
      {
        if ((double) val >= -(double) maxDist)
          return;
        val = -maxDist;
      }
    }
  }

  private void VerifyClimbHelper()
  {
    if (!((UnityEngine.Object) this.climbHelper == (UnityEngine.Object) null) && !((UnityEngine.Object) this.climbHelper.gameObject == (UnityEngine.Object) null))
      return;
    this.climbHelper = new GameObject("Climb Helper").transform;
  }

  public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
  {
    if ((UnityEngine.Object) hand != (UnityEngine.Object) this.currentClimber)
      return;
    hand.SetCanRelease(true);
    if (!startingNewClimb)
      this.enablePlayerGravity(true);
    Rigidbody component = (Rigidbody) null;
    if ((bool) (UnityEngine.Object) this.currentClimbable)
    {
      this.currentClimbable.TryGetComponent<Rigidbody>(out component);
      this.currentClimbable.isBeingClimbed = false;
    }
    Vector3 force = Vector3.zero;
    if ((bool) (UnityEngine.Object) this.currentClimber)
    {
      this.currentClimber.isClimbing = false;
      this.currentClimber.dontReclimbLast = !doDontReclimb ? (GorillaClimbable) null : this.currentClimbable;
      this.currentClimber.queuedToBecomeValidToGrabAgain = true;
      this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
      if (!startingNewClimb && (bool) (UnityEngine.Object) this.currentClimbable)
      {
        GorillaVelocityTracker pointVelocityTracker = this.GetInteractPointVelocityTracker(this.currentClimber.xrNode == XRNode.LeftHand);
        this.playerRigidBody.linearVelocity = !(bool) (UnityEngine.Object) component ? (!(bool) (UnityEngine.Object) this.currentSwing ? (!(bool) (UnityEngine.Object) this.currentZipline ? Vector3.zero : this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed) : this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f)) : component.linearVelocity;
        force = Vector3.ClampMagnitude(this.turnParent.transform.rotation * -pointVelocityTracker.GetAverageVelocity(maxTimeFromPast: 0.1f, doMagnitudeCheck: true) * this.scale, 5.5f * this.scale);
        this.playerRigidBody.AddForce(force, ForceMode.VelocityChange);
      }
    }
    if ((bool) (UnityEngine.Object) this.currentSwing)
      this.currentSwing.DetachLocalPlayer();
    if (this.currentClimbable.TryGetComponent<PhotonView>(out PhotonView _) || this.currentClimbable.TryGetComponent<PhotonViewXSceneRef>(out PhotonViewXSceneRef _) || this.currentClimbable.IsPlayerAttached)
      VRRig.DetachLocalPlayerFromPhotonView();
    if (!startingNewClimb && (double) force.magnitude > 2.0 && (bool) (UnityEngine.Object) this.currentClimbable && (bool) (UnityEngine.Object) this.currentClimbable.clipOnFullRelease)
      GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
    this.currentClimbable = (GorillaClimbable) null;
    this.currentClimber = (GorillaHandClimber) null;
    this.currentSwing = (GorillaRopeSwing) null;
    this.currentZipline = (GorillaZipline) null;
    this.isClimbing = false;
  }

  public void ResetRigidbodyInterpolation()
  {
    this.playerRigidBody.interpolation = this.playerRigidbodyInterpolationDefault;
  }

  public RigidbodyInterpolation RigidbodyInterpolation
  {
    get => this.playerRigidBody.interpolation;
    set => this.playerRigidBody.interpolation = value;
  }

  private void enablePlayerGravity(bool useGravity) => this.playerRigidBody.useGravity = useGravity;

  public void SetVelocity(Vector3 velocity) => this.playerRigidBody.linearVelocity = velocity;

  internal void RigidbodyMovePosition(Vector3 pos) => this.playerRigidBody.MovePosition(pos);

  public void TempFreezeHand(bool isLeft, float freezeDuration)
  {
    (isLeft ? this.leftHand : this.rightHand).TempFreezeHand(freezeDuration);
  }

  private void StoreVelocities()
  {
    this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
    this.currentVelocity = (this.transform.position - this.lastPosition - this.MovingSurfaceMovement()) / this.calcDeltaTime;
    this.velocityHistory[this.velocityIndex] = this.currentVelocity;
    this.averagedVelocity = ((IEnumerable<Vector3>) this.velocityHistory).Average();
    this.lastPosition = this.transform.position;
  }

  private void AntiTeleportTechnology()
  {
    Vector3 vector3 = this.headCollider.transform.position - this.lastHeadPosition;
    double magnitude = (double) vector3.magnitude;
    double teleportThresholdNoVel = (double) this.teleportThresholdNoVel;
    vector3 = this.playerRigidBody.linearVelocity;
    double num1 = (double) vector3.magnitude * (double) this.calcDeltaTime;
    double num2 = teleportThresholdNoVel + num1;
    if (magnitude < num2)
      return;
    this.ForceRigidBodySync();
    this.transform.position = this.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
  }

  private bool MaxSphereSizeForNoOverlap(
    float testRadius,
    Vector3 checkPosition,
    bool ignoreOneWay,
    out float overlapRadiusTest)
  {
    overlapRadiusTest = testRadius;
    this.overlapAttempts = 0;
    for (int index1 = 100; this.overlapAttempts < index1 && (double) overlapRadiusTest > (double) testRadius * 0.75; ++this.overlapAttempts)
    {
      this.ClearColliderBuffer(ref this.overlapColliders);
      this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
      if (ignoreOneWay)
      {
        int num = 0;
        for (int index2 = 0; index2 < this.bufferCount; ++index2)
        {
          if (this.overlapColliders[index2].CompareTag("NoCrazyCheck"))
            ++num;
        }
        if (num == this.bufferCount)
          return true;
      }
      if (this.bufferCount > 0)
      {
        overlapRadiusTest = Mathf.Lerp(testRadius, 0.0f, (float) this.overlapAttempts / (float) index1);
      }
      else
      {
        overlapRadiusTest *= 0.995f;
        return true;
      }
    }
    return false;
  }

  private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
  {
    for (int index = 0; index < this.crazyCheckVectors.Length; ++index)
    {
      if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[index] * sphereSize) > 0)
        return false;
    }
    return true;
  }

  private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
  {
    Vector3 direction = endPosition - startPosition;
    int num1 = Physics.RaycastNonAlloc(startPosition, direction, this.rayCastNonAllocColliders, direction.magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
    int num2 = 0;
    for (int index = 0; index < num1; ++index)
    {
      if (!this.rayCastNonAllocColliders[index].collider.gameObject.CompareTag("NoCrazyCheck"))
        ++num2;
    }
    return num2;
  }

  private void ClearColliderBuffer(ref Collider[] colliders)
  {
    for (int index = 0; index < colliders.Length; ++index)
      colliders[index] = (Collider) null;
  }

  private void ClearRaycasthitBuffer(ref UnityEngine.RaycastHit[] raycastHits)
  {
    for (int index = 0; index < raycastHits.Length; ++index)
      raycastHits[index] = this.emptyHit;
  }

  private Vector3 MovingSurfaceMovement() => this.refMovement + this.movingSurfaceOffset;

  private static bool ComputeLocalHitPoint(UnityEngine.RaycastHit hit, out Vector3 localHitPoint)
  {
    if ((UnityEngine.Object) hit.collider == (UnityEngine.Object) null || (double) hit.point.sqrMagnitude < 1.0 / 1000.0)
    {
      localHitPoint = Vector3.zero;
      return false;
    }
    localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
    return true;
  }

  private static bool ComputeWorldHitPoint(
    UnityEngine.RaycastHit hit,
    Vector3 localPoint,
    out Vector3 worldHitPoint)
  {
    if ((UnityEngine.Object) hit.collider == (UnityEngine.Object) null)
    {
      worldHitPoint = Vector3.zero;
      return false;
    }
    worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
    return true;
  }

  private float ExtraVelMultiplier()
  {
    float a = 1f;
    if ((UnityEngine.Object) this.leftHand.surfaceOverride != (UnityEngine.Object) null)
      a = Mathf.Max(a, this.leftHand.surfaceOverride.extraVelMultiplier);
    if ((UnityEngine.Object) this.rightHand.surfaceOverride != (UnityEngine.Object) null)
      a = Mathf.Max(a, this.rightHand.surfaceOverride.extraVelMultiplier);
    return a;
  }

  private float ExtraVelMaxMultiplier()
  {
    float a = 1f;
    if ((UnityEngine.Object) this.leftHand.surfaceOverride != (UnityEngine.Object) null)
      a = Mathf.Max(a, this.leftHand.surfaceOverride.extraVelMaxMultiplier);
    if ((UnityEngine.Object) this.rightHand.surfaceOverride != (UnityEngine.Object) null)
      a = Mathf.Max(a, this.rightHand.surfaceOverride.extraVelMaxMultiplier);
    return a * this.scale;
  }

  public void SetMaximumSlipThisFrame()
  {
    this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
    this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
  }

  public void SetLeftMaximumSlipThisFrame() => this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;

  public void SetRightMaximumSlipThisFrame()
  {
    this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
  }

  public void ChangeLayer(string layerName)
  {
    if (!((UnityEngine.Object) this.layerChanger != (UnityEngine.Object) null))
      return;
    this.layerChanger.ChangeLayer(this.transform.parent, layerName);
  }

  public void RestoreLayer()
  {
    if (!((UnityEngine.Object) this.layerChanger != (UnityEngine.Object) null))
      return;
    this.layerChanger.RestoreOriginalLayers();
  }

  public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
  {
    if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireInWater)
      this.SetNativeScale((NativeSizeChangerSettings) null);
    if ((UnityEngine.Object) playerCollider == (UnityEngine.Object) this.headCollider)
    {
      if (this.headOverlappingWaterVolumes.Contains(volume))
        return;
      this.headOverlappingWaterVolumes.Add(volume);
    }
    else
    {
      if (!((UnityEngine.Object) playerCollider == (UnityEngine.Object) this.bodyCollider) || this.bodyOverlappingWaterVolumes.Contains(volume))
        return;
      this.bodyOverlappingWaterVolumes.Add(volume);
    }
  }

  public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
  {
    if ((UnityEngine.Object) playerCollider == (UnityEngine.Object) this.headCollider)
    {
      this.headOverlappingWaterVolumes.Remove(volume);
    }
    else
    {
      if (!((UnityEngine.Object) playerCollider == (UnityEngine.Object) this.bodyCollider))
        return;
      this.bodyOverlappingWaterVolumes.Remove(volume);
    }
  }

  private bool GetSwimmingVelocityForHand(
    Vector3 startingHandPosition,
    Vector3 endingHandPosition,
    Vector3 palmForwardDirection,
    float dt,
    ref WaterVolume contactingWaterVolume,
    ref WaterVolume.SurfaceQuery waterSurface,
    out Vector3 swimmingVelocityChange)
  {
    contactingWaterVolume = (WaterVolume) null;
    this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, QueryTriggerInteraction.Collide);
    if (this.bufferCount > 0)
    {
      float num = float.MinValue;
      for (int index = 0; index < this.bufferCount; ++index)
      {
        WaterVolume component = this.overlapColliders[index].GetComponent<WaterVolume>();
        WaterVolume.SurfaceQuery result;
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && component.GetSurfaceQueryForPoint(endingHandPosition, out result) && (double) result.surfacePoint.y > (double) num)
        {
          num = result.surfacePoint.y;
          contactingWaterVolume = component;
          waterSurface = result;
        }
      }
    }
    if ((UnityEngine.Object) contactingWaterVolume != (UnityEngine.Object) null)
    {
      Vector3 vector3_1 = endingHandPosition - startingHandPosition;
      Vector3 vector3_2 = Vector3.zero;
      Vector3 vector3_3 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
      if (this.turnedThisFrame)
      {
        Vector3 vector3_4 = startingHandPosition - this.headCollider.transform.position;
        vector3_2 = Quaternion.AngleAxis(this.degreesTurnedThisFrame, Vector3.up) * vector3_4 - vector3_4;
      }
      Vector3 vector3_5 = vector3_2;
      float num1 = Vector3.Dot(vector3_1 - vector3_5 - vector3_3, palmForwardDirection);
      float num2 = 0.0f;
      if ((double) num1 > 0.0)
      {
        Plane surfacePlane = waterSurface.surfacePlane;
        float distanceToPoint1 = surfacePlane.GetDistanceToPoint(startingHandPosition);
        float distanceToPoint2 = surfacePlane.GetDistanceToPoint(endingHandPosition);
        if ((double) distanceToPoint1 <= 0.0 && (double) distanceToPoint2 <= 0.0)
          num2 = 1f;
        else if ((double) distanceToPoint1 > 0.0 && (double) distanceToPoint2 <= 0.0)
          num2 = (float) (-(double) distanceToPoint2 / ((double) distanceToPoint1 - (double) distanceToPoint2));
        else if ((double) distanceToPoint1 <= 0.0 && (double) distanceToPoint2 > 0.0)
          num2 = (float) (-(double) distanceToPoint1 / ((double) distanceToPoint2 - (double) distanceToPoint1));
        if ((double) num2 > (double) Mathf.Epsilon)
        {
          float resistance = this.liquidPropertiesList[(int) contactingWaterVolume.LiquidType].resistance;
          swimmingVelocityChange = -palmForwardDirection * num1 * 2f * resistance * num2;
          Vector3 forward = this.mainCamera.transform.forward;
          if ((double) forward.y < 0.0)
          {
            Vector3 rhs = forward.x0z();
            float magnitude = rhs.magnitude;
            rhs /= magnitude;
            float num3 = Vector3.Dot(swimmingVelocityChange, rhs);
            if ((double) num3 > 0.0)
            {
              Vector3 vector3_6 = rhs * num3;
              swimmingVelocityChange = swimmingVelocityChange - vector3_6 + vector3_6 * magnitude + Vector3.up * forward.y * num3;
            }
          }
          return true;
        }
      }
    }
    swimmingVelocityChange = Vector3.zero;
    return false;
  }

  private bool CheckWaterSurfaceJump(
    Vector3 startingHandPosition,
    Vector3 endingHandPosition,
    Vector3 palmForwardDirection,
    Vector3 handAvgVelocity,
    PlayerSwimmingParameters parameters,
    WaterVolume contactingWaterVolume,
    WaterVolume.SurfaceQuery waterSurface,
    out Vector3 jumpVelocity)
  {
    if ((UnityEngine.Object) contactingWaterVolume != (UnityEngine.Object) null)
    {
      Plane surfacePlane = waterSurface.surfacePlane;
      bool flag = (double) handAvgVelocity.sqrMagnitude > (double) parameters.waterSurfaceJumpHandSpeedThreshold * (double) parameters.waterSurfaceJumpHandSpeedThreshold;
      if (((!surfacePlane.GetSide(startingHandPosition) ? 0 : (!surfacePlane.GetSide(endingHandPosition) ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      {
        float num1 = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
        float num2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
        float num3 = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(num1, 0.01f, 0.99f));
        float num4 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(num2, 0.01f, 0.99f));
        jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * num3 * num4;
        return true;
      }
    }
    jumpVelocity = Vector3.zero;
    return false;
  }

  private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
  {
    magnitude = input.magnitude;
    if ((double) magnitude > (double) eps)
    {
      normalized = input / magnitude;
      return true;
    }
    normalized = Vector3.zero;
    return false;
  }

  private bool TryNormalizeDown(
    Vector3 input,
    out Vector3 normalized,
    out float magnitude,
    float eps = 0.0001f)
  {
    magnitude = input.magnitude;
    if ((double) magnitude > 1.0)
    {
      normalized = input / magnitude;
      return true;
    }
    if ((double) magnitude >= (double) eps)
    {
      normalized = input;
      return true;
    }
    normalized = Vector3.zero;
    return false;
  }

  private float FreezeTagSlidePercentage()
  {
    return this.materialData[this.currentMaterialIndex].overrideSlidePercent && (double) this.materialData[this.currentMaterialIndex].slidePercent > (double) this.freezeTagHandSlidePercent ? this.materialData[this.currentMaterialIndex].slidePercent : this.freezeTagHandSlidePercent;
  }

  private void OnCollisionStay(UnityEngine.Collision collision)
  {
    this.bodyCollisionContactsCount = collision.GetContacts(this.bodyCollisionContacts);
    float num1 = -1f;
    for (int index = 0; index < this.bodyCollisionContactsCount; ++index)
    {
      float num2 = Vector3.Dot(this.bodyCollisionContacts[index].normal, Vector3.up);
      if ((double) num2 > (double) num1)
      {
        this.bodyGroundContact = this.bodyCollisionContacts[index];
        num1 = num2;
      }
    }
    float num3 = 0.5f;
    if ((double) num1 <= (double) num3)
      return;
    this.bodyGroundContactTime = Time.time;
  }

  public async void DoLaunch(Vector3 velocity)
  {
    if (this.isClimbing)
      this.EndClimbing(this.CurrentClimber, false);
    this.playerRigidBody.linearVelocity = velocity;
    this.disableMovement = true;
    await Task.Delay(1);
    this.disableMovement = false;
  }

  private void OnEnable()
  {
    RoomSystem.JoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.JoinedRoomEvent + new Action(this.OnJoinedRoom);
  }

  private void OnJoinedRoom()
  {
    if (this.activeSizeChangerSettings == null || !this.activeSizeChangerSettings.ExpireOnRoomJoin)
      return;
    this.SetNativeScale((NativeSizeChangerSettings) null);
  }

  private void OnDisable()
  {
    RoomSystem.JoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.JoinedRoomEvent - new Action(this.OnJoinedRoom);
  }

  public void ForceRigidBodySync() => this.forceRBSync = true;

  internal void ClearHandHolds()
  {
    this.leftHand.isHolding = false;
    this.rightHand.isHolding = false;
    this.wasHoldingHandhold = false;
    this.activeHandHold = new GTPlayer.HandHoldState();
    this.secondaryHandHold = new GTPlayer.HandHoldState();
    this.OnChangeActiveHandhold();
  }

  internal void AddHandHold(
    Transform objectHeld,
    Vector3 localPositionHeld,
    GorillaGrabber grabber,
    bool forLeftHand,
    bool rotatePlayerWhenHeld,
    out Vector3 grabbedVelocity)
  {
    if (!this.leftHand.isHolding && !this.rightHand.isHolding)
    {
      grabbedVelocity = -this.bodyCollider.attachedRigidbody.linearVelocity;
      this.playerRigidBody.AddForce(grabbedVelocity, ForceMode.VelocityChange);
    }
    else
      grabbedVelocity = Vector3.zero;
    this.secondaryHandHold = this.activeHandHold;
    Vector3 position = grabber.transform.position;
    this.activeHandHold = new GTPlayer.HandHoldState()
    {
      grabber = grabber,
      objectHeld = objectHeld,
      localPositionHeld = localPositionHeld,
      localRotationalOffset = grabber.transform.rotation.eulerAngles.y - objectHeld.rotation.eulerAngles.y,
      applyRotation = rotatePlayerWhenHeld
    };
    if (forLeftHand)
      this.leftHand.isHolding = true;
    else
      this.rightHand.isHolding = true;
    this.OnChangeActiveHandhold();
  }

  internal void RemoveHandHold(GorillaGrabber grabber, bool forLeftHand)
  {
    int num = (UnityEngine.Object) this.activeHandHold.objectHeld == (UnityEngine.Object) grabber ? 1 : 0;
    if ((UnityEngine.Object) this.activeHandHold.grabber == (UnityEngine.Object) grabber)
      this.activeHandHold = this.secondaryHandHold;
    this.secondaryHandHold = new GTPlayer.HandHoldState();
    if (forLeftHand)
      this.leftHand.isHolding = false;
    else
      this.rightHand.isHolding = false;
    this.OnChangeActiveHandhold();
  }

  private void OnChangeActiveHandhold()
  {
    if ((UnityEngine.Object) this.activeHandHold.objectHeld != (UnityEngine.Object) null)
    {
      PhotonView component1;
      if (this.activeHandHold.objectHeld.TryGetComponent<PhotonView>(out component1))
      {
        VRRig.AttachLocalPlayerToPhotonView(component1, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
        return;
      }
      PhotonViewXSceneRef component2;
      if (this.activeHandHold.objectHeld.TryGetComponent<PhotonViewXSceneRef>(out component2))
      {
        PhotonView photonView = component2.photonView;
        if (photonView != null)
        {
          VRRig.AttachLocalPlayerToPhotonView(photonView, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
          return;
        }
      }
      BuilderPieceHandHold component3;
      if (this.activeHandHold.objectHeld.TryGetComponent<BuilderPieceHandHold>(out component3) && component3.IsHandHoldMoving())
      {
        this.isHandHoldMoving = true;
        this.lastHandHoldRotation = component3.transform.rotation;
        this.movingHandHoldReleaseVelocity = this.playerRigidBody.linearVelocity;
      }
      else
      {
        this.isHandHoldMoving = false;
        this.lastHandHoldRotation = Quaternion.identity;
        this.movingHandHoldReleaseVelocity = Vector3.zero;
      }
    }
    VRRig.DetachLocalPlayerFromPhotonView();
  }

  private void FixedUpdate_HandHolds(float timeDelta)
  {
    if ((UnityEngine.Object) this.activeHandHold.objectHeld == (UnityEngine.Object) null)
    {
      if (this.wasHoldingHandhold)
        this.playerRigidBody.linearVelocity = Vector3.ClampMagnitude(this.secondLastPreHandholdVelocity, 5.5f * this.scale);
      this.wasHoldingHandhold = false;
    }
    else
    {
      Vector3 vector3_1 = this.activeHandHold.objectHeld.TransformPoint(this.activeHandHold.localPositionHeld);
      Vector3 position = this.activeHandHold.grabber.transform.position;
      this.secondLastPreHandholdVelocity = this.lastPreHandholdVelocity;
      this.lastPreHandholdVelocity = this.playerRigidBody.linearVelocity;
      this.wasHoldingHandhold = true;
      if (this.isHandHoldMoving)
      {
        this.lastPreHandholdVelocity = this.movingHandHoldReleaseVelocity;
        this.playerRigidBody.linearVelocity = Vector3.zero;
        Vector3 vector3_2 = vector3_1 - position;
        this.playerRigidBody.transform.position += vector3_2;
        this.movingHandHoldReleaseVelocity = vector3_2 / timeDelta;
        double num = (double) this.RotateWithSurface(this.activeHandHold.objectHeld.rotation * Quaternion.Inverse(this.lastHandHoldRotation), vector3_1);
        this.lastHandHoldRotation = this.activeHandHold.objectHeld.rotation;
      }
      else
      {
        this.playerRigidBody.linearVelocity = (vector3_1 - position) / timeDelta;
        if (!this.activeHandHold.applyRotation)
          return;
        this.turnParent.transform.RotateAround(vector3_1, this.transform.up, this.activeHandHold.localRotationalOffset - (this.activeHandHold.grabber.transform.rotation.eulerAngles.y - this.activeHandHold.objectHeld.rotation.eulerAngles.y));
      }
    }
  }

  [Serializable]
  public struct HandState
  {
    [NonSerialized]
    public Vector3 lastPosition;
    [NonSerialized]
    public Quaternion lastRotation;
    [NonSerialized]
    public bool isLeftHand;
    [NonSerialized]
    public bool wasColliding;
    [NonSerialized]
    public bool isColliding;
    [NonSerialized]
    public bool wasSliding;
    [NonSerialized]
    public bool isSliding;
    [NonSerialized]
    public bool isHolding;
    [NonSerialized]
    public Vector3 slideNormal;
    [NonSerialized]
    public float slipPercentage;
    [NonSerialized]
    public Vector3 hitPoint;
    [NonSerialized]
    private Vector3 boostVectorThisFrame;
    [NonSerialized]
    public Vector3 finalPositionThisFrame;
    [NonSerialized]
    public int slipSetToMaxFrameIdx;
    [NonSerialized]
    public int materialTouchIndex;
    [NonSerialized]
    public GorillaSurfaceOverride surfaceOverride;
    [NonSerialized]
    public UnityEngine.RaycastHit hitInfo;
    [NonSerialized]
    public UnityEngine.RaycastHit lastHitInfo;
    [NonSerialized]
    private GTPlayer gtPlayer;
    [SerializeField]
    public Transform handFollower;
    [SerializeField]
    public Transform controllerTransform;
    [SerializeField]
    public GorillaVelocityTracker velocityTracker;
    [SerializeField]
    public GorillaVelocityTracker interactPointVelocityTracker;
    [SerializeField]
    public Vector3 handOffset;
    [SerializeField]
    public Quaternion handRotOffset;
    [NonSerialized]
    public float tempFreezeUntilTimestamp;
    [NonSerialized]
    public bool canTag;
    [NonSerialized]
    public bool canStun;
    private float maxArmLength;
    [NonSerialized]
    public bool isActive;
    [NonSerialized]
    public float customBoostFactor;
    [NonSerialized]
    public bool hasCustomBoost;

    public void Init(GTPlayer gtPlayer, bool isLeftHand, float maxArmLength)
    {
      this.gtPlayer = gtPlayer;
      this.isLeftHand = isLeftHand;
      this.maxArmLength = maxArmLength;
      this.lastPosition = this.controllerTransform.position;
      this.lastRotation = this.controllerTransform.rotation;
      if ((UnityEngine.Object) this.handFollower != (UnityEngine.Object) null)
      {
        this.handFollower.transform.position = this.lastPosition;
        this.handFollower.transform.rotation = this.lastRotation;
      }
      this.wasColliding = false;
      this.slipSetToMaxFrameIdx = -1;
    }

    public void OnTeleport()
    {
      this.wasColliding = false;
      this.isColliding = false;
      this.isSliding = false;
      this.wasSliding = false;
      this.handFollower.position = this.controllerTransform.position;
      this.handFollower.rotation = this.controllerTransform.rotation;
      this.lastPosition = this.handFollower.transform.position;
      this.lastRotation = this.handFollower.transform.rotation;
    }

    public Vector3 GetLastPosition() => this.lastPosition + this.gtPlayer.MovingSurfaceMovement();

    public bool SlipOverriddenToMax() => this.slipSetToMaxFrameIdx == Time.frameCount;

    public void FirstIteration(ref Vector3 totalMove, ref int divisor, float paddleBoostFactor)
    {
      this.boostVectorThisFrame = !this.hasCustomBoost ? (this.gtPlayer.enableHoverMode ? this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity() * paddleBoostFactor : Vector3.zero) : this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity() * this.customBoostFactor;
      Vector3 vector3_1 = this.GetCurrentHandPosition() + this.gtPlayer.movingSurfaceOffset;
      Vector3 lastPosition = this.GetLastPosition();
      Vector3 vector3_2 = vector3_1 - lastPosition;
      int num1 = this.gtPlayer.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT ? 1 : 0;
      if (!this.gtPlayer.didAJump && this.wasSliding && (double) Vector3.Dot(this.gtPlayer.slideAverageNormal, Vector3.up) > 0.0)
        vector3_2 += Vector3.Project(-this.gtPlayer.slideAverageNormal * this.gtPlayer.stickDepth * this.gtPlayer.scale, Vector3.down);
      float sphereRadius = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
      if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
        sphereRadius = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
      Vector3 vector3_3 = Vector3.zero;
      if (num1 != 0 && !this.gtPlayer.exitMovingSurface)
      {
        vector3_3 = Vector3.Project(-this.gtPlayer.lastMovingSurfaceHit.normal * (this.gtPlayer.stickDepth * this.gtPlayer.scale), Vector3.down);
        if ((double) this.gtPlayer.scale < 0.5)
        {
          Vector3 normalized = this.gtPlayer.MovingSurfaceMovement().normalized;
          if (normalized != Vector3.zero)
          {
            float num2 = Vector3.Dot(Vector3.up, normalized);
            if ((double) num2 > 0.9 || (double) num2 < -0.9)
            {
              vector3_3 *= 6f;
              sphereRadius *= 1.1f;
            }
          }
        }
      }
      Vector3 endPosition;
      UnityEngine.RaycastHit iterativeHitInfo;
      Vector3 vector3_4;
      if (this.gtPlayer.IterativeCollisionSphereCast(lastPosition, sphereRadius, vector3_2 + vector3_3, this.boostVectorThisFrame, out endPosition, true, out this.slipPercentage, out iterativeHitInfo, this.SlipOverriddenToMax()) && !this.isHolding && !this.gtPlayer.InReportMenu)
      {
        vector3_4 = !this.wasColliding || (double) this.slipPercentage > (double) this.gtPlayer.defaultSlideFactor || this.boostVectorThisFrame.IsLongerThan(0.0f) ? endPosition - vector3_1 : lastPosition - vector3_1;
        this.isSliding = (double) this.slipPercentage > (double) this.gtPlayer.iceThreshold;
        this.slideNormal = this.gtPlayer.tempHitInfo.normal;
        this.isColliding = true;
        this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
        this.surfaceOverride = this.gtPlayer.currentOverride;
        this.gtPlayer.lastHitInfoHand = iterativeHitInfo;
        this.lastHitInfo = iterativeHitInfo;
      }
      else
      {
        vector3_4 = Vector3.zero;
        this.slipPercentage = 0.0f;
        this.isSliding = false;
        this.slideNormal = Vector3.up;
        this.isColliding = false;
        this.materialTouchIndex = 0;
        this.surfaceOverride = (GorillaSurfaceOverride) null;
      }
      bool flag = this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid;
      this.isColliding &= flag;
      this.isSliding &= flag;
      if (this.isColliding)
      {
        this.gtPlayer.anyHandIsColliding = true;
        if (this.isSliding)
          this.gtPlayer.anyHandIsSliding = true;
        else
          this.gtPlayer.anyHandIsSticking = true;
      }
      if (!this.isColliding && !this.wasColliding)
        return;
      if (!(bool) (UnityEngine.Object) this.surfaceOverride || !this.surfaceOverride.disablePushBackEffect)
        totalMove += vector3_4;
      ++divisor;
    }

    public void FinalizeHandPosition()
    {
      Vector3 lastPosition = this.GetLastPosition();
      if ((double) Time.time < (double) this.tempFreezeUntilTimestamp)
      {
        this.finalPositionThisFrame = lastPosition;
      }
      else
      {
        Vector3 movementVector = this.GetCurrentHandPosition() - lastPosition;
        float sphereRadius = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
        if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
          sphereRadius = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
        Vector3 endPosition;
        float slipPercentage;
        UnityEngine.RaycastHit iterativeHitInfo;
        if (this.gtPlayer.IterativeCollisionSphereCast(lastPosition, sphereRadius, movementVector, this.boostVectorThisFrame, out endPosition, this.gtPlayer.areBothTouching, out slipPercentage, out iterativeHitInfo, false) && !this.isHolding)
        {
          this.isColliding = true;
          this.isSliding = (double) slipPercentage > (double) this.gtPlayer.iceThreshold;
          this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
          this.surfaceOverride = this.gtPlayer.currentOverride;
          this.gtPlayer.lastHitInfoHand = iterativeHitInfo;
          this.lastHitInfo = iterativeHitInfo;
          this.finalPositionThisFrame = endPosition;
        }
        else
          this.finalPositionThisFrame = this.GetCurrentHandPosition();
      }
      bool flag = this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid;
      this.isColliding &= flag;
      this.isSliding &= flag;
      if (!this.isColliding)
        return;
      this.gtPlayer.anyHandIsColliding = true;
      if (this.isSliding)
        this.gtPlayer.anyHandIsSliding = true;
      else
        this.gtPlayer.anyHandIsSticking = true;
    }

    public bool IsSlipOverriddenToMax() => this.slipSetToMaxFrameIdx == Time.frameCount;

    public Vector3 GetCurrentHandPosition()
    {
      Vector3 position = this.gtPlayer.headCollider.transform.position;
      if (this.gtPlayer.inOverlay)
        return position + this.gtPlayer.headCollider.transform.up * -0.5f * this.gtPlayer.scale;
      Vector3 vector3 = this.gtPlayer.PositionWithOffset(this.controllerTransform, this.handOffset);
      return (vector3 - position).IsShorterThan(this.maxArmLength * this.gtPlayer.scale) ? vector3 : position + (vector3 - position).normalized * this.maxArmLength * this.gtPlayer.scale;
    }

    public void PositionHandFollower()
    {
      this.handFollower.position = this.finalPositionThisFrame;
      this.handFollower.rotation = this.lastRotation;
    }

    public void OnEndOfFrame()
    {
      this.wasColliding = this.isColliding;
      this.wasSliding = this.isSliding;
      this.lastPosition = this.finalPositionThisFrame;
      if ((double) Time.time <= (double) this.tempFreezeUntilTimestamp)
        return;
      this.lastRotation = this.controllerTransform.rotation * this.handRotOffset;
    }

    public void TempFreezeHand(float freezeDuration)
    {
      this.tempFreezeUntilTimestamp = Math.Max(this.tempFreezeUntilTimestamp, Time.time + freezeDuration);
    }

    public void GetHandTapData(
      out bool wasHandTouching,
      out bool wasSliding,
      out int handMatIndex,
      out GorillaSurfaceOverride surfaceOverride,
      out UnityEngine.RaycastHit handHitInfo,
      out Vector3 handPosition,
      out GorillaVelocityTracker handVelocityTracker)
    {
      wasHandTouching = this.wasColliding;
      wasSliding = this.wasSliding;
      handMatIndex = this.materialTouchIndex;
      surfaceOverride = this.surfaceOverride;
      handHitInfo = this.lastHitInfo;
      handPosition = this.finalPositionThisFrame;
      handVelocityTracker = this.velocityTracker;
    }
  }

  private enum MovingSurfaceContactPoint
  {
    NONE,
    RIGHT,
    LEFT,
    BODY,
  }

  [Serializable]
  public struct MaterialData
  {
    public string matName;
    public bool overrideAudio;
    public AudioClip audio;
    public bool overrideSlidePercent;
    public float slidePercent;
    public int surfaceEffectIndex;
  }

  [Serializable]
  public struct LiquidProperties
  {
    [Range(0.0f, 2f)]
    [Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
    public float resistance;
    [Range(0.0f, 3f)]
    [Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
    public float buoyancy;
    [Range(0.0f, 3f)]
    [Tooltip("Damping Half-life Multiplier")]
    public float dampingFactor;
    [Range(0.0f, 1f)]
    public float surfaceJumpFactor;
  }

  public enum LiquidType
  {
    Water,
    Lava,
  }

  private struct HoverBoardCast
  {
    public Vector3 localOrigin;
    public Vector3 localDirection;
    public float sphereRadius;
    public float distance;
    public float intersectToVelocityCap;
    public bool isSolid;
    public bool didHit;
    public Vector3 pointHit;
    public Vector3 normalHit;
  }

  private struct HandHoldState
  {
    public GorillaGrabber grabber;
    public Transform objectHeld;
    public Vector3 localPositionHeld;
    public float localRotationalOffset;
    public bool applyRotation;
  }
}
