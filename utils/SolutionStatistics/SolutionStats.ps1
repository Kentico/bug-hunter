# Runs the console application SolutionStatistics over CMS solutoin (path hardcoded in app) and outputs the it to a file in current folder
.\SolutionStatistics\bin\Release\SolutionStatistics.exe -OnlyProjectsWithInstalledAnalyzer > .\solutionStats-analyzerProjectsOnly.txt
.\SolutionStatistics\bin\Release\SolutionStatistics.exe > .\solutionStats-allProjects.txt
