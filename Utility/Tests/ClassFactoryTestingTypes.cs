namespace Tests.Odin.Utility;

public interface Interface1
{
    public string Member1 { get; set; }
}

public interface Inherited2 : Interface1
{
    public string Member2 { get; set; }
}

public class Class3
{
}
