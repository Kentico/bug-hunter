namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class HttpResponseCookies : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(HttpResponseCookies), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var response = new System.Web.HttpResponse(null);
            var cookies1 = new System.Web.HttpResponse(null).Cookies;
            var cookies2 = response.Cookies;
            var numberOfCookies = response.Cookies.Count;
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var response = new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null));
            var cookies1 = new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null)).Cookies;
            var cookies2 = response?.Cookies;
        }}";
        }
    }
}