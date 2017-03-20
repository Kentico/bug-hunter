﻿using System.Collections.Generic;
using System.Linq;		
using BugHunter.Core.Extensions;		
using Microsoft.CodeAnalysis;		
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;		
using Microsoft.CodeAnalysis.CSharp.Syntax;		

namespace BugHunter.Core.Analyzers
{
    public abstract class BaseClassDeclarationSyntaxAnalyzer : DiagnosticAnalyzer
    {
        protected static IEnumerable<ClassDeclarationSyntax> GetAllClassDeclarations(SyntaxTreeAnalysisContext syntaxTreeAnalysisContext)
        {		
            return syntaxTreeAnalysisContext		
                .Tree		
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>();		
        }		
		
        protected static INamedTypeSymbol GetBaseTypeSymbol(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {		
            // if class is not extending nor implementing anything, it has no base type		
            if (classDeclaration.BaseList == null || classDeclaration.BaseList.Types.IsNullOrEmpty())		
            {		
                return null;		
            }		
		
            return semanticModel.GetDeclaredSymbol(classDeclaration).BaseType;		
        }		
        		
        protected static Diagnostic CreateDiagnostic(SyntaxTreeAnalysisContext syntaxTreeAnalysisContext, ClassDeclarationSyntax classDeclaration, DiagnosticDescriptor rule)
        {		
            var location = syntaxTreeAnalysisContext.Tree.GetLocation(classDeclaration.Identifier.FullSpan);		
            var diagnostic = Diagnostic.Create(rule, location, classDeclaration.Identifier.ToString());		
            return diagnostic;		
        }		
		
        protected static Diagnostic CreateDiagnostic(INamedTypeSymbol namedTypeSymbol, DiagnosticDescriptor rule)
        {		
            var locations = namedTypeSymbol.Locations;		
            var diag = Diagnostic.Create(rule, locations.FirstOrDefault(), locations, namedTypeSymbol.ToDisplayString());		
            return diag;		
        }
    }
}
