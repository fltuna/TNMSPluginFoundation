using System.Globalization;
using Microsoft.Extensions.Localization;
using Sharp.Shared.Objects;
using TnmsLocalizationPlatform.Shared;

namespace TnmsLocalizationPlatform.Internal;

public class CustomStringLocalizer(Dictionary<string, Dictionary<string, string>> translations): ITnmsLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            if (!translations.TryGetValue(TnmsLocalizationPlatform.Instance.ServerDefaultCulture.TwoLetterISOLanguageName, out var translation))
            {
                translation = translations.First().Value;
            }
            
            var value = translation.GetValueOrDefault(name, name);
            return new LocalizedString(name, value, !translations.ContainsKey(name));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = this[name];
            var value = string.Format(format.Value, arguments);
            return new LocalizedString(name, value, format.ResourceNotFound);
        }
    }
    
    
    public LocalizedString this[string name, CultureInfo culture]
    {
        get
        {
            LocalizedString value;
            if (!translations.TryGetValue(culture.TwoLetterISOLanguageName, out var translation))
            {
                value = this[name];
            }
            else
            {
                value = new LocalizedString(name, translation.GetValueOrDefault(name, name), !translations.ContainsKey(name));
            }
            return value;
        }
    }
    
    public LocalizedString this[string name, CultureInfo culture, params object[] arguments]
    {
        get
        {
            LocalizedString value;
            if (!translations.TryGetValue(culture.TwoLetterISOLanguageName, out var translation))
            {
                value = this[name, arguments];
            }
            else
            {
                var format = this[name, culture];
                var toVar = string.Format(format.Value, arguments);
                value = new LocalizedString(name, toVar, format.ResourceNotFound);
            }
            return value;
        }
    }

    public CultureInfo GetClientCulture(IGameClient client)
    {
        if (TnmsLocalizationPlatform.Instance.ClientCultures.TryGetValue(client.Slot, out var culture))
            return culture;

        return TnmsLocalizationPlatform.Instance.ServerDefaultCulture;
    }

    public LocalizedString ForClient(IGameClient client, string name, params object[] arguments)
    {
        LocalizedString format;
        string value;
        
        if (!TnmsLocalizationPlatform.Instance.ClientCultures.TryGetValue(client.Slot, out var culture))
        {
            format = this[name];
            value = string.Format(format.Value, arguments);
        }
        else
        {
            format = this[name, culture];
            value = string.Format(format.Value, arguments);
        }
        return new LocalizedString(name, value, format.ResourceNotFound);
    }

    public IEnumerable<LocalizedString> GetAllStringsByCulture(CultureInfo culture)
    {
        if (!translations.TryGetValue(culture.TwoLetterISOLanguageName, out var translation))
        {
            translation = translations.First().Value;
        }
        
        return translation.Select(t => 
            new LocalizedString(t.Key, t.Value, false));
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        if (!translations.TryGetValue(TnmsLocalizationPlatform.Instance.ServerDefaultCulture.TwoLetterISOLanguageName, out var translation))
        {
            translation = translations.First().Value;
        }
        
        return translation.Select(t => 
            new LocalizedString(t.Key, t.Value, false));
    }
}