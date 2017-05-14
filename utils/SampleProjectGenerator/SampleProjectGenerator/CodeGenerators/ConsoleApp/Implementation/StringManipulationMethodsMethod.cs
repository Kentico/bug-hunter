namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class StringManipulationMethodsMethod : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(StringManipulationMethodsMethod), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public string SampleMethod{iterationNumber}()
        {{
            // allowed usages
            // var res1 = ""Original string"".ToLowerInvariant();
            // var res1 = ""Original string"".ToLower(CultureInfo.CurrentCulture);
            // var res1 = ""Original string"".ToUpperInvariant();
            // var res1 = ""Original string"".ToUpper(CultureInfo.CurrentCulture);
            
            // usages raising diagnostic 
            var result1 = ""Original string"".ToUpper();
            var result2 = ""Original string"".ToLower();

            var original = ""Original string"";
            var result3 = original.Substring(0).ToUpper().ToString();
            var result4 = original.Substring(0).ToLower().ToString();
            var result5 = original?.Substring(0)?.ToLower().ToString();
            
            return result1 + result2 + result3 + result4;
        }}";
        }
    }
}