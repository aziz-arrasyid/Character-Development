using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;

    [Header("Shooting Specifics")]
    public ShootType shootType;
    public float fireRate = 1;
    private float lastShootTime;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;
    [Range(1, 2)]
    public float reloadSpeed = 1;
    [Range(1, 2)]
    public float equipmentSpeed = 1;

    [Header("Spreads")]
    public float baseSpread;
    private float currentSpread = 2;
    public float maximumSpread = 3;

    public float spreadIncreaseRate = .15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;

    #region Spread Method
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {

        if(Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion


    public bool CanShoot()
    {
        if(HaveEnoughBullets() && ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
    }

    private bool ReadyToFire()
    {
        if(Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    #region Reload Method
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;
    public bool CanReload()
    {
        if(bulletsInMagazine == magazineCapacity)
        {
            return false;
        }

        if(totalReserveAmmo > 0)
        {
            return true;
        }

        return false;
    }
    public void RefillBullets()
    {
        // totalReserveAmmo += bulletsInMagazine;

        int bulletsToReload = magazineCapacity;

        if(bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if(totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }
    #endregion
}
