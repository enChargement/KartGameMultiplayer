using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected abstract float Damage { get; set; }
    protected abstract float Velocity { get; set; }


    public void DamagePlayer(GameObject gameObject)
    {
        WeaponsManager player = gameObject.GetComponent<WeaponsManager>();
        if (player != null)
        {
            player.GetDamaged(Damage);
        }
    }
    public bool CheckTags(string otherTag,Collider other)
    {
        string myTag = tag;
        switch (myTag)
        {
            case "RedTeam":
                if (otherTag.Equals("BlueTeam"))
                {
                    DamagePlayer(other.gameObject);
                   Destroy(gameObject);
                    return true;
                }
                else if (otherTag.Equals("RedTeam"))
                {
                    return false;
                }
                return false;
                break;
            case "BlueTeam":
                if (otherTag.Equals("RedTeam"))
                {
                    DamagePlayer(other.gameObject);
                    Destroy(gameObject);
                    return true;
                }
                else if (otherTag.Equals("BlueTeam"))
                {
                    return false;
                }
                return false;
                break;
            default:
                Destroy(gameObject);
                return true;
                break;
        }
    }

    public void CheckTagsForState(string otherTag,Collider other)
    {
        string myTag = tag;
        switch (myTag)
        {
            case "RedTeam":
                if (otherTag.Equals("BlueTeam"))
                {
                    DamagePlayer(other.gameObject);
                }
                else if (otherTag.Equals("RedTeam"))
                {
                    return;
                }
                break;
            case "BlueTeam":
                if (otherTag.Equals("RedTeam"))
                {
                    DamagePlayer(other.gameObject);
                }
                else if (otherTag.Equals("BlueTeam"))
                {
                    return;
                }
                break;
        }
    }

}
