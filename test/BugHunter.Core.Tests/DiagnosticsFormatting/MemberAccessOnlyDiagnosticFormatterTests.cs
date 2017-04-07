﻿using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    [TestFixture]
    public class MemberAccessOnlyDiagnosticFormatterTests
    {
        private IDiagnosticFormatter<MemberAccessExpressionSyntax> _diagnosticFormatter;
        private readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor("FakeID", "Title", "{0}", "Category", DiagnosticSeverity.Warning, true);

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = new MemberAccessOnlyDiagnosticFormatter();
        }

        [Test]
        public void SimpleMemberAccess()
        {
            var memberAccess = SyntaxFactory.ParseExpression(@"someClass.PropA") as MemberAccessExpressionSyntax;

            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(10, 15));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, memberAccess);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual("PropA", diagnostic.GetMessage());
            Assert.IsTrue(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsFalse(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void MultipleNestedMemberAccesses()
        {
            var memberAccess = SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().SomeProperty") as MemberAccessExpressionSyntax;

            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(41, 53));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, memberAccess);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual(@"SomeProperty", diagnostic.GetMessage());
            Assert.IsTrue(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsFalse(diagnostic.IsMarkedAsConditionalAccess());
        }
    }
}
