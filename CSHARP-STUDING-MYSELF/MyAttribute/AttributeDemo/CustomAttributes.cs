using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class LogAttribute : Attribute
{
    public string Message { get; }

    public LogAttribute(string message)
    {
        Message = message;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
}
