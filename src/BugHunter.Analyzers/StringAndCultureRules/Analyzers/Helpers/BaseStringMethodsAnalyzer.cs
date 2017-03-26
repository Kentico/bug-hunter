﻿using System;
using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers
{
    public abstract class BaseStringMethodsAnalyzer : DiagnosticAnalyzer
    {
        protected abstract DiagnosticDescriptor Rule { get; }

        private readonly string[] _forbiddenMethods;

        private static readonly IDiagnosticFormatter<InvocationExpressionSyntax> diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();

        protected BaseStringMethodsAnalyzer(string forbiddenMethod, params string[] additionalForbiddenMethods)
        {
            _forbiddenMethods = additionalForbiddenMethods.Concat(new []{ forbiddenMethod }).ToArray();
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }
        
        protected void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            IMethodSymbol methodSymbol;
            if (!IsForbiddenStringMethod(context, invocationExpression, out methodSymbol))
            {
                return;
            }

            if (!IsForbiddenOverload(context, invocationExpression, methodSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(invocationExpression);
            context.ReportDiagnostic(diagnostic);
        }

        protected bool IsForbiddenStringMethod(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, out IMethodSymbol methodSymbol)
        {
            methodSymbol = null;
            if (CanBeSkippedBasedOnSyntaxOnly(invocation))
            {
                return false;
            }

            var invokedMethodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            methodSymbol = invokedMethodSymbol;

            return IsInvokedOnString(invokedMethodSymbol) && IsForbiddenMethodName(invokedMethodSymbol?.Name);
        }
        
        // If method is already called with StringComparison argument, no need for diagnostic
        protected virtual bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            => methodSymbol.Parameters.All(argument => !IsStringComparison(argument) && !IsCultureInfo(argument));

        private bool CanBeSkippedBasedOnSyntaxOnly(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return false;
            }

            return !IsForbiddenMethodName(methodNameNode.Identifier.ValueText);
        }

        private bool IsForbiddenMethodName(string invokedMethodName)
           => _forbiddenMethods.Contains(invokedMethodName);

        private bool IsInvokedOnString(IMethodSymbol invokedMethodSymbol)
            => invokedMethodSymbol?.ReceiverType?.SpecialType == SpecialType.System_String;

        private bool IsStringComparison(IParameterSymbol parameter)
            => parameter.Type.ToDisplayString() == "System.StringComparison";

        private bool IsCultureInfo(IParameterSymbol parameter)
            => parameter.Type.ToDisplayString() == "System.Globalization.CultureInfo";

        protected virtual Diagnostic CreateDiagnostic(InvocationExpressionSyntax syntaxNode)
        {
            var usedAs = diagnosticFormatter.GetDiagnosedUsage(syntaxNode);
            var location = diagnosticFormatter.GetLocation(syntaxNode);

            return Diagnostic.Create(Rule, location, usedAs);
        }
    }
}