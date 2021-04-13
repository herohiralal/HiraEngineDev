namespace UnityEngine.Internal
{
    [HiraManager]
    public class HiraConsoleController : MonoBehaviour
    {
        private bool _consoleActive = false;
        [SerializeField] private HiraConsoleGUI gui = null;

        private void Awake()
        {
            gui = gameObject.AddComponent<HiraConsoleGUI>();
            gui.enabled = false;
        }

        private void OnDestroy()
        {
            gui.enabled = true;
            Destroy(gui);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote)) Toggle();
        }

        public void Toggle()
        {
            _consoleActive = !_consoleActive;
            Time.timeScale = _consoleActive ? 0f : 1f;
            gui.enabled = _consoleActive;
        }

        public void Handle(string command)
        {
            
        }
    }
}