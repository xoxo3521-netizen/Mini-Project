using UnityEngine;
using System.Collections;

public class BossController : EnemyController
{
    [Header("보스 고유 설정 (노말 기준)")]
    [SerializeField] private float bossMaxHp = 200f;
    [SerializeField] private float bossContactDamage = 15f;
    [SerializeField] private float bossAttackDamage = 25f;
    [SerializeField] private float bossDashForce = 400f;

    [Header("하드모드 보스 보정 계수")]
    [SerializeField] private float bossHardHpMultiplier = 1.6f;
    [SerializeField] private float bossHardDamageMultiplier = 1.3f;

    [Header("공격 범위 및 스킬 설정")]
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private GameObject minionSlimePrefab;
    [SerializeField] private GameObject poisonFloorPrefab;
    [SerializeField] private GameObject jumpSmashFloorPrefab;

    [Header("스킬 쿨타임")]
    [SerializeField] private float skillCooldown = 15f;

    [Header("보스 사운드 설정")]
    [SerializeField] private AudioClip bossDeathSound;

    private Transform playerTransform;
    private bool hasSummonedMinions = false;

    public new float MaxHp => bossMaxHp;
    public new float CurrentHp => _currentHp;

    protected override void Start()
    {
        if (GameData.Instance != null && GameData.Instance.selectedMode == GameMode.Hard)
        {
            bossMaxHp *= bossHardHpMultiplier;
            bossContactDamage *= bossHardDamageMultiplier;
            bossAttackDamage *= bossHardDamageMultiplier;
            bossDashForce *= 1.25f;
            skillCooldown *= 0.75f;
        }

        base.Start();

        maxHp = bossMaxHp;
        _currentHp = maxHp;
        contactDamage = bossContactDamage;
        attackDamage = bossAttackDamage;
        dashForce = bossDashForce;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        StartCoroutine(SkillRoutine());
    }

    protected void FixedUpdate()
    {
        if (!hasSummonedMinions && _currentHp <= bossMaxHp * 0.5f)
        {
            SpawnMinionSlime();
            hasSummonedMinions = true;
        }
    }

    public new void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    private IEnumerator SkillRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(skillCooldown);

            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                }
            }

            if (playerTransform != null)
            {
                UseSpecialSkill();
            }
        }
    }

    public void UseSpecialSkill()
    {
        int randomSkill = Random.Range(0, 2);

        if (randomSkill == 0)
        {
            SpawnPoisonFloor();
        }
        else
        {
            SpawnJumpSmash();
        }
    }

    private void SpawnPoisonFloor()
    {
        Debug.Log("보스 스킬: 독성 장판 생성!");
        if (poisonFloorPrefab != null)
        {
            Instantiate(poisonFloorPrefab, transform.position, Quaternion.identity);
        }
    }

    private void SpawnJumpSmash()
    {
        Debug.Log("보스 스킬: 점프 스매시 장판 생성!");
        if (jumpSmashFloorPrefab != null && playerTransform != null)
        {
            Instantiate(jumpSmashFloorPrefab, playerTransform.position, Quaternion.identity);
        }
    }

    private void SpawnMinionSlime()
    {
        Debug.Log("보스 페이즈 전환: 체력 50% 이하! 부하 슬라임 소환!");
        if (minionSlimePrefab != null)
        {
            int spawnCount = (GameData.Instance != null && GameData.Instance.selectedMode == GameMode.Hard) ? 3 : 2;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                Instantiate(minionSlimePrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    protected override void Die()
    {
        Debug.Log("보스가 처치되었습니다!");

        AudioSource audioSource = GetComponent<AudioSource>();
        if (bossDeathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(bossDeathSound);
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ShowGameClear();
        }

        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 centerOffset = transform.position + new Vector3(0, 0f, 0);
        Gizmos.DrawWireSphere(centerOffset, attackRange);
    }
}