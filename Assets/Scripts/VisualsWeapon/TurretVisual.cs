using System;
using System.Collections;
using UnityEngine;

public class TurretVisual : WeaponVisual
{
    private string _name = "Turret";
    public override string WeaponName { get { return _name; } set => _name = value; }
    [SerializeField] private Turret _weaponInstance;
    private bool _isFiring = false;
    private float timer = 0f;
    private bool _shoot = false;
    [SerializeField] private Transform _head;
    [SerializeField] private ParticleSystem _muzzleFlash;

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject turret = Instantiate(Weapon, transform);
        _weaponInstance = turret.GetComponent<Turret>();
        turret.tag = gameObject.tag;
        _isFiring = true;
        yield return new WaitForSeconds(10f);
        reset?.Invoke();
    }

    void Update()
    {
        if (_isFiring)
        {
            timer+= Time.deltaTime;
            _shoot = false;
        }
        if(timer >= 1f)
        {
            timer = 0f;
            _shoot = true;
        }
        LookAtCurrentEnemy();
        
    }
    private void LookAtCurrentEnemy()
    {
        if (_weaponInstance)
        {
            GameObject enemy = _weaponInstance.GetEnnemyTransform();
            if (enemy is null)
            {
                return;
            }
            _head.LookAt(enemy.transform);
            _head.rotation *= Quaternion.Euler(-90, 90, 1);
            if(_shoot)
                Instantiate(_muzzleFlash, FirePoint);
        }
    }
}
