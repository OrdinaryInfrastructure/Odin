using NUnit.Framework;
using Odin.BackgroundProcessing;

namespace Tests.Odin.BackgroundProcessing
{
    [TestFixture]
    public sealed class JobDetailsTests
    {
        [Test]
        public void Constructor_sets_properties()
        {
            string id = "123";
            DateTimeOffset when = DateTimeOffset.Now;
            
            JobDetails sut = new JobDetails(id,when);
         
            Assert.That(id, Is.EqualTo(sut.JobId)); 
            Assert.That(when , Is.EqualTo(sut.ScheduledFor));   
        }
    }
}