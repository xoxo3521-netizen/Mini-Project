using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("플레이어 스탯")]
    [SerializeField] private float maxHp = 150f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float invincibilityDuration = 1.0f;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;

    [Header("사운드 설정")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource _audioSource;

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
        _audioSource = GetComponentInChildren<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        bool isHardMode = (GameData.Instance != null && GameData.Instance.selectedMode == GameMode.Hard);

        if (isHardMode && GameData.Instance != null && GameData.Instance.savedPlayerHp > 0)
        {
            _currentHp = GameData.Instance.savedPlayerHp;
        }
        else
        {
            _currentHp = maxHp;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHPBar(_currentHp, maxHp);
        }

        InitializeSceneObjects();
    }

    private void OnDisable()
    {
        if (GameData.Instance != null && GameData.Instance.selectedMode == GameMode.Hard)
        {
            GameData.Instance.savedPlayerHp = _currentHp;
        }
    }

    private void Update()
    {
        float moveX = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float moveY = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        _moveDir = new Vector2(moveX, moveY).normalized;

        if (_anim != null)
        {
            _anim.SetBool("isRun", _moveDir.magnitude > 0);
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        float scaleX = (mouseWorldPos.x > transform.position.x) ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        if (Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
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

        if (_anim != null)
        {
            _anim.SetTrigger("doAttack");
        }

        yield return new WaitForSeconds(0.8f);
        Attack();
        isAttacking = false;
    }

    void Attack()
    {
        if (attackSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(attackSound);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);
        bool hasHitEnemy = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                hasHitEnemy = true;
            }
        }

        if (hasHitEnemy && hitSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_invisibility.IsInvincible) return;

        _currentHp = Mathf.Max(0f, _currentHp - damage);
        UpdateHpUI();
        Debug.Log($"플레이어가 {damage}의 데미지를 입었습니다! 남은 HP : {_currentHp}");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHPBar(_currentHp, maxHp);
        }

        if (_currentHp <= 0)
        {
            Die();
        }
        else
        {
            _invisibility.TriggerInvincibility(invincibilityDuration);
        }
    }

    public void Die()
    {
        Debug.Log("플레이어가 사망하였습니다.");
        if (deathSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(deathSound);
        }

        if (MazeManager.Instance != null)
        {
            MazeManager.Instance.PlayerDeath();
            gameObject.SetActive(false);
        }
        else if (GameManager.Instance != null && GameManager.Instance.currentMode == GameMode.Normal)
        {
            GameManager.Instance.PlayerDeath();
            gameObject.SetActive(false);
        }
        else
        {
            if (MazeManager.Instance != null)
            {
                MazeManager.Instance.TriggerGameOver();
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            Destroy(gameObject, 1f);
        }
    }

    public void FindNewHpSlider()
    {
        GameObject sliderObj = GameObject.Find("hpSlider");
        if (sliderObj != null)
        {
            hpSlider = sliderObj.GetComponent<Slider>();
            UpdateHpUI();
            Debug.Log("새로운 씬의 hpSlider를 찾아서 연결");
        }
    }

    public void UpdateHpUI()
    {
        if (hpSlider != null)
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
        if (startPointObj != null)
        {
            transform.position = startPointObj.transform.position;
        }

        FindNewHpSlider();

        bool isHardMode = (GameData.Instance != null && GameData.Instance.selectedMode == GameMode.Hard);
        if (!isHardMode)
        {
            _currentHp = maxHp;
        }

        UpdateHpUI();

        if (GameManager.Instance != null && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMissionText(GameManager.Instance.currentKillCount, GameManager.Instance.targetKillCount);
        }
    }

    public void ResetHealth()
    {
        _currentHp = maxHp;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        isAttacking = false;
        UpdateHpUI();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHPBar(_currentHp, maxHp);
        }
    }
}