using System;

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

            if (input.Contains("`"))
            {
                controller.Toggle();
                GUI.FocusControl(null);
            }
            else if (input.Contains("\n"))
            {
	            var currentInput = input.Replace("\n", "");
	            input = "";
	            if (!HiraConsoleCommandRegistry.TryInvoke(currentInput))
		            throw new InvalidOperationException("Command unrecognized.");
            }
        }
    }
}