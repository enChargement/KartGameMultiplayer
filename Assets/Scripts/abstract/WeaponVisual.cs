using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponVisual : MonoBehaviour
{
    public GameObject Weapon;
    public Transform FirePoint;

    public abstract string WeaponName { get; set; }

    public abstract IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset);
}