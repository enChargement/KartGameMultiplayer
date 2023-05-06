using System;
using System.Collections;
using UnityEngine;

public class SpikesVisual : WeaponVisual
{
    private string _name = "Spikes";
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject spikes = Instantiate(Weapon, transform);
        spikes.tag = gameObject.tag;
        yield return new WaitForSeconds(5f);
        reset?.Invoke();
    }
}
