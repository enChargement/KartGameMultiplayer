using UnityEngine;

public class Missile : Weapon
{
    private float _damage = 60f;
    private float _velocity = 20f;

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
        string myTag = tag;
        string otherTag = other.tag;
        if (otherTag.Equals("Ground"))
        {
            Destroy(gameObject);
        }
        else
        {
            CheckTags(otherTag,other);
        }
    }

}
