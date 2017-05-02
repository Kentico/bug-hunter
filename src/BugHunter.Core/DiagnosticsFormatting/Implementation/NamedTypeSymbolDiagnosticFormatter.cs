﻿using System.Linq;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Default diagnostic formatter for all <see cref="INamedTypeSymbol"/>s
    /// </summary>
    public class NamedTypeSymbolDiagnosticFormatter : ISymbolDiagnosticFormatter<INamedTypeSymbol>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <param name="descriptor"></param> od passed <param name="namedTypeSymbol"></param>
        /// 
        /// MessageFormat will be passed name of the symbol.
        /// Location will be the first location of the symbol. For partial classes it is only one location.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="namedTypeSymbol">Named type symbol that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given named type symbol</returns>
        public Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, INamedTypeSymbol namedTypeSymbol)
        {
            // TODO
            var location = namedTypeSymbol.Locations.FirstOrDefault();
            var diagnostic = Diagnostic.Create(descriptor, location, namedTypeSymbol.Name);

            return diagnostic;
        }
    }
}