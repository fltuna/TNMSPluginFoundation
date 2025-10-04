using System.Text.Json;

namespace TnmsLocalizationPlatform.Internal;

public class LanguageDataParser(string languageDataDir)
{
    public Dictionary<string, Dictionary<string, string>> Parse()
    {
        var translations = new Dictionary<string, Dictionary<string, string>>();
        
        if (!Directory.Exists(languageDataDir))
        {
            return translations;
        }

        var jsonFiles = Directory.GetFiles(languageDataDir, "*.json");
        
        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(jsonFile);
                var jsonContent = File.ReadAllText(jsonFile);
                
                var languageData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                
                if (languageData != null)
                {
                    translations[fileName] = languageData;
                }
            }
            catch (Exception ex)
            {
                // Ignore invalid files
            }
        }
        
        return translations;
    }
}