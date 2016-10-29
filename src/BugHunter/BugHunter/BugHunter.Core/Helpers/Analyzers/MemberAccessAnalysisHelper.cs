﻿using System;
using System.Linq;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Helpers.Analyzers
{
    public class MemberAccessAnalysisHelper
    {
        private readonly DiagnosticDescriptor _rule;

        private readonly Type _accessedType;

        private readonly string[] _memberNames;
        
        public MemberAccessAnalysisHelper(DiagnosticDescriptor rule, Type accessedType, string memberName)
            : this(rule, accessedType, new []{ memberName}) { }

        public MemberAccessAnalysisHelper(DiagnosticDescriptor rule, Type accessedType, string[] memberNames)
        {
            _rule = rule;
            _accessedType = accessedType;
            _memberNames = memberNames;
        }

        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            var memberName = memberAccess.Name.ToString();
            if (!_memberNames.Contains(memberName))
            {
                return;
            }

            var searchedTargetType = _accessedType.GetITypeSymbol(context);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (searchedTargetType == null || actualTargetType == null || !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return;
            }

            var usedAs = $"{memberAccess.Expression}.{memberName}";
            var diagnostic = Diagnostic.Create(_rule, memberAccess.GetLocation(), usedAs);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
