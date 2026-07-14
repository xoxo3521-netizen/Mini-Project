using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject slimePrefab;
    public float spawnInterval = 5f;
    public float spawnRange = 5f;

    private float timer = 0f;


    void Update()
    {
        timer += Time.deltaTime;
        
        if(timer >= spawnInterval )
        {
            SpawnSlime();
            timer = 0f;
        }
    }

    void SpawnSlime()
    {
        if(slimePrefab == null )
        {
            Debug.Log("슬라임 프리팹이 없습니다.");
            return;
        }

        float randomX = Random.Range(-spawnRange, spawnRange);
        float randomY = Random.Range(-spawnRange, spawnRange);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0f);

        Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
        Debug.Log("슬라임 스폰 완료! ");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}

