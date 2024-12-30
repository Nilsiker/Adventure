namespace Shellguard.Save;

using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using Chickensoft.Collections;
using Chickensoft.SaveFileBuilder;
using Chickensoft.Serialization;
using Godot;
using Shellguard.Game;

public interface ISaveService
{
  Task Save();
  Task Load();
}

public class SaveService : ISaveService
{
  private readonly string _saveFilePath;
  private readonly IFileSystem _fileSystem;
  private readonly JsonSerializerOptions _jsonOptions;
  private readonly SaveFile<GameData> _saveFile;

  public SaveService(ISaveChunk<GameData> chunk)
  {
    var upgradeDependencies = new Blackboard();
    var resolver = new SerializableTypeResolver();

    _saveFilePath = Path.Join(OS.GetUserDataDir(), "game.json");
    _fileSystem = new FileSystem();
    _jsonOptions = new JsonSerializerOptions
    {
      Converters = { new SerializableTypeConverter(upgradeDependencies) },
      TypeInfoResolver = resolver,
      WriteIndented = true,
    };

    _saveFile = new SaveFile<GameData>(
      root: chunk,
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

  public Task Load() => _saveFile.Load();

  public Task Save() => _saveFile.Save();
}
