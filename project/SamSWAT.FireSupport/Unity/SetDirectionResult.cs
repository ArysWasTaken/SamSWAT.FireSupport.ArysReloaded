using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public readonly struct SetDirectionResult(
	Vector3 startPosition,
	Vector3 endPosition,
	Quaternion rotation,
	bool success)
{
	public Vector3 StartPosition { get; } = startPosition;
	public Vector3 EndPosition { get; } = endPosition;
	public Quaternion Rotation { get; } = rotation;
	public bool Success { get; } = success;
		
	public static SetDirectionResult InvalidDirection { get; } = new(default, default,default, success: false);
}