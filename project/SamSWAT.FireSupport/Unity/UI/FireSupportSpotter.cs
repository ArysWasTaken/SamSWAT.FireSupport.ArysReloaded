using Comfort.Common;
using Cysharp.Threading.Tasks;
using EFT;
using SamSWAT.FireSupport.ArysReloaded.Utils;
using System.Threading;
using UnityEngine;

namespace SamSWAT.FireSupport.ArysReloaded.Unity;

public class FireSupportSpotter : ScriptableObject
{
	[SerializeField] private GameObject[] spotterParticles;

	private GameObject _inputManager;
	private Player _player;
	private LayerMask _layerMask;

	private ColliderReporter _colliderCheckerObj;
	private GameObject _spotterPositionObj;
	private GameObject _spotterRotationObj;
	private GameObject _spotterConfirmationObj;
	private Transform _spotterDirectionStartTransform;
	private Transform _spotterDirectionEndTransform;

	public static async UniTask<FireSupportSpotter> Load()
	{
		var instance =
			await AssetLoader.LoadAssetAsync<FireSupportSpotter>("assets/content/ui/firesupport_spotter.bundle");

		while (InputManagerUtil.GetInputManager() == null)
		{
			await UniTask.Yield();
		}

		instance.Initialize();

		return instance;
	}

	private void Initialize()
	{
		_inputManager = InputManagerUtil.GetInputManager().gameObject;
		_player = Singleton<GameWorld>.Instance.MainPlayer;
		_layerMask = LayerMaskClass.TerrainLowPoly;

		_spotterPositionObj = Instantiate(spotterParticles[0]);
		_colliderCheckerObj = _spotterPositionObj.GetComponentInChildren<ColliderReporter>();
		_spotterPositionObj.SetActive(false);

		_spotterRotationObj = Instantiate(spotterParticles[1]);
		_spotterDirectionStartTransform = _spotterRotationObj.transform.Find("Spotter Arrow Core (6)");
		_spotterDirectionEndTransform = _spotterRotationObj.transform.Find("Spotter Arrow Core (1)");
		_spotterRotationObj.SetActive(false);

		_spotterConfirmationObj = Instantiate(spotterParticles[2]);
		_spotterConfirmationObj.SetActive(false);
	}

	public async UniTask<SetLocationResult> SetLocation(bool checkSpace, CancellationToken cancellationToken)
	{
		await UniTask.WaitForSeconds(0.1f, cancellationToken: cancellationToken);

		_spotterPositionObj.SetActive(true);

		while (!Input.GetMouseButtonDown(0) && !cancellationToken.IsCancellationRequested)
		{
			if (IsRequestCancelled())
			{
				_spotterPositionObj.SetActive(false);
				FireSupportUI.Instance.SpotterNotice.SetActive(false);
				FireSupportUI.Instance.SpotterHeliNotice.SetActive(false);

				return SetLocationResult.InvalidLocation;
			}

			Transform cameraT = _player.CameraPosition;
			
			bool hasHit = Physics.Raycast(
				origin: cameraT.position + cameraT.forward,
				direction: cameraT.forward,
				out RaycastHit hitInfo,
				maxDistance: 500,
				_layerMask);
			
			FireSupportUI.Instance.SpotterNotice.SetActive(!hasHit);
			
			if (checkSpace && hasHit)
			{
				FireSupportUI.Instance.SpotterHeliNotice.SetActive(_colliderCheckerObj.HasCollision);

				if (_colliderCheckerObj.HasCollision)
				{
					_colliderCheckerObj.Rotate(5f);
				}
			}

			_spotterPositionObj.transform.position = hitInfo.point;
			
			await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
		}

		if (_spotterPositionObj.transform.position.Equals(Vector3.zero) ||
			checkSpace && _colliderCheckerObj.HasCollision)
		{
			FireSupportAudio.Instance.PlayVoiceover(EVoiceoverType.StationDoesNotHear);
			FireSupportUI.Instance.SpotterNotice.SetActive(false);
			FireSupportUI.Instance.SpotterHeliNotice.SetActive(false);
			
			_spotterPositionObj.SetActive(false);
			return SetLocationResult.InvalidLocation;
		}

		_spotterPositionObj.SetActive(false);
		return new SetLocationResult(_spotterPositionObj.transform.position, success: true);
	}

	public async UniTask<SetDirectionResult> SetSupportDirection(
		CancellationToken cancellationToken)
	{
		await UniTask.WaitForSeconds(0.1f, cancellationToken: cancellationToken);

		_spotterRotationObj.transform.SetPositionAndRotation(_spotterPositionObj.transform.position, Quaternion.identity);
		_spotterRotationObj.SetActive(true);
		_inputManager.SetActive(false);

		while (!Input.GetMouseButtonDown(0) && !cancellationToken.IsCancellationRequested)
		{
			if (IsRequestCancelled())
			{
				_spotterRotationObj.SetActive(false);
				_inputManager.SetActive(true);
				
				return SetDirectionResult.InvalidDirection;
			}

			float xAxisRotation = Input.GetAxis("Mouse X") * 5;
			_spotterRotationObj.transform.Rotate(Vector3.down, xAxisRotation);

			await UniTask.NextFrame(cancellationToken);
		}

		_inputManager.SetActive(true);
		_spotterRotationObj.SetActive(false);
		
		return new SetDirectionResult(
			_spotterDirectionStartTransform.position,
			_spotterDirectionEndTransform.position,
			_spotterRotationObj.transform.rotation,
			success: true);
	}

	public async UniTask ConfirmLocation(CancellationToken cancellationToken)
	{
		_spotterConfirmationObj.transform.SetPositionAndRotation(
			_spotterPositionObj.transform.position + Vector3.up,
			Quaternion.identity);
		
		_spotterConfirmationObj.SetActive(true);
		
		await UniTask.WaitForSeconds(0.8f, cancellationToken: cancellationToken);

		_spotterConfirmationObj.SetActive(false);
	}

	private bool IsRequestCancelled()
	{
		bool isCancelRequestInput = Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftAlt);
		bool hasRangefinder =
			_player != null &&
			_player.HandsController.Item?.TemplateId == ItemConstants.RANGEFINDER_TPL;

		return isCancelRequestInput || !hasRangefinder;
	}
}