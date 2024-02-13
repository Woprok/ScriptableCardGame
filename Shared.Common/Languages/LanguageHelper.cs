using SecondLanguage;

namespace Shared.Common.Languages
{
    /// <summary>
    /// ToDo implement option to change language
    /// </summary>
    public static class LanguageHelper
    {
        private static readonly Translator Translator = Translator.Default;
        
        public static string TranslateContextual(string context, string content) => Translator.TranslateContextual(context, content);
    }
}