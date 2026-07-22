using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("능력치")]
    [SerializeField] protected float maxHp = 25f;
    [SerializeField] protected float contactDamage = 5f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float dashForce = 225f;    /// Mass 55
    [SerializeField] protected float patternInterval = 5f;
    [SerializeField] protected float deathFadeDuration = 0.5f;

    private EnemyHitFlash _hitFlash;
    private EnemyFadeOut _fadeOut;
    private Collider2D _enemyColider;

    protected float _currentHp;
    protected Rigidbody2D _rb;
    protected Transform _playerTransform;
    private float _timer = 0f;
    private Vector3 _originalScale;
    private bool _isDead = false;

    public float CurrentHp => _currentHp;
    public float MaxHp => maxHp;

    private void Awake()
    {
        _enemyColider = GetComponent<Collider2D>();
        _hitFlash = GetComponent<EnemyHitFlash>();
        _fadeOut = GetComponent<EnemyFadeOut>();
    }
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _originalScale = transform.localScale;
        _currentHp = maxHp;

        GameObject playerObj = GameObject.Find("Player");
        if(playerObj != null)
        {
            _playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if(_isDead) return;

        HandleRotation();
        HandlePatternTimer();
    }

    private void HandleRotation()
    {
        if(_playerTransform == null) return;

        if(_playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(_originalScale.x),_originalScale.y, _originalScale.z);
        }
    }

    private void HandlePatternTimer()
    {
        _timer += Time.deltaTime;
        if(_timer >= patternInterval)
        {
            ExecuteRandomPattern();
            _timer = 0f;
        }
    }
    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHp -= damage;
        Debug.Log($"슬라임이 {damage}의 데미지를 입었습니다. 남은 HP : {_currentHp}");

        if(_currentHp <= 0)
        {
            Die();
        }
        else
        {
            _hitFlash.PlayFlash();
        }
    }

    protected virtual void Die()
    {
        _isDead = true;
        Debug.Log("몬스터 사망!");

        if(GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
        }

        if(_enemyColider != null)
        {
            _enemyColider.enabled = false;
        }

        _fadeOut.PlayFadeOut(deathFadeDuration, () => {Destroy(gameObject); });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_isDead) return;
            
        if(collision.gameObject.CompareTag("Player"))
        {
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if(player != null)
            {
                player.TakeDamage(contactDamage);
            }
        }
    }
    void ExecuteRandomPattern()
    {
        if (_playerTransform == null) return;

        int randomPattern = Random.Range(0, 2);

        switch(randomPattern)
        {
            case 0: BodySlam();
                break;
            case 1: Attack();
                break;
        }
    }

    void BodySlam()
    {
        Debug.Log("슬라임 패턴 : 몸통박치기! ");
        _rb.linearVelocity = Vector2.zero;

        Vector2 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        _rb.AddForce(directionToPlayer * dashForce, ForceMode2D.Impulse);
    }

    void Attack()
    {
        Debug.Log("슬라임 패턴 : 공격! ");
        _rb.linearVelocity = Vector2.zero;

        Transform weapon = transform.Find("weapon");
        if(weapon != null)
        {
            StartCoroutine(SwingWeaponRoutine(weapon));
        }

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach(Collider2D target in hitTargets)
        {
            if (target.CompareTag("Player"))
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
    }
    
    IEnumerator SwingWeaponRoutine(Transform weaponTransform)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Quaternion startRot = weaponTransform.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0, 0, -90f);

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            weaponTransform.localRotation = Quaternion.Slerp(startRot,targetRot,elapsed / duration);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        elapsed = 0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            weaponTransform.localRotation = Quaternion.Slerp(targetRot,startRot,elapsed / duration);
            yield return null;
        }
    }
}
