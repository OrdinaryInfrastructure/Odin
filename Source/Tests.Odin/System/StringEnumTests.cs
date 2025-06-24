using System.Globalization;
using NUnit.Framework;
using Odin.System;
using System.Text.Json;

namespace Tests.Odin.System
{
    [TestFixture]
    public sealed class StringEnumTests
    {
        [Test]
        public void Succeed()
        {
            NormalEnum sut1 = NormalEnum.;
            string sut2 = FourValsStringEnum.T;
            
            var result = FourValsStringEnum.Validate().TryValidate("val1")
                
        }
    }

    public sealed class FourValsStringEnum : StringEnum<FourValsStringEnum>
    {
        public const string Val1 = "val1";
        public const string Val2 = "val2";  
        public const string Val3 = "val3"; 
        public const string Val4 = "val4";
    }

    public enum NormalEnum
    {
        Val1, Val2, Val3, Val4
    }
}