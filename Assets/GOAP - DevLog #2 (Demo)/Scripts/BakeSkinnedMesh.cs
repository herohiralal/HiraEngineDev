using System.Collections;
using UnityEngine.VFX;

namespace UnityEngine.Internal
{
    public class BakeSkinnedMesh : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer meshRenderer = null;
        [SerializeField] private VisualEffect visualFXGraph = null;
        [SerializeField] private float refreshRate = 0.2f;
        private Coroutine _currentCoroutine = null;
        private WaitForSeconds _currentWaitForSeconds = null;

        private void OnEnable()
        {
            _currentWaitForSeconds = new WaitForSeconds(refreshRate);
            _currentCoroutine = StartCoroutine(UpdateCoroutine());
        }

        private void OnDisable()
        {
            StopCoroutine(_currentCoroutine);
            _currentWaitForSeconds = null;
        }

        private IEnumerator UpdateCoroutine()
        {
            while (enabled)
            {
                var m = new Mesh();
                meshRenderer.BakeMesh(m);
                visualFXGraph.SetMesh("Mesh", m);
                
                yield return _currentWaitForSeconds;
            }
        }
    }
}