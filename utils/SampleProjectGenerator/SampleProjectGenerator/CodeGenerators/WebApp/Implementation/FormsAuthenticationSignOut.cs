namespace SampleProjectGenerator.CodeGenerators.WebApp.Implementation
{
    public class FormsAuthenticationSignOut : BaseWebAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(FormsAuthenticationSignOut), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;
        
        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
       public void SampleMethod{iterationNumber}()
       {{
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.Security.FormsAuthentication.SignOut();
       }}";
        }
    }
}