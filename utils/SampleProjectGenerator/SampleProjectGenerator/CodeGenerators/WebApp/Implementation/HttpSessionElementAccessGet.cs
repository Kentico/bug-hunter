namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class HttpSessionElementAccessGet : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(HttpSessionElementAccessGet), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var session = System.Web.HttpContext.Current.Session;
            var aValue = session[""aKey""];
            var aValueString1 = session[""aKey""].ToString();
            var aValueString2 = System.Web.HttpContext.Current.Session[""aKey""].ToString();
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var session = new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session);
            var aValue = session[""aKey""];
            var aValueString1 = session[""aKey""].ToString();
        }}";
        }
    }
}