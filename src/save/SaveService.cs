namespace Shellguard.Save;

using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using Chickensoft.Collections;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
using Godot;
using Shellguard.Game;
using Shellguard.Player;

public interface ISaveService
{
  ISaveFile<GameData> GetSaveFile();
  ISaveChunk<GameData> GetGameChunk();
  void SaveGame();
  void LoadGame();
}

public class SaveOptions { }

public class SaveService : ISaveService
{
  private string _saveFilePath = Path.Join(OS.GetUserDataDir(), "game.json");

  private readonly IFileSystem _fileSystem;
  private readonly JsonSerializerOptions _jsonOptions;

  public SaveService()
  {
    var upgradeDependencies = new Blackboard();
    var resolver = new SerializableTypeResolver();

    _fileSystem = new FileSystem();
    _jsonOptions = new JsonSerializerOptions
    {
      Converters = { new SerializableTypeConverter(upgradeDependencies) },
      TypeInfoResolver = resolver,
      WriteIndented = true,
    };
  }

  public ISaveChunk<GameData> GetGameChunk() =>
    new SaveChunk<GameData>(
      (chunk) =>
      {
        var gameData = new GameData() { PlayerData = chunk.GetChunkSaveData<PlayerData>() };
        return gameData;
      },
      onLoad: (chunk, data) => chunk.LoadChunkSaveData(data.PlayerData)
    );

  public ISaveFile<GameData> GetSaveFile()
  {
    var gameChunk = GetGameChunk();
    return new SaveFile<GameData>(
      root: gameChunk,
      onSave: async (GameData data) =>
      {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var jsonasAString = json.ToString();
        await _fileSystem.File.WriteAllTextAsync(_saveFilePath, json);
      },
      onLoad: async () =>
      {
        // Load the game data from disk.
        if (!_fileSystem.File.Exists(_saveFilePath))
        {
          return null;
        }

        var json = await _fileSystem.File.ReadAllTextAsync(_saveFilePath);
        return JsonSerializer.Deserialize<GameData>(json, _jsonOptions);
      }
    );
  }

  public void LoadGame() => throw new System.NotImplementedException();

  public void SaveGame() => throw new System.NotImplementedException();
}
