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
        private const float REBUILD_DELAY = 6f;
        private static bool _isBuilding = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public static void Initialize(GameObject terrainParent)
        {
            if (_sharedNavMeshSurface != null) return;
            
            _sharedNavMeshSurface = terrainParent.AddComponent<NavMeshSurface>();
            _sharedNavMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            _sharedNavMeshSurface.collectObjects = CollectObjects.Children;
        }

        public static void RequestNavMeshBuild()
        {
            if (_instance == null || _sharedNavMeshSurface == null || _isBuilding) return;

            if (_buildCoroutine != null)
                _instance.StopCoroutine(_buildCoroutine);

            _buildCoroutine = _instance.StartCoroutine(_instance.BuildNavMeshAsync());
        }

        private IEnumerator BuildNavMeshAsync()
        {
            _isBuilding = true;
            yield return new WaitForSeconds(REBUILD_DELAY);

            if (_sharedNavMeshSurface != null)
            {
                _sharedNavMeshSurface.BuildNavMesh();
            }

            _buildCoroutine = null;
            _isBuilding = false;
        }
    }

}