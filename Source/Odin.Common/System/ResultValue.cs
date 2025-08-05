using Odin.DesignContracts;

namespace Odin.System;

/// <summary>
/// Represents the success or failure of an operation that returns a Value\Result on success,
/// and list of messages, of type TMessage.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public record ResultValue<TValue, TMessage> : Result<TMessage> where TMessage : class
{
    /// <summary>
    /// Value is not null when Success is True. Value is null when Success is false.
    /// </summary>
    public TValue? Value { get; init; }

    // /// <summary>
    // /// Underlying value.
    // /// </summary>
    // private TValue? _value;

    /// <summary>
    /// Parameterless constructor for serialisation, etc.
    /// </summary>
    public ResultValue()
    {
        Value = default(TValue);
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="success">true or false</param>
    /// <param name="value">Required if successful</param>
    /// <param name="messages">Optional, but good practice is to provide messages for failed results.</param>
    protected ResultValue(bool success, TValue? value, IEnumerable<TMessage>? messages)
    {
        PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
        Value = value;
        _messages = messages?.ToList();
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="success">true or false</param>
    /// <param name="value">Required if successful</param>
    /// <param name="message">Optional, but good practice is to provide messages for failed results.</param>
    protected ResultValue(bool success, TValue? value, TMessage? message = null)
    {
        PreCondition.Requires(!(value == null && success), "Value is required for a successful result.");
        Value = value;
        _messages = message != null ? [message] : null;
    }

    /// <summary>
    /// Success.
    /// </summary>
    /// <param name="value">Required.</param>
    /// <param name="messages">Not normally used for successful operations, but can be for informational purposes.</param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static ResultValue<TValue, TMessage> Succeed(TValue value, IEnumerable<TMessage> messages)
    {
        PreCondition.RequiresNotNull(value);
        return new ResultValue<TValue, TMessage>(true, value, messages);
    }

    /// <summary>
    /// Success.
    /// </summary>
    /// <param name="value">Required.</param>
    /// <param name="message">Not normally used for successful operations, but can be for informational purposes.</param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static ResultValue<TValue, TMessage> Succeed(TValue value, TMessage? message = null)
    {
        PreCondition.RequiresNotNull(value);
        return new ResultValue<TValue, TMessage>(true, value, new List<TMessage>() { message });
    }

    /// <summary>
    /// Success.
    /// </summary>
    /// <param name="messages">Normally included as best practice for failed operations, but not mandatory.</param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static ResultValue<TValue, TMessage> Fail(IEnumerable<TMessage>? messages = null)
    {
        return new ResultValue<TValue, TMessage>(false, default(TValue), messages);
    }

    /// <summary>
    /// Success.
    /// </summary>
    /// <param name="message">Normally included as best practice for failed operations, but not mandatory.</param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static ResultValue<TValue, TMessage> Fail(TMessage? message = null)
    {
        return new ResultValue<TValue, TMessage>(false, default(TValue), new List<TMessage>() { message });
    }
}