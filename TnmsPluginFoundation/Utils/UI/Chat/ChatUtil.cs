using System.Reflection;
using System.Text.RegularExpressions;

namespace TnmsPluginFoundation.Utils.UI.Chat;

/// <summary>
/// Utility class for manipulating ColorText
/// </summary>
public static class ChatUtil
{
    
    /// <summary>
    /// Replaces color text such as {DarkRed}.
    /// </summary>
    /// <param name="text">The text want to replace</param>
    /// <returns>Replaced text</returns>
    public static string ReplaceColorStrings(string text)
    {
        throw new NotImplementedException("Not implemented yet");
        
        var colorFields = typeof(Object)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(char));

        foreach (var field in colorFields)
        {
            string colorName = field.Name;
            text = Regex.Replace(text, "{" + Regex.Escape(colorName) + "}", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "{/" + Regex.Escape(colorName) + "}", "", RegexOptions.IgnoreCase);
        }

        return text;
    }
}