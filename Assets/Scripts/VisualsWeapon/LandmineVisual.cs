using System;
using System.Collections;
using UnityEngine;

public class LandmineVisual : WeaponVisual
{
    private string _name = "LandMine";
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject landMine = Instantiate(Weapon, transform.position, transform.rotation);
            landMine.tag = gameObject.tag;
            yield return new WaitForSeconds(.2f);
        }
        reset?.Invoke();
    }
}
