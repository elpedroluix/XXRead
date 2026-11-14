namespace XXRead.UI.Theming
{
	/// <summary>
	/// Background colors and the font colors that go with them (dark or light theme).
	/// </summary>
	public class ThemeBackground
	{
		public string Name { get; private set; }

		public string Primary { get; private set; }

		public string Secondary { get; private set; }

		public string FontPrimary { get; private set; }

		public string FontSecondary { get; private set; }

		public ThemeBackground(string name, string primary, string secondary, string fontPrimary, string fontSecondary)
		{
			Name = name;
			Primary = primary;
			Secondary = secondary;
			FontPrimary = fontPrimary;
			FontSecondary = fontSecondary;
		}
	}
}
