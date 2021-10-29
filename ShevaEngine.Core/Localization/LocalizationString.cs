namespace ShevaEngine.Core
{
    /// <summary>
    /// Localization string.
    /// </summary>
    public class LocalizationString
    {
        public string TextKey { get; set; } = null!;


        /// <summary>
        /// Constructor.
        /// </summary>
        protected LocalizationString()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>		
        public LocalizationString(string textKey)
        {
            TextKey = textKey;
        }

        /// <summary>
        /// Override ToString() method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LocalizationManager.Instance.GetValue(TextKey);
        }
    }
}
