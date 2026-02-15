using Infra.Data.SqlServer;
using Xunit;

namespace Test.Infra.Data.SqlServer
{
    public class EnsureDatabaseExistsTests
    {
        private const string DbName = "AuthDb";

        [Fact]
        public void EnsureDatabaseExists_WhenConnectionStringInvalid_Throws()
        {
            // invalid host makes SqlConnection.Open fail deterministically
            Assert.ThrowsAny<Exception>(() => DatabaseInitializer.EnsureDatabaseExists(
                "Server=invalid-host;Database=master;User Id=sa;Password=bad;TrustServerCertificate=True;Connect Timeout=1;",
                DbName));
        }

        [Fact]
        public void EnsureDatabaseExists_WhenConnectionStringMalformed_Throws()
        {
            Assert.ThrowsAny<Exception>(() => DatabaseInitializer.EnsureDatabaseExists(
                "not-a-connection-string",
                DbName));
        }

        [Fact]
        public void EnsureDatabaseExists_WhenSqlServerAvailable_CreatesDbAndReturnsFalseThenTrue()
        {
            // Positive-path integration test.
            // Provide a real connection string to a SQL Server instance via environment variable.
            // Example: Server=localhost,1433;User Id=sa;Password=Your_password123;TrustServerCertificate=True;
            var cs = Environment.GetEnvironmentVariable("TEST_SQLSERVER_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(cs))
                return; // not configured => don't run (keeps test suite stable)

            // 1st run: should create DB if missing (returns false when it didn't exist)
            var existedBefore = DatabaseInitializer.EnsureDatabaseExists(cs!, DbName);

            // 2nd run: now it must exist
            var existedAfter = DatabaseInitializer.EnsureDatabaseExists(cs!, DbName);

            Assert.False(existedBefore);
            Assert.True(existedAfter);
        }
    }
}
