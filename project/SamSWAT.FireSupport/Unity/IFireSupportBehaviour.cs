using System.Threading;
using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public interface IFireSupportBehaviour
{
	void ProcessRequest(Vector3 position, Vector3 direction, Vector3 rotation, CancellationToken cancellationToken);
}