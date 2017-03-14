﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    public static class SyntaxNodeExtensions
    {
        /// <summary>
        /// Tries to determine whether syntaxNode is being assigned a value
        /// The algorithm looks at the first ancestor of type <see cref="AssignmentExpressionSyntax"/> and checks whether <paramref name="syntaxNode"/> is on the LHS of the expression
        /// </summary>
        /// <param name="syntaxNode">Node to be checked for assignment</param>
        /// <returns><c>True</c> if node is on the left hand side of first ancestor assignment expression, <c>false</c> otherwise</returns>
        public static bool IsBeingAssigned(this SyntaxNode syntaxNode)
        {
            var assignmentExpression = syntaxNode.FirstAncestorOrSelf<AssignmentExpressionSyntax>();

            return assignmentExpression?.Left.Contains(syntaxNode) ?? false;
        }
        
    }
}