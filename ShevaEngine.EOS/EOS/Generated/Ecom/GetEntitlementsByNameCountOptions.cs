// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
	/// <summary>
	/// Input parameters for the <see cref="EcomInterface.GetEntitlementsByNameCount" /> function.
	/// </summary>
	public class GetEntitlementsByNameCountOptions
	{
		/// <summary>
		/// The Epic Account ID of the local user for which to retrieve the entitlement count
		/// </summary>
		public EpicAccountId LocalUserId { get; set; }

		/// <summary>
		/// Name of the entitlement to count in the cache
		/// </summary>
		public string EntitlementName { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct GetEntitlementsByNameCountOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_EntitlementName;

		public EpicAccountId LocalUserId
		{
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public string EntitlementName
		{
			set
			{
				Helper.TryMarshalSet(ref m_EntitlementName, value);
			}
		}

		public void Set(GetEntitlementsByNameCountOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = EcomInterface.GetentitlementsbynamecountApiLatest;
				LocalUserId = other.LocalUserId;
				EntitlementName = other.EntitlementName;
			}
		}

		public void Set(object other)
		{
			Set(other as GetEntitlementsByNameCountOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_LocalUserId);
			Helper.TryMarshalDispose(ref m_EntitlementName);
		}
	}
}