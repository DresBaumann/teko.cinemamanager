using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TopMovie.CinemaManager.Framework.Data;

public class CinemaManagerDbContextFactory : IDesignTimeDbContextFactory<CinemaManagerDbContext>
{
	public CinemaManagerDbContext CreateDbContext(string[] args)
	{
		// Pfad zum Projekt, in dem sich die appsettings.json befindet
		var basePath = Directory.GetCurrentDirectory() +
		               string.Format("{0}..{0}TopMovie.CinemaManager.Web", Path.DirectorySeparatorChar);
		var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

		// Konfiguration laden
		IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile("appsettings.json")
			.AddJsonFile($"appsettings.{environmentName}.json", true)
			.Build();

		// DbContextOptions bauen
		var builder = new DbContextOptionsBuilder<CinemaManagerDbContext>();
		var connectionString = configuration.GetConnectionString("DefaultConnection");

		builder.UseSqlServer(connectionString);

		return new CinemaManagerDbContext(builder.Options);
	}
}