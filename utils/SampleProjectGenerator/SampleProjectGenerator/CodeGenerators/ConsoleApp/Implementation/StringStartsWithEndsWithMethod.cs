namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class StringStartsWithEndsWithMethod : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(StringStartsWithEndsWithMethod), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public bool SampleMethod{iterationNumber}()
        {{
            // allowed usages
            // var res1 = ""Original string"".StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase);
            // var res1 = ""Original string"".StartsWith(""a"", false, CultureInfo.CurrentCulture);
            // var res1 = ""Original string"".EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase);
            // var res1 = ""Original string"".EndsWith(""a"", false, CultureInfo.CurrentCulture);
            
            // usages raising diagnostic 
            var result1 = ""Original string"".StartsWith(""a"");
            var result2 = ""Original string"".EndsWith(""a"");

            var original = ""Original string"";
            var result3 = original.Substring(0).StartsWith(""a"").ToString();
            var result4 = original.Substring(0).EndsWith(""a"").ToString();
            var result5 = original?.Substring(0)?.EndsWith(""a"").ToString();
            
            return result1 || result2 && (result3 + result4) != ""falsefalse"";
        }}";
        }
    }
}