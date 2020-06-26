using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E3J.Models.ValueObjects;
using E3J.Utilities;

namespace E3J.Models.Syntax_Check
{
    /// <summary>
    /// SyntaxChecker class
    /// </summary>
    public class SyntaxChecker
    {
        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>
        /// The commands.
        /// </value>
        public ISet<Command> Commands { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxChecker"/> class.
        /// </summary>
        public SyntaxChecker()
        {
            Commands = Session.Instance.Commands.CommandsMap;
        }

        /// <summary>
        /// Validates the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public bool Validate(string line)
        {
            return Commands.Any(command => command.Regex.IsMatch(line) || string.IsNullOrWhiteSpace(line));
        }

        /// <summary>
        /// Validates the asynchronous.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string line)
        {
            return await Task.Run(() => Commands.Any(command => command.Regex.IsMatch(line)) || string.IsNullOrWhiteSpace(line));
        }
    }
}
