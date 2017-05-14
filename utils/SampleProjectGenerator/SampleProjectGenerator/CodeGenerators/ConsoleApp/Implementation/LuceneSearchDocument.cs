namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class LuceneSearchDocument : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(LuceneSearchDocument), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassPrefix(int index)
        {
            return $@"using CMS.Search.Lucene3; 

namespace SampleTestProject.CsSamples
{{
    public class {GetFakeFileInfo(index).FileName}
    {{
 ";
        }

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        private LuceneSearchDocument Method{iterationNumber}(LuceneSearchDocument doc)
        {{
            LuceneSearchDocument badVariable1 = new LuceneSearchDocument();
            LuceneSearchDocument badVariable2 = new LuceneSearchDocument();
            LuceneSearchDocument badVariable3 = new LuceneSearchDocument();

            return new System.Random().Next(2) < 1 ? null : new LuceneSearchDocument();
        }}";
        }
    }
}