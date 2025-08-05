using Odin.System;
using NUnit.Framework;

namespace Tests.Odin.System;

[NUnit.Framework.TestFixture]
public sealed class ResultValueTests
{
    [Test]
    [TestCaseSource(nameof(ValueTypesToTest))]
    public void Succeed_with_value_and_message_types(Type valueType,Type messageType)
    {
        var obj = Activator.CreateInstance(valueType);
        Assert.That(obj, Is.Not.Null);
        // Create the generic type ResultValue<valueType, string>
        var genericType = typeof(ResultValue<>).MakeGenericType(valueType, typeof(string));
        // Get the Succeed method
        var succeedMethod = genericType.GetMethod("Succeed", new[] { valueType, typeof(string) });
        // Call the Succeed method
        object sut = succeedMethod!.Invoke(null, new[] { obj, null });

        // Cast to the base Result<string> type for assertions
        var result = (ResultValue<T,string>)sut!;

        Assert.That(sut.Success, Is.True);
        Assert.That(sut.Messages, Is.Empty);
        Assert.That(obj, Is.EqualTo(sut.Value));
    }
    
    [Test]
    public void Succeed_with_string_value_and_no_message()
    {
        string stringVal = "123";
        Result<string><string> sut = Result<string>.Succeed<string>(stringVal);
            
        Assert.That(sut.Success, Is.True);
        Assert.That(sut.MessagesToString(), Is.Empty.Or.Null);
        Assert.That(sut.Messages, Is.Empty);
        Assert.That(stringVal,Is.EqualTo(sut.Value));
        Assert.That(stringVal, Is.EqualTo(sut.Value));      
    }
    
    internal static IReadOnlyList<Type> ValueTypesToTest()
    {
        return new List<Type>()
        {
            typeof(string),
            typeof(int),
            typeof(object)
        };
    }
    
}