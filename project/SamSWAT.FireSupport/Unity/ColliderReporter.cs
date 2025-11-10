using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public class ColliderReporter : UpdatableComponentBase
{
	private Transform _transform;
	private BoxCollider[] _colliders;
	private Collider[] _intersectedColliders;
	private int _mask;

	public bool HasCollision { get; private set; }

	public void Rotate(float angle)
	{
		_transform.Rotate(Vector3.up, angle);
	}

	public override void ManualUpdate()
	{
		foreach (BoxCollider col in _colliders)
		{
			Transform colTransform = col.transform;
			Quaternion colRotation = colTransform.rotation;
			Vector3 center = colTransform.position + colRotation * col.center;
			Vector3 extents = Vector3.Scale(col.size * 0.5f, colTransform.lossyScale);

			int hits = Physics.OverlapBoxNonAlloc(
				center,
				extents,
				_intersectedColliders,
				colRotation,
				_mask,
				QueryTriggerInteraction.Ignore);

			if (hits > 0)
			{
				HasCollision = true;
				break;
			}

			HasCollision = false;
		}
	}

	protected override void OnAwake()
	{
		_transform = transform;
		_intersectedColliders = new Collider[5];
		_colliders = GetComponents<BoxCollider>();
		_mask = LayerMaskClass.LowPolyColliderLayerMask | LayerMaskClass.HighPolyCollider;

		HasFinishedInitialization = true;
	}
}