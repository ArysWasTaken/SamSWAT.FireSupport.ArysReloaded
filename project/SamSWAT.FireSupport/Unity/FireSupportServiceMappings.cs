using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ZLinq;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public class FireSupportServiceMappings(
	IEqualityComparer<ESupportType> comparer) : Dictionary<ESupportType, IFireSupportService>(comparer)
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool AnyAvailableRequests()
	{
		return Count > 0 &&
			Values.AsValueEnumerable().Any(service => service.IsRequestAvailable()) &&
			FireSupportController.Instance.IsSupportAvailable();
	}
}