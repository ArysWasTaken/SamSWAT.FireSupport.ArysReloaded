using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public sealed class JetStrafeService(
	FireSupportSpotter spotter,
	CancellationToken token,
	int maxRequests) : FireSupportService(maxRequests)
{
	public override ESupportType SupportType => ESupportType.Strafe;

	public override async UniTaskVoid PlanRequest()
	{
		try
		{
			await spotter.SetLocation(checkSpace: false, token);
			(Vector3 startPos, Vector3 endPos, Quaternion _) directionData = await spotter.SetSupportDirection(token);
			await spotter.ConfirmLocation(token);
			ConfirmRequest(
				strafeStartPos: directionData.startPos,
				strafeEndPos: directionData.endPos).Forget();
		}
		catch (OperationCanceledException) {}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}
	
	private async UniTaskVoid ConfirmRequest(Vector3 strafeStartPos, Vector3 strafeEndPos)
	{
		try
		{
			requestAvailable = false;
			availableRequests--;
			
			IFireSupportBehaviour a10 = FireSupportPoolManager.Instance.TakeFromPool(SupportType);
			FireSupportController.Instance
				.StartCooldown(PluginSettings.RequestCooldown.Value, token, OnCooldownOver)
				.Forget();
			FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.StationStrafeRequest);
			await UniTask.WaitForSeconds(8f);
			
			FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.JetArriving);
			Vector3 pos = (strafeStartPos + strafeEndPos) / 2;
			Vector3 dir = (strafeEndPos - strafeStartPos).normalized;
			a10.ProcessRequest(pos, dir, Vector3.zero, token);
		}
		catch (OperationCanceledException) {}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}
	
	private void OnCooldownOver()
	{
		requestAvailable = true;
	}
}