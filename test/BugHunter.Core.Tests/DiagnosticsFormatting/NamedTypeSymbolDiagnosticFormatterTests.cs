// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    [TestFixture]
    public class NamedTypeSymbolDiagnosticFormatterTests
    {
        private readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor("FakeID", "Title", "{0}", "Category", DiagnosticSeverity.Warning, true);
        private ISymbolDiagnosticFormatter<INamedTypeSymbol> _diagnosticFormatter;

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = new NamedTypeSymbolDiagnosticFormatter();
        }

        [TestCase(@"class ClassName : System.IDisposable { }", 6)]
        [TestCase(@"partial class ClassName : System.IDisposable { }", 14)]
        [TestCase(@"abstract class ClassName : System.IDisposable { }", 15)]
        [TestCase(@"class ClassName : A, System.IDisposable { }", 6)]
        public void SimpleInvocation(string sourceText, int locationStart)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceText);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("Compilation", new[] { tree }, new[] { mscorlib });

            var classDeclaration = tree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
            var semanticModel = compilation.GetSemanticModel(tree);
            var namedSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            var expectedLocation = Location.Create(classDeclaration?.SyntaxTree, TextSpan.FromBounds(locationStart, locationStart + 9));
            var diagnostic = _diagnosticFormatter.CreateDiagnostics(_rule, namedSymbol)
                .Single();

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual("ClassName", diagnostic.GetMessage());
        }
    }
}
