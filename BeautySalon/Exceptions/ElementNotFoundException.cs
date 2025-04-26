namespace BeautySalon.Exceptions;

public class ElementNotFoundException : Exception
{
    public string Value { get; private set; }

    public ElementNotFoundException(string value) : base($"Element not found at value = {value}")
    {
        Value = value;
    }
}
