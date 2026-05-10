using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;
    // [SerializeField] private float tipLenght = .05f;

    [Header("Aim Control")]
    [SerializeField] private Transform aim;
    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingToTarget;

    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(1, 15f)]
    [SerializeField] private float maxCameraDistance;
    [Range(.5f, 1)]
    [SerializeField] private float minCameraDistance;
    [Range(3, 5)]
    [SerializeField] private float cameraSensetivity;

    [Space]

    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;
    private Vector2 aimInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            isAimingPrecisly = !isAimingPrecisly;
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            isLockingToTarget = !isLockingToTarget;
        }

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        aimLaser.enabled = player.weapon.WeaponReady();

        if(aimLaser.enabled == false)
        {
            return;
        }

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);

        Transform gunPoint = player.weapon.GunPoint(); 
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLength = .5f;
        float gunDistance = 4f;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if(Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }

    public Transform Target()
    {
        Transform target = null;
        if(GetMouseInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseInfo().transform;
        }

        return target;
    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();
        if(target != null && isLockingToTarget)
        {
            aim.position = target.position;
            return;
        }

        aim.position = GetMouseInfo().point;
        if (!isAimingPrecisly)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
        }
    }

    public bool CanAimPrecicly()
    {
        if(isAimingPrecisly)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);

       float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
       desiredCameraPosition = transform.position + aimDirection * clampedDistance;

        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    public RaycastHit GetMouseInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }
}
