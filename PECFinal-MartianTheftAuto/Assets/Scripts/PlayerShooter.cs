using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Script para representar al jugador y sus estadisticas dentro del juego, así como las armas que están en su posesión,
 * junto con todas las acciones y comportamientos que tiene habilitados para completar la parte de Shooter. 
 * Esteban.Hernandez
 */
public class PlayerShooter : MonoBehaviour
{
    public float MaxHealth = 100;
    public float MaxShield = 100;
    public int ActiveWeaponIndex;

    public event Action<float> HealthChanged;
    public float CurrentHealth;

    public event Action<float> ShieldChanged;
    public float CurrentShield;

    public event Action<int> AmmoChanged;
    public int CurrentAmmo;

    public event Action<int> ClipChanged;
    public int CurrentClip;

    public event Action<string> ItemChanged;
    public string LastItem;

    public event Action<bool> DriveStateChanged;
    public bool DriveState;

    public event Action<bool> FinishedChanged;
    public bool LevelFinished = false;

    public List<Weapon> PlayerWeapons;
    public List<AudioClip> DamageClips;
    public AudioClip HealthClip;
    public AudioClip ShieldClip;
    private AudioClip _currentDamageClip;
    [SerializeField]
    private Transform _respawnPoint;
    private Animator _animator;
    private ThirdPersonController _thirdPersonController;

    void Awake()
    {
        ResetPlayerValues();
        _animator = GetComponentInChildren<Animator>();
        _thirdPersonController = GetComponentInChildren<ThirdPersonController>();
        _animator.SetBool("IsAlive", true);
        FinishedChanged?.Invoke(false);
    }

    private void ResetPlayerValues()
    {
        foreach (var weapon in PlayerWeapons)
        {
            weapon.InitWeapon();
            weapon.gameObject.SetActive(false);
        }
        ActiveWeaponIndex = 0;
        CurrentShield = 0;
        CurrentHealth = MaxHealth;
        PlayerWeapons[ActiveWeaponIndex].gameObject.SetActive(true);
        CurrentAmmo = PlayerWeapons[ActiveWeaponIndex].CurrentAmmo;
        CurrentClip = PlayerWeapons[ActiveWeaponIndex].CurrentClip;
        //GameManager.instance.CleanInventory();
    }

    void Update()
    {
        ProcessFireInput();
    }



    private void ProcessFireInput()
    {
        var weapon = PlayerWeapons[ActiveWeaponIndex];
        //Mientras el arma se esté recargando, no se van a poder realizar otras acciones
        if (!weapon.IsReloading)
        {
            if (Input.GetMouseButtonDown(0) && !weapon.IsAutomatic)
            {
                _animator.SetBool("Firing", true);
                weapon.Shoot();
            }
            else if (Input.GetMouseButton(0) && weapon.IsAutomatic)
            {
                _animator.SetBool("Firing", true);
                weapon.AutomaticShoot();
            }

            else if (Input.GetKeyDown(KeyCode.R))
            {
                _animator.SetBool("Firing", false);
                _animator.SetBool("Reload", true);
                weapon.Reload();
            }

            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                //Deshabilita el arma actual
                weapon.gameObject.SetActive(false);
                ActiveWeaponIndex++;
                //Calcula el indice del arma siguiente
                if (ActiveWeaponIndex == PlayerWeapons.Count)
                {
                    ActiveWeaponIndex = 0;
                }
                //Habilita la siguiente arma
                PlayerWeapons[ActiveWeaponIndex].gameObject.SetActive(true);
            }


            else
            {
                _animator.SetBool("Firing", false);
                _animator.SetBool("Reload", false);
            }
        }
        //Actualiza mediante eventos los datos que se muestran en el HUD
        CurrentClip = weapon.CurrentClip;
        CurrentAmmo = weapon.CurrentAmmo;
        AmmoChanged?.Invoke(CurrentAmmo);
        ClipChanged?.Invoke(CurrentClip);
    }

    public void ReceiveHit(float damage)
    {
        _animator.SetTrigger("Damaged");
        float healthDamage = damage;
        StartCoroutine(DamageCoroutine());
        if (CurrentShield > 0)
        {
            if (CurrentShield - damage * 0.9f <= 0)
            {
                CurrentShield = 0;
            }
            else
            {
                CurrentShield -= damage * 0.7f;
            }
            healthDamage = damage * 0.3f;
        }
        if (CurrentHealth - healthDamage <= 0)
        {
            CurrentHealth = 0;
        }
        else
        {
            CurrentHealth -= healthDamage;
        }
        HealthChanged?.Invoke(CurrentHealth);
        ShieldChanged?.Invoke(CurrentShield);
    }

    private IEnumerator DamageCoroutine()
    {
        WaitForSeconds updateRate = new WaitForSeconds(0.02f);
        DamageClips = SoundSingleton.instance.RandomizePlayerFx(DamageClips);
        yield return updateRate;
    }

    public int GetCurrentMaxAmmo()
    {
        return PlayerWeapons[ActiveWeaponIndex].MaxAmmo;
    }


    public void RecoverFromConsumable(ConsumableType type, int amount)
    {
        switch (type)
        {
            case ConsumableType.HEALTH:
                float resultingHealth = CurrentHealth + amount;
                if (resultingHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }
                else
                {
                    CurrentHealth = resultingHealth;
                }
                break;
            case ConsumableType.AMMO:
                int resultingAmmo = CurrentAmmo + amount;
                if (resultingAmmo > GetCurrentMaxAmmo())
                {
                    PlayerWeapons[ActiveWeaponIndex].SetAmmoFull();
                }
                else
                {
                    PlayerWeapons[ActiveWeaponIndex].AddAmmo(amount);
                }
                break;
            case ConsumableType.SHIELD:
                float resultingShield = CurrentShield + amount;
                if (resultingShield > MaxShield)
                {
                    CurrentShield = MaxShield;
                }
                else
                {
                    CurrentShield = resultingShield;
                }
                break;
        }
        HealthChanged?.Invoke(CurrentHealth);
        ShieldChanged?.Invoke(CurrentShield);
        AmmoChanged?.Invoke(CurrentAmmo);
        ClipChanged?.Invoke(CurrentClip);
        ItemChanged?.Invoke(LastItem);
    }

    public void ChangeRespawnPoint(Transform location)
    {
        _respawnPoint = location;
    }
    public void Respawn()
    {
        //El ThirdPersonController impide operar sobre el transform del player, hay que deshabilitarlo
        _thirdPersonController.enabled = false;
        transform.position = _respawnPoint.position;
        transform.rotation = _respawnPoint.rotation;
        ResetPlayerValues();
        _thirdPersonController.enabled = true;
        HealthChanged?.Invoke(CurrentHealth);
        ShieldChanged?.Invoke(CurrentShield);
        AmmoChanged?.Invoke(CurrentAmmo);
        ClipChanged?.Invoke(CurrentClip);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            FinishedChanged?.Invoke(true);
        }
    }

    public void NotifyDriveChange(bool canDrive)
    {
        this.DriveStateChanged.Invoke(canDrive);
    }

    

}
