using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Odin.RemoteFiles;
using Odin.System;

namespace Tests.Odin.RemoteFiles;

public class RemoteFileSessionsTests
{
    [Test]
    //[Ignore("TestCases required SFTP password")]
    [TestCase("Sandulela_Daily_Electricity_Transaction_Recon_Flash_client_2023-08-13.csv", 1)]
    [TestCase("Sandulela_Daily_Electricity_Transaction_Recon_Flash_??????_2023-08-13.csv",1)]
    [TestCase("Sandulela_Daily_Electricity_Transaction_*_2023-08-13.csv",1)]
    [TestCase("Sandulela_Daily_Electricity_Transaction_*.csv", Description = "expected count should be greater than 1")]
    public void GetFiles_gets_all_files_successfully(string filePath, int expectedCount = -1)
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "local", $"Protocol=sftp;Host=mattbook.local;Port=22;UserName=matthewderman;Password=90-*()"}
            }
        };
        string baseDirectory = "/Users/matthewderman/Code/Flash/SFTP/Sandulela/";
        RemoteFileSessionFactory factory = new RemoteFileSessionFactory(remoteFileConfig);
        Outcome<IRemoteFileSession> sut = factory.CreateRemoteFileSession("local");
        var results = sut.Value.GetFiles(baseDirectory ,filePath);
        if (expectedCount != -1)
        {
            Assert.That(results.Count(), Is.EqualTo(expectedCount));
        }
        else
        {
            Assert.That(results.Count() > 1);
        }
        //Assert.That(result,resultFileSession.Value.Exists(results.FirstOrDefault().FullName););
    }
    
}