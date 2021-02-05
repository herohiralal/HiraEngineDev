using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueNode : Node
	{
		public DialogueNode(string value, int responseCount = 0)
		{
			capabilities |= Capabilities.Renamable;
			
			Input = base.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Dialogue));
			Input.portName = "Dialogue";
			inputContainer.Add(Input);
			
			Responses = new List<Port>(responseCount);
			
			for (var i = 0; i < responseCount; i++) AddOutputPort(i);

			_addChoiceButton = new Button(AddChoice) {text = "+"};
			outputContainer.Add(_addChoiceButton);

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

		~DialogueNode()
		{
			titleContainer.Remove(_textField);
			_textField.UnregisterValueChangedCallback(OnTextFieldValueChange);

			outputContainer.Remove(_addChoiceButton);
			foreach (var response in Responses) outputContainer.Remove(response);
			inputContainer.Remove(Input);
		}

		private void AddOutputPort(int i)
		{
			var outputPort = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Response));
			outputPort.portName = $"Choice {i + 1}";
			outputContainer.Add(outputPort);
			Responses.Add(outputPort);
		}

		private void AddChoice()
		{
			outputContainer.Remove(_addChoiceButton);
			AddOutputPort(Responses.Count);
			outputContainer.Add(_addChoiceButton);
			
			RefreshExpandedState();
			RefreshPorts();
		}

		public string Text;
		private readonly Button _addChoiceButton = null;
		public readonly Port Input = null;
		public readonly List<Port> Responses = null;
		private readonly TextField _textField;
	}
}