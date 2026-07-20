using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("UI")]
    [SerializeField] private Slider hpSlider;

    private float _currentHp;
    private Rigidbody2D _rb;
    private Collider2D _col;
    private Animator _anim;
    private PlayerInvisibility _invisibility;

    private Vector2 _moveDir;

    private bool isAttacking = false;

    public float CurrentHp => _currentHp;
    public float MaxHp => maxHp;

    public static PlayerController Instance { get; private set; }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _anim = GetComponentInChildren<Animator>();
        _invisibility = GetComponent<PlayerInvisibility>();

        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        _currentHp = maxHp;

        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHPBar(_currentHp,maxHp);
        }

        InitializeSceneObjects();
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

        if(Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
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
        isAttacking = true;

        if(_anim != null)
        {
            _anim.SetTrigger("doAttack");
        }

        yield return new WaitForSeconds(0.8f);
        Attack();
        isAttacking = false;
    }
    void Attack()
    {
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
        UpdateHpUI();
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

        if(MazeManager.Instance != null )
        {
            MazeManager.Instance.TriggerGameOver();
        }
        else if(GameManager.Instance != null )
        {
            GameManager.Instance.GameOver();
        }
            Destroy(gameObject);
    }

    public void FindNewHpSlider()
    {
        GameObject sliderObj = GameObject.Find("hpSlider");
        if(sliderObj != null )
        {
            hpSlider = sliderObj.GetComponent<Slider>();
            UpdateHpUI();
            Debug.Log("새로운 씬의 hpSlider를 찾아서 연결");
        }
    }

    public void UpdateHpUI()
    {
        if(hpSlider != null)
        {
            hpSlider.value = _currentHp / maxHp;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeSceneObjects();
    }

    private void InitializeSceneObjects()
    {
        GameObject startPointObj = GameObject.Find("StartPoint");
        if(startPointObj != null )
        {
            transform.position = startPointObj.transform.position;
        }

        FindNewHpSlider();
        UpdateHpUI();

        if(GameManager.Instance != null && UIManager.Instance != null )
        {
            UIManager.Instance.UpdateMissionText(GameManager.Instance.currentKillCount, GameManager.Instance.targetKillCount);
        }
    }
}   
