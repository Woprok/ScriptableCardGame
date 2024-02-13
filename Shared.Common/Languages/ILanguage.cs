namespace Shared.Common.Languages
{
    /// <summary>
    /// This should be implemented by all views and viewModels that sets strings displayed by user.
    /// </summary>
    public interface ILanguage
    {
        void SetTranslations();
    }
}