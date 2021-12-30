// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Metrics
{
	/// <summary>
	/// BeginPlayerSession.
	/// </summary>
	public class BeginPlayerSessionOptions
	{
		public BeginPlayerSessionOptionsAccountId AccountId { get; set; }

		/// <summary>
		/// The in-game display name for the user as UTF-8 string.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// The user's game controller type.
		/// </summary>
		public UserControllerType ControllerType { get; set; }

		/// <summary>
		/// IP address of the game server hosting the game session. For a localhost session, set to NULL.
		/// 
		/// @details Must be in either one of the following IPv4 or IPv6 string formats:
		/// "127.0.0.1".
		/// "1200:0000:AB00:1234:0000:2552:7777:1313".
		/// If both IPv4 and IPv6 addresses are available, use the IPv6 address.
		/// </summary>
		public string ServerIp { get; set; }

		/// <summary>
		/// Optional, application-defined custom match session identifier. If the identifier is not used, set to NULL.
		/// 
		/// @details The game can tag each game session with a custom session match identifier,
		/// which will be shown in the Played Sessions listing at the user profile dashboard.
		/// </summary>
		public string GameSessionId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct BeginPlayerSessionOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private BeginPlayerSessionOptionsAccountIdInternal m_AccountId;
		private System.IntPtr m_DisplayName;
		private UserControllerType m_ControllerType;
		private System.IntPtr m_ServerIp;
		private System.IntPtr m_GameSessionId;

		public BeginPlayerSessionOptionsAccountId AccountId
		{
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public string DisplayName
		{
			set
			{
				Helper.TryMarshalSet(ref m_DisplayName, value);
			}
		}

		public UserControllerType ControllerType
		{
			set
			{
				m_ControllerType = value;
			}
		}

		public string ServerIp
		{
			set
			{
				Helper.TryMarshalSet(ref m_ServerIp, value);
			}
		}

		public string GameSessionId
		{
			set
			{
				Helper.TryMarshalSet(ref m_GameSessionId, value);
			}
		}

		public void Set(BeginPlayerSessionOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = MetricsInterface.BeginplayersessionApiLatest;
				AccountId = other.AccountId;
				DisplayName = other.DisplayName;
				ControllerType = other.ControllerType;
				ServerIp = other.ServerIp;
				GameSessionId = other.GameSessionId;
			}
		}

		public void Set(object other)
		{
			Set(other as BeginPlayerSessionOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_AccountId);
			Helper.TryMarshalDispose(ref m_DisplayName);
			Helper.TryMarshalDispose(ref m_ServerIp);
			Helper.TryMarshalDispose(ref m_GameSessionId);
		}
	}
}