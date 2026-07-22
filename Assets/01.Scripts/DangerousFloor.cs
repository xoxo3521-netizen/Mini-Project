using UnityEngine;

public class DangerousFloor : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float duration = 5f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
