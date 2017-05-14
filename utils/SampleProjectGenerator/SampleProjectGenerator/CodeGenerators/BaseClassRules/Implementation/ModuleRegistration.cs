namespace SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation
{
    internal class ModuleRegistration : BaseForBaseClassesCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index)
        {
            return new FakeFileInfo(nameof(ModuleRegistration), index)
            {
                FileLocation = "Modules"
            };
        }

        protected override string GenerateSingleClass(int index)
        {
            return $@"
    public class ModuleRegistration{index} : CMS.DataEngine.Module
    {{
        public ModuleRegistration{index}(CMS.Core.ModuleMetadata metadata, bool isInstallable = false) : base(metadata, isInstallable)
        {{
        }}

        public ModuleRegistration{index}(string moduleName, bool isInstallable = false) : base(moduleName, isInstallable)
        {{
        }}
    }}";
        }
    }
}