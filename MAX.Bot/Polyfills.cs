using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !NET462

namespace MAX.Bot
{
    using System.Web;

    internal static class FrameworkSpecificMethods
    {
        internal static string HttpUtility_UrlEncode(string value) => HttpUtility.UrlEncode(value);

        internal static HttpMethod HttpMethod_Patch => HttpMethod.Patch;

        internal static void ArgumentNullException_ThrowIfNull(object argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) =>
            ArgumentNullException.ThrowIfNull(argument, paramName);

        internal static bool Char_IsAsciiLetterOrDigit(char c) =>
            char.IsAsciiLetterOrDigit(c);
    }
}

#else

namespace MAX.Bot
{
    internal static class FrameworkSpecificMethods
    {
        internal static string HttpUtility_UrlEncode(string value) => Uri.EscapeDataString(value);

        internal static HttpMethod HttpMethod_Patch => new("PATCH");

        internal static void ArgumentNullException_ThrowIfNull(object argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }

        // this overload with CancellationToken parameter is missing in .NET 4.6.2
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken) =>
            content.ReadAsStringAsync();

        // char.IsLetterOrDigit accepts non-ascii letters
        internal static bool Char_IsAsciiLetterOrDigit(char c) =>
            (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

        // this overload is missing in .NET 4.6.2
        internal static bool StartsWith(this string str, char value) =>
            str != null && str.Length != 0 && str[0] == value;
    }
}

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Runtime.CompilerServices
{
    // for required members
    internal static class IsExternalInit { }

    // for RequiredMember attribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    internal class RequiredMemberAttribute : Attribute { }

    // for CompilerFeatureRequired attribute
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName) { FeatureName = featureName!; }
        public string FeatureName { get; }
    }

    // for caller arguments
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "This is a polyfill attribute")]
        public CallerArgumentExpressionAttribute(string parameterName) { }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    // for SetsRequiredMembers attribute
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal class SetsRequiredMembersAttribute : Attribute { }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure

#endif
