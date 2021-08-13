using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;

namespace ShevaEngine.UserAccounts
{
	/// <summary>
	/// User data.
	/// </summary>
	public class UserData
	{
        public static UserData Connecting = new UserData();
        
        public string GamerName { get; internal set; }
        public Texture2D GamerPicture { get; internal set; }
#if WINDOWS_UAP
        public Microsoft.Xbox.Services.System.XboxLiveUser XboxLiveUser { get; internal set; }
#endif

        /// <summary>
        /// Update score.
        /// </summary>
        public Task<bool> UpdateScore<T>(string name, T value)
        {
#if WINDOWS_UAP
            return Task.Run(() =>
            {
                if (typeof(T) == typeof(byte) ||
                    typeof(T) == typeof(short) ||
                    typeof(T) == typeof(ushort) ||
                    typeof(T) == typeof(int) ||
                    typeof(T) == typeof(uint) ||
                    typeof(T) == typeof(long) ||
                    typeof(T) == typeof(ulong))
                    Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance.SetStatisticIntegerData(XboxLiveUser, name, Convert.ToInt64(value));
                else if (typeof(T) == typeof(float) ||
                         typeof(T) == typeof(double))
                    Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance.SetStatisticNumberData(XboxLiveUser, name, Convert.ToDouble(value));
                else if (typeof(T) == typeof(string))
                    Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance.SetStatisticStringData(XboxLiveUser, name, Convert.ToString(value));

                Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance.RequestFlushToService(XboxLiveUser);                

                while (true)
                {
                    foreach (var statEvent in Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance.DoWork())
                    {
                        if (statEvent.EventType == Microsoft.Xbox.Services.Statistics.Manager.StatisticEventType.StatisticUpdateComplete)
                            return true;
                    }
                }
            });
#else
            return Task.FromResult(false);
#endif
        }

        /// <summary>
        /// Update score.
        /// </summary>
        public T GetScore<T>(string name)
        {
#if WINDOWS_UAP
            Microsoft.Xbox.Services.Statistics.Manager.StatisticValue scoreValue =
                Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance.GetStatistic(XboxLiveUser, name);

            if (typeof(T) == typeof(byte) ||
                typeof(T) == typeof(short) ||
                typeof(T) == typeof(ushort) ||
                typeof(T) == typeof(int) ||
                typeof(T) == typeof(uint) ||
                typeof(T) == typeof(long) ||
                typeof(T) == typeof(ulong))
                return (T)(object)scoreValue.AsInteger;
            else if (typeof(T) == typeof(float) ||
                     typeof(T) == typeof(double))
                return (T)(object)scoreValue.AsNumber;
            else if (typeof(T) == typeof(string))
                return (T)(object)scoreValue.AsString;

            return default;
#else
            return default;
#endif
        }
    }
}
