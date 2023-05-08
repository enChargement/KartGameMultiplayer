using UnityEngine;

public class Bullet : Weapon
{
    private float _damage = 20f;
    private float _velocity = 25f;

    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }

    void Update()
    {
        MoveWeapon();
    }

    private void MoveWeapon()
    {
        transform.Translate(transform.forward * Velocity * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.tag;
        if (otherTag.Equals("Ground") || otherTag.Equals("Wall"))
        {
            Destroy(gameObject);
        }
        else
        {
            CheckTags(otherTag, other);
        }
    }
}
