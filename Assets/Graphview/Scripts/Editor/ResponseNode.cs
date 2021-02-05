using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class ResponseNode : Node
	{
		public ResponseNode(string value)
		{
			capabilities |= Capabilities.Renamable;
			
			Input = base.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Response));
			Input.portName = "Response";
			inputContainer.Add(Input);

			Next = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
			Next.portName = "Next";
			outputContainer.Add(Next);

			var titleLabelField = typeof(Node).GetField("m_TitleLabel", BindingFlags.NonPublic | BindingFlags.Instance);
			var titleLabel = (Label) titleLabelField?.GetValue(this);

			Text = value;

			titleContainer.Remove(titleLabel);
			titleContainer.Remove(titleButtonContainer);
			{
				_textField = new TextField {value = value};
				_textField.RegisterValueChangedCallback(OnTextFieldValueChange);
				titleContainer.Add(_textField);
			}
			titleContainer.Add(titleButtonContainer);
		}

		private void OnTextFieldValueChange(ChangeEvent<string> evt)
		{
			Text = evt.newValue;
		}

		~ResponseNode()
		{
			titleContainer.Remove(_textField);
			_textField.UnregisterValueChangedCallback(OnTextFieldValueChange);

			outputContainer.Remove(Next);
			inputContainer.Remove(Input);
		}

		public string Text;
		public readonly Port Input = null;
		public readonly Port Next = null;
		private readonly TextField _textField;
	}
}