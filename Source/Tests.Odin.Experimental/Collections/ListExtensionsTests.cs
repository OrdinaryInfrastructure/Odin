using System.Collections.Generic;
using System.Linq;
using Odin.Collections;
using NUnit.Framework;

namespace Tests.Odin.Collections
{
    [TestFixture]
    public sealed class ListExtensionsTests
    {
        [Test]
        public void SplitIntoListsOfSize_splits_correctly()
        {
            // Arrange
            List<int> aList = new List<int>(100);
            for (int i = 0; i < 100; i++)
            {
                aList.Add(i);
            }
            
            // Act
            List<List<int>> twentyLists = aList.SplitIntoListsOfSize(5).ToList();
            List<List<int>> fiveLists = aList.SplitIntoListsOfSize(24).ToList();

            // Assert
            Assert.That(twentyLists.Count, Is.EqualTo(20));
            Assert.That(twentyLists[19].Count, Is.EqualTo(5));
            Assert.That(fiveLists.Count, Is.EqualTo(5));
            Assert.That(fiveLists[4].Count, Is.EqualTo(4));
        }

    }
}