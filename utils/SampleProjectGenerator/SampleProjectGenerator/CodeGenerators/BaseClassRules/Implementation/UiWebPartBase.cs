namespace SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation
{
    public class UiWebPartBase : BaseForBaseClassesCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index)
        {
            return new FakeFileInfo(nameof(UiWebPartBase), index)
            {
                FileLocation = @"CMSModules\AdminControls\Controls\UIControls"
            };
        }

        protected override string GenerateSingleClass(int index)
        {
            return $@"
    public class UiWebPartBase{index}: System.Web.UI.WebControls.WebParts.WebPart
    {{
    }}";
        }
    }
}