using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Utility class for generating and reading Guids.
/// </summary>
public static class GuidUtil {

	/// <summary>
	/// Generate a GUID for an asset, with the given pack identifier.
	/// </summary>
	/// <param name="packIdentifier">The pack identifier for the start of the GUID.</param>
	/// <returns>The generated GUID.</returns>
	public static Guid GenerateAssetGuid (uint packIdentifier) {
		return new Guid(packIdentifier, RandomShort(), RandomShort(), RandomByte(), RandomByte(), RandomByte(),
			RandomByte(), RandomByte(), RandomByte(), RandomByte(), RandomByte());
	}

	/// <summary>
	/// Get the pack identifier from the GUID.
	/// </summary>
	/// <param name="guid">The GUID to get the pack identifier from.</param>
	/// <returns>The pack identifier from the GUID.</returns>
	public static uint GetPackIdentifier (Guid guid) {
		return BitConverter.ToUInt32(guid.ToByteArray(), 0);
	}

	public static bool IsMainGameContent(Guid guid) {
		return GetPackIdentifier(guid) == 0;
	}
	
	public static bool IsOfficialExtensionContent(Guid guid) {
		uint packId = GetPackIdentifier(guid);
		return packId >= 1 && packId <= 255;
	}
	
	public static bool IsPackagelessContent(Guid guid) {
		uint packId = GetPackIdentifier(guid);
		return packId >= 256 && packId <= 65535;
	}
	
	public static bool IsPackagedContent(Guid guid) {
		uint packId = GetPackIdentifier(guid);
		return packId >= 65536;
	}

	private static ushort RandomShort () {
		return (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);
	}

	private static byte RandomByte () {
		return (byte) Random.Range(byte.MinValue, byte.MaxValue);;
	}
	
}
