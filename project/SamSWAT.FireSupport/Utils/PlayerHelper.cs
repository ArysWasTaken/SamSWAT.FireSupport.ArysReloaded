using EFT;
using System.Runtime.CompilerServices;

namespace SamSWAT.FireSupport.ArysReloaded.Utils;

internal static class PlayerHelper
{
	/// <summary>
	/// Checks if the main player is alive.
	/// </summary>
	/// <returns>True if alive, otherwise false.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsMainPlayerAlive(this GameWorld gameWorld)
	{
		return gameWorld.MainPlayer != null && gameWorld.MainPlayer.ActiveHealthController.IsAlive;
	}
}