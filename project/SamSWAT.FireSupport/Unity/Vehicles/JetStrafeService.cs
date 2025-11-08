using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public sealed class JetStrafeService(FireSupportSpotter spotter, int maxRequests) : FireSupportService(maxRequests)
{
	public override ESupportType SupportType => ESupportType.Strafe;

	public override async UniTaskVoid PlanRequest(CancellationToken cancellationToken)
	{
		try
		{
			await spotter.SetLocation(checkSpace: false, cancellationToken);
			(Vector3 startPos, Vector3 endPos, Quaternion _) directionData = await spotter.SetSupportDirection(cancellationToken);
			await spotter.ConfirmLocation(cancellationToken);
			ConfirmRequest(
				strafeStartPos: directionData.startPos,
				strafeEndPos: directionData.endPos,
				cancellationToken)
				.Forget();
		}
		catch (OperationCanceledException) {}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}
	
	private async UniTaskVoid ConfirmRequest(Vector3 strafeStartPos, Vector3 strafeEndPos, CancellationToken cancellationToken)
	{
		requestAvailable = false;
		availableRequests--;
		
		IFireSupportBehaviour a10 = FireSupportPoolManager.Instance.TakeFromPool(SupportType);
		FireSupportController.Instance
			.StartCooldown(PluginSettings.RequestCooldown.Value, cancellationToken, OnCooldownOver)
			.Forget();
		FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.StationStrafeRequest);
		await UniTask.WaitForSeconds(8f, cancellationToken: cancellationToken);
		
		FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.JetArriving);
		Vector3 pos = (strafeStartPos + strafeEndPos) / 2;
		Vector3 dir = (strafeEndPos - strafeStartPos).normalized;
		a10.ProcessRequest(pos, dir, Vector3.zero, cancellationToken);
	}
	
	private void OnCooldownOver()
	{
		requestAvailable = true;
	}
}