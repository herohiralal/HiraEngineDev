namespace UnityEngine.Internal
{
    public class HiraConsoleGUI : MonoBehaviour
    {
        [SerializeField] private string input = "";

        [SerializeField] private HiraConsoleController controller = null;

        private void Awake()
        {
            controller = GetComponent<HiraConsoleController>();
        }

        private void OnDestroy()
        {
            controller = null;
        }

        private void OnEnable()
        {
            input = "";
        }

        private void OnGUI()
        {
            var y = 0f;

            GUI.Box(new Rect(0, y, Screen.width, 40), "");

            const string controlName = "Console";

            GUI.SetNextControlName(controlName);
            input = GUI.TextArea(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
            GUI.FocusControl(controlName);

            if (input.EndsWith("`"))
            {
                controller.Toggle();
            }
            else if (input.EndsWith("\n"))
            {
                controller.Toggle();
            }
        }
    }
}