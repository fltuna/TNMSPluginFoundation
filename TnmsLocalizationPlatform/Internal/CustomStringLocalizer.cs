using System.Globalization;
using Microsoft.Extensions.Localization;
using Sharp.Shared.Objects;
using TnmsLocalizationPlatform.Shared;
using TnmsLocalizationPlatform.Util;

namespace TnmsLocalizationPlatform.Internal;

public class CustomStringLocalizer(Dictionary<string, Dictionary<string, string>> translations): ITnmsLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            if (!translations.TryGetValue(TnmsLocalizationPlatform.Instance.ServerDefaultCulture.TwoLetterISOLanguageName, out var translation))
            {
                if(translation == null)
                    return new LocalizedString(name, name, false);
            }
            
            var value = translation.GetValueOrDefault(name, name);
            return new LocalizedString(name, ChatColorUtil.FormatChatMessage(value), !translations.ContainsKey(name));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            if (!translations.TryGetValue(TnmsLocalizationPlatform.Instance.ServerDefaultCulture.TwoLetterISOLanguageName, out var translation))
            {
                translation = translations.First().Value;
            }
            var format = translation.GetValueOrDefault(name, name);
        
            format = ChatColorUtil.FormatChatMessage(format);
            var value = string.Format(format, arguments);
            return new LocalizedString(name, value, !translations.ContainsKey(name));
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
                var rawValue = translation.GetValueOrDefault(name, name);
                value = new LocalizedString(name, ChatColorUtil.FormatChatMessage(rawValue), !translations.ContainsKey(name));
            }
            return value;
        }
    }
    
    public LocalizedString this[string name, CultureInfo culture, params object[] arguments]
    {
        get
        {
            if (!translations.TryGetValue(culture.TwoLetterISOLanguageName, out var translation))
            {
                return this[name, arguments];
            }
        
            var format = translation.GetValueOrDefault(name, name);
            format = ChatColorUtil.FormatChatMessage(format);
            var value = string.Format(format, arguments);
            return new LocalizedString(name, value, !translations.ContainsKey(name));
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
        string format;
        
        if (!TnmsLocalizationPlatform.Instance.ClientCultures.TryGetValue(client.Slot, out var culture))
        {
            if (!translations.TryGetValue(TnmsLocalizationPlatform.Instance.ServerDefaultCulture.TwoLetterISOLanguageName, out var translation))
            {
                translation = translations.First().Value;
            }
            format = translation.GetValueOrDefault(name, name);
        }
        else
        {
            if (!translations.TryGetValue(culture.TwoLetterISOLanguageName, out var translation))
            {
                if (!translations.TryGetValue(TnmsLocalizationPlatform.Instance.ServerDefaultCulture.TwoLetterISOLanguageName, out translation))
                {
                    translation = translations.First().Value;
                }
            }
            format = translation.GetValueOrDefault(name, name);
        }
        
        format = ChatColorUtil.FormatChatMessage(format);
        var value = string.Format(format, arguments);
        return new LocalizedString(name, value, !translations.ContainsKey(name));
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