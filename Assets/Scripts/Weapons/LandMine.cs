using UnityEngine;

public class LandMine : Weapon
{

    private float _damage = 100f;
    private float _velocity = 0f;
    [SerializeField]
    private ParticleSystem _explosion;
    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }

    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.tag;
        bool isFired =CheckTags(otherTag,other);
        if (isFired)
        {
            Instantiate(_explosion, transform.position, Quaternion.identity);
        }
    }
}
