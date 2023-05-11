using UnityEngine;

public class Nuke : Weapon
{
    private float _damage = 100f;
    private float _velocity = 10f;
    [SerializeField] private GameObject _explosionCollider;
    [SerializeField] private ParticleSystem _explosion;
    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }

    void Update()
    {
        transform.Translate(transform.forward * Velocity * Time.deltaTime, Space.World);
    }

    private void Explosion()
    {
        Velocity = 0;
        Instantiate(_explosion,transform.position,Quaternion.identity);
        _explosionCollider.SetActive(true);
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

    private void OnCollisionEnter(Collision collision)
    {
        string myTag = tag;
        string otherTag = collision.gameObject.tag;
        Debug.Log("mytag : " + myTag + "otherTag : " + otherTag);
        if (otherTag.Equals("Ground") || otherTag.Equals("Wall"))
        {
            Explosion();
        }
        else
        {
            switch (myTag)
            {
                case "RedTeam":
                    if (otherTag.Equals("BlueTeam"))
                    {
                        DamagePlayer(collision.gameObject);
                        Explosion();
                    }
                    else if (otherTag.Equals("RedTeam"))
                    {
                        return;
                    }
                    break;
                case "BlueTeam":
                    if (otherTag.Equals("RedTeam"))
                    {
                        DamagePlayer(collision.gameObject);
                        Explosion();
                    }
                    else if (otherTag.Equals("BlueTeam"))
                    {
                        return;
                    }
                    break;
                default:
                    Debug.Log("default");
                    Explosion();
                    break;
            }
        }

    }
}
