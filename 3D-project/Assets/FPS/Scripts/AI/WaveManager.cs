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

        EnemyManager m_EnemyManager;

        private void Awake()
        {
            EventManager.AddListener<WaveStartedEvent>(OnWaveStarted);
        }

        void Start()
        {
            if (m_EnemyManager == null)
                m_EnemyManager = FindObjectOfType<EnemyManager>();

            WaveStartedEvent firstWave = Events.WaveStartedEvent;
            firstWave.WaveNumber = CurrentWave + 1;
            EventManager.Broadcast(firstWave);
        }

        private void OnWaveStarted(WaveStartedEvent evt) {  StartNextWave(); }

        public void StartNextWave()
        {
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

        void Update()
        {
            if (WaveInProgress && m_EnemyManager.NumberOfEnemiesRemaining == 0)
            {
                WaveInProgress = false;
            }

            Debug.Log(m_EnemyManager.NumberOfEnemiesRemaining);
        }

        void SpawnEnemy()
        {
            if (EnemyPrefabs.Length == 0 || SpawnPoints.Length == 0)
                return;

            var prefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];
            var point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];

            Instantiate(prefab, point.position, point.rotation);
        }
    }
}
