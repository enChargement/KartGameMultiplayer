using UnityEngine;


public class Spikes : Weapon
{
    private float _damage = 100f;
    private float _velocity = 0f;

    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }

    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.tag;
        CheckTagsForState(otherTag,other);
    }
}
