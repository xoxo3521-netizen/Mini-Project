using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BossSpawnController : MonoBehaviour
{
    [Header("보스 설정")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("타일 문 설정")]
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private TileBase openDoorTile;
    [SerializeField] private Vector3Int[] doorPositions;
   
    [Header("타이밍 설정")]
    [SerializeField] private float inWait = 3f;
    [SerializeField] private float doorOpenDelay = 3f;


    private bool isFightStarted = false;

    public void StartBossFight()
    {
        if(isFightStarted) return;
        isFightStarted = true;

        StartCoroutine(BossSpawnSequence());
    }

    IEnumerator BossSpawnSequence()
    {
        yield return new WaitForSeconds(inWait);

        if(doorTilemap != null && openDoorTile != null && doorPositions != null)
        {
            foreach(Vector3Int pos in doorPositions)
            {
                doorTilemap.SetTile(pos, openDoorTile);
            }
            Debug.Log("보스방 문이 열립니다.");
        }

        yield return new WaitForSeconds(doorOpenDelay);

        if (bossPrefab != null && bossSpawnPoint != null)
        {
            Instantiate(bossPrefab,bossSpawnPoint.position, Quaternion.identity);
            Debug.Log("보스가 등장했습니다!");
        }

    }
}
