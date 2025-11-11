using Cysharp.Text;
using Cysharp.Threading.Tasks;
using EFT.InputSystem;
using EFT.UI;
using EFT.UI.Gestures;
using SamSWAT.FireSupport.ArysReloaded.Utils;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

/// <summary>
/// Entry point for Fire Support in-raid logic.
/// </summary>
public class FireSupportController : UIInputNode
{
	[NonSerialized] private FireSupportAudio _audio;
	[NonSerialized] private FireSupportUI _ui;
	[NonSerialized] private FireSupportSpotter _spotter;
	[NonSerialized] private GesturesMenu _gesturesMenu;

	[NonSerialized] private readonly FireSupportServiceMappings _services = new(new SupportTypeComparer());

	[NonSerialized] private bool _canCallSupport = true;
	[NonSerialized] private int _cooldownTimer;

	public static FireSupportController Instance { get; private set; }

	public static async UniTask<FireSupportController> Create(GesturesMenu gesturesMenu)
	{
		Instance = new GameObject("FireSupportController").AddComponent<FireSupportController>();
		await Instance.Initialize(gesturesMenu);
		return Instance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CanCallSupport(bool canCall)
	{
		_canCallSupport = canCall;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSupportAvailable()
	{
		return _cooldownTimer == 0 && _canCallSupport;
	}

	private async UniTask Initialize(GesturesMenu gesturesMenu)
	{
		_gesturesMenu = gesturesMenu;
		_audio = await FireSupportAudio.Create();
		_spotter = await FireSupportSpotter.Load();

		var heliExfil = new HeliExfiltrationService(
			_spotter,
			PluginSettings.AmountOfExtractionRequests.Value);
		_services[heliExfil.SupportType] = heliExfil;

		var jetStrafe = new JetStrafeService(
			_spotter,
			PluginSettings.AmountOfStrafeRequests.Value);
		_services[jetStrafe.SupportType] = jetStrafe;

		_ui = await FireSupportUI.Load(_services, gesturesMenu);
		_ui.SupportRequested += OnSupportRequested;

		await FireSupportPoolManager.Initialize(10);

		_audio.PlayVoiceover(EVoiceoverType.StationReminder);
	}

	private void OnDestroy()
	{
		_ui.SupportRequested -= OnSupportRequested;
		AssetLoader.UnloadAllBundles();
		FireSupportAudio.Instance.Dispose();
		Destroy(_spotter);
		FireSupportUI.Instance.Dispose();
		Destroy(gameObject);
		Instance = null;
	}

	private void OnSupportRequested(ESupportType supportType)
	{
		try
		{
			if (!_services.TryGetValue(supportType, out IFireSupportService service))
			{
				throw new ArgumentException($"No service registered for support type {supportType}");
			}

			if (!service.IsRequestAvailable())
			{
				FireSupportPlugin.LogSource.LogWarning("Should not be able to reach this line, bad logic somewhere...");
				return;
			}

			_gesturesMenu.Close();
			service.PlanRequest(destroyCancellationToken).Forget();
		}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}

	public async UniTaskVoid StartCooldown(int time, CancellationToken cancellationToken, Action callback = null)
	{
		try
		{
			_ui.timerText.enabled = true;
			_cooldownTimer = time;

			while (_cooldownTimer > 0)
			{
				_cooldownTimer--;

				float minutes = Mathf.FloorToInt(_cooldownTimer / 60f);
				float seconds = Mathf.FloorToInt(_cooldownTimer % 60);

				using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder())
				{
					sb.AppendFormat("{0:00}.{1:00}", minutes, seconds);
					_ui.timerText.text = sb.ToString();
				}

				await UniTask.WaitForSeconds(1, cancellationToken: cancellationToken);
			}

			_ui.timerText.enabled = false;

			if (_services.AnyAvailableRequests())
			{
				FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.StationAvailable);
			}

			CanCallSupport(true);
			callback?.Invoke();
		}
		catch (Exception ex)
		{
			FireSupportPlugin.LogSource.LogError(ex);
		}
	}

	public override ETranslateResult TranslateCommand(ECommand command)
	{
		return ETranslateResult.Ignore;
	}

	public override void TranslateAxes(ref float[] axes)
	{
	}

	public override ECursorResult ShouldLockCursor()
	{
		return ECursorResult.Ignore;
	}
}