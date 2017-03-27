﻿using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public interface IDiagnosticFormatter<in TSyntaxNode>
        where TSyntaxNode : SyntaxNode
    {
        Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TSyntaxNode syntaxNode);

        Location GetLocation(TSyntaxNode syntaxNode);

        string GetDiagnosedUsage(TSyntaxNode syntaxNode);
    }
}