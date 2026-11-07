using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.BL.Web.XStory.Helpers
{
	public static class StaticUtils
	{
		public static readonly Dictionary<string, string> CategoryNameToUrlDictionary = new Dictionary<string, string>()
		{
			{ "Hétéro",       "https://www.xstory-fr.com/histoires-erotiques/hetero" },
			{ "Lesbiennes",   "https://www.xstory-fr.com/histoires-erotiques/lesbiennes" },
			{ "Gays",         "https://www.xstory-fr.com/histoires-erotiques/gays" },
			{ "Bi",           "https://www.xstory-fr.com/histoires-erotiques/bi" },
			{ "Trans & QIA+", "https://www.xstory-fr.com/histoires-erotiques/trans-qia-plus" },
		};

		public static string GetCategoryUrl(string categoryName)
		{
			if (!string.IsNullOrWhiteSpace(categoryName)
				&& CategoryNameToUrlDictionary.TryGetValue(categoryName, out string url))
				return url;
			return string.Empty;
		}
	}
}
