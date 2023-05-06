using System;
using System.Collections;
using UnityEngine;

public class CanonBallVisual : WeaponVisual
{
    private string _name = "CanonBall";
    public override string WeaponName { get { return _name; } set => _name = value; }

    [SerializeField] private float _forwardForce = 80f;
    [SerializeField] private float _upForce = 150f;

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject canonBall = Instantiate(Weapon, FirePoint.position, FirePoint.rotation);
            canonBall.tag = gameObject.tag;
            Rigidbody body = canonBall.GetComponent<Rigidbody>();
            body.velocity = spawnVelocity;

            Vector3 forward = gameObject.transform.forward * (_forwardForce + i * 70);
            Vector3 up = gameObject.transform.up * _upForce;

            body.AddForce((forward + up) * 100000 * Time.fixedDeltaTime);
            yield return new WaitForSeconds(.08f);
        }
        reset?.Invoke();
    }

}