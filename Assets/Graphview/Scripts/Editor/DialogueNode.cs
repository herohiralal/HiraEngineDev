using System;
using System.Collections.Generic;
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
			
			Responses = new List<RemovablePort>(responseCount);
			
			for (var i = 0; i < responseCount; i++) AddOutputPort(i);

			_addChoiceButton = new Button(AddChoice) {text = "+"};
			outputContainer.Add(_addChoiceButton);

			Text = value;

			titleContainer.RemoveAt(0);
			_textField = new TextField {value = value};
			_textField.RegisterValueChangedCallback(OnTextFieldValueChange);
			titleContainer.Insert(0, _textField);
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
			
			var responseCount = Responses.Count;
			for (var i = responseCount; i > -1; i--) RemoveOutputPort(i);
			
			inputContainer.Remove(Input);
		}

		private void AddOutputPort(int i)
		{
			var removablePort = new RemovablePort(i);
			removablePort.OnRemoveButtonPress += RemoveOutputPort;
			outputContainer.Add(removablePort);
			Responses.Add(removablePort);
		}

		private void RemoveOutputPort(int i)
		{
			var response = Responses[i];
			response.OnRemoveButtonPress -= RemoveOutputPort;
			
			GetFirstAncestorOfType<GraphView>()?.DeleteElements(new[] {response.Port});
			outputContainer.Remove(response);
			Responses.RemoveAt(i);
			
			RefreshExpandedState();
			RefreshPorts();
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
		public readonly List<RemovablePort> Responses = null;
		private readonly TextField _textField;
	}

	public class RemovablePort : VisualElement
	{
		public RemovablePort(int index)
		{
			style.flexDirection = FlexDirection.Row;
			
			_removeButton = new Button {text = "X"};
			Add(_removeButton);
			
			_removeButton.clickable.clicked += OnRemoveButtonPressInternal;
			
			var outputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Response));
			outputPort.portName = $"Choice {index + 1}";
			choiceIndex = index;
			Port = outputPort;
			Add(outputPort);
		}

		~RemovablePort()
		{
			Remove(Port);
			
			_removeButton.clickable.clicked -= OnRemoveButtonPressInternal;
			
			Remove(_removeButton);
		}

		private void OnRemoveButtonPressInternal() => OnRemoveButtonPress.Invoke(choiceIndex);

		public event Action<int> OnRemoveButtonPress = delegate { };

		private readonly Button _removeButton;
		public readonly Port Port;

		private int choiceIndex;
		public int ChoiceIndex
		{
			set
			{
				choiceIndex = value;
				Port.portName = $"Choice {value + 1}";
			}
		}
	}
}