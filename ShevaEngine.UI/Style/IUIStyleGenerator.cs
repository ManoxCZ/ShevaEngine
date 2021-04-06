namespace ShevaEngine.UI
{
    public interface IUIStyleGenerator
    {
        /// <summary>
        /// Create new style.
        /// </summary>
        T Create<T>() where T : Control;
    }
}
