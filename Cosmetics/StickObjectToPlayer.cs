// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.StickObjectToPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class StickObjectToPlayer : MonoBehaviour, ITickSystemTick
{
  [Header("Shared Settings")]
  [Tooltip("Must be in the global object pool and have a tag.")]
  [SerializeField]
  private GameObject objectToSpawn;
  [Tooltip("Optional: how many objects can be active at once")]
  [SerializeField]
  private int maxActiveStickies = 1;
  [SerializeField]
  private StickObjectToPlayer.SpawnLocation spawnLocation;
  [SerializeField]
  private float stickRadius = 0.5f;
  [SerializeField]
  private bool alignToHitNormal = true;
  [SerializeField]
  private Rigidbody spawnerRigidbody;
  [SerializeField]
  private string parentTag = "GorillaHead";
  [SerializeField]
  private float cooldown;
  [Header("Third Person View")]
  [Tooltip("If you are only interested in the FPV, don't check this box so that others don't see it.")]
  [SerializeField]
  private bool thirdPersonView = true;
  [SerializeField]
  private Vector3 positionOffset = new Vector3(0.0f, 0.02f, 0.17f);
  [Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
  [SerializeField]
  private Vector3 localEulerAngles = Vector3.zero;
  [Header("First Person View")]
  [SerializeField]
  private bool firstPersonView;
  [SerializeField]
  private Vector3 FPVOffset = new Vector3(0.0f, 0.02f, 0.17f);
  [Tooltip("Local rotation to apply to the spawned object (Euler angles, degrees)")]
  [SerializeField]
  private Vector3 FPVlocalEulerAngles = Vector3.zero;
  [Header("Events")]
  public UnityEvent OnStickShared;
  private GameObject stickyObject;
  private float lastSpawnedTime;
  private bool canSpawn = true;
  private NetPlayer ownerPlayer;

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.canSpawn || (double) Time.time - (double) this.lastSpawnedTime < (double) this.cooldown)
      return;
    this.canSpawn = true;
  }

  private void OnEnable()
  {
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
    this.canSpawn = true;
  }

  private void OnDisable() => TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);

  public void SetOwner(NetPlayer player) => this.ownerPlayer = player;

  private Transform MakeOrGetStickyContainer(Transform parent)
  {
    Transform parent1 = parent;
    foreach (Transform componentsInChild in parent.GetComponentsInChildren<Transform>(true))
    {
      if (!this.firstPersonView && componentsInChild.CompareTag(this.parentTag))
      {
        parent1 = componentsInChild;
        break;
      }
    }
    string str = "StickyObjects_" + this.objectToSpawn.name;
    Transform stickyContainer = parent1.Find(str);
    if ((Object) stickyContainer != (Object) null)
      return stickyContainer;
    GameObject gameObject = new GameObject(str);
    gameObject.transform.SetParent(parent1, false);
    return gameObject.transform;
  }

  public void Stick(bool leftHand, Collider other)
  {
    if (!this.canSpawn || (Object) other == (Object) null || !this.enabled)
      return;
    VRRig componentInParent = other.GetComponentInParent<VRRig>();
    if (!(bool) (Object) componentInParent || this.ownerPlayer != null && componentInParent.creator == this.ownerPlayer)
      return;
    Vector3 vector3_1 = Time.fixedDeltaTime * 2f * ((Object) this.spawnerRigidbody != (Object) null ? this.spawnerRigidbody.linearVelocity : Vector3.zero);
    Vector3 direction = vector3_1.normalized;
    if (direction == Vector3.zero)
    {
      direction = this.transform.forward;
      vector3_1 = direction * 0.01f;
    }
    Vector3 vector3_2 = this.transform.position - vector3_1;
    Vector3 vector3_3;
    if (this.alignToHitNormal)
    {
      float magnitude = vector3_1.magnitude;
      UnityEngine.RaycastHit hitInfo;
      vector3_3 = !other.Raycast(new Ray(vector3_2, direction), out hitInfo, 2f * magnitude) ? other.ClosestPoint(vector3_2) : hitInfo.point;
    }
    else
      vector3_3 = other.ClosestPoint(vector3_2);
    Vector3 position = this.GetSpawnPosition(this.spawnLocation, componentInParent).TransformPoint(this.positionOffset);
    if ((double) (vector3_3 - position).magnitude > (double) this.stickRadius * (double) componentInParent.scaleFactor)
      return;
    if (NetworkSystem.Instance.LocalPlayer == componentInParent.creator)
    {
      if (this.firstPersonView && this.spawnLocation == StickObjectToPlayer.SpawnLocation.Head)
        this.StickFirstPersonView();
    }
    else
    {
      if (!this.thirdPersonView)
        return;
      this.StickTo(this.MakeOrGetStickyContainer(componentInParent.transform), position, this.localEulerAngles);
    }
    this.OnStickShared?.Invoke();
  }

  private void StickFirstPersonView()
  {
    Transform cosmeticsHeadTarget = GTPlayer.Instance.CosmeticsHeadTarget;
    Vector3 position = cosmeticsHeadTarget.TransformPoint(this.FPVOffset);
    this.StickTo(this.MakeOrGetStickyContainer(cosmeticsHeadTarget), position, this.FPVlocalEulerAngles);
  }

  private void StickTo(Transform parent, Vector3 position, Vector3 eulerAngle)
  {
    int num = 0;
    for (int index = 0; index < parent.childCount; ++index)
    {
      if (parent.GetChild(index).gameObject.activeInHierarchy)
        ++num;
    }
    if (num >= this.maxActiveStickies)
      return;
    this.stickyObject = ObjectPools.instance.Instantiate(this.objectToSpawn);
    if ((Object) this.stickyObject == (Object) null)
      return;
    this.stickyObject.transform.SetParent(parent, false);
    this.stickyObject.transform.position = position;
    this.stickyObject.transform.localEulerAngles = eulerAngle;
    this.lastSpawnedTime = Time.time;
    this.canSpawn = false;
  }

  private Transform GetSpawnPosition(StickObjectToPlayer.SpawnLocation spawnType, VRRig hitRig)
  {
    switch (spawnType)
    {
      case StickObjectToPlayer.SpawnLocation.Head:
        return hitRig.head.rigTarget.transform;
      case StickObjectToPlayer.SpawnLocation.RightHand:
        return hitRig.rightHand.rigTarget.transform;
      case StickObjectToPlayer.SpawnLocation.LeftHand:
        return hitRig.leftHand.rigTarget.transform;
      default:
        return (Transform) null;
    }
  }

  public void Debug_StickToLocalPlayer()
  {
    this.StickTo(VRRig.LocalRig.transform, this.GetSpawnPosition(this.spawnLocation, VRRig.LocalRig).TransformPoint(this.positionOffset), this.localEulerAngles);
  }

  public void Debug_StickToLocalPlayerFPV() => this.StickFirstPersonView();

  private enum SpawnLocation
  {
    Head,
    RightHand,
    LeftHand,
  }
}
