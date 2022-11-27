using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class SaveStates
{
    public static void SetKey(string path, string key)
    {
        if (key.Length == 0)
        { DeleteKey(path); return; }

        
        if (key.ToCharArray()[0] == ' ')
            key = key.Remove(0, 1);
        var bytes = System.Text.Encoding.UTF8.GetBytes(key);
        path = Application.persistentDataPath + path + ".dat";
        if (!Directory.Exists(path.Remove(path.LastIndexOf("/")))) Directory.CreateDirectory(path.Remove(path.LastIndexOf("/")));
        FileStream file = File.Open(path, FileMode.Create);
        file.Write(bytes);
        file.Close();
       // StreamWriter writer = new StreamWriter(path, false);
       // writer.WriteLine(key);
       // writer.Close();
    }
    public static string GetKey(string path)
    {
        if (File.Exists(Application.persistentDataPath + path + ".dat"))
        {
            StreamReader reader = new StreamReader(Application.persistentDataPath + path + ".dat");
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }
        else return "";
    }
    public static bool GetKey(string path, out string result)
    {
        result = "";
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        if (File.Exists(Application.persistentDataPath + path + ".dat"))
        {
            sw.Stop();
            UnityEngine.Debug.Log("if took: " + sw.ElapsedMilliseconds);
            sw.Restart();
            StreamReader reader = new StreamReader(Application.persistentDataPath + path + ".dat");
            string str = reader.ReadToEnd();
            reader.Close();
            result = str;
            sw.Stop();
            UnityEngine.Debug.Log("rest took: "+sw.ElapsedMilliseconds);
            return true;
        }
        else return false;
    }

    public static void DeleteKey(string path)
    {
        File.Delete(Application.persistentDataPath + path + ".dat");
    }
    public static void DeleteWorld(string name)
    {
        if (HasWorld(name))
            Directory.Delete(Application.persistentDataPath + "/Saves/Save-" + name, true); ;
        if (Directory.GetDirectories(Application.persistentDataPath + "/Saves/").Length == 0)
            DeleteKey("/Worlds");
    }
    public static bool HasWorld(string name)
    {
        return Directory.Exists(Application.persistentDataPath + "/Saves/Save-" + name);
    }
}
