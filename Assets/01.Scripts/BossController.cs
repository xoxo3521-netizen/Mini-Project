using UnityEngine;
using System.Collections;

public class BossController : EnemyController
{
    [Header("보스 고유 설정")]
    [SerializeField] private float bossMaxHp = 100f;
    [SerializeField] private float bossContactDamage = 20f;
    [SerializeField] private float bossAttackDamage = 25f;
    [SerializeField] private float bossDashForce = 500f;

    [Header("공격 범위 및 스킬 설정")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private GameObject minionSlimePrefab;
    [SerializeField] private GameObject poisonFloorPrefab;
    [SerializeField] private GameObject jumpSmashFloorPrefab;

    [Header("스킬 쿨타임")]
    [SerializeField] private float skillCooldown = 20f;

    private Transform playerTransform;
    private bool hasSummonedMinions = false;

    public new float MaxHp => bossMaxHp;
    public new float CurrentHp => _currentHp;

    protected override void Start()
    {
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
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                Instantiate(minionSlimePrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    protected override void Die()
    {
        Debug.Log("보스가 처치되었습니다!");
        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}