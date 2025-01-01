namespace Shellguard.Save;

using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using Chickensoft.Collections;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
using Chickensoft.Serialization.Godot;
using Godot;
using Shellguard.Game;

public interface ISaveService
{
  ISaveChunk<GameData> Chunk { get; set; }
  Task Save();
}

public interface ILoadService
{
  Task Load();
  bool GameFileExists(int slot);
}

public interface IGameFileService : ISaveService, ILoadService
{
  void SelectGameFile(int slot);
}

public class GameFileService : IGameFileService
{
  public ISaveChunk<GameData> Chunk { get; set; } = default!;
  private readonly IFileSystem _fileSystem;
  private readonly JsonSerializerOptions _jsonOptions;

  private SaveFile<GameData> _saveFile = default!;

  public GameFileService()
  {
    var upgradeDependencies = new Blackboard();
    var resolver = new SerializableTypeResolver();
    GodotSerialization.Setup();

    _fileSystem = new FileSystem();
    _jsonOptions = new JsonSerializerOptions
    {
      Converters = { new SerializableTypeConverter(upgradeDependencies) },
      TypeInfoResolver = resolver,
      WriteIndented = true,
    };
  }

  public bool GameFileExists(int slot) => File.Exists(GetSaveFilePath(slot));

  public Task Load() => _saveFile.Load();

  public Task Save() => _saveFile.Save();

  public void SelectGameFile(int slot)
  {
    var path = GetSaveFilePath(slot);

    _saveFile = new SaveFile<GameData>(
      root: Chunk,
      onSave: async (GameData data) =>
      {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var jsonasAString = json.ToString();
        await _fileSystem.File.WriteAllTextAsync(path, json);
      },
      onLoad: async () =>
      {
        if (!_fileSystem.File.Exists(path))
        {
          return null;
        }

        var json = await _fileSystem.File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<GameData>(json, _jsonOptions);
      }
    );
  }

  private static string GetSaveFilePath(int slot) =>
    Path.Join(OS.GetUserDataDir(), $"game{slot}.json");
}
