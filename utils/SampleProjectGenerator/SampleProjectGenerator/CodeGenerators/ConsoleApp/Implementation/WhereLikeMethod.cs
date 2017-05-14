namespace SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation
{
    public class WhereLikeMethod : BaseConsoleAppClassCodeGenerator
    {
        public override FakeFileInfo GetFakeFileInfo(int index) => new FakeFileInfo(nameof(WhereLikeMethod), index);

        protected override int NumberOfDiagnosticsInBody { get; } = 5;

        protected override string GetClassBodyToRepeat(int iterationNumber)
        {
            return $@"
        public void SampleMethodA{iterationNumber}()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            var yesWhereCondition = whereCondition.WhereLike(""columnName"", ""value"");
            var noWhereCondition = whereCondition.WhereNotLike(""columnName"", ""value"");
        }}

        public void SampleMethodB{iterationNumber}()
        {{
            var yesWhereCondition = new CMS.DataEngine.WhereCondition().WhereLike(""columnName"", ""value"");
            var noWhereCondition1 = new CMS.DataEngine.WhereCondition().WhereNotLike(""columnName"", ""value"");
            var noWhereCondition2 = new CMS.DataEngine.WhereCondition()?.WhereNotLike(""columnName"", ""value"");
        }}";
        }
    }
}