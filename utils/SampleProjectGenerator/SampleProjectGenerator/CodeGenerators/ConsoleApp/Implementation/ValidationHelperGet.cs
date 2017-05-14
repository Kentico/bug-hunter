namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class ValidationHelperGet : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(ValidationHelperGet), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 10;
        
        protected override string GetClassPrefix(int index)
        {
            return $@"using System;
using System.Globalization;
using CMS.Helpers;

namespace ConsoleApp.Controllers
{{
    public class {GetFakeFileInfo(index).FileName}
    {{";
        }

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var a1 = ValidationHelper.GetDouble("""", 0);
            var b1 = ValidationHelper.GetDouble("""", 0);
            var a2 = ValidationHelper.GetDouble(""2"", 2);
            var a3 = ValidationHelper.GetDouble("""", 0, System.Globalization.CultureInfo.CurrentCulture);
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var a1 = ValidationHelper.GetDateTime("""", System.DateTime.MaxValue);
            var b1 = ValidationHelper.GetDateTime("""", System.DateTime.MaxValue);
            var a2 = ValidationHelper.GetDateTime("""", System.DateTime.MaxValue, ""en-us"");
            var a3 = ValidationHelper.GetDateTime("""", System.DateTime.MaxValue, System.Globalization.CultureInfo.CurrentCulture);
        }}

        public void SampleMethodC{iterationNumber}()
        {{
            var a1 = ValidationHelper.GetDate("""", System.DateTime.MaxValue);
            var a2 = ValidationHelper.GetDate("""", System.DateTime.MaxValue, System.Globalization.CultureInfo.CurrentCulture);
        }}";
        }
    }
}