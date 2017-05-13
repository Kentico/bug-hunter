using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Default diagnostic formatter for <see cref="MemberAccessExpressionSyntax"/> nodes
    /// </summary>
    internal class MemberAccessDiagnosticFormatter : DefaultDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="memberAccess" />.
        ///
        /// MessageFormat will be passed an argument in form 'expression.name' of passed <paramref name="memberAccess" />.
        /// Location will be of a whole <paramref name="memberAccess" />.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="memberAccess">Member access that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given member access</returns>
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, MemberAccessExpressionSyntax memberAccess)
            => base.CreateDiagnostic(descriptor, memberAccess).MarkAsSimpleMemberAccess();

        /// <summary>
        /// Returns string in form 'expression.name' of passed <paramref name="memberAccess" />. Any whitespaces will be discarded.
        /// </summary>
        /// <param name="memberAccess">Member access whose location is being requested</param>
        /// <returns>String representation of member access withou whitespaces; empty string if member access is null</returns>
        protected override string GetDiagnosedUsage(MemberAccessExpressionSyntax memberAccess)
            => memberAccess == null
                ? string.Empty
                : $"{memberAccess.Expression}.{memberAccess.Name}";
    }
}