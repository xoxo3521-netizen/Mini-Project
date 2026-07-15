using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("플레이어 스탯")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float invincibilityDuration = 1.0f;

    [Header("공격 설정")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayers;

    private float _currentHp;
    private Rigidbody2D _rb;
    private Collider2D _col;
    private Animator _anim;
    private PlayerInvisibility _invisibility;

    private Vector2 _moveDir;

    public float CurrentHp => _currentHp;
    public float MaxHp => maxHp;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _anim = GetComponentInChildren<Animator>();
        _invisibility = GetComponent<PlayerInvisibility>();
    }

    private void Start()
    {
        _currentHp = maxHp;

        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHPBar(_currentHp,maxHp);
        }
    }

    private void Update()
    {
        float moveX = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float moveY = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        _moveDir = new Vector2(moveX, moveY).normalized;

        if(_anim != null )
        {
            _anim.SetBool("isRun", _moveDir.magnitude > 0);
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        float scaleX = (mouseWorldPos.x > transform.position.x) ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(AttackDelayRoutine());
        }
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDir * moveSpeed;
    }

    IEnumerator AttackDelayRoutine()
    {
        if(_anim != null)
        {
            _anim.SetTrigger("doAttack");
        }

        yield return new WaitForSeconds(0.2f);
        Attack();
    }
    void Attack()
    {
        if(_anim != null)
        {
            _anim.SetTrigger("doAttack");
        }
        Debug.Log("기본 공격");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_invisibility.IsInvincible) return;

        _currentHp = Mathf.Max(0f, _currentHp - damage);
        Debug.Log($"플레이어가 {damage}의 데미지를 입었습니다! 남은 HP : {_currentHp}");

        if(UIManager.Instance != null )
        {
            UIManager.Instance.UpdateHPBar(_currentHp, maxHp);
        }

        if(_currentHp <= 0)
        {
            Die();
        }
        else
        {
            _invisibility.TriggerInvincibility(invincibilityDuration);
        }
    }

    void Die()
    {
        Debug.Log("플레이어가 사망하였습니다.");
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
