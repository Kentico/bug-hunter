namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class StringIndexOfMethod : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(StringIndexOfMethod), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public string SampleMethod{iterationNumber}()
        {{
            // allowed usages
            // var res1 = ""Original string"".IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase);
            // var res1 = ""Original string"".IndexOf(""a"", false, CultureInfo.CurrentCulture);
            // var res1 = ""Original string"".IndexOf('a');
            // var res1 = ""Original string"".IndexOf('a', 0);
            // var res1 = ""Original string"".IndexOf('a', 0, 1);
            // var res1 = ""Original string"".LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase);
            // var res1 = ""Original string"".LastIndexOf(""a"", false, CultureInfo.CurrentCulture);
            // var res1 = ""Original string"".LastIndexOf('a');
            // var res1 = ""Original string"".LastIndexOf('a', 0);
            // var res2 = ""Original string"".LastIndexOf('a', 0, 1);
            
            // usages raising diagnostic 
            var result1 = ""Original string"".IndexOf(""a"");
            var result2 = ""Original string"".LastIndexOf(""a"");

            var original = ""Original string"";
            var result3 = original.Substring(0).IndexOf(""a"").ToString();
            var result4 = original.Substring(0).LastIndexOf(""a"").ToString();
            var result5 = original?.Substring(0)?.LastIndexOf(""a"").ToString();
            
            return result1 + result2 + result3 + result4;
        }}";
        }
    }
}