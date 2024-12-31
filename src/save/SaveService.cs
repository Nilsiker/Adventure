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

public interface ISaveService
{
  Task Save();
}

public interface ILoadService
{
  Task Load();
  bool GameFileExists();
}

public interface IGameFileService : ISaveService, ILoadService
{
  void SelectGameFile(int slot);
}

public class GameFileService<T> : IGameFileService
  where T : class
{
  private readonly ISaveChunk<T> _saveChunk = default!;
  private readonly IFileSystem _fileSystem;
  private readonly JsonSerializerOptions _jsonOptions;

  private int _slot;
  private SaveFile<T> _saveFile = default!;

  public GameFileService(ISaveChunk<T> chunk)
  {
    var upgradeDependencies = new Blackboard();
    var resolver = new SerializableTypeResolver();
    GodotSerialization.Setup();

    _saveChunk = chunk;
    _fileSystem = new FileSystem();
    _jsonOptions = new JsonSerializerOptions
    {
      Converters = { new SerializableTypeConverter(upgradeDependencies) },
      TypeInfoResolver = resolver,
      WriteIndented = true,
    };

    SelectGameFile(_slot);
  }

  public bool GameFileExists() => File.Exists(GameFileService<T>.GetSaveFilePath(_slot));

  public Task Load() => _saveFile.Load();

  public Task Save() => _saveFile.Save();

  public void SelectGameFile(int slot)
  {
    _slot = slot;

    var path = GameFileService<T>.GetSaveFilePath(_slot);

    _saveFile = new SaveFile<T>(
      root: _saveChunk,
      onSave: async (T data) =>
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
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
      }
    );
  }

  private static string GetSaveFilePath(int slot) =>
    Path.Join(OS.GetUserDataDir(), $"game{slot}.json");
}
