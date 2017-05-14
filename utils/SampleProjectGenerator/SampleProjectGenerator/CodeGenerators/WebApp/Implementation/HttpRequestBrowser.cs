namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class HttpRequestBrowser : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(HttpRequestBrowser), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var request = new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"");
            var browserInfo = request.Browser;
            var useless = request.Browser.Browser == browserInfo.Browser;
            var completelyUseless = browserInfo.Browser.Contains(""Ooops..."");
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var request = new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""));
            var browserInfo = request.Browser;
            var useless = request.Browser.Browser == browserInfo.Browser;
        }}";
        }
    }
}