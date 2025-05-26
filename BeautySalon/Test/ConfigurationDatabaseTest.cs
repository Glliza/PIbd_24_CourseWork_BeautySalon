using BeautySalon.Infrastructure;

namespace BeautySalon.Test;

internal class ConfigurationDatabaseTest : IConfigurationDatabase
{
    public string ConnectionString =>
        "Server=127.0.0.7;Port=5472;Database=beautysalonDB;Uid=Del8a;Pwd=del8almond;";
}

// Can you make sure, the cashboxes are taken from DB and can you first of all check if connection with DB is actually working? And where it need to be stated, to connect the code. 