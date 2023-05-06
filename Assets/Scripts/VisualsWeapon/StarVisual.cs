using System;
using System.Collections;
using UnityEngine;

public class StarVisual : WeaponVisual
{
    private string _name = "Star";
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject star = Instantiate(Weapon, transform);

        star.tag = gameObject.tag;
        yield return new WaitForSeconds(5f);
        reset?.Invoke();
    }
}
