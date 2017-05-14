namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class PageIsCallback : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(PageIsCallback), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 2;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var isPostBack = new System.Web.UI.Page().IsCallback;
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var page = new System.Web.UI.Page();
            var answer = page.IsCallback.ToString();
        }}";
        }
    }
}