namespace SampleProjectGenerator.CodeGenerators
{
    /// <summary>
    /// Interface that all class code generators must implement
    /// </summary>
    public interface IClassCodeGenerator
    {
        /// <summary>
        /// Type of project resulting class should be placed to
        /// </summary>
        ProjectType ProjectType { get; }

        /// <summary>
        /// Given index, generates fake file info for class to be generated
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        FakeFileInfo GetFakeFileInfo(int index);

        /// <summary>
        /// Generates classes that should be added to the project
        /// </summary>
        /// <param name="desiredNumberOfDiagnostics">Total number of diagnostics to be present in classes for its analyzers</param>
        /// <param name="numberOfFiles">Number of files to be generated, one file might contain more classes</param>
        /// <returns>Array of strings representing generated files to be added to the solution</returns>
        string[] GenerateClasses(int desiredNumberOfDiagnostics, int numberOfFiles);
    }
}