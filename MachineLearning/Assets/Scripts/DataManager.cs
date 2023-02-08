using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;

[XmlRoot("Data")]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public        string      path;

    XmlSerializer serializer = new XmlSerializer(typeof(Data));
    Encoding      encoding   = Encoding.UTF8;

    void Awake()
    {
        instance = this;
        SetPath();
    }

    public void Save(List<NeuralNetwork> _nets, int generationCount)
    {
        StreamWriter streamWriter = new StreamWriter(path, false, encoding);
        Data         data         = new Data { nets = _nets, generationCount = generationCount};

        serializer.Serialize(streamWriter, data);
        streamWriter.Close();
    }

    public Data Load()
    {
        if (File.Exists(path))
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            Data data = serializer.Deserialize(fileStream) as Data; 
            fileStream.Close();
            return data;
        }

        return null;
    }

    void SetPath()
    {
        path = Path.Combine(Application.persistentDataPath, "Data.xml");
    }
    
    
}