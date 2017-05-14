using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleProjectGenerator.CodeGenerators;

namespace SampleProjectGenerator
{
    internal class Program
    {
        // must be divisible by 10 and *NumberOfFilesPerAnalyzer*
        private static readonly int DesiredNumberOfDiagnosticsTotal = 1000;

        private static readonly int NumberOfFilesPerAnalyzer = 10;
        
        private static void Main(string[] args)
        {
            if (DesiredNumberOfDiagnosticsTotal % 10 != 0 || NumberOfFilesPerAnalyzer % NumberOfFilesPerAnalyzer != 0)
            {
                Console.WriteLine("Make sure to adjust the constants correctly. 'DesiredNumberOfDiagnosticsTotal' must be divisible by 10 and 'NumberOfFilesPerAnalyzer'");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var sampleProjectFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\..\thesis-sample-test-project\SampleProject"));
            if (!Directory.Exists(sampleProjectFolder))
            {
                Console.WriteLine("This console app expects you have https://github.com/Suzii/thesis-sample-test-project cloned in a folder next to thesis-roslyn project");
                Console.WriteLine("Run git clone to create desired folder structure in " + sampleProjectFolder);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Generating project...");
            GenerateSolutionFiles(sampleProjectFolder);

            Console.WriteLine($"Project files generated to {sampleProjectFolder}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void GenerateSolutionFiles(string sampleProjectFolder)
        {
            var classCodeGenerators = GetAllClassCodeGenerators();

            foreach (var codeGenerator in classCodeGenerators)
            {
                if (codeGenerator.ProjectType == ProjectType.None)
                {
                    continue;
                }

                var generatedClasses = codeGenerator.GenerateClasses(DesiredNumberOfDiagnosticsTotal, NumberOfFilesPerAnalyzer);

                for (int i = 0; i < NumberOfFilesPerAnalyzer; i++)
                {
                    var documentPath = Path.Combine(sampleProjectFolder, RelativeDocumentPaths.Get(codeGenerator.ProjectType),
                        codeGenerator.GetFakeFileInfo(i).FullFilePath);
                    File.WriteAllText(documentPath, generatedClasses[i]);
                }
            }
        }

        private static IEnumerable<IClassCodeGenerator> GetAllClassCodeGenerators()
        {
            var generatorInterface = typeof(IClassCodeGenerator);
            var codeGenerators = generatorInterface.Assembly
                .GetTypes()
                .Where(type => generatorInterface.IsAssignableFrom(type) && !type.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IClassCodeGenerator>();

            return codeGenerators;
        }
    }
}
