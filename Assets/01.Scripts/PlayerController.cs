using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    public float maxHp = 100f;
    public float currentHp;
    public float attackDamage = 10f;

    [Header("공격 설정")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    [SerializeField] float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    Vector2 moveDir;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        currentHp = maxHp;
    }

    private void Update()
    {
        float moveX = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float moveY = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        moveDir = new Vector2(moveX, moveY).normalized;

        if(anim != null )
        {
            anim.SetBool("isRun", moveDir.magnitude > 0);
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        float scaleX = (mouseWorldPos.x > transform.position.x) ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir * moveSpeed;
    }
    void Attack()
    {
        if(anim != null)
        {
            anim.SetTrigger("doAttack");
        }
        Debug.Log("기본 공격");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyController enemyScript = enemy.GetComponent<EnemyController>();
            if(enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"플레이어가 {damage}의 데미지를 입었습니다! 남은 HP : {currentHp}");

        if(currentHp < 0)
        {
            Die();
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
