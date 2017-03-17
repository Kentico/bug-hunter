﻿using BugHunter.Core.Extensions;
using System;

namespace BugHunter.Web.Analyzers
{
    public static class SolutionFolders
    {
        /// <summary>
        /// This folder contains webparts for UI elements
        /// </summary>
        public const string UI_WEB_PARTS = @"CMSModules\AdminControls\Controls\UIControls\";

        /// <summary>
        /// This folder contains webparts
        /// </summary>
        public const string WEB_PARTS = @"CMSWebParts\";

        /// <summary>
        /// Determines whether given <param name="filePath"></param> is located in WebParts folder
        /// </summary>
        /// <param name="filePath">File path to be checked</param>
        /// <returns>True if file is in one of WebParts folders</returns>
        public static bool FileIsInWebPartsFolder(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) &&
                   !filePath.Contains("_files\\") &&
                   (filePath.Contains(UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) || filePath.Contains(WEB_PARTS, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the given file is UI web part.
        /// </summary>
        /// <param name="path">Path to web part file.</param>
        public static bool IsUIWebPart(string path)
        {
            return path?.Contains(UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}