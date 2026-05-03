using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControls controls {get; private set;}
    public PlayerAim aim {get; private set;}
    public PlayerMovement movement {get; private set;}
    public PlayerWeaponController weapon {get; private set;}

    private void Awake()
    {
        controls = new PlayerControls();
        
        if (GetComponent<PlayerAim>() != null)
            aim = GetComponent<PlayerAim>();

        if (GetComponent<PlayerMovement>() != null)
            movement = GetComponent<PlayerMovement>();

        if (GetComponent<PlayerWeaponController>() != null)
            weapon = GetComponent<PlayerWeaponController>();
    }

    private void OnEnable()
    {
        controls.Enable();   
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
