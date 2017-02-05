﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.PerformanceTest
{
    internal sealed class TesterDiagnosticProvider : FixAllContext.DiagnosticProvider
    {
        private readonly ImmutableDictionary<ProjectId, ImmutableDictionary<string, ImmutableArray<Diagnostic>>> documentDiagnostics;
        private readonly ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>> projectDiagnostics;

        public TesterDiagnosticProvider(ImmutableDictionary<ProjectId, ImmutableDictionary<string, ImmutableArray<Diagnostic>>> documentDiagnostics, ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>> projectDiagnostics)
        {
            this.documentDiagnostics = documentDiagnostics;
            this.projectDiagnostics = projectDiagnostics;
        }

        public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> filteredProjectDiagnostics;
            if (!projectDiagnostics.TryGetValue(project.Id, out filteredProjectDiagnostics))
            {
                filteredProjectDiagnostics = ImmutableArray<Diagnostic>.Empty;
            }

            ImmutableDictionary<string, ImmutableArray<Diagnostic>> filteredDocumentDiagnostics;
            if (!documentDiagnostics.TryGetValue(project.Id, out filteredDocumentDiagnostics))
            {
                filteredDocumentDiagnostics = ImmutableDictionary<string, ImmutableArray<Diagnostic>>.Empty;
            }

            return Task.FromResult(filteredProjectDiagnostics.Concat(filteredDocumentDiagnostics.Values.SelectMany(i => i)));
        }

        public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
        {
            ImmutableDictionary<string, ImmutableArray<Diagnostic>> projectDocumentDiagnostics;
            if (!documentDiagnostics.TryGetValue(document.Project.Id, out projectDocumentDiagnostics))
            {
                return Task.FromResult(Enumerable.Empty<Diagnostic>());
            }

            ImmutableArray<Diagnostic> diagnostics;
            if (!projectDocumentDiagnostics.TryGetValue(document.FilePath, out diagnostics))
            {
                return Task.FromResult(Enumerable.Empty<Diagnostic>());
            }

            return Task.FromResult(diagnostics.AsEnumerable());
        }

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
        {
            ImmutableArray<Diagnostic> diagnostics;
            if (!projectDiagnostics.TryGetValue(project.Id, out diagnostics))
            {
                return Task.FromResult(Enumerable.Empty<Diagnostic>());
            }

            return Task.FromResult(diagnostics.AsEnumerable());
        }
    }
}