using System;
using System.Collections;
using UnityEngine;

public class FlagVisual : WeaponVisual
{
    private string _name = "Flag";
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject flag = Instantiate(gameObject,transform.position,transform.rotation);
        Collider col = flag.GetComponent<Collider>();
        string myTag = tag;
        flag.name = "Flag";
        Debug.Log("mon tag : " + myTag);
        string flagTag = myTag.Equals("BlueTeam") ? "RedTeam" : "BlueTeam";
        flag.tag = flagTag;
        col.enabled = true;
        yield return null;
        reset?.Invoke();
    }
}
