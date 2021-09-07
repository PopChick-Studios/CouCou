using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveInventory(InventoryList inventory)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/SaveFiles/" + inventory.name + ".save";

        FileStream stream = new FileStream(path, FileMode.Create);

        InventoryData data = new InventoryData(inventory);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static InventoryData LoadInventory(InventoryList inventory)
    {
        string path = Application.persistentDataPath + "/SaveFiles/" + inventory.name + ".save";
        string directoryPath = Application.persistentDataPath + "/SaveFiles/";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            InventoryData data = formatter.Deserialize(stream) as InventoryData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteAllSaveFiles()
    {
        string path = Application.persistentDataPath + "/SaveFiles/";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
    }
}
