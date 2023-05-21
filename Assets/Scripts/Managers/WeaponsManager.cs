using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WeaponsManager : NetworkBehaviour
{

    public GameObject _currentWeapon = null;
    [SerializeField] private Transform _groundWeaponSpawn;
    [SerializeField] private Transform _aerianWeaponSpawn;
    [SerializeField] private Transform _backWeaponSpawn;
    [SerializeField] private Transform _stateWeaponSpawn;

    private float _health = 100f;
    private bool _isInvincible = false;
    private bool _isFiring = false;

    private Rigidbody rb;
    private CarManager carManager;

    public override void OnNetworkSpawn()
    {
        carManager = GetComponent<CarManager>();
    }

    public bool GiveWeapon(GameObject prefab)
    {
        if (!_currentWeapon)
        {
            WeaponVisual weapon = prefab.GetComponent<WeaponVisual>();
            string name = weapon.WeaponName;
            Debug.Log("NAME : " + name);
            prefab.tag = tag;
            switch (name)
            {
                case "CanonBall":
                    _currentWeapon = Instantiate(prefab,_aerianWeaponSpawn);
                    break;
                case "Missile":
                    _currentWeapon = Instantiate(prefab,_groundWeaponSpawn);
                    break;
                case "LandMine":
                    _currentWeapon = Instantiate(prefab, _backWeaponSpawn);
                    break;
                case "Spikes":
                    _currentWeapon = Instantiate(prefab, _stateWeaponSpawn);
                    break;
                case "Star":
                    _currentWeapon = Instantiate(prefab, _stateWeaponSpawn);
                    break;
                case "Nuke":
                    _currentWeapon = Instantiate(prefab, _groundWeaponSpawn);
                    break;
                case "Turret":
                    _currentWeapon = Instantiate(prefab, _aerianWeaponSpawn);
                    break;
                case "Bullet":
                    _currentWeapon = Instantiate(prefab, _groundWeaponSpawn);
                    break;
                case "Flag":
                    prefab.transform.position = new Vector3(0,0,-.3f);
                    _currentWeapon = Instantiate(prefab, _backWeaponSpawn);
                    _currentWeapon.name = "Flag";
                    break;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private void UseWeapon()
    {
        _isFiring = true;
        WeaponVisual weaponVisual = _currentWeapon.GetComponent<WeaponVisual>();

        if (weaponVisual.WeaponName.Equals("Star"))
        {
            _isInvincible = true;
            carManager.additionnalSpeed = 15f;
        }
        Debug.Log(DateTime.Now);
        if (!rb)
        {
            rb = carManager.rb;
        }
            StartCoroutine(weaponVisual.ActivateWeapon(rb.velocity, ResetWeapon));

        
    }

    private void ResetWeapon()
    {

        Debug.Log(DateTime.Now);
        carManager.additionnalSpeed = 0f;
        _isInvincible = false;
        _isFiring = false;
        Destroy(_currentWeapon);
        _currentWeapon = null;
    }

    public void GetDamaged(float damage)
    {
        float absDamage = Mathf.Abs(damage);
        if(!_isInvincible)
        _health -= absDamage;
    }

    private void CheckHealth()
    {
        if (_health > 0) return;
        if (!carManager.IsDead)
            _health = 100f;
        Debug.Log("invoke deadstate");
            StartCoroutine(carManager.DeadState());
    }

    public float GetHealth()
    {
        return _health;
    }

    void Update()
    {
        CheckHealth();
        if (Input.GetKeyUp(KeyCode.Space) && _currentWeapon && !_isFiring)
        {
            UseWeapon();
        }
    }
}
