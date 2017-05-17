#Running MSBuild with ReportAnalyzers
This simple guide explains how to run MSBuild with so that the compiler outputs the statistics about analyzers' execution times.

As per https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Report%20Analyzer%20Format.md, it should be possible to run the compiler (csc.exr) with `/ReportAnalyzer` switch and all the stats should be there. 
The MSBuild internally uses the csc.exe, therefore, it should be (somehow) possible to switch it on. 
As it turns out, the MSBuild official documentation (https://msdn.microsoft.com/en-us/library/ms164311.aspx) does not contain any information on that matter.

**TL;DR;**

```MSBuild.exe /t:Clean,Build /p:ReportAnalyzer=True /verbosity:diagnostic .\SampleProject.sln > msbuild-output.txt```
 
Be carefull to redirect the output and prepare yourself for the output file to be rather large (~100MB-500MB). 
Parameter `/p:ReportAnalyzer=True` turns `/ReportAnalyzer` for `csc.exe`, but only `/verbosity:diagnostic` makes it appear in the output itself.

NOTE: If the diagnostics are only warnings, build seems to ignore them after first run, therefore the Clean target.