using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Chart Data")]
public class ChartData : ScriptableObject
{
    public List<float> Xs = new List<float>(new float[9]);
    public List<float> YOpenValues = new List<float>(new float[9]);
    public List<float> YClosedValues = new List<float>(new float[9]);
    public List<float> YMinValues = new List<float>(new float[9]);
    public List<float> YMaxValues = new List<float>(new float[9]);
}
