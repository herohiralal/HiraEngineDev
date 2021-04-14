using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Internal
{

	internal class HiraConsoleCommand
	{
		private enum HiraConsoleCommandArgumentType : byte
		{
			Boolean,
			Integer,
			Float,
			String,
		}

		public HiraConsoleCommand(MethodInfo method, ParameterInfo[] parameters)
		{
			_method = method;
			byte i;
			var count = parameters.Length;
			for (i = 0; i < count; i++)
			{
				switch (i)
				{
					case 0: _arg1 = supported_types[parameters[i].ParameterType];
						break;
					case 1: _arg2 = supported_types[parameters[i].ParameterType];
						break;
					case 2: _arg3 = supported_types[parameters[i].ParameterType];
						break;
					case 3: _arg4 = supported_types[parameters[i].ParameterType];
						break;
					case 4: _arg5 = supported_types[parameters[i].ParameterType];
						break;
					case 5: _arg6 = supported_types[parameters[i].ParameterType];
						break;
					default: throw new ArgumentOutOfRangeException();
				}
			}

			_argumentCount = i;
		}

		public const byte MAX_SUPPORTED_ARGUMENTS = 5;

		private readonly MethodInfo _method;
		private readonly byte _argumentCount;
		private readonly HiraConsoleCommandArgumentType _arg1;
		private readonly HiraConsoleCommandArgumentType _arg2;
		private readonly HiraConsoleCommandArgumentType _arg3;
		private readonly HiraConsoleCommandArgumentType _arg4;
		private readonly HiraConsoleCommandArgumentType _arg5;
		private readonly HiraConsoleCommandArgumentType _arg6;

		private static readonly Dictionary<Type, HiraConsoleCommandArgumentType> supported_types =
			new Dictionary<Type, HiraConsoleCommandArgumentType>
			{
				{typeof(bool), HiraConsoleCommandArgumentType.Boolean},
				{typeof(int), HiraConsoleCommandArgumentType.Integer},
				{typeof(float), HiraConsoleCommandArgumentType.Float},
				{typeof(string), HiraConsoleCommandArgumentType.String},
			};

		public static bool IsTypeSupported(Type type) => supported_types.ContainsKey(type);

		public bool TryInvoke(string args)
		{
			var parsedArgs = args.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			var parameters = new object[_argumentCount];

			if (parsedArgs.Length < _argumentCount)
				return false;

			for (byte i = 0; i < _argumentCount; i++)
			{
				if (!TryParse(parsedArgs[i], i, out var parsedParameter))
					return false;

				parameters[i] = parsedParameter;
			}

			_method.Invoke(null, parameters);
			return true;
		}

		private bool TryParse(string arg, byte index, out object output)
		{
			var argType = index switch
			{
				0 => _arg1,
				1 => _arg2,
				2 => _arg3,
				3 => _arg4,
				4 => _arg5,
				5 => _arg6,
				_ => throw new ArgumentOutOfRangeException()
			};

			switch (argType)
			{
				case HiraConsoleCommandArgumentType.Boolean:
					if (bool.TryParse(arg, out var result))
					{
						output = result;
						return true;
					}

					break;
				case HiraConsoleCommandArgumentType.Integer:
					if (int.TryParse(arg, out var intResult))
					{
						output = intResult;
						return true;
					}

					break;
				case HiraConsoleCommandArgumentType.Float:
					if (float.TryParse(arg, out var floatResult))
					{
						output = floatResult;
						return true;
					}

					break;
				case HiraConsoleCommandArgumentType.String:
					output = arg;
					
					return true;
				default:
					throw new ArgumentOutOfRangeException(nameof(argType), argType, null);
			}

			output = null;
			return false;
		}
	}
}