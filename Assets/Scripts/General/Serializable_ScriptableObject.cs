using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializable_ScriptableObject : ScriptableObject
{
    /// <summary>
    /// Handles saving and loading of scriptable objects 
    /// [System.Serializable] [SerializeField] 
    /// Directory: C:/Users/<user>/AppData/LocalLow/<ProjectName>/Save 
    /// </summary>

    private readonly string directory = "Save";

    public bool IsSave(string fileName)
    {
        return File.Exists($"{Application.streamingAssetsPath}/{directory}/{fileName}");
    }
    public void Save(string fileName)
    {
        if (!Directory.Exists($"{Application.streamingAssetsPath}/{directory}"))
            Directory.CreateDirectory($"{Application.streamingAssetsPath}/{directory}");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.streamingAssetsPath}/{directory}/{fileName}");
        string json = JsonUtility.ToJson(this);
        bf.Serialize(file, json);
        file.Close();
    }
    public void Load(string fileName)
    {
        if (IsSave(fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open($"{Application.streamingAssetsPath}/{directory}/{fileName}", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), this);
            file.Close();
        }
    }
    public void Delete(string fileName)
    {
        File.Delete($"{Application.streamingAssetsPath}/{directory}/{fileName}");
    }
}