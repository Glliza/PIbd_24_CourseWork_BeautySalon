namespace BeautySalon.Exceptions;

public class ElementDeletedException : Exception
{
    public ElementDeletedException(string id) : base($"[ ! ][ Cannot modify a deleted item (id: {id}) ]") { }
}
