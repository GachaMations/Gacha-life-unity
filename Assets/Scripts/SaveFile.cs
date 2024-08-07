using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class SaveFile : MonoBehaviour
{
    public string GUPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GachaUnity";
    public string SaveDirectory = "";
    void Awake() {
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            GUPath = Application.persistentDataPath;
        } else {
            GUPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GachaUnity";
        }
        SaveDirectory = GUPath+"\\Life";
    }
    public void Save(string path, string varName, string varData) {
        string purePath = SaveDirectory+"\\"+path;
        if (!File.Exists(purePath)) {
            Directory.CreateDirectory(Path.GetDirectoryName(purePath));
            using (StreamWriter file = File.CreateText(purePath))
            {
                file.WriteLine("{");
                file.WriteLine("\t\""+varName+"\": \""+varData+"\",");
                file.WriteLine("}");
            }
        }
        else
        {
            string rewritten = "";
            bool foundKey = false;
            foreach (var line in File.ReadAllLines(purePath)) {
                if (line.StartsWith("\t\""+varName+"\": \"")) {
                    rewritten+="\t\""+varName+"\": \""+varData+"\",\n";
                    foundKey = true;
                } else {
                    if (line == "}" && !foundKey) {
                        rewritten+="\t\""+varName+"\": \""+varData+"\",\n}";
                    } else rewritten+=line+"\n";
                }
            }
            File.WriteAllText(purePath, rewritten);
        }
    }
    public string Load(string path, string varName) {
        string purePath = SaveDirectory+"\\"+path;
        try {
            if (File.Exists(purePath)) {
                if (JObject.Parse(File.ReadAllText(purePath))[varName] == null) return "";
                return (string)JObject.Parse(File.ReadAllText(purePath))[varName];
            }
            else return "";
        }
        catch (Exception) {
            File.Delete(purePath);
            Save(path, varName, "");
            return "";
        }
    }

    void OnEnable() { if (!GameObject.Find("File") || GameObject.Find("File") == gameObject) DontDestroyOnLoad(gameObject); else Destroy(gameObject); }
}
