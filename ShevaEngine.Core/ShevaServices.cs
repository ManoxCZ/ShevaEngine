namespace ShevaEngine.Core
{
    public static class ShevaServices
    {
        public static T GetService<T>() where T : class
        {
            return ShevaGame.Instance.Services.GetService<T>();
        }
    }
}
