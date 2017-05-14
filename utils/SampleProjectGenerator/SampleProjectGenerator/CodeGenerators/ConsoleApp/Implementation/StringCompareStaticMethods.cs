namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class StringCompareStaticMethods : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(StringCompareStaticMethods), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public int SampleMethod{iterationNumber}()
        {{
            // allowed usages
            // string.Compare().""a"", ""b"", false, CultureInfo.CurrentCulture)
            // string.Compare().""a"", ""b"", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase)
            // string.Compare().""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)
            // string.Compare().""a"", 0, ""b"", 1, 1, false, CultureInfo.CurrentCulture)
            // string.Compare().""a"", 0, ""b"", 1, 1, StringComparison.InvariantCultureIgnoreCase)
            
            // usages raising diagnostic 
            var result1 = string.Compare(""a"", ""aa"");
            var result2 = string.Compare(""a"", ""aa"", true);
            var result3 = string.Compare(""a"", ""aa"", false);
            var result4 = string.Compare(""a"", 0, ""aa"", 0, 1, true);
            var result5 = string.Compare(""a"", 0, ""aa"", 0, 1, false);

            return result1 + result2 + result3 + result4 + result5;
        }}";
        }
    }
}