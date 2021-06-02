// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.AntiCheatClient
{
	/// <summary>
	/// Anti-cheat integrity violation types
	/// </summary>
	public enum AntiCheatClientViolationType : int
	{
		/// <summary>
		/// Not used
		/// </summary>
		Invalid = 0,
		/// <summary>
		/// An anti-cheat asset integrity catalog file could not be found
		/// </summary>
		IntegrityCatalogNotFound = 1,
		/// <summary>
		/// An anti-cheat asset integrity catalog file is corrupt or invalid
		/// </summary>
		IntegrityCatalogError = 2,
		/// <summary>
		/// An anti-cheat asset integrity catalog file's certificate has been revoked.
		/// </summary>
		IntegrityCatalogCertificateRevoked = 3,
		/// <summary>
		/// The primary anti-cheat asset integrity catalog does not include an entry for the game's
		/// main executable, which is required.
		/// </summary>
		IntegrityCatalogMissingMainExecutable = 4,
		/// <summary>
		/// A disallowed game file modification was detected
		/// </summary>
		GameFileMismatch = 5,
		/// <summary>
		/// A disallowed game file removal was detected
		/// </summary>
		RequiredGameFileNotFound = 6,
		/// <summary>
		/// A disallowed game file addition was detected
		/// </summary>
		UnknownGameFileForbidden = 7,
		/// <summary>
		/// A system file failed an integrity check
		/// </summary>
		SystemFileUntrusted = 8,
		/// <summary>
		/// A disallowed code module was loaded into the game process
		/// </summary>
		ForbiddenModuleLoaded = 9,
		/// <summary>
		/// A disallowed game process memory modification was detected
		/// </summary>
		CorruptedMemory = 10,
		/// <summary>
		/// A disallowed tool was detected running in the system
		/// </summary>
		ForbiddenToolDetected = 11,
		/// <summary>
		/// An internal anti-cheat integrity check failed
		/// </summary>
		InternalAntiCheatViolation = 12,
		/// <summary>
		/// Integrity checks on messages between the game client and game server failed
		/// </summary>
		CorruptedNetworkMessageFlow = 13,
		/// <summary>
		/// The game is running inside a disallowed virtual machine
		/// </summary>
		VirtualMachineNotAllowed = 14,
		/// <summary>
		/// A forbidden operating system configuration was detected
		/// </summary>
		ForbiddenSystemConfiguration = 15
	}
}