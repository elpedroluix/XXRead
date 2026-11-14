using Microsoft.Extensions.DependencyInjection;
using XXRead.Core.Storage;

namespace XXRead.Storage
{
	public static class DependencyInjection
	{
		/// <summary>
		/// Registers the local database (SQLite) and the repositories.
		/// </summary>
		public static IServiceCollection AddXXReadStorage(this IServiceCollection services)
		{
			// Singleton : opens the connection and creates the tables once
			services.AddSingleton<XXReadDatabase>();

			services.AddTransient<IRepositoryStory, RepositoryStory>();
			services.AddTransient<IRepositoryCategory, RepositoryCategory>();
			services.AddTransient<IRepositoryAuthor, RepositoryAuthor>();

			return services;
		}
	}
}
