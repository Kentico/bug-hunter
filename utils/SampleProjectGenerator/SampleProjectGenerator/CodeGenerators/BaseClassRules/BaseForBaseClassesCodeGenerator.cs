using System.Collections.Generic;
using System.Text;

namespace SampleProjectGenerator.CodeGenerators.BaseClassRules
{
    public abstract class BaseForBaseClassesCodeGenerator : IClassCodeGenerator
    {
        public ProjectType ProjectType => ProjectType.WebApp;

        protected abstract string GenerateSingleClass(int index);

        public abstract FakeFileInfo GetFakeFileInfo(int index);

        public string[] GenerateClasses(int desiredNumberOfDiagnostics, int numberOfFiles)
        {
            var numberOfDiagnosticsPerFile = desiredNumberOfDiagnostics / numberOfFiles;

            var result = new List<string>();

            for (var i = 0; i < numberOfFiles; i++)
            {
                var fileBuilder = new StringBuilder();
                fileBuilder.AppendLine("namespace WebApp\r\n{");
                for (var j = 0; j < numberOfDiagnosticsPerFile; j++)
                {
                    fileBuilder.AppendLine(GenerateSingleClass(i * numberOfDiagnosticsPerFile + j + 1));
                }
                fileBuilder.AppendLine("}");

                result.Add(fileBuilder.ToString());
            }

            return result.ToArray();
        }

    }
}
