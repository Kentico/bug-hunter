namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class SystemIo : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(SystemIo), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 10;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void AllowedExceptionsMethod{iterationNumber}()
        {{
            // Possible false positives 
            throw new System.IO.IOException(""Ooops..."");
            throw new System.IO.DirectoryNotFoundException(""Ooops..."");
        }}

        public System.IO.Stream AllowedStreamMethod{iterationNumber}()
        {{
            // Possible false positive
            return System.IO.Stream.Null;
        }}

        public System.IO.BinaryReader SampleMethod{iterationNumber}(System.IO.BinaryReader param)
        {{ 
            var result = new System.IO.BinaryReader(System.IO.Stream.Null);
            System.IO.BinaryReader badVariable = param;
            return result;
        }}

        public void StaticMemberMethod{iterationNumber}()
        {{ 
            var badVariable1 = System.IO.Path.DirectorySeparatorChar;
        }}

        public static void HelperMethod{iterationNumber}(char c) 
        {{
            // Do nothing
        }}

        public void ComplexNestedMemberAccessMethod{iterationNumber}()
        {{
            var directory = new System.IO.DirectoryInfo(System.IO.Path.GetFullPath(""./some/path""));
            HelperMethod{iterationNumber}(System.IO.Path.DirectorySeparatorChar);
            HelperMethod{iterationNumber}(System.IO.Path.DirectorySeparatorChar);
            HelperMethod{iterationNumber}(System.IO.Path.DirectorySeparatorChar);
        }}";
        }
    }
}