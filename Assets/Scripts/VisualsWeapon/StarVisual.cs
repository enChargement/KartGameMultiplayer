using System;
using System.Collections;
using UnityEngine;

public class StarVisual : WeaponVisual
{
    private string _name = "Star";
    public override string WeaponName { get { return _name; } set => _name = value; }

    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject _center;
    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        GameObject star = Instantiate(Weapon, transform);
        _renderer.enabled = true;
        _center.SetActive(true);
        star.tag = gameObject.tag;
        yield return new WaitForSeconds(5f);
        reset?.Invoke();
    }
}
