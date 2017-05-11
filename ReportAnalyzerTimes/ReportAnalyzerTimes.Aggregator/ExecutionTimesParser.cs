﻿using System.Collections.Generic;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Class for parsing the contents of aggregated analyzer execution times per solution
    /// </summary>
    internal class ExecutionTimesParser
    {
        /// <summary>
        /// Parses the lines and yields the analyzer executiontimes
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public IEnumerable<AnalyzerExecutionTime> ParseExecutionTimes(string[] lines)
        {
            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(AnalyzerExecutionTime.FromString);
        }
    }
}