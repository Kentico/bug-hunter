namespace SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation
{
    public class WebPartBase : BaseForBaseClassesCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index)
        {
            return new FakeFileInfo(nameof(WebPartBase), index)
            {
                FileLocation = @"CMSWebParts"
            };
        }

        protected override string GenerateSingleClass(int index)
        {
            return $@"
    public class WebPartBase{index}: System.Web.UI.WebControls.WebParts.WebPart
    {{
    }}";
        }
    }
}