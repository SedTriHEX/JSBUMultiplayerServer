using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JustShapesBeatsMultiplayerServer
{
    public class CommandParser
    {
        public class CommandInfo
        {
            public string CommandName { get; private set; }

            public List<string> Arguments { get; private set; }

            public bool FailedParse { private set; get; }

            public CommandInfo(string commandName, List<string> arguments, bool failedParse)
            {
                CommandName = commandName;
                Arguments = arguments;
                FailedParse = failedParse;
            }
        }

        public static CommandInfo Parse(string input)
        {
            input = input.Trim();

            if (string.IsNullOrEmpty(input)) return new CommandInfo(null, null, true);

            var pattern = @"^(\w+)(?:\s+(.*))?$";
            var match = Regex.Match(input, pattern);

            if (!match.Success) return new CommandInfo(null, null, true);

            string commandName = match.Groups[1].Value;
            string rawArguments = match.Groups[2].Value;

            var arguments = new List<string>();

            if (!string.IsNullOrEmpty(rawArguments))
            {
                var argumentPattern = @"(?<=\s|^)(?:""([^""]*)""|([^\s""]+))";
                var argumentMatches = Regex.Matches(rawArguments, argumentPattern);

                foreach (Match argumentMatch in argumentMatches)
                {
                    arguments.Add(argumentMatch.Groups[1].Success ? argumentMatch.Groups[1].Value : argumentMatch.Groups[2].Value);
                }
            }

            return new CommandInfo(commandName, arguments, false);
        }
    }
}
