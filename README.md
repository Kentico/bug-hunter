**🛈 This repository contains Kentico's internal code that is of no use to the general public. Please explore our [other repositories](https://github.com/Kentico).**

# thesis-roslyn
This repository contains BugHunter analyzers for Kentico CMS solution. They were implemented as part of the master thesis at Faculty of Informatics, Masaryk University.

## Repository structure:
- `docs/` - contains the HTML pages with online documentation available also at https://kentico.github.io/bug-hunter/
- `src/` - contains projects with BugHunter analyzers (project BugHunter.AnalyzersVersions contains different iteratively optimized versions of analyzers and is not part of the production version)
- `test/` - contains tests for BugHunter analyzers and util functions from BH.Core project
- `utils/` - contains projects that were used for performance measuring in the thesis

## Licence
All the source codes are published under MIT licence.

## Versions
The current version of analyzers is v1.0.1, compatible with Kentico.Libraries v10.0.13 and using Microsoft.CodeAnalysis v1.3.2.
Due to Roslyn version analyzers are only compatible with Visual Studio 2015 Update 3 and higher. Update to Microsoft.CodeAnalysis v2.0, which only works in Visual Studio 2017, is currently impossible due to company restrictions at Kentico (.NET framework and Visual Studio backward compatibility).

## NuGet Availability
The NuGet packages with BugHunter analyzers are currently only available via inernal Kentico NuGet feed. Publishing to the official NuGet Gallery is considered for the future.
