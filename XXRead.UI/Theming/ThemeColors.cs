namespace XXRead.UI.Theming
{
	/// <summary>
	/// Predefined theme colors (same values as the XAML UI's Helpers/Themes/Theme).
	/// </summary>
	public static class ThemeColors
	{
		public const string DarkPrimary = "#111111";
		public const string DarkSecondary = "#222222";
		public const string FontDarkPrimary = "#333333";
		public const string FontDarkSecondary = "#444444";

		public const string LightPrimary = "#FFFFFF";
		public const string LightSecondary = "#FBF9FF";
		public const string FontLightPrimary = "#EEEEEE";
		public const string FontLightSecondary = "#CCCCCC";

		public const string MainRed = "#A12F3C";
		public const string MainBlack = "#000000";
		public const string MainPink = "#E03577";
		public const string MainPurple = "#B200A3";
		public const string MainLightBlue = "#66ACFF";
		public const string MainGreen = "#238200";
		public const string MainGreeyn = "#135553";
		public const string MainDarkBlue = "#33467F";
		public const string MainYellow = "#FFD966";
		public const string MainOrange = "#F07700";
		public const string MainBrown = "#B06700";

		/// <summary>
		/// Dark background : light fonts.
		/// </summary>
		public static readonly ThemeBackground Dark = new ThemeBackground("Sombre", DarkPrimary, DarkSecondary, FontLightPrimary, FontLightSecondary);

		/// <summary>
		/// Light background : dark fonts.
		/// </summary>
		public static readonly ThemeBackground Light = new ThemeBackground("Clair", LightPrimary, LightSecondary, FontDarkPrimary, FontDarkSecondary);

		public static readonly IReadOnlyList<ThemeBackground> Backgrounds = new[] { Dark, Light };

		/// <summary>
		/// Main colors offered in the settings, in the order of the XAML UI.
		/// </summary>
		public static readonly IReadOnlyList<string> MainColors = new[]
		{
			MainRed, MainBlack, MainPink, MainPurple,
			MainLightBlue, MainGreen, MainGreeyn, MainDarkBlue,
			MainYellow, MainOrange, MainBrown,
		};
	}
}
