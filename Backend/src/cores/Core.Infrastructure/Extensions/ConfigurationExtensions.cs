// Konum: cores/Core.Infrastructure/Extensions/ConfigurationExtensions.cs
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// IConfiguration arayüzü için yapılandırma ayarlarını daha kolay ve
/// güvenli bir şekilde okumayı sağlayan genişletme metotları içerir.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Belirtilen section (bölüm) adındaki yapılandırma ayarlarını T tipindeki bir nesneye bağlar.
    /// Eğer section bulunamazsa veya boşsa, bir istisna fırlatarak uygulamanın
    /// eksik yapılandırma ile başlamasını engeller.
    /// </summary>
    /// <typeparam name="T">Ayarların bağlanacağı sınıfın türü.</typeparam>
    /// <param name="configuration">IConfiguration nesnesi.</param>
    /// <param name="sectionName">appsettings.json dosyasındaki bölümün adı.</param>
    /// <returns>Ayarlarla doldurulmuş T tipinde bir nesne.</returns>
    /// <exception cref="InvalidOperationException">Belirtilen section bulunamazsa veya boşsa fırlatılır.</exception>
    /// <example>
    /// <code>
    /// // Program.cs içinde:
    /// MailSettings mailSettings = builder.Configuration.GetRequiredSectionAs<MailSettings>("MailSettings");
    /// builder.Services.AddSingleton(mailSettings);
    /// </code>
    /// </example>
    public static T GetRequiredSectionAs<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var section = configuration.GetSection(sectionName);

        if (!section.Exists())
        {
            throw new InvalidOperationException($"'{sectionName}' adlı yapılandırma bölümü bulunamadı.");
        }

        T settings = new();
        section.Bind(settings);
        return settings;
    }
}