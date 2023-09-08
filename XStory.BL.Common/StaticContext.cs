using System;
using System.Collections.Generic;
using System.Text;
using XStory.DTO.Config;

namespace XStory.BL.Common
{
	internal static class StaticContext
	{
		public static DataSources DataSource { get; set; }
		public static DTO.Category CurrentCategory { get; set; }
		public static int PageNumber { get; set; } = 1;
		public static List<string> HiddenCategories { get; set; } = new List<string>();
	}
}
