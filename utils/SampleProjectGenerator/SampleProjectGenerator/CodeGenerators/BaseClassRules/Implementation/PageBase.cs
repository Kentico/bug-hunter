namespace SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation
{
    public class PageBase : BaseForBaseClassesCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index)
        {
            return new FakeFileInfo(nameof(PageBase), index)
            {
                FileExtension = "aspx.cs",
                FileLocation = @"Pages"
            };
        }

        protected override string GenerateSingleClass(int index)
        {
            return $@"
    public partial class PageBase{index}: System.Web.UI.Page
    {{
    }}";
        }
    }
}