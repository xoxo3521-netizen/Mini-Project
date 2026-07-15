using UnityEngine;

public interface IDamageable
{
    float CurrentHp { get; }
    float MaxHp { get; }
    void TakeDamage(float damage);
}
