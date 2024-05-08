using UnityEngine;

// Structure for basic color data
public class ColorData : ScriptableObject
{
    public new string name { get; set; }
    public float r { get; set; }
    public float g { get; set; }
    public float b { get; set; }
    public float a { get; set; }
    public string hide_flags { get; set; }

    public ColorData() { }

    public ColorData(string name, float r, float g, float b, float a)
    {
        this.name = name;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}