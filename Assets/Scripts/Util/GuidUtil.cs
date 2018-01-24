using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Utility class for generating and reading Guids.
/// </summary>
public class GuidUtil : MonoBehaviour {

	/// <summary>
	/// Generate a GUID for an asset, with the given pack identifier.
	/// </summary>
	/// <param name="packIdentifier">The pack identifier for the start of the GUID.</param>
	/// <returns>The generated GUID.</returns>
	public static Guid GenerateAssetGuid (int packIdentifier) {
		return new Guid(packIdentifier, RandomShort(), RandomShort(), RandomByteSegment());
	}

	/// <summary>
	/// Get the pack identifier from the GUID.
	/// </summary>
	/// <param name="guid">The GUID to get the pack identifier from.</param>
	/// <returns>The pack identifier from the GUID.</returns>
	public static int GetPackIdentifier (Guid guid) {
		return BitConverter.ToInt32(guid.ToByteArray(), 0);
	}

	private static short RandomShort () {
		return (short) Random.Range(short.MinValue, short.MaxValue);
	}

	private static byte[] RandomByteSegment () {
		byte[] bytes = new byte[8];
		for (int i = 0; i < bytes.Length; i++) {
			bytes[i] = (byte) Random.Range(byte.MinValue, byte.MaxValue);
		}
		return bytes;
	}
	
}
