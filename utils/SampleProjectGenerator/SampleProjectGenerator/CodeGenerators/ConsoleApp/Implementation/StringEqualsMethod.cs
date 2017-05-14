namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class StringEqualsMethod : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(StringEqualsMethod), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public string SampleMethodInstanceVariant{iterationNumber}()
        {{
            // allowed usages
            // var res1 = ""Original string"".Equals(""a"", StringComparison.InvariantCultureIgnoreCase);
            // var res2 = ""Original string"".Equals(""a"", false, CultureInfo.CurrentCulture);
            
            // usages raising diagnostic 
            var result1 = ""a"".Equals(""a"");

            var original = ""Original string"";
            var result2 = original.Substring(0).Equals(""a"").ToString();
            var result3 = original?.Substring(0)?.Equals(""a"").ToString();
            
            return (result1) ? result2 : string.Empty;
        }}

        public string SampleMethodStaticVariant{iterationNumber}()
        {{
            // allowed usages
            // var res1 = string.Equals(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase);
            
            // usages raising diagnostic 
            var result1 = string.Equals(""a"", ""b"");
            var result2 = string.Equals(""a"", ""b"").ToString();
            
            return (result1) ? result2 : string.Empty;
        }}";
        }
    }
}