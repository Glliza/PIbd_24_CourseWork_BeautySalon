using BeautySalon.Infrastructure;

namespace BeautySalon.Test;

internal class ConfigurationDatabaseTest : IConfigurationDatabase
{
    public string ConnectionString =>
        "Server=127.0.0.7;Port=5472;Database=BeautySalonDB;Uid=Del8a;Pwd=del8almond;";
}
