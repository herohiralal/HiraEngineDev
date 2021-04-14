namespace UnityEngine.Internal
{
    [HiraManager, HiraConsole]
    public class HiraConsoleController : MonoBehaviour
    {
        private bool _consoleActive = false;
        [SerializeField] private HiraConsoleGUI gui = null;
        [SerializeField] private string[] commands = null;

        private void Awake()
        {
	        commands = HiraConsoleCommandRegistry.COMMANDS;
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
            enabled = !_consoleActive;
        }
    }
}