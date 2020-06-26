using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace E3J.Utilities
{
    /// <summary>
    /// ProgramContentConverter class
    /// </summary>
    public static class ProgramContentConverter
    {

        #region Fields

        //difference between lines, e.g. 5 GC; 10 GO; 15 WH
        /// <summary>
        /// The line number addition
        /// </summary>
        private static int lineNumberAddition = 5;

        #endregion

        #region Actions

        /// <summary>
        /// Removes numbers at begging of line and converts command parameters.
        /// </summary>
        /// <param name="content">Program from manipulator.</param>
        /// <returns>
        /// Program to be displayed to user.
        /// </returns>
        public static string ToPC(string content)
        {
            var lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            lines = ToPC(lines);

            string returnContent = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                //append lines and EOL to return content
                if (i == 0)
                {
                    returnContent += lines[i];
                }
                else
                {
                    returnContent += "\r\n" + lines[i];
                }
            }

            return returnContent;
        }

        /// <summary>
        /// Removes numbers at begging of line and converts command parameters.
        /// </summary>
        /// <param name="content">Program from manipulator.</param>
        /// <returns>
        /// Program to be displayed to user.
        /// </returns>
        public static string[] ToPC(string[] content)
        {
            /* What we want to do here is:
             *  1. Get program from manipulator and split it into lines
             *  2. Remove numbers from beggining of every line
             *  3. Find all occurances of commands that require line reference (e.g. GS)
             *  4. Replace parameters of those command with calculated corresponding numbers
             */

            for (int i = 0; i < content.Length; i++)
            {
                content[i] = Regex.Replace(content[i], @"^\s*\d+\s*", string.Empty);

                if (Regex.IsMatch(content[i], @"^\s*GS\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])\s*$"))
                {
                    //GS command found so replace it with corresponding number || newNumber = (oldNumber + difference - 1)/difference
                    var commandParameterToReplace = ((Convert.ToInt32(new string(content[i].Where(char.IsDigit).ToArray())) +
                        lineNumberAddition - 1)/lineNumberAddition).ToString();
                    content[i] = Regex.Replace(content[i], @"([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])\s*$", commandParameterToReplace);
                }
            }

            return content;
        }



        /// <summary>
        /// Adds numbers at begging of line and converts command parameters.
        /// </summary>
        /// <param name="content">Program from user.</param>
        /// <returns>
        /// Program to be send to manipulator.
        /// </returns>
        public static string ToManipulator(string content)
        {
            var lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            lines = ToManipulator(lines);

            string returnContent = string.Empty;
            
            for (int i = 0; i < lines.Length; i++)
            {
                //append lines and EOL to return content
                if (i == 0)
                {
                    returnContent += lines[i];
                }
                else
                {
                    returnContent += "\r\n" + lines[i];
                }
            }

            return returnContent;
        }

        /// <summary>
        /// Adds numbers at begging of line and converts command parameters.
        /// </summary>
        /// <param name="content">Program from user.</param>
        /// <returns>
        /// Program to be send to manipulator.
        /// </returns>
        public static string[] ToManipulator(string[] content)
        {
            /* What we want to do here is:
             *  1. Get program typed by user and split it into lines
             *  2. Find all occurances of commands that require line reference (e.g. GS)
             *  3. Replace parameters of those command with calculated corresponding numbers
             *  3. Add numbers at beggining of every line
             */

            var lineNumber = 1;

            for (int i = 0; i < content.Length; i++)
            {
                if (Regex.IsMatch(content[i], @"^\s*GS\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])\s*$"))
                {
                    //GS command found so replace it with corresponding number || newNumber = oldNumber * difference - difference + 1
                    var commandParameterToReplace = (Convert.ToInt32(new string(content[i].Where(char.IsDigit).ToArray())) *
                        lineNumberAddition - lineNumberAddition + 1).ToString();
                    content[i] = Regex.Replace(content[i], @"([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])\s*$", commandParameterToReplace);
                }
                content[i] = lineNumber.ToString() + " " + content[i];
                lineNumber += lineNumberAddition;
            }

            return content;
        }

        #endregion

    }
}
