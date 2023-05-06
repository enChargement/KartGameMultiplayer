using System;
using System.Collections;
using UnityEngine;

public class NukeVisual : WeaponVisual
{
    private string _name = "Nuke";
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject nuke = Instantiate(Weapon, FirePoint.position, FirePoint.rotation);
        Rigidbody body = nuke.GetComponent<Rigidbody>();

        body.velocity = spawnVelocity;

        nuke.tag = gameObject.tag;
        yield return null;
        reset?.Invoke();
    }

}
