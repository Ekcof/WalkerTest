using Scene.Character;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System.Threading;

namespace Serialization
{
	public interface ISerializationManager
	{
		UniTask<bool> TryToLoadAsync(CancellationToken token);
		UniTask<bool> TryToSaveAsync(CancellationToken token);
		UniTask DeleteProgress();
	}

	public class SerializationManager : ISerializationManager
	{
		[Inject] private Player _player;
		private const string SAVE_FILE_NAME = "player_save.json";
		private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

		public async UniTask<bool> TryToSaveAsync(CancellationToken token)
		{
			var save = _player.Serialize();
			var path = SavePath;

			try
			{
				var json = JsonUtility.ToJson(save, true);
				await UniTask.RunOnThreadPool(() =>
				{
					File.WriteAllText(path, json);
				}, cancellationToken: token);
				return true;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Failed to save player data: {ex.Message}");
				return false;
			}
		}

		public async UniTask<bool> TryToLoadAsync(CancellationToken token)
		{
			var path = SavePath;
			if (!File.Exists(path))
			{
				Debug.LogWarning("Save file not found.");
				return false;
			}

			try
			{
				var json = await UniTask.RunOnThreadPool(() =>
				{
					return File.ReadAllText(path);
				}, cancellationToken: token);

				var data = JsonUtility.FromJson<PlayerSerializationData>(json);
				if (data != null)
				{
					_player.Deserialize(data);
					return true;
				}
				else
				{
					Debug.LogError("Failed to deserialize player data: data is null.");
					return false;
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Failed to load player data: {ex.Message}");
				return false;
			}
		}

		public async UniTask DeleteProgress()
		{
			if (File.Exists(SavePath))
			{
				try
				{
					File.Delete(SavePath);
					await UniTask.NextFrame();
					Debug.Log("Player progress deleted successfully.");
				}
				catch (System.Exception ex)
				{
					Debug.LogError($"Failed to delete player progress: {ex.Message}");
				}
			}
			else
			{
				Debug.LogWarning("No save file found to delete.");
			}
		}
	}
}