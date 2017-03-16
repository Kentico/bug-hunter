﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    public class ClassDeclarationCodeFixHelper : CodeFixHelper
    {
        public ClassDeclarationCodeFixHelper(CodeFixContext context) : base(context)
        {
        }

        public async Task<ClassDeclarationSyntax> GetDiagnosedClassDeclarationSyntax(Diagnostic diagnostic)
        {
            var root = await GetDocumentRoot();
            var classDeclaration = root
                .FindNode(diagnostic.Location.SourceSpan)
                .AncestorsAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            return classDeclaration;
        }
    }
}