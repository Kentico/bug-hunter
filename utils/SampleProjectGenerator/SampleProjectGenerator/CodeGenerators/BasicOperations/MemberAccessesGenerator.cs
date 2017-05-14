using System.Linq;

namespace SampleProjectGenerator.CodeGenerators.BasicOperations
{
    public class MemberAccessesGenerator : IClassCodeGenerator
    {
        public ProjectType ProjectType => ProjectType.None;
        public FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo("MemberAccess", index);

        public string[] GenerateClasses(int desiredNumberOfDiagnostics, int numberOfFiles)
        {
            var memberAccessesPerFile = desiredNumberOfDiagnostics / numberOfFiles;

            return Enumerable.Range(1, numberOfFiles).Select(index => GenerateSingleClass(index, memberAccessesPerFile)).ToArray();
        }

        private string GenerateSingleClass(int index, int numberOfMemeberAccesses)
        {
            var classWrapper = $@"namespace ConsoleApp
{{
    public class {GetFakeFileInfo(index).FileName}
    {{

        private void SomeMethod() {{
            var a = ""som random string"";";

            var memberAccesses = string.Concat(Enumerable.Repeat("\t\t\ta.IndexOf(\"a\");\r\n", numberOfMemeberAccesses));
            return classWrapper + memberAccesses + "\t\t}\r\n\t}\r\n}";
        }
    }
}