using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro stageText;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public GameObject eaglePrefab;
    public int poolSize = 30;
    private List<GameObject> enemySpawnPool = new List<GameObject>();

    [Header("Stage Settings")]
    public LevelData levelData; // 更新為 LevelData
    public List<StageExcel> stages; // 使用 StageExcel 替代 StageArrange

[Header("當前stage")]
    public int currentStageIndex = -1;

    public float timer = 180f;
    private bool isSpawning;

    // 場景中的敵人數量統計
    public int currentEnemiesInScene = 0;
    private int currentEaglesInScene = 0;

	public static LevelManager instance;

    void Start()
    {
        InitializePool();
		if (instance == null){instance = this;}
		else{Destroy(gameObject);}

        if (levelData != null)
        {
            stages = levelData.工作表1; // 初始化階段數據
            StartCoroutine(StageManager());
        }
        else
        {
            Debug.LogError("LevelData is not assigned!");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
		Debug.Log("currentEnemiesInScene: " + currentEnemiesInScene);
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = (i < poolSize / 2) ? Instantiate(enemyPrefab) : Instantiate(eaglePrefab);
            enemy.SetActive(false);
            enemySpawnPool.Add(enemy);
        }
    }

   IEnumerator StageManager()
{
    bool isStageActive = false; // 旗標：是否有階段正在進行

    while (timer > 0)
    {
        // 確保不超出 stages 的範圍
        if (currentStageIndex + 1 < stages.Count)
        {
            // 檢查是否進入下一階段
            if (timer <= stages[currentStageIndex + 1].timerSpawn && !isStageActive)
            {
                currentStageIndex++;
                isStageActive = true; // 設定階段已經開始
                Debug.Log($"Transitioning to Stage: {currentStageIndex + 1}");

                // 開始新階段的生成並在階段完成時重置旗標
                StartCoroutine(ManageSpawning(currentStageIndex, () => { isStageActive = false; }));
                stageText.text = $"Stage: {currentStageIndex + 1}";
            }
        }
        else if (!isStageActive)
        {
            // 最後一個階段且旗標未啟動
            currentStageIndex++;
            isStageActive = true;
            Debug.Log($"Transitioning to Final Stage: {currentStageIndex + 1}");

            StartCoroutine(ManageSpawning(currentStageIndex, () => { isStageActive = false; }));
            stageText.text = $"Final Stage: {currentStageIndex + 1}";
        }

        yield return null; // 每幀執行一次檢查
    }
}


IEnumerator ManageSpawning(int stageIndex, System.Action onStageComplete)
{
    int maxEnemiesInStage = stages[stageIndex].timerMaxSpawn;
    Debug.Log($"Stage {stageIndex + 1} - Max Enemies to Spawn: {maxEnemiesInStage}");

    while (currentStageIndex == stageIndex)
    {
        // 檢查當前敵人數量是否少於最大生成數量
        if (currentEnemiesInScene + currentEaglesInScene < maxEnemiesInStage && !isSpawning)
        {
            isSpawning = true;
            SpawnEnemy();
            isSpawning = false;
        }

        // 檢查是否所有敵人都已經被消滅
        if (currentEnemiesInScene == 0 && currentEaglesInScene == 0)
        {
            Debug.Log("All enemies defeated, ending current stage.");
            onStageComplete?.Invoke(); // 執行回呼來重置旗標
            break;
        }

        yield return new WaitForSeconds(1f); // 每秒檢查一次
    }

    // 確保結束階段時重置旗標
    Debug.Log($"Stage {stageIndex + 1} complete.");
    onStageComplete?.Invoke();
}




    void SpawnEnemy()
    {
        GameObject enemy = GetEnemyOrEagleFromPool();
        if (enemy != null)
        {
            enemy.transform.position = GetRandomSpawnPoint().position;
            enemy.SetActive(true);

            if (enemy.CompareTag("Enemy"))
            {
                currentEnemiesInScene++;
            }
            else if (enemy.CompareTag("Eagle"))
            {
                currentEaglesInScene++;
            }
        }
    }

    GameObject GetEnemyOrEagleFromPool()
    {
        GameObject chosenEnemy = null;

        // 判斷當前場景中的 Eagle 和 Enemy 比例
        float eagleRatio = (float)currentEaglesInScene / (currentEnemiesInScene + currentEaglesInScene + 1);

        // 如果場景中Eagle比例過低，則優先生成Eagle
        if (eagleRatio < 0.3f && currentEaglesInScene < currentEnemiesInScene)
        {
            chosenEnemy = GetSpecificEnemyFromPool(eaglePrefab);
            if (chosenEnemy == null)
            {
                // 如果沒有可用的Eagle，退回生成Enemy
                chosenEnemy = GetSpecificEnemyFromPool(enemyPrefab);
            }
        }
        else
        {
            // 否則隨機選擇生成敵人類型
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                chosenEnemy = GetSpecificEnemyFromPool(eaglePrefab);
                if (chosenEnemy == null)
                {
                    // 如果沒有可用的Eagle，退回生成Enemy
                    chosenEnemy = GetSpecificEnemyFromPool(enemyPrefab);
                }
            }
            else
            {
                chosenEnemy = GetSpecificEnemyFromPool(enemyPrefab);
                if (chosenEnemy == null)
                {
                    // 如果沒有可用的Enemy，退回生成Eagle
                    chosenEnemy = GetSpecificEnemyFromPool(eaglePrefab);
                }
            }
        }

        return chosenEnemy;
    }

    GameObject GetSpecificEnemyFromPool(GameObject prefab)
    {
        foreach (GameObject enemy in enemySpawnPool)
        {
            if (!enemy.activeInHierarchy && enemy.CompareTag(prefab.tag))
            {
                return enemy;
            }
        }
        Debug.LogWarning("No available enemy of type " + prefab.tag + " in pool!");
        return null;
    }

    Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (enemy.CompareTag("Enemy"))
        {
            currentEnemiesInScene--;
			Debug.Log("Enemy destroyed. Current enemies in scene: " + currentEnemiesInScene);
        }
        else if (enemy.CompareTag("Eagle"))
        {
            currentEaglesInScene--;
        }
        enemy.SetActive(false);
    }
}
