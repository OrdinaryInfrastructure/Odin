﻿using System.IO;
using System.Text;
using NUnit.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Odin.BackgroundProcessing;

namespace Tests.Odin.BackgroundProcessing
{
    [TestFixture]
    public sealed class ServiceInjectorTests
    {
        
        [Test]
        public void AddBackgroundProcessing_adds_FakeBackgroundProcessor_to_application_from_configuration()
        {
            WebApplicationBuilder Builder = WebApplication.CreateBuilder();
            Builder.Configuration.AddJsonStream(Stream(GetFakeBackgroundProcessorConfigJson()));
            Builder.Services.AddBackgroundProcessing(Builder.Configuration);
            WebApplication sut = Builder.Build();
            
            IBackgroundProcessor provider = sut.Services.GetService<IBackgroundProcessor>();
            BackgroundProcessingOptions config = sut.Services.GetService<BackgroundProcessingOptions>();
            
            Assert.NotNull(provider);
            Assert.IsInstanceOf<FakeBackgroundProcessor>(provider);
            Assert.NotNull(config);
        }

        [Test]
        [Ignore("Ignoring for GitHub actions")]
        public void AddBackgroundProcessing_adds_HangfireBackgroundProcessor_to_application_from_configuration()
        {
            WebApplicationBuilder Builder = WebApplication.CreateBuilder();
            Builder.Configuration.AddJsonStream(Stream(GetHangfireBackgroundProcessorConfigJson()));
            Builder.Services.AddBackgroundProcessing(Builder.Configuration);
            WebApplication sut = Builder.Build();
            
            IBackgroundProcessor provider = sut.Services.GetService<IBackgroundProcessor>();
            BackgroundProcessingOptions config = sut.Services.GetService<BackgroundProcessingOptions>();
            // We don't put HangfireOptions into config... It is only used to start Hangfire on app start only.
            // HangfireOptions providerConfig = sut.Services.GetService<HangfireOptions>();
            
            Assert.NotNull(provider);
            Assert.IsInstanceOf<HangfireBackgroundProcessor>(provider);
            Assert.NotNull(config);
        }
        
        public static string GetFakeBackgroundProcessorConfigJson()
        {
            return @"{
  ""BackgroundProcessing"": {
    ""Provider"": ""Fake"",
  }
}";
        }

        public static string GetHangfireBackgroundProcessorConfigJson()
        {
            return @"{
  ""BackgroundProcessing"": {
    ""Provider"": ""Hangfire"",
    ""Hangfire"": {
      ""ConnectionStringName"": ""WolfDatabase"",
      ""DashboardPath"": ""/bgprocessing"",
      ""DashboardAuthorizationFilters"": ""IsAuthenticated"",
      ""NumberOfAutomaticRetries"": 10,
      ""ServerWorkerCount"": 0,
      ""StartServer"": true,
      ""StartDashboard"": true
    }
  },
  ""ConnectionStrings"": {
      ""WolfDatabase"": ""Data Source=tcp:127.0.0.1,1433;Initial Catalog=Odin;User Id=sa;Password=XXXX;TrustServerCertificate=true""
        },
}";
        }
        
        public static Stream Stream(string input)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            writer.Write(input);
            writer.Flush();

            memoryStream.Position = 0;
            return memoryStream;
        }
        
    }
}