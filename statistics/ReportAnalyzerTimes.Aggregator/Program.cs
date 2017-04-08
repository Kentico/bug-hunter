﻿using System;
using System.Globalization;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Reads the aggregated analyzer execution times from files with prefix "/inPrefix=XY" and suffix as number up to "/count=XY",
    /// aggregates the times per analyzer into string,
    /// outputs the times per analyzer as csv file
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            var inputFilePathPrefix = args.FirstOrDefault(arg => arg.StartsWith("/inPrefix="))?.Substring(10) ?? @"C:\tmp\aggregated_";
            var outputFilePath = args.FirstOrDefault(arg => arg.StartsWith("/out="))?.Substring(5) ?? @"C:\tmp\aggregated-results.csv";
            var numberOfInputFilesString = args.FirstOrDefault(arg => arg.StartsWith("/count="))?.Substring(7) ?? "3";
            int numberOfInputFiles;

            if (string.IsNullOrEmpty(inputFilePathPrefix) 
                || string.IsNullOrEmpty(outputFilePath) 
                || !int.TryParse(numberOfInputFilesString,NumberStyles.Integer, CultureInfo.InvariantCulture, out numberOfInputFiles) 
                || numberOfInputFiles < 1)
            {
                PrintHelp();
                return -1;
            }

            Console.WriteLine($"Aggregating the ReportAnalyzer times from files {inputFilePathPrefix} 0-{numberOfInputFiles-1}");
            var filesProvider = new InputFilesProvider(inputFilePathPrefix, numberOfInputFiles, "txt");
            var aggregator = new ExecutionTimesAggregator();

            filesProvider
                .GetLinesOfFiles()
                .SelectMany(filesLines => filesLines.Select(fileLines => fileLines))
                .Select(line => AnalyzerExecutionTime.FromString(line))
                .ToList()
                .ForEach(line => aggregator.AddAnalyzerExecutionTime(line));

            var result = aggregator.GetAggregatedResults();


            Console.WriteLine($"Writing results to {outputFilePath}");

            var csvExporter = new CsvResultsExporter();
            csvExporter.Export(outputFilePath, result);

            Console.WriteLine($"Execution times aggregated in CSV format into {outputFilePath}");

            return 0;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Input files should have txt extension");
            Console.WriteLine(@"Usage:");
            Console.WriteLine(@"ReportAnalyzerTimes.Aggregator.exe /inPrefix=C:\tmp\analyzers-execution-times-aggregated /count=100 /out=C:\tmp\analyzers-execution-times.csv");
        }
    }
}
