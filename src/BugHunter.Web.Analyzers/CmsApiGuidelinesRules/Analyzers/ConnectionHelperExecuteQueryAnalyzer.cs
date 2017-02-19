using System.Collections.Immutable;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConnectionHelperExecuteQueryAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CONNECTION_HELPER_EXECUTE_QUERY;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_Title), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_MessageFormat), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)),
                category: nameof(AnalyzerCategories.CmsApiGuidelines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsApiGuidelinesResources.ConnectionHelperExecuteQuery_Description), CmsApiGuidelinesResources.ResourceManager, typeof(CmsApiGuidelinesResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            RegisterAction(Rule, context, "CMS.DataEngine.ConnectionHelper", "ExecuteQuery");
        }

        protected override bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            return base.CheckPreConditions(context) && IsUiFile(context.Node.SyntaxTree.FilePath);
        }

        private bool IsUiFile(string filePath)
        {
            return filePath.EndsWith(FileExtensions.PAGES) 
                || filePath.EndsWith(FileExtensions.CONTROLS) 
                || filePath.EndsWith(FileExtensions.HANDLERS)
                || filePath.EndsWith(FileExtensions.MASTER_PAGE);
        }
    }
}
