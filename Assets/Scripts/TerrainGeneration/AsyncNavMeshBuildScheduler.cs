using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace TerrainGeneration
{
    public class AsyncNavMeshBuildScheduler : MonoBehaviour
    {
        private static AsyncNavMeshBuildScheduler _instance;
        private static NavMeshSurface _sharedNavMeshSurface;
        private static Coroutine _buildCoroutine;
        private const float REBUILD_DELAY = 3f;
        private const float BUILD_COOLDOWN = 5f;
        private static float _lastBuildRequestTime = -999f;
        private static bool _isBuilding = false;
        public static bool isNavMeshBaked = false;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public static void Initialize(GameObject terrainParent)
        {
            if (_sharedNavMeshSurface != null) return;

            if (_instance == null)
            {
                // Create instance if it doesn't exist
                if (_instance == null)
                {
                    GameObject schedulerObject = new GameObject("AsyncNavMeshBuildScheduler");
                    _instance = schedulerObject.AddComponent<AsyncNavMeshBuildScheduler>();
                    DontDestroyOnLoad(schedulerObject);
                }
            }
            // Setup NavMeshSurface
            if (_sharedNavMeshSurface == null)
            {
                _sharedNavMeshSurface = terrainParent.GetComponent<NavMeshSurface>();
                if (_sharedNavMeshSurface == null)
                {
                    _sharedNavMeshSurface = terrainParent.AddComponent<NavMeshSurface>();
                }
                _sharedNavMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                _sharedNavMeshSurface.collectObjects = CollectObjects.Children;
            }
            _sharedNavMeshSurface = terrainParent.AddComponent<NavMeshSurface>();
            _sharedNavMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            _sharedNavMeshSurface.collectObjects = CollectObjects.Children;

            _instance.StartCoroutine(_instance.InitialBuildCoroutine());
        }

        public static void RequestNavMeshBuild()
        {
            if (_instance == null || _sharedNavMeshSurface == null || _isBuilding) return;

            if (_isBuilding) return;

            if (Time.time - _lastBuildRequestTime < BUILD_COOLDOWN)
            {
                Debug.Log("[NavMesh] Build request ignored due to cooldown.");
                return;
            }
            
            if(_buildCoroutine != null)
                _instance.StopCoroutine(_buildCoroutine);
            
            _buildCoroutine = _instance.StartCoroutine(_instance.BuildNavMeshAsync());
        }
        private IEnumerator InitialBuildCoroutine()
        {
            _isBuilding = true;
            yield return new WaitForSeconds(1f);

            _sharedNavMeshSurface.BuildNavMesh();
            isNavMeshBaked = true;
            _isBuilding = false;
            _lastBuildRequestTime = Time.time;

            Debug.Log("[NavMesh] Initial build completed. isNavMeshBaked = true");
        }
        private IEnumerator BuildNavMeshAsync()
        {
            yield return new WaitForSeconds(REBUILD_DELAY);

            if (_sharedNavMeshSurface.navMeshData == null)
            {
                _sharedNavMeshSurface.BuildNavMesh();
            }
            else
            {
                _sharedNavMeshSurface.UpdateNavMesh(_sharedNavMeshSurface.navMeshData);
            }
            isNavMeshBaked = true;
            Debug.Log("NavMesh build completed.");
            
            _buildCoroutine = null;
            _isBuilding = false;
        }
    }

}