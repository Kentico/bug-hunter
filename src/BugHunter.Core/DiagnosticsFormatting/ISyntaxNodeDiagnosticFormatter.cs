// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    /// <summary>
    /// Generic interface for diagnostic formatters accepting any <see cref="SyntaxNode"/>
    /// </summary>
    /// <typeparam name="TSyntaxNode">Diagnosed syntax node to be used for diagnostic</typeparam>
    public interface ISyntaxNodeDiagnosticFormatter<in TSyntaxNode>
        where TSyntaxNode : SyntaxNode
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="syntaxNode" />.
        ///
        /// Arguments for MessageFormat diagnostic's location depend on specific implementations
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="syntaxNode">Syntax node that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given node</returns>
        Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TSyntaxNode syntaxNode);
    }
}