using System;
using System.ComponentModel;

namespace E3J.Utilities.Extensions
{
    /// <summary>
    /// EnumExtensions class
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="enummeration">The enummeration.</param>
        /// <returns></returns>
        private static string GetDescription(object enummeration)
        {
            var field = enummeration.GetType().GetField(enummeration.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : enummeration.ToString();
        }

        /// <summary>
        /// Descriptions the specified enummeration.
        /// </summary>
        /// <param name="enummeration">The enummeration.</param>
        /// <returns></returns>
        public static string Description(this Enum enummeration)
        {
            return GetDescription(enummeration);
        }

        /// <summary>
        /// Gets the value from description.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }
    }
}
