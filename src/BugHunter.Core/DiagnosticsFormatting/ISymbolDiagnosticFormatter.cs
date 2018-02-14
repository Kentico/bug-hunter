// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    /// <summary>
    /// Generic interface for diagnostic formatters accepting any <see cref="ISymbol"/>
    /// </summary>
    /// <typeparam name="TSymbol">Diagnosed symbol to be used for diagnostic</typeparam>
    public interface ISymbolDiagnosticFormatter<in TSymbol>
        where TSymbol : ISymbol
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="symbol" /> for each of its locations.
        ///
        /// Arguments for MessageFormat diagnostic's location depend on specific implementations
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="symbol">Syntax symbol that the diagnostic should be raised for</param>
        /// <returns>Diagnostics created from descriptor for each location of given symbol</returns>
        IEnumerable<Diagnostic> CreateDiagnostics(DiagnosticDescriptor descriptor, TSymbol symbol);
    }
}