namespace SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation
{
    public class UserControlBase : BaseForBaseClassesCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index)
        {
            return new FakeFileInfo(nameof(UserControlBase), index)
            {
                FileExtension = "ascx.cs",
                FileLocation = @"UserControls"
            };
        }

        protected override string GenerateSingleClass(int index)
        {
            return $@"
    public partial class UserControlBase{index}: System.Web.UI.UserControl
    {{
    }}";
        }
    }
}