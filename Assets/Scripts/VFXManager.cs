using TMPro;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    [SerializeField] private GameObject toastPrefab;
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        Instantiate(playerPrefab);
    }

    public void CreateToast(string text, Vector3 pos)
    {
        CreateToast(text, pos, Color.white);
    }

    public void CreateToast(string text, Vector3 pos, Color color)
    {
        var toast = Instantiate(toastPrefab, pos, Quaternion.identity);
        var tmp = toast.GetComponent<TextMeshPro>();
        tmp.text = text;
        tmp.color = color;
    }
}
