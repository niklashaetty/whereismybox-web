namespace Infrastructure.UserRepository;

public class UserRepositoryConfiguration
{
    public string ConnectionString { get; }
    public string DatabaseName { get; }
    public string ContainerName { get; }

    public UserRepositoryConfiguration(string connectionString, string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(databaseName);
        ArgumentNullException.ThrowIfNull(containerName);
        ConnectionString = connectionString;
        DatabaseName = databaseName;
        ContainerName = containerName;
    }
}