using UnityEngine;
using System.Collections.Generic;

public class GraphInput : MonoBehaviour
{
    [SerializeField] private ChartData chartData;
    [SerializeField] private GraphOutput graphOutput;

    private int currentValueCount;
    private bool lul = true;

    private void OnEnable()
    {
        chartData.YClosedValues = new List<float>(new float[9]);
        chartData.YMaxValues = new List<float>(new float[9]);
        chartData.YMinValues = new List<float>(new float[9]);
        chartData.YOpenValues = new List<float>(new float[9]);
        chartData.Xs = new List<float>(new float[9]);

        for(int i=0; i<chartData.Xs.Count;i++)
        {
            chartData.Xs[i] = i+1;
        }

        chartData.YClosedValues[0] = 1000;
        chartData.YOpenValues[0] = 0;
        chartData.YMinValues[0] = 0;
        chartData.YMaxValues[0] = 1050;

        for (int i = 1; i < chartData.Xs.Count; i++)
        {
            chartData.YClosedValues[i] = 0;
            chartData.YOpenValues[i] = 0;
            chartData.YMinValues[i] = 0;
            chartData.YMaxValues[i] = 0;
        }
    }

    public void MakeStep(int value)
    {
        if (currentValueCount > chartData.Xs.Count - 2 || !lul)
        {
            chartData.YClosedValues.Add(0);
            chartData.YOpenValues.Add(0);
            chartData.YMinValues.Add(0);
            chartData.YMaxValues.Add(0);
            chartData.Xs.Add(chartData.Xs.Count + 1);

            GenerateNewData(chartData.Xs.Count - 1, value);
            lul = false;
        }
        else
        {
            currentValueCount++;
            GenerateNewData(currentValueCount, value);
        }


        graphOutput.Replot();
    }

    private void GenerateNewData(int index, int value)
    {
        if (value > 0)
        {
            int random = Random.Range(180, 220);
            chartData.YOpenValues[index] = chartData.YClosedValues[index - 1];
            chartData.YClosedValues[index] = chartData.YOpenValues[index] + random;


            float delta = Mathf.Abs(chartData.YOpenValues[index] - chartData.YClosedValues[index]);
            chartData.YMinValues[index] = chartData.YOpenValues[index] - Random.Range(0.1f, 0.3f) * delta;
            chartData.YMaxValues[index] = chartData.YClosedValues[index] + Random.Range(0.1f, 0.3f) * delta;

            if (chartData.YMinValues[index] < 0)
            {
                chartData.YMinValues[index] = 0;
            }
        }
        else if (value < 0)
        {
            int random = Random.Range(180, 220);
            chartData.YOpenValues[index] = chartData.YClosedValues[index - 1];
            chartData.YClosedValues[index] = chartData.YOpenValues[index] - random;
            chartData.YClosedValues[index] = Mathf.Max(0, chartData.YClosedValues[index]);

            float delta = Mathf.Abs(chartData.YOpenValues[index] - chartData.YClosedValues[index]);
            chartData.YMinValues[index] = chartData.YOpenValues[index] + Random.Range(0.1f, 0.3f) * delta;
            chartData.YMaxValues[index] = chartData.YClosedValues[index] - Random.Range(0.1f, 0.3f) * delta;

            if(chartData.YClosedValues[index] <= 0)
            {
                AudioPlayer.instance.PlayLose();
                SceneManager.instance.LoadLoseScene();
                chartData.YClosedValues[index] = 0;
            }

            if(chartData.YMaxValues[index] < 0)
            {
                chartData.YMaxValues[index] = 0;
            }
        }
        else
        {
            int random = Random.Range(120, 150);
            chartData.YOpenValues[index] = chartData.YClosedValues[index - 1];
            chartData.YClosedValues[index] = chartData.YOpenValues[index] - random;
            chartData.YClosedValues[index] = Mathf.Max(0, chartData.YClosedValues[index]);

            float delta = Mathf.Abs(chartData.YOpenValues[index] - chartData.YClosedValues[index]);
            chartData.YMinValues[index] = chartData.YOpenValues[index] + Random.Range(0.1f, 0.3f) * delta;
            chartData.YMaxValues[index] = chartData.YClosedValues[index] - Random.Range(0.1f, 0.3f) * delta;

            if (chartData.YClosedValues[index] <= 0)
            {
                AudioPlayer.instance.PlayLose();
                SceneManager.instance.LoadLoseScene();
                chartData.YClosedValues[index] = 0;
            }

            if (chartData.YMaxValues[index] < 0)
            {
                chartData.YMaxValues[index] = 0;
            }
        }
    }
}
