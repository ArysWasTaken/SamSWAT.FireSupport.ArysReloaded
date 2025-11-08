using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public abstract class FireSupportService(int maxRequests) : IFireSupportService
{
	protected int availableRequests = maxRequests;
	protected bool requestAvailable = true;

	public abstract ESupportType SupportType { get; }
	public int AvailableRequests => availableRequests;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsRequestAvailable()
	{
		return availableRequests > 0 && requestAvailable;
	}

	public abstract UniTaskVoid PlanRequest(CancellationToken cancellationToken);
}