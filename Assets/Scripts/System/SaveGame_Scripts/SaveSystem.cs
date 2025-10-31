using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography; 

public static class SaveSystem
{
    private static string _saveFileName = "user.cfg"; 
    private static byte[] _cryptoKey = System.Text.Encoding.UTF8.GetBytes("MySuperSecretKey"); 
    private static byte[] _cryptoIV = System.Text.Encoding.UTF8.GetBytes("1234567890123456"); 

    
    public static void SaveGame(GameData data)
    {
        string path = Path.Combine(Application.persistentDataPath, _saveFileName);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        using (Aes aes = Aes.Create())
        using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(_cryptoKey, _cryptoIV), CryptoStreamMode.Write))
        using (BinaryWriter writer = new BinaryWriter(cryptoStream))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(cryptoStream, data);
        }

    }

    
    public static GameData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, _saveFileName);

        if (!File.Exists(path))
        {
            return null;
        }

        GameData data;
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            using (Aes aes = Aes.Create())
            using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(_cryptoKey, _cryptoIV), CryptoStreamMode.Read))
            using (BinaryReader reader = new BinaryReader(cryptoStream))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                data = (GameData)formatter.Deserialize(cryptoStream);
            }
        }
        catch (System.Exception)
        {
            return null; 
        }

        return data;
    }
}