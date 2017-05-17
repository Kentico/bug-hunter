// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using NUnit.Framework;

namespace BugHunter.Core.Tests.Helpers
{
    public class HelpLinkUriProviderTest
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Null_GetHelpLink_ThrowsAnException(string emptyId)
        {
            Assert.Throws<ArgumentException>(() => HelpLinkUriProvider.GetHelpLink(emptyId));
        }

        [TestCase("123")]
        [TestCase("AB1234")] // ID must start with BH
        [TestCase("BH12345")] // ID must be 6 chars long
        [TestCase("BH 132")] // ID must not contain special characters
        [TestCase("BH*123")] // ID must not contain special characters

        public void IdInIncorrectFormat_GetHelpLink_ThrowsAnException(string incorrectId)
        {
            Assert.Throws<ArgumentException>(() => HelpLinkUriProvider.GetHelpLink(incorrectId));
        }

        [TestCase("BH0000")]
        [TestCase("BH1234")]
        [TestCase("BHXXXX")]
        [TestCase("BHabcd")]
        [TestCase("BHABCD")]
        public void CorrectId_GetHelpLink_ReturnsCorrectUri(string analyzerId)
        {
            var expected = $"http://kentico.github.io/bug-hunter/{analyzerId}.html";
            var actual = HelpLinkUriProvider.GetHelpLink(analyzerId);
            Assert.AreEqual(expected, actual);
        }
    }
}