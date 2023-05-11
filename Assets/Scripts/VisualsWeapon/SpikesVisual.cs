using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesVisual : WeaponVisual
{
    private string _name = "Spikes";
    [SerializeField] List<ParticleSystem> _particles = new();
    public override string WeaponName { get { return _name; } set => _name = value; }

    public override IEnumerator ActivateWeapon(Vector3 spawnVelocity, Action reset)
    {
        _particles.ForEach(ps => { ps.Play(); });
        GameObject spikes = Instantiate(Weapon, transform);
        spikes.tag = gameObject.tag;
        yield return new WaitForSeconds(5f);
        reset?.Invoke();
    }
}
