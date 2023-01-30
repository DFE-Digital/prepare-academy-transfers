using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System;

namespace Dfe.PrepareTransfers.Web.Utils
{
    public class Typespace
    {
        private static readonly Regex NonAlphaNumeric =
            new Regex("[^a-z0-9-]", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

        private Typespace() { }

        /// <summary>
        ///     Enables or disables the Typespace functionality
        /// </summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        ///     Generates the textual representation of the nested type hierarchy
        /// </summary>
        /// <param name="cachedValue">parameter in to which to cache the generated value</param>
        /// <param name="disabledDefault">the value to return when Typespace is disabled. (default: string.Empty)</param>
        /// <returns>string representing the type hierarchy if enabled, otherwise <paramref name="disabledDefault" /> is returned</returns>
        public static string Name(ref string cachedValue, string disabledDefault = "")
        {
            return IsEnabled ? cachedValue ??= new Typespace() : disabledDefault;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static implicit operator string(Typespace _)
        {
            StackFrame frame = new StackFrame(2);
            MethodBase method = frame.GetMethod();
            Type declaringType = method?.DeclaringType;

            if (declaringType is { Namespace: { } })
            {
                ReadOnlySpan<char> typeName = declaringType.FullName.AsSpan(declaringType.Namespace.Length + 1);
                ReadOnlySpan<char> memberName = RemovePropertyPrefixes(method.Name);

                return NonAlphaNumeric.Replace($"{typeName.ToString()}-{memberName.ToString()}", "-").ToLowerInvariant();
            }

            return string.Empty;
        }

        private static ReadOnlySpan<char> RemovePropertyPrefixes(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return ReadOnlySpan<char>.Empty;

            ReadOnlySpan<char> nameSpan = name.AsSpan();

            return nameSpan.StartsWith("get_", StringComparison.InvariantCultureIgnoreCase) ||
                   nameSpan.StartsWith("set_", StringComparison.InvariantCultureIgnoreCase)
                ? nameSpan[4..]
                : nameSpan;
        }
    }
}