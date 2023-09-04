using System;
using System.Collections.Generic;
using System.Text;
using XStory.DTO;

namespace XStory.Helpers
{
	/// <summary>
	/// Class used to keep and reuse some static data for the current app run.
	/// </summary>
	public static class StaticContext
	{
		/// <summary>
		/// List of Story already loaded during this app run.
		/// </summary>
		public static List<Story> ListAlreadyLoadedStories = new List<Story>();

		/// <summary>
		/// List of Author already loaded during this app run.
		/// </summary>
		public static List<Author> ListAlreadyLoadedAuthors = new List<Author>();

		/// <summary>
		/// Current datasource used for Content (ex : XStory, HDS...)
		/// </summary>
		public static string DATASOURCE;
	}
}
