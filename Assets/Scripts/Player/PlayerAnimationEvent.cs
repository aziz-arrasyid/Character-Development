using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private WeaponVisualController visualController;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<WeaponVisualController>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();
        weaponController.CurrentWeapon().RefillBullets();

        weaponController.SetWeaponReady(true);

    }

    public void ReturnWig()
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight();
    }

    public void WeaponEquipingIsOver()
    {
        // visualController.SetBusyGrabbingWeaponTo(false);
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => visualController.SwitchOnCurrentWeaponModel();
}
