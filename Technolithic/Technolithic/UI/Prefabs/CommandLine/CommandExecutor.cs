using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Technolithic
{
    public class CommandExecutor
    {
        private readonly Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

        public CommandExecutor(Assembly assembly)
        {
            IEnumerable<MethodInfo> methods = assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

            foreach (MethodInfo method in methods)
            {
                Command command = method.GetCustomAttribute<Command>();

                if (command == null) continue;

                _commands.Add(command.Name, method);
            }
        }

        // Mistakes in the input are returned as an error message.
        // Exceptions thrown by the command itself propagate as TargetInvocationException.
        public bool TryExecute(string input, out string error)
        {
            if (TryTokenize(input ?? string.Empty, out List<string> tokens, out error) == false) return false;

            if (tokens.Count == 0)
            {
                error = "Empty command";
                return false;
            }

            string name = tokens[0];

            if (_commands.TryGetValue(name, out MethodInfo method) == false)
            {
                error = $"Unknown command \"{name}\"";
                return false;
            }

            ParameterInfo[] parameters = method.GetParameters();
            int argsCount = tokens.Count - 1;

            if (argsCount > parameters.Length)
            {
                error = $"Too many arguments. Usage: {GetUsage(name, parameters)}";
                return false;
            }

            object[] values = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                if (i < argsCount)
                {
                    if (TryConvert(tokens[i + 1], parameter.ParameterType, out values[i], out string conversionError) == false)
                    {
                        error = $"Argument \"{parameter.Name}\": {conversionError}. Usage: {GetUsage(name, parameters)}";
                        return false;
                    }
                }
                else if (parameter.HasDefaultValue)
                {
                    values[i] = parameter.DefaultValue;
                }
                else
                {
                    error = $"Missing argument \"{parameter.Name}\". Usage: {GetUsage(name, parameters)}";
                    return false;
                }
            }

            method.Invoke(null, values);

            error = null;
            return true;
        }

        private static bool TryTokenize(string input, out List<string> tokens, out string error)
        {
            tokens = new List<string>();
            error = null;

            StringBuilder token = new StringBuilder();
            bool isInsideQuotes = false;
            bool hasToken = false;

            foreach (char c in input)
            {
                if (c == '"')
                {
                    isInsideQuotes = !isInsideQuotes;
                    hasToken = true;
                }
                else if (char.IsWhiteSpace(c) && isInsideQuotes == false)
                {
                    if (hasToken == false) continue;

                    tokens.Add(token.ToString());
                    token.Clear();
                    hasToken = false;
                }
                else
                {
                    token.Append(c);
                    hasToken = true;
                }
            }

            if (isInsideQuotes)
            {
                error = "Missing closing quote";
                return false;
            }

            if (hasToken)
            {
                tokens.Add(token.ToString());
            }

            return true;
        }

        private static bool TryConvert(string text, Type type, out object value, out string error)
        {
            value = null;
            error = null;

            if (type == typeof(string))
            {
                value = text;
                return true;
            }

            if (type == typeof(int))
            {
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                {
                    value = intValue;
                    return true;
                }

                error = $"\"{text}\" is not an integer";
                return false;
            }

            if (type == typeof(float))
            {
                if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                {
                    value = floatValue;
                    return true;
                }

                error = $"\"{text}\" is not a number";
                return false;
            }

            if (type == typeof(bool))
            {
                if (bool.TryParse(text, out bool boolValue))
                {
                    value = boolValue;
                    return true;
                }

                error = $"\"{text}\" is not true or false";
                return false;
            }

            if (type.IsEnum)
            {
                string enumName = Enum.GetNames(type).FirstOrDefault(n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase));

                if (enumName != null)
                {
                    value = Enum.Parse(type, enumName);
                    return true;
                }

                error = $"\"{text}\" is not one of: {string.Join(", ", Enum.GetNames(type))}";
                return false;
            }

            error = $"parameter type {type.Name} is not supported";
            return false;
        }

        private static string GetUsage(string name, ParameterInfo[] parameters)
        {
            IEnumerable<string> parts = parameters.Select(p => p.HasDefaultValue
                ? $"[{p.Name}={Convert.ToString(p.DefaultValue, CultureInfo.InvariantCulture)}]"
                : $"<{p.Name}>");

            return string.Join(" ", parts.Prepend(name));
        }
    }
}
