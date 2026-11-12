using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels
{
	public class AuthorPageWebViewModel : BaseViewModel
	{
		private string _startPath;

		public string StartPath
		{
			get { return _startPath; }
			set { _startPath = value; }
		}

		public AuthorPageWebViewModel(IServiceNavigation serviceNavigation) : base(serviceNavigation)
		{
			Title = "Saperlipopette";

			StartPath = "/";

		}
	}
}
