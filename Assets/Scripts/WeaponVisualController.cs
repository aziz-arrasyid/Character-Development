using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    private Player player;
    private Animator anim;
    private Rig rig;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModel;

    [Header("Rig")]
    [SerializeField] private float rigIncreaseStep;
    private bool rigShouldBeIncreased;

    [Header("Left hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_target;
    [SerializeField] private float leftHandIK_IncreaseStep;
    private bool shouldIncreaseLeftHandIKWeight;

    private bool isEquipingWeapon;

    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModel = GetComponentsInChildren<BackupWeaponModel>(true);

        // SwitchOn(pistol);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;
        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;
    }

    public void PlayReloadAnimation()
    {
        if (isEquipingWeapon)
        {
            return;
        }

        float reloadSpeed = player.weapon.CurrentWeapon().reloadSpeed;

        anim.SetFloat("ReloadSpeed", reloadSpeed);
        anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    #region Animation Rigging Method

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_target.localPosition = targetTransform.localPosition;
        leftHandIK_target.localRotation = targetTransform.localRotation;
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIK_IncreaseStep * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                shouldIncreaseLeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (rigShouldBeIncreased)
        {
            rig.weight += rigIncreaseStep * Time.deltaTime;
            if (rig.weight >= 1)
            {
                rigShouldBeIncreased = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = .15f;
    }

    public void MaximizeRigWeight() => rigShouldBeIncreased = true;
    public void MaximizeLeftHandWeight() => shouldIncreaseLeftHandIKWeight = true;

    #endregion
    public void PlayWeaponEquipAnimation()
    {
        EquipType grabType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = player.weapon.CurrentWeapon().equipmentSpeed;

        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetFloat("EquipType", ((float)grabType));
        anim.SetTrigger("EquipWeapon");
        anim.SetFloat("EquipSpeed", equipmentSpeed);

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        isEquipingWeapon = busy;
        anim.SetBool("BusyEquipingWeapon", isEquipingWeapon);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = ((int)CurrentWeaponModel().holdType);

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if (player.weapon.HasOnlyOneWeapon() == false)
        {
            SwitchOnBackupWeaponModel();
        }

        SwitchAnimatorLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        Debug.Log(CurrentWeaponModel().weaponType);

        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {

            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupModel in backupWeaponModel)
        {
            backupModel.gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        WeaponType weaponType = player.weapon.BackupWeapon().weaponType;

        foreach (BackupWeaponModel backupModel in backupWeaponModel)
        {
            if (backupModel.weaponType == weaponType)
            {
                backupModel.gameObject.SetActive(true);
            }
        }
    }

    private void SwitchAnimatorLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }



}
