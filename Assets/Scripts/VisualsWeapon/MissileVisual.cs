using System;
using System.Collections;
using UnityEngine;

public class MissileVisual : WeaponVisual
{
    private string _name = "Missile";
    public override string WeaponName { get { return _name; } set { _name = value; } }


    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        for (int i = -1; i < 2; i++)
        {
            Vector3 position = FirePoint.position;
            Vector3 right = FirePoint.right;
            position += right * i * 0.2f;

            GameObject missile = Instantiate(Weapon, position, FirePoint.rotation);
            Rigidbody body = missile.GetComponent<Rigidbody>();

            body.velocity = spawnVelocity;

            missile.tag = gameObject.tag;
        }
        yield return null;
        reset?.Invoke();
    }
}
