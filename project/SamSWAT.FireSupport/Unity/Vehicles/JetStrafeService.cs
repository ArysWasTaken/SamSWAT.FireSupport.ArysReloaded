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
		SetLocationResult locationResult = await spotter.SetLocation(checkSpace: false, cancellationToken);

		if (!locationResult.Success) return;

		SetDirectionResult directionResult = await spotter.SetSupportDirection(cancellationToken);

		if (directionResult.Success)
		{
			await spotter.ConfirmLocation(cancellationToken);
			ConfirmRequest(
					strafeStartPos: directionResult.StartPosition,
					strafeEndPos: directionResult.EndPosition,
					cancellationToken)
				.Forget();
		}
	}

	private async UniTaskVoid ConfirmRequest(Vector3 strafeStartPos, Vector3 strafeEndPos,
		CancellationToken cancellationToken)
	{
		requestAvailable = false;
		availableRequests--;
		FireSupportController.Instance.CanCallSupport(false);

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