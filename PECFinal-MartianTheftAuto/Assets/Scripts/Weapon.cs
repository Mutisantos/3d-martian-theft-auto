using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Script para crear los valores de cualquier arma que el jugador pueda utilizar
 * Se definen parametros de Cargador, munición, daño, cadencia de fuego, tiempo de recarga, si es de disparo automatico.
 * Esteban.Hernandez
 */
public class Weapon : MonoBehaviour
{

    public float FireRate = 0.05f;
    public int MaxAmmo; // Cantidad maxima de municion que puede tener el arma
    public int MagazineSize; // Cuantas balas puede almacenar antes de volver a recargar
    public int CurrentAmmo; // 40
    public int CurrentClip; // 20 / 40
    public int Damage = 1;
    public float ReloadTime = 1;
    public List<AudioClip> ShotClips;
    public AudioClip EmptyClip;
    public AudioClip ReloadClip;
    public float MaxRange;
    public bool IsAutomatic = false;
    public bool IsReloading;
    public int MaxDecals = 5;
    public ParticleSystem MuzzleEffects;
    public GameObject BulletDecal;
    public Transform BulletSource;
    public AudioSource SFXSource;
    private Quaternion _weaponAngle;
    private List<GameObject> _decalList;
    private float _lastShootFrame = 0;


    void Awake()
    {
        _decalList = new List<GameObject>();
        _weaponAngle = transform.localRotation;
    }

    public void InitWeapon()
    {
        CurrentAmmo = MaxAmmo / 2;
        CurrentClip = MagazineSize;
    }

    public void AutomaticShoot()
    {
        if (_lastShootFrame == 0)
        {
            _lastShootFrame = Time.time;
        }
        if (_lastShootFrame + FireRate < Time.time)
        {
            _lastShootFrame = Time.time;
            Shoot();
        }
    }


    public void Shoot()
    {
        //El arma no va a poder dispararse si el cargador esta vacio o si se esta recargando
        if (CurrentClip > 0 && !IsReloading)
        {

            MuzzleEffects.Play();
            CurrentClip--;
            PlayFireEffect();
            RaycastHit shotHit;
            Ray ray = new Ray(BulletSource.position, BulletSource.forward);
            int layer = 1 << LayerMask.NameToLayer("Default");
            if (Physics.Raycast(ray, out shotHit, MaxRange, layer, QueryTriggerInteraction.Ignore))
            {

                if (_decalList.Count == MaxDecals)
                {
                    Destroy(_decalList[0]);
                    _decalList.RemoveAt(0);
                }
                var newDecal = GameObject.Instantiate(BulletDecal, shotHit.point + shotHit.normal * 0.01f, Quaternion.FromToRotation(Vector3.forward, -shotHit.normal));
                _decalList.Add(newDecal);
                if (shotHit.collider.CompareTag("Enemy"))
                {
                    shotHit.collider.GetComponent<EnemyIA>().Hit(Damage, transform.position);
                    Destroy(newDecal, 0.05f);
                    _decalList.RemoveAt(_decalList.Count -1); 
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * MaxRange, Color.white);
        }
        else
        {
            SFXSource.PlayOneShot(EmptyClip);
        }
    }

    private void PlayFireEffect()
    {
        SFXSource.PlayOneShot(ShotClips[0]);
    }

    public void Reload()
    {
        if (!IsReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void SetAmmoFull()
    {
        CurrentAmmo = MaxAmmo;
    }

    public void AddAmmo(int ammoCount)
    {
        CurrentAmmo += ammoCount;
    }


    private IEnumerator ReloadCoroutine()
    {
        IsReloading = true;
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(-30f, 0f, 0f));
        yield return new WaitForSeconds(ReloadTime);
        transform.localRotation = _weaponAngle;
        IsReloading = false;
        if (CurrentAmmo == MagazineSize)
        {
            SFXSource.PlayOneShot(EmptyClip);
        }
        else if (CurrentAmmo == 0)
        {
            SFXSource.PlayOneShot(EmptyClip);
        }
        else if (CurrentAmmo < MagazineSize)
        {
            CurrentClip = CurrentAmmo;
            CurrentAmmo = 0;
            SFXSource.PlayOneShot(ReloadClip);
        }
        else
        {
            int reload = MagazineSize - CurrentClip;
            CurrentClip = MagazineSize;
            CurrentAmmo -= reload;
            SFXSource.PlayOneShot(ReloadClip);
        }
    }


}
