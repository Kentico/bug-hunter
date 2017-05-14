namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class HttpRequestQueryString : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(HttpRequestQueryString), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 2;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var request = new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"");
            var value = request.QueryString[""key""].Contains(""Ooops..."");
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var request = new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""));
            var value = request.QueryString[""key""].Contains(""Ooops..."");
        }}";
        }
    }
}