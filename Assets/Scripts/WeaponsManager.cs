using System;
using System.Collections;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool GiveWeapon(GameObject prefab)
    {
        if (!_currentWeapon)
        {
            WeaponVisual weapon = prefab.GetComponent<WeaponVisual>();
            string name = weapon.WeaponName;
            Debug.Log("NAME : " + name);
            prefab.tag = tag;
            //prefab.transform.rotation =  Quaternion.Euler(Vector3.zero);
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
        CarManager player = GetComponent<CarManager>();

        if (weaponVisual.WeaponName.Equals("Star"))
        {
            _isInvincible = true;
            player.additionnalSpeed = 15f;
        }
        Debug.Log(DateTime.Now);
        StartCoroutine(weaponVisual.ActivateWeapon(rb.velocity, ResetWeapon));
        
    }

    private void ResetWeapon()
    {
        CarManager player = GetComponent<CarManager>();
        Debug.Log(DateTime.Now);
        player.additionnalSpeed = 0f;
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

    private IEnumerator CheckHealth()
    {
        if (_health <= 0)
        {
            rb.transform.position = new Vector3(0, 0, 0);
            yield return new WaitForFixedUpdate();
            Destroy(gameObject);
        }
    }

    public float GetHealth()
    {
        return _health;
    }

    void Update()
    {
        StartCoroutine(CheckHealth());
        if (Input.GetKeyUp(KeyCode.Space) && _currentWeapon && !_isFiring)
        {
            UseWeapon();
        }
    }
}
