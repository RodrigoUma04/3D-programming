using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Settings")]
        public GameObject[] EnemyPrefabs;
        public Transform[] SpawnPoints;
        public int BaseEnemyCount = 5;
        public float DifficultyMultiplier = 1.5f;

        public int CurrentWave { get; private set; } = 0;
        public bool WaveInProgress { get; private set; }

        private void Awake()
        {
            EventManager.AddListener<WaveStartedEvent>(OnWaveStarted);
            EventManager.AddListener<AllEnemiesKilledEvent>(OnWaveCompleted);
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }

        void Start()
        {
            WaveStartedEvent firstWave = Events.WaveStartedEvent;
            firstWave.WaveNumber = CurrentWave + 1 ;
            EventManager.Broadcast(firstWave);
        }

        private void OnWaveStarted(WaveStartedEvent evt) {  StartNextWave(); }
        private void OnWaveCompleted(AllEnemiesKilledEvent evt) {
            if (WaveInProgress)
            {
                WaveInProgress = false;
                WaveCompletedEvent waveCompletedEvent = Events.WaveCompletedEvent;
                waveCompletedEvent.WaveNumber = CurrentWave;
                EventManager.Broadcast(waveCompletedEvent);
            }
        }

        public void StartNextWave()
        {
            Debug.Log("Wave nr. " +  CurrentWave);
            CurrentWave++;
            WaveInProgress = true;

            WaveStartedEvent evt = Events.WaveStartedEvent;
            evt.WaveNumber = CurrentWave;
            Events.WaveStartedEvent = evt;

            int enemyCount = Mathf.RoundToInt(BaseEnemyCount * Mathf.Pow(DifficultyMultiplier, CurrentWave - 1));

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }
        }

        void SpawnEnemy()
        {
            if (EnemyPrefabs.Length == 0 || SpawnPoints.Length == 0)
                return;

            var prefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
            var point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];

            Instantiate(prefab, point.position, point.rotation);
        }

        private void OnPlayerDeath(PlayerDeathEvent evt)
        {
            Debug.Log("Player died, resetting waves.");

            WaveInProgress = false;

            CurrentWave = 0;

            var enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                foreach (var enemy in enemyManager.Enemies)
                {
                    if (enemy != null)
                        Destroy(enemy.gameObject);
                }
                enemyManager.Enemies.Clear();
            }
        }


        private void OnDestroy()
        {
            EventManager.RemoveListener<WaveStartedEvent>(OnWaveStarted);
            EventManager.RemoveListener<AllEnemiesKilledEvent>(OnWaveCompleted);
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
    }
}
