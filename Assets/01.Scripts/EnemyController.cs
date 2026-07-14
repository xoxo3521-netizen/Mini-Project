using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("슬라임 능력치")]
    public float dashForce = 7f;
    public float patternInterval = 5f;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private float timer = 0f;

    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;

        GameObject playerObj = GameObject.Find("Player");
        if(playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("플레이어 감지 오류");
        }
    }

    void Update()
    {
        if(playerTransform  != null)
        {
            if(playerTransform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
        }

        timer += Time.deltaTime;

        if(timer >= patternInterval)
        {
            ExecuteRandomPattern();
            timer = 0f;
        }
    }

    void ExecuteRandomPattern()
    {
        if(playerTransform == null)
        {
            return;
        }

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
        rb.linearVelocity = Vector2.zero;

        Vector2 directionToPlayer = (playerTransform.position - transform.position);
        directionToPlayer.Normalize();
        rb.AddForce(directionToPlayer * dashForce, ForceMode2D.Impulse);
    }

    void Attack()
    {
        Debug.Log("슬라임 패턴 : 공격! ");
        rb.linearVelocity = Vector2.zero;

        Transform weapon = transform.Find("weapon");
        if(weapon != null)
        {
            StartCoroutine(SwingWeaponRoutine(weapon));
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
