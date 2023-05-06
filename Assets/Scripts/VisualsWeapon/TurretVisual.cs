using System;
using System.Collections;
using UnityEngine;

public class TurretVisual : WeaponVisual
{
    private string _name = "Turret";
    public override string WeaponName { get { return _name; } set => _name = value; }
    private Turret _weaponInstance;
    [SerializeField] private Transform _head;

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject turret = Instantiate(Weapon, transform);
        _weaponInstance = turret.GetComponent<Turret>();
        turret.tag = gameObject.tag;
        yield return new WaitForSeconds(10f);
        reset?.Invoke();
    }

    void Update()
    {
        LookAtCurrentEnemy();
    }
    private void LookAtCurrentEnemy()
    {
        if (_weaponInstance.IsTherAnEnemy())
        {
            GameObject enemy = _weaponInstance.GetEnnemyTransform();

            _head.LookAt(enemy.transform);
            _head.rotation *= Quaternion.Euler(-90, 90, 1);
            
        }
    }
}
