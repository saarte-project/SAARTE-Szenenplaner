using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// This class has a list to add multiple objects.
/// The object is from the class ObjectData.
/// </summary>
[XmlRoot("ObjectDataLists")]
[XmlInclude(typeof(ObjectData))]

/// <summary>
/// DataList for XML serialization
/// </summary>
public class ObjectDataList
{
    [XmlArray("ObjectDatasArray")]
    [XmlArrayItem("ObjectData")]
    public List<ObjectData> objectDatas = new List<ObjectData>();

    [XmlElement("Listname")]
    public string Listname { get; set; }

    public ObjectDataList() { }

    public ObjectDataList(string name)
    {
        this.Listname = name;
    }

    public void AddGameObject(ObjectData objectData)
    {
        objectDatas.Add(objectData);
    }

}


/// <summary>
/// ObjectData defines several attributes,
/// to serialize/deserialize the gameObject.
/// </summary>
[XmlType("ObjectData")]
public class ObjectData
{
    [XmlElement(Namespace = "Object Name")]
    public string objectName;
    [XmlElement(Namespace = "Object Tag")]
    public string tag;
    [XmlElement(Namespace = "Position X-Axis")]
    public float positionX;
    [XmlElement(Namespace = "Position Y-Axis")]
    public float positionY;
    [XmlElement(Namespace = "Position Z-Axis")]
    public float positionZ;
    [XmlElement(Namespace = "Rotation X-Axis")]
    public float rotationX;
    [XmlElement(Namespace = "Rotation Y-Axis")]
    public float rotationY;
    [XmlElement(Namespace = "Rotation Z-Axis")]
    public float rotationZ;
    [XmlElement(Namespace = "Scale X-Axis")]
    public float scaleX;
    [XmlElement(Namespace = "Scale Y-Axis")]
    public float scaleY;
    [XmlElement(Namespace = "Scale Z-Axis")]
    public float scaleZ;

    [XmlElement(Namespace = "Object Head")]
    public bool hasHead;
    [XmlElement(Namespace = "Head Position X-Axis")]
    public float childPositionX;
    [XmlElement(Namespace = "Head Position Y-Axis")]
    public float childPositionY;
    [XmlElement(Namespace = "Head Position Z-Axis")]
    public float childPositionZ;
    [XmlElement(Namespace = "Head Rotation X-Axis")]
    public float childRotationX;
    [XmlElement(Namespace = "Head Rotation Y-Axis")]
    public float childRotationY;
    [XmlElement(Namespace = "Head Rotation Z-Axis")]
    public float childRotationZ;
    [XmlElement(Namespace = "Head Scale X-Axis")]
    public float childScaleX;
    [XmlElement(Namespace = "Head Scale Y-Axis")]
    public float childScaleY;
    [XmlElement(Namespace = "Head Scale Z-Axis")]
    public float childScaleZ;

    [XmlElement(Namespace = "Note Text")]
    public string noteText;

    public ObjectData()
    {

    }

    public ObjectData(string objectName, string tag, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float scaleX, float scaleY, float scaleZ,
            bool hasHead, float dataChildPositionX, float dataChildPositionY, float dataChildPositionZ, float dataChildRotationX, float dataChildRotationY, float dataChildRotationZ, float dataChildScaleX, float dataChildScaleY, float dataChildScaleZ,
             string dataNoticeText)
    {
        this.objectName = objectName;
        this.tag = tag;
        this.positionX = positionX;
        this.positionY = positionY;
        this.positionZ = positionZ;
        this.rotationX = rotationX;
        this.rotationY = rotationY;
        this.rotationZ = rotationZ;
        //kann weg
        this.scaleX = scaleX;
        this.scaleY = scaleY;
        this.scaleZ = scaleZ;
        this.hasHead = hasHead;
        this.childPositionX = dataChildPositionX;
        this.childPositionY = dataChildPositionY;
        this.childPositionZ = dataChildPositionZ;
        this.childRotationX = dataChildRotationX;
        this.childRotationY = dataChildRotationY;
        this.childRotationZ = dataChildRotationZ;
        this.childScaleX = dataChildScaleX;
        this.childScaleY = dataChildScaleY;
        this.childScaleZ = dataChildScaleZ;
        this.noteText = dataNoticeText;
    }
}
