// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;

using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using NUnit.Framework;

namespace BugHunter.TestUtils.Verifiers
{
#pragma warning disable SA1402 // File may only contain a single class
    public abstract class CodeFixVerifier<TAnalyzer, TCodeFix> : CodeFixVerifier
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        protected override DiagnosticAnalyzer CSharpDiagnosticAnalyzer => new TAnalyzer();

        protected override CodeFixProvider CSharpCodeFixProvider => new TCodeFix();
    }

    public abstract class CodeFixVerifier<TAnalyzer> : CodeFixVerifier
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        protected override DiagnosticAnalyzer CSharpDiagnosticAnalyzer => new TAnalyzer();

        protected override CodeFixProvider CSharpCodeFixProvider => null;
    }

    /// <summary>
    /// Superclass of all Unit tests made for diagnostics with codefixes.
    /// Contains methods used to verify correctness of codefixes
    /// </summary>
    public abstract class CodeFixVerifier : DiagnosticVerifier
    {
        /// <summary>
        /// Gets the codefix being tested (C#) - to be implemented in non-abstract class
        /// </summary>
        /// <returns>The CodeFixProvider to be used for CSharp code</returns>
        protected abstract CodeFixProvider CSharpCodeFixProvider { get; }

        /// <summary>
        /// Called to test a C# codefix when applied on the inputted string as a source
        /// </summary>
        /// <param name="oldSource">A class in the form of a string before the CodeFix was applied to it</param>
        /// <param name="newSource">A class in the form of a string after the CodeFix was applied to it</param>
        /// <param name="codeFixIndex">Index determining which codefix to apply if there are multiple</param>
        /// <param name="allowNewCompilerDiagnostics">A bool controlling whether or not the test will fail if the CodeFix introduces other warnings after being applied</param>
        /// <param name="fakeFileInfo">Fake file info generated files should have</param>
        protected void VerifyCSharpFix(string oldSource, string newSource, int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false, FakeFileInfo fakeFileInfo = null)
        {
            VerifyFix(CSharpDiagnosticAnalyzer, CSharpCodeFixProvider, oldSource, newSource, AdditionalReferences, codeFixIndex, allowNewCompilerDiagnostics, fakeFileInfo);
        }

        /// <summary>
        /// General verifier for codefixes.
        /// Creates a Document from the source string, then gets diagnostics on it and applies the relevant codefixes.
        /// Then gets the string after the codefix is applied and compares it with the expected result.
        /// Note: If any codefix causes new diagnostics to show up, the test fails unless allowNewCompilerDiagnostics is set to true.
        /// </summary>
        /// <param name="analyzer">The analyzer to be applied to the source code</param>
        /// <param name="codeFixProvider">The codefix to be applied to the code wherever the relevant Diagnostic is found</param>
        /// <param name="oldSource">A class in the form of a string before the CodeFix was applied to it</param>
        /// <param name="newSource">A class in the form of a string after the CodeFix was applied to it</param>
        /// <param name="references">An array of additional metadata references, source files are dependent on</param>
        /// <param name="codeFixIndex">Index determining which codefix to apply if there are multiple</param>
        /// <param name="allowNewCompilerDiagnostics">A bool controlling whether or not the test will fail if the CodeFix introduces other warnings after being applied</param>
        /// <param name="fakeFileInfo">Fake file info generated files should have</param>
        private void VerifyFix(DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider, string oldSource, string newSource, MetadataReference[] references, int? codeFixIndex, bool allowNewCompilerDiagnostics, FakeFileInfo fakeFileInfo)
        {
            codeFixIndex = codeFixIndex ?? 0;
            var document = ProjectCompilation.CreateDocument(oldSource, references, fakeFileInfo);
            var compilerDiagnostics = ProjectCompilation.GetCompilerDiagnostics(document).ToList();
            Assert.IsEmpty(compilerDiagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Warning || diag.Severity == DiagnosticSeverity.Error), "Unable to compile original source code.");

            var analyzerDiagnostics = AnalyzerExecution.GetSortedDiagnosticsFromDocuments(analyzer, new[] { document });
            var attempts = analyzerDiagnostics.Length;

            for (int i = 0; i < attempts; ++i)
            {
                var actions = new List<CodeAction>();
                var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, d) => actions.Add(a), CancellationToken.None);
                codeFixProvider.RegisterCodeFixesAsync(context).Wait();

                if (!actions.Any())
                {
                    break;
                }

                document = ApplyFix(document, actions.ElementAt(codeFixIndex.Value));
                analyzerDiagnostics = AnalyzerExecution.GetSortedDiagnosticsFromDocuments(analyzer, new[] { document });

                var newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, ProjectCompilation.GetCompilerDiagnostics(document).ToArray()).ToArray();

                // check if applying the code fix introduced any new compiler diagnostics
                if (!allowNewCompilerDiagnostics && newCompilerDiagnostics.Any())
                {
                    // Format and get the compiler diagnostics again so that the locations make sense in the output
                    document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result, Formatter.Annotation, document.Project.Solution.Workspace));

                    Assert.Fail(
                        $"Fix introduced new compiler diagnostics:\r\n{string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString()))}\r\n\r\nNew document:\r\n{document.GetSyntaxRootAsync().Result.ToFullString()}\r\n");
                }

                // check if there are analyzer diagnostics left after the code fix
                if (!analyzerDiagnostics.Any())
                {
                    break;
                }
            }

            // after applying all of the code fixes, compare the resulting string to the inputted one
            var actual = ProjectCompilation.GetStringFromDocument(document);
            Assert.AreEqual(newSource, actual);
        }

        /// <summary>
        /// Apply the inputted CodeAction to the inputted document.
        /// Meant to be used to apply codefixes.
        /// </summary>
        /// <param name="document">The Document to apply the fix on</param>
        /// <param name="codeAction">A CodeAction that will be applied to the Document.</param>
        /// <returns>A Document with the changes from the CodeAction</returns>
        private Document ApplyFix(Document document, CodeAction codeAction)
        {
            var operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
            return solution.GetDocument(document.Id);
        }

        /// <summary>
        /// Compare two collections of Diagnostics,and return a list of any new diagnostics that appear only in the second collection.
        /// Note: Considers Diagnostics to be the same if they have the same Ids.  In the case of multiple diagnostics with the same Id in a row,
        /// this method may not necessarily return the new one.
        /// </summary>
        /// <param name="diagnostics">The Diagnostics that existed in the code before the CodeFix was applied</param>
        /// <param name="newDiagnostics">The Diagnostics that exist in the code after the CodeFix was applied</param>
        /// <returns>A list of Diagnostics that only surfaced in the code after the CodeFix was applied</returns>
        private IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics, IEnumerable<Diagnostic> newDiagnostics)
        {
            var oldArray = diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
            var newArray = newDiagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();

            int oldIndex = 0;
            int newIndex = 0;

            while (newIndex < newArray.Length)
            {
                if (oldIndex < oldArray.Length && oldArray[oldIndex].Id == newArray[newIndex].Id)
                {
                    ++oldIndex;
                    ++newIndex;
                }
                else
                {
                    yield return newArray[newIndex++];
                }
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single class
}