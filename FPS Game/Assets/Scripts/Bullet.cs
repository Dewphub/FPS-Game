using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int timer;

    Transform shooter;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();
        damageable?.TakeDamage(damage);
        if(other.gameObject.CompareTag("Player"))
        {
            Register();
        }

        Destroy(gameObject);
    }

    public void SetShooter(Transform _shooter)
    {
        this.shooter = _shooter;
    }

    void Register()
    {
        if(!DamageIndicatorSystem.CheckIfObjectInSight(shooter))
        {
            DamageIndicatorSystem.CreateIndicator(shooter);
        }
    }
}
