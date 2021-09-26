using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphOutput : MonoBehaviour
{
    [SerializeField] private ChartData chartData;

    [Range(0, 15)]
    public float L_width = 5f;

    [Range(0, 15)]
    public float M_width = 5f;

    [Range(2, 10)]
    public int nb__div = 5;

    [Range(0, 10)]
    public int HL_width = 1;

    public GameObject prefab_UP, prefab_DOWN, prefab_HORIZ;

    public bool isResult;

    public float xmax = float.MinValue;
    public float ymax = float.MinValue;
    public float xmin = float.MaxValue;
    public float ymin = float.MaxValue;

    List<Vector3> vMax = new List<Vector3>();
    List<Vector3> vMin = new List<Vector3>();
    List<Vector3> vOp = new List<Vector3>();
    List<Vector3> vCl = new List<Vector3>();

    float width;
    float height;

    GameObject line_container;
    GameObject Hline_container;

    List<GameObject> goMaxMin;
    List<GameObject> goOpenClose;
    GameObject[] horiz_line;

    private void Start()
    {
        width = transform.GetComponent<RectTransform>().rect.width;
        height = transform.GetComponent<RectTransform>().rect.height;

        vMax = new List<Vector3>(new Vector3[10]);
        vMin = new List<Vector3>(new Vector3[10]);
        vOp = new List<Vector3>(new Vector3[10]);
        vCl = new List<Vector3>(new Vector3[10]);

        goMaxMin = new List<GameObject>(new GameObject[10]);
        goOpenClose = new List<GameObject>(new GameObject[10]);

        line_container = GameObject.Find("lines");
        Hline_container = GameObject.Find("Hlines");

        horiz_line = new GameObject[nb__div + 1];

        if (isResult)
            PlotResult();
        else
            Plot();
    }

    public void Replot()
    {
        Clear();
        Plot();
    }
    
    public void Clear()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("line");

        for (int i = 0; i < lines.Length; i++)
        {
            Destroy(lines[i]);
        }        
    }

    public void Plot()
    {
        int start = chartData.YClosedValues[8] == 0 ? 0 : chartData.Xs.Count - 8;
        int end = chartData.YClosedValues[8] == 0 ? 8 : chartData.Xs.Count;

        for (int i = start; i < end; i++)
        {
            xmax = Mathf.Max(xmax, chartData.Xs[i]);
            ymax = Mathf.Max(ymax, chartData.YMaxValues[i]);

            xmin = start;
            ymin = Mathf.Min(ymin, chartData.YMinValues[i]);
        }

        float tf_FactorA, tf_FactorB;
        DrawHorizontalLines(out tf_FactorA, out tf_FactorB);

        float offset = 50;        

        for (int i = start, j = 0; i < end; i++, j++)
        {
            vMax[j] = new Vector3((chartData.Xs[j] * width / 8 - width / 2) - offset, chartData.YMaxValues[i] * tf_FactorA + tf_FactorB - height / 2, 0);
            vMin[j] = new Vector3((chartData.Xs[j] * width / 8 - width / 2) - offset, chartData.YMinValues[i] * tf_FactorA + tf_FactorB - height / 2, 0);
            vOp[j] = new Vector3((chartData.Xs[j] * width / 8 - width / 2) - offset, (chartData.YOpenValues[i] * tf_FactorA + tf_FactorB - height / 2), 0);
            vCl[j] = new Vector3((chartData.Xs[j] * width / 8 - width / 2) - offset, chartData.YClosedValues[i] * tf_FactorA + tf_FactorB - height / 2, 0);

            if (chartData.YClosedValues[i] >= chartData.YOpenValues[i])
            {
                goMaxMin[j] = Instantiate(prefab_UP, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                goMaxMin[j] = Instantiate(prefab_DOWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            }

            goMaxMin[j].transform.SetParent(line_container.transform);

            Vector3 dir = (vMax[j] - vMin[j]) / 2;

            goMaxMin[j].transform.localPosition = vMin[j] + dir;
            goMaxMin[j].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, L_width);
            goMaxMin[j].transform.right = dir;
            goMaxMin[j].transform.localScale = Vector3.one;

            if (chartData.YClosedValues[i] >= chartData.YOpenValues[i])
            {
                goOpenClose[j] = Instantiate(prefab_UP, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                goOpenClose[j].transform.SetParent(line_container.transform);

                dir = (vCl[j] - vOp[j]) / 2;

                goOpenClose[j].transform.localPosition = vOp[j] + dir;
                goOpenClose[j].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, M_width);
                goOpenClose[j].transform.right = dir;
                goOpenClose[j].transform.localScale = Vector3.one;
            }
            else
            {
                goOpenClose[j] = Instantiate(prefab_DOWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                goOpenClose[j].transform.SetParent(line_container.transform);

                dir = (vOp[j] - vCl[j]) / 2;

                goOpenClose[j].transform.localPosition = vCl[j] + dir;
                goOpenClose[j].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, M_width);
                goOpenClose[j].transform.right = dir;
            }
        }
    }

    private void DrawHorizontalLines(out float tf_FactorA, out float tf_FactorB)
    {
        tf_FactorA = height / (ymax - ymin);
        tf_FactorB = -ymin / (ymax - ymin) * height;
        for (int j = 0; j <= nb__div; j++)
        {
            horiz_line[j] = Instantiate(prefab_HORIZ, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            horiz_line[j].transform.SetParent(Hline_container.transform);

            float y_HLvalue = (float)j / nb__div * (ymax - ymin) + ymin;

            horiz_line[j].transform.localPosition = new Vector3(0, (y_HLvalue) * tf_FactorA + tf_FactorB - height / 2, 0);

            horiz_line[j].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(width, HL_width);
            horiz_line[j].transform.right = transform.right;
            horiz_line[j].transform.localScale = Vector3.one;

            horiz_line[j].transform.GetChild(0).GetComponent<Text>().text = "$" + Mathf.Round(y_HLvalue);
        }
    }

    public void PlotResult()
    {
        int start = 0;
        int end = chartData.Xs.Count;

        for (int i = start; i < end; i++)
        {
            xmax = Mathf.Max(xmax, chartData.Xs[i]);
            ymax = Mathf.Max(ymax, chartData.YMaxValues[i]);

            xmin = start;
            ymin = Mathf.Min(ymin, chartData.YMinValues[i]);
        }

        float tf_FactorA, tf_FactorB;
        DrawHorizontalLines(out tf_FactorA, out tf_FactorB);

        vMax = new List<Vector3>(new Vector3[chartData.Xs.Count]);
        vMin = new List<Vector3>(new Vector3[chartData.Xs.Count]);
        vOp = new List<Vector3>(new Vector3[chartData.Xs.Count]);
        vCl = new List<Vector3>(new Vector3[chartData.Xs.Count]);

        goMaxMin = new List<GameObject>(new GameObject[chartData.Xs.Count]);
        goOpenClose = new List<GameObject>(new GameObject[chartData.Xs.Count]);

        float offset = 50;

        for (int i = start; i < end; i++)
        {
            vMax[i] = new Vector3((chartData.Xs[i] * width / xmax - width / 2) - offset, chartData.YMaxValues[i] * tf_FactorA + tf_FactorB - height / 2, 0);
            vMin[i] = new Vector3((chartData.Xs[i] * width / xmax - width / 2) - offset, chartData.YMinValues[i] * tf_FactorA + tf_FactorB - height / 2, 0);
            vOp[i] = new Vector3((chartData.Xs[i] * width / xmax - width / 2) - offset, (chartData.YOpenValues[i] * tf_FactorA + tf_FactorB - height / 2), 0);
            vCl[i] = new Vector3((chartData.Xs[i] * width / xmax - width / 2) - offset, chartData.YClosedValues[i] * tf_FactorA + tf_FactorB - height / 2, 0);

            if (chartData.YClosedValues[i] >= chartData.YOpenValues[i])
            {
                goMaxMin[i] = Instantiate(prefab_UP, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                goMaxMin[i] = Instantiate(prefab_DOWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            }

            goMaxMin[i].transform.SetParent(line_container.transform);

            Vector3 dir = (vMax[i] - vMin[i]) / 2;

            goMaxMin[i].transform.localPosition = vMin[i] + dir;
            goMaxMin[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, L_width);
            goMaxMin[i].transform.right = dir;
            goMaxMin[i].transform.localScale = Vector3.one;

            if (chartData.YClosedValues[i] >= chartData.YOpenValues[i])
            {
                goOpenClose[i] = Instantiate(prefab_UP, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                goOpenClose[i].transform.SetParent(line_container.transform);

                dir = (vCl[i] - vOp[i]) / 2;

                goOpenClose[i].transform.localPosition = vOp[i] + dir;
                goOpenClose[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, M_width);
                goOpenClose[i].transform.right = dir;
                goOpenClose[i].transform.localScale = Vector3.one;
            }
            else
            {
                goOpenClose[i] = Instantiate(prefab_DOWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                goOpenClose[i].transform.SetParent(line_container.transform);

                dir = (vOp[i] - vCl[i]) / 2;

                goOpenClose[i].transform.localPosition = vCl[i] + dir;
                goOpenClose[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, M_width);
                goOpenClose[i].transform.right = dir;
            }
        }
    }
}