// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Ecom
{
	/// <summary>
	/// Input parameters for the <see cref="EcomInterface.CopyOfferImageInfoByIndex" /> function.
	/// </summary>
	public class CopyOfferImageInfoByIndexOptions
	{
		/// <summary>
		/// The Epic Account ID of the local user whose offer image is being copied.
		/// </summary>
		public EpicAccountId LocalUserId { get; set; }

		/// <summary>
		/// The ID of the offer to get the images for.
		/// </summary>
		public string OfferId { get; set; }

		/// <summary>
		/// The index of the image to get.
		/// </summary>
		public uint ImageInfoIndex { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct CopyOfferImageInfoByIndexOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_OfferId;
		private uint m_ImageInfoIndex;

		public EpicAccountId LocalUserId
		{
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public string OfferId
		{
			set
			{
				Helper.TryMarshalSet(ref m_OfferId, value);
			}
		}

		public uint ImageInfoIndex
		{
			set
			{
				m_ImageInfoIndex = value;
			}
		}

		public void Set(CopyOfferImageInfoByIndexOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = EcomInterface.CopyofferimageinfobyindexApiLatest;
				LocalUserId = other.LocalUserId;
				OfferId = other.OfferId;
				ImageInfoIndex = other.ImageInfoIndex;
			}
		}

		public void Set(object other)
		{
			Set(other as CopyOfferImageInfoByIndexOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_LocalUserId);
			Helper.TryMarshalDispose(ref m_OfferId);
		}
	}
}