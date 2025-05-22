namespace BeautySalon.Exceptions;

public class StorageException : Exception
{
    public StorageException(Exception ex) : base($"[ ! ][ Error while working in storage: {ex.Message} ]", ex) { }
}