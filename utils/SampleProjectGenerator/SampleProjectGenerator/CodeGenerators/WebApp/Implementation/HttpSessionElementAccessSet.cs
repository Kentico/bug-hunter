namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class HttpSessionElementAccessSet : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(HttpSessionElementAccessSet), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var session = System.Web.HttpContext.Current.Session;
            session[""aKey""] = ""aValue"";
            System.Web.HttpContext.Current.Session[""aKey""] = ""aValue"";
            System.Web.HttpContext.Current.Session[""BKey""] = ""BValue"";
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var session = new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session);
            session[""aKey""] = ""aValue"";
            new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)[""aKey""] = ""aValue"";
        }}";
        }
    }
}