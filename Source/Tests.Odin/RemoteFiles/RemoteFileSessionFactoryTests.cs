using System;
using System.Collections.Generic;
using NUnit.Framework;
using Odin.RemoteFiles;
using Odin.System;

namespace Tests.Odin.RemoteFiles;


[TestFixture]
public class RemoteFileSessionFactoryTests
{
    [Test]
    [TestCase("")]
    [TestCase(null)]
    public void CreateRemoteFileSession_throws_exception_if_connection_name_is_malformed(string connectionName)
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>()
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        Assert.Throws<ArgumentNullException>(() => sut.CreateRemoteFileSession(connectionName));
    }
    
    [Test]
    public void CreateRemoteFileSession_fails_gracefully_if_connection_name_is_not_configured()
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>()
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        
        Outcome<IRemoteFileSession> result = sut.CreateRemoteFileSession("test.connection.co.za");
        
        Assert.IsFalse(result.Success);
        StringAssert.Contains("Connection name not supported or configured: test.connection.co.za", result.MessagesToString());
    }
    
    [Test]
    public void CreateRemoteFileSession_fails_gracefully_if_connection_setting_is_missing_protocol()
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "test.connection.co.za", "Host=test.connection.co.za;UserName=dale.warncke@flash.co.za"}
            }
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        
        Outcome<IRemoteFileSession> result = sut.CreateRemoteFileSession("test.connection.co.za");
        
        Assert.IsFalse(result.Success);
        Assert.That(result.MessagesToString(), Is.EqualTo("Unable to determine protocol from connection string. Connection: test.connection.co.za"));
    }
    
    [Test]
    [TestCase("TCP")]
    [TestCase("AMQP")]
    public void CreateRemoteFileSession_fails_gracefully_if_protocol_cannot_be_parsed_to_enum(string protocol)
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "test.connection.co.za", $"Protocol={protocol};Host=test.connection.co.za;UserName=dale.warncke@flash.co.za"}
            }
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        
        Outcome<IRemoteFileSession> result = sut.CreateRemoteFileSession("test.connection.co.za");
        
        Assert.IsFalse(result.Success);
        Assert.That(result.MessagesToString(), Is.EqualTo("Unable to determine protocol from connection string. Connection: test.connection.co.za"));
    }
    
    [Test]
    public void CreateRemoteFileSession_fails_gracefully_if_protocol_is_not_supported()
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "test.connection.co.za", $"Protocol={ConnectionProtocol.Https};Host=test.connection.co.za;UserName=dale.warncke@flash.co.za"}
            }
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        
        Outcome<IRemoteFileSession> result = sut.CreateRemoteFileSession("test.connection.co.za");
        
        Assert.IsFalse(result.Success);
        Assert.That(result.MessagesToString(), Is.EqualTo($"Protocol is not supported: {ConnectionProtocol.Https}"));
    }
    
    [Test]
    [TestCase(ConnectionProtocol.Sftp, typeof(SftpRemoteFileSession))]
    public void CreateRemoteFileSession_successfully_creates_file_providers(ConnectionProtocol protocol, Type resultType)
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "test.connection.co.za", $"Protocol={protocol};Host=test.connection.co.za;UserName=dale.warncke@flash.co.za"}
            }
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        
        Outcome<IRemoteFileSession> result = sut.CreateRemoteFileSession("test.connection.co.za");
        
        Assert.IsTrue(result.Success);
        Assert.IsInstanceOf(resultType, result.Value);
    }
    
    [Test]
    [Ignore("TestCases required SFTP password")]
    [TestCase("/Sandulela/Import/Sandulela_Daily_Electricity_Transaction_Recon_Flash_client_2023-08-13.csv")]
    [TestCase("/Sandulela/Import/Sandulela_Daily_Electricity_Transaction_Recon_Flash_??????_2023-08-13.csv")]
    [TestCase("/Sandulela/Import/Sandulela_Daily_Electricity_Transaction_*_2023-08-13.csv")]
    [TestCase("/Sandulela/Import/Sandulela_Daily_Electricity_Transaction_*.csv", false, Description = "file exists should return false if multiple files match.")]
    public void FileSession_successfully_checks_if_file_exists(string filePath, bool expectedMatch = true)
    {
        RemoteFilesOptions remoteFileConfig = new RemoteFilesOptions
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                { "transfer.flash.co.za", $"Protocol=sftp;Host=transfer.flash.co.za;Port=22;UserName=svc-FinanceRecons;Password=Rubbish!"}
            }
        };
        RemoteFileSessionFactory sut = new RemoteFileSessionFactory(remoteFileConfig);
        Outcome<IRemoteFileSession> resultFileSession = sut.CreateRemoteFileSession("transfer.flash.co.za");
        bool result = resultFileSession.Value.Exists(filePath);
        Assert.That(result, Is.EqualTo(expectedMatch));
    }
}