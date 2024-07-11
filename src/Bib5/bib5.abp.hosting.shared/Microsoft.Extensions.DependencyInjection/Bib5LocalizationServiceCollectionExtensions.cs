using Volo.Abp.Localization;

namespace Microsoft.Extensions.DependencyInjection;

public static class Bib5LocalizationServiceCollectionExtensions
{
	public static IServiceCollection ConfigureBib5Localization(this IServiceCollection services)
	{
		services.Configure(delegate(AbpLocalizationOptions options)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Expected O, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Expected O, but got Unknown
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Expected O, but got Unknown
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Expected O, but got Unknown
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Expected O, but got Unknown
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Expected O, but got Unknown
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Expected O, but got Unknown
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Expected O, but got Unknown
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Expected O, but got Unknown
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Expected O, but got Unknown
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Expected O, but got Unknown
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Expected O, but got Unknown
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Expected O, but got Unknown
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Expected O, but got Unknown
			options.Languages.Add(new LanguageInfo("ar", "ar", "العربية", (string)null));
			options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština", (string)null));
			options.Languages.Add(new LanguageInfo("en", "en", "English", (string)null));
			options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)", (string)null));
			options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish", (string)null));
			options.Languages.Add(new LanguageInfo("fr", "fr", "Français", (string)null));
			options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
			options.Languages.Add(new LanguageInfo("is", "is", "Icelandic", "is"));
			options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
			options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Română", (string)null));
			options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar", (string)null));
			options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português", (string)null));
			options.Languages.Add(new LanguageInfo("ru", "ru", "Русский", (string)null));
			options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak", (string)null));
			options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe", (string)null));
			options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文", (string)null));
			options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文", (string)null));
			options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
			options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
			options.Languages.Add(new LanguageInfo("el", "el", "Ελληνικά", (string)null));
		});
		return services;
	}
}
