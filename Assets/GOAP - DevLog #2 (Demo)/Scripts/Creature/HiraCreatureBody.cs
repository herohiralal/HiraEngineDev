namespace UnityEngine
{
    public class HiraCreatureBody : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private new Renderer renderer = null;

        public Animator Animator => animator;
        public Renderer Renderer => renderer;

        private void Reset()
        {
            Populate(ref animator);
            Populate(ref renderer);
        }

        private void OnValidate()
        {
            Populate(ref animator);
            Populate(ref renderer);
        }

        private void Populate<T>(ref T o) where T : Component
        {
            if (o == null)
                o = GetComponent<T>();
            if (o == null)
                o = GetComponentInChildren<T>();
        }
    }
}