using System;
using System.Collections;
using UnityEngine;

public class BulletVisual : WeaponVisual
{
    private string _name = "Bullet";
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector3 position = FirePoint.position;
                Vector3 right = FirePoint.right;
                position += right * j * 0.2f;

                GameObject missile = Instantiate(Weapon, position, FirePoint.rotation);
                Rigidbody body = missile.GetComponent<Rigidbody>();

                body.velocity = spawnVelocity;

                missile.tag = gameObject.tag;
            }
            yield return new WaitForSeconds(.15f);
        }
        reset?.Invoke();
    }
}
