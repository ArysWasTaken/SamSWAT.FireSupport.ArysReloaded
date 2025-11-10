using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public readonly struct SetLocationResult(Vector3 targetLocation, bool success)
{
	public Vector3 TargetLocation { get; } = targetLocation;
	public bool Success { get; } = success;

	public static SetLocationResult InvalidLocation { get; } = new(default, success: false);
}