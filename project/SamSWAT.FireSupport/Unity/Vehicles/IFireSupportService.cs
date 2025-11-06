using Cysharp.Threading.Tasks;
using System.Threading;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public interface IFireSupportService
{
	public ESupportType SupportType { get; }
	public int AvailableRequests { get; }
	
	public bool IsRequestAvailable();
	public UniTaskVoid PlanRequest(CancellationToken cancellationToken);
}