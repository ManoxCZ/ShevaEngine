namespace ShevaEngine.Core
{
    public static class ShevaServices
    {
        public static T AddService<T>(T serviceInstance) where T : class
        {
            ShevaGame.Instance.Services.AddService(serviceInstance);

            return serviceInstance;
        }

        public static T GetService<T>() where T : class
        {
            return ShevaGame.Instance.Services.GetService<T>();
        }
    }
}
