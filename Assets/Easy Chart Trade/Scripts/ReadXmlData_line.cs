using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReadXmlData_line : MonoBehaviour
{
    // Use this for initialization

    //this is the name of the file
    public string DataStr;
    
    // line parameters
    [Range(0,15)]
    public float L_width = 5f;

    [Range(0, 15)]
    public float M_width = 5f;

    [Range(2, 10)]
    public int nb__div = 5;

    [Range(0, 10)]
    public int HL_width = 1;




    // line prefab for UP and DOWN behaviour
    public GameObject prefab_UP, prefab_DOWN, prefab_HORIZ;
        
    //this is the list for the diccionary of x,y values
    public List<Dictionary<string, string>> allTextDic;

    //these two arrays contain the values for the line chart
    public float[] x_value;
    public float[] y_Open_value;
    public float[] y_Close_value;
    public float[] y_min_value;
    public float[] y_Max_value;

    //maximum values for x and y
    public float xmax, ymax, xmin, ymin;


    //this is the number of point to plot;
    public int nb_points;

    // in the start function the data is loaded and the chart is drawn
    void Start()
    {
        
        allTextDic = parseFile();

        //we use float values
        x_value = new float[allTextDic.Count];
        y_Open_value = new float[allTextDic.Count];
        y_Close_value = new float[allTextDic.Count];
        y_min_value = new float[allTextDic.Count];
        y_Max_value = new float[allTextDic.Count];


        int i = 0;

        xmax = -100000000;
        ymax = -100000000;

        xmin = 100000000;
        ymin = 100000000;

        nb_points = allTextDic.Count;

        GameObject.FindGameObjectWithTag("subtitle").GetComponent<Text>().text = "" + nb_points + " ticks";

        while ( i < nb_points)
        {
            Dictionary<string, string> dic = allTextDic[i];

            x_value[i]= float.Parse(dic["x"]);
            y_Open_value[i]= float.Parse(dic["yo"]);
            y_Close_value[i] = float.Parse(dic["yc"]);
            y_min_value[i] = float.Parse(dic["ym"]);
            y_Max_value[i] = float.Parse(dic["yM"]);

            xmax = Mathf.Max(xmax, x_value[i]);
            ymax = Mathf.Max(ymax, y_Max_value[i]);

            xmin = Mathf.Min(xmin, x_value[i]);
            ymin = Mathf.Min(ymin, y_min_value[i]);

            i++;
        }


        plotChart();

    }


    // this function is called when a variable is changed in the inspector
    public void replot()
    {
        clearChart();
        plotChart();

    }


    //deletes the chart data
    public void clearChart()
    {

        // get containers for the lines and the markers
        GameObject[] lines = GameObject.FindGameObjectsWithTag("line");

        for (int i = 0; i < lines.Length; i++)
        {
            Destroy(lines[i]);
        }
        
    }

    //draws the chart
    public void plotChart()
    {

        //graphic parameters
        Vector2 sizeChart=transform.GetComponent<RectTransform>().sizeDelta;

        //size of the chart in two components
        float a = sizeChart[0];
        float b = sizeChart[1];


        // get containers for the lines and the markers
        GameObject line_container = GameObject.Find("lines");
        GameObject Hline_container = GameObject.Find("Hlines");


        //reference to the gameobjects used in the plot
        Vector3[] vMax = new Vector3[nb_points];
        Vector3[] vmin = new Vector3[nb_points];
        Vector3[] vop = new Vector3[nb_points];
        Vector3[] vcl = new Vector3[nb_points];

        GameObject[] goMaxMin = new GameObject[nb_points]; 
        GameObject[] goOpenClose = new GameObject[nb_points]; 
        GameObject[] horiz_line = new GameObject[nb__div+1]; 


        float tf_FactorA = b / (ymax-ymin);
        float tf_FactorB = -ymin / (ymax - ymin)*b;


        
        // create horizontal lines
        for (int j = 0; j <=nb__div; j++)
        {
            horiz_line[j] = GameObject.Instantiate(prefab_HORIZ, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            horiz_line[j].transform.SetParent(Hline_container.transform);

            float y_HLvalue = (float)j/nb__div * (ymax - ymin) + ymin;

            horiz_line[j].transform.localPosition = new Vector3(0, (y_HLvalue)* tf_FactorA + tf_FactorB - b / 2,0);

            horiz_line[j].transform.GetComponent<RectTransform>().sizeDelta = new Vector2( a, HL_width);
            horiz_line[j].transform.right = transform.right;

            // set text with the value
            horiz_line[j].transform.GetChild(0).GetComponent<Text>().text = "$" + Mathf.Round(y_HLvalue);

        }



        for (int i=0; i<nb_points;i++)
        {
            //get maximum point
            vMax[i] = new Vector3(x_value[i]*a/xmax-a/2, y_Max_value[i]*tf_FactorA+tf_FactorB-b/2,0);
            
            //get the minimum point
            vmin[i] = new Vector3(x_value[i] * a / xmax - a / 2, y_min_value[i] *tf_FactorA +tf_FactorB- b / 2,0);

            //get the open point
            vop[i] = new Vector3(x_value[i] * a / xmax - a / 2, y_Open_value[i] * tf_FactorA + tf_FactorB - b / 2, 0);

            //get the close point
            vcl[i] = new Vector3(x_value[i] * a / xmax - a / 2, y_Close_value[i] * tf_FactorA + tf_FactorB - b / 2, 0);


            
            //instantiate a line between max and min markers
            if (y_Close_value[i]>=y_Open_value[i])
            {
                goMaxMin[i] = GameObject.Instantiate(prefab_UP, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                goMaxMin[i] = GameObject.Instantiate(prefab_DOWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            }

            //set the text to the limits
            goMaxMin[i].transform.GetChild(2).GetComponent<Text>().text = "$"+ Mathf.Round(y_Max_value[i]);
            goMaxMin[i].transform.GetChild(3).GetComponent<Text>().text = "$" + Mathf.Round(y_min_value[i]);


            goMaxMin[i].transform.SetParent(line_container.transform);

            Vector3 dir = (vMax[i] - vmin[i]) / 2;

            goMaxMin[i].transform.localPosition = vmin[i] + dir;
            goMaxMin[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2*dir.magnitude, L_width);
            goMaxMin[i].transform.right = dir;


            //instantiate a line between open and close markers
            if (y_Close_value[i] >= y_Open_value[i])
            {
                goOpenClose[i] = GameObject.Instantiate(prefab_UP, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

                goOpenClose[i].transform.SetParent(line_container.transform);

                dir = (vcl[i] - vop[i]) / 2;

                goOpenClose[i].transform.localPosition = vop[i] + dir;
                goOpenClose[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, M_width);
                goOpenClose[i].transform.right = dir;


                //set the text to the limits
                goOpenClose[i].transform.GetChild(2).GetComponent<Text>().text = "$" + Mathf.Round(y_Close_value[i]);
                goOpenClose[i].transform.GetChild(3).GetComponent<Text>().text = "$" + Mathf.Round(y_Open_value[i]);


            }
            else   //invert direction
            {
                goOpenClose[i] = GameObject.Instantiate(prefab_DOWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));


                goOpenClose[i].transform.SetParent(line_container.transform);

                dir = (vop[i] - vcl[i]) / 2;

                goOpenClose[i].transform.localPosition = vcl[i] + dir;
                goOpenClose[i].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2 * dir.magnitude, M_width);
                goOpenClose[i].transform.right = dir;


                //set the text to the limits
                goOpenClose[i].transform.GetChild(2).GetComponent<Text>().text = "$" + Mathf.Round(y_Open_value[i]);
                goOpenClose[i].transform.GetChild(3).GetComponent<Text>().text = "$" + Mathf.Round(y_Close_value[i]);
            }
            
            

        }


    }

    
    //reads the xml document
    public List<Dictionary<string, string>> parseFile()
    {
        TextAsset txtXmlAsset = Resources.Load<TextAsset>(DataStr);
        var doc = XDocument.Parse(txtXmlAsset.text);


        //get the xml data points
        var allDict = doc.Element("data").Elements("point");
        List<Dictionary<string, string>> allTextDic = new List<Dictionary<string, string>>();
        foreach (var oneDict in allDict)
        {
            var firstX = oneDict.Elements("x");
            var Yo = oneDict.Elements("yo");
            var Yc = oneDict.Elements("yc");
            var Ym = oneDict.Elements("ym");
            var YM = oneDict.Elements("yM");

            XElement element1 = firstX.ElementAt(0);
            XElement element2 = Yo.ElementAt(0);
            XElement element3 = Yc.ElementAt(0);
            XElement element4 = Ym.ElementAt(0);
            XElement element5 = YM.ElementAt(0);

            //these are the two x and y points
            string first = element1.ToString().Replace("<x>", "").Replace("</x>", "");
            string second = element2.ToString().Replace("<yo>", "").Replace("</yo>", "");
            string third = element3.ToString().Replace("<yc>", "").Replace("</yc>", "");
            string forth = element4.ToString().Replace("<ym>", "").Replace("</ym>", "");
            string fifth = element5.ToString().Replace("<yM>", "").Replace("</yM>", "");

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("x", first);
            dic.Add("yo", second);
            dic.Add("yc", third);
            dic.Add("ym", forth);
            dic.Add("yM", fifth);

            allTextDic.Add(dic);
        }

        return allTextDic;

       

    }

}