using UnityEngine;

public class GraphSize : MonoBehaviour
{
    [SerializeField] private RectTransform chartTransform;
    [SerializeField] private ChartData chartData;

    private void OnEnable()
    {
        chartTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, chartData.Xs.Count*80);
        chartTransform.position = new Vector3(-1500000, Screen.height/2, 0);
    }
}
