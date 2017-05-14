using System.IO;

namespace SampleProjectGenerator
{
    public class FakeFileInfo
    {
        public FakeFileInfo()
            : this("Test")
        {
        }

        public FakeFileInfo(string namePrefix, int index = 0)
        {
            FileLocation = string.Empty;
            FileName = $"{namePrefix}{index}";
            FileExtension = "cs";
        }

        public string FileExtension
        {
            get; set;
        }

        public string FileName
        {
            get; set;
        }

        public string FileLocation
        {
            get; set;
        }

        public string FullFilePath => Path.Combine(FileLocation, $"{FileName}.{FileExtension}");

        /// <summary>
        /// Combine FileInfo into file name with path and ensure uniqueness by passing additional index parameter
        /// </summary>
        /// <param name="index">Index of file to be added as suffix of filename in combined full path</param>
        /// <returns>Full file path with index as file name suffix</returns>
        public string GetFullFilePath(int index) => $"{FileLocation}{FileName}{index}.{FileExtension}";
    }
}