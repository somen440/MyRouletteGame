using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Eye : MonoBehaviour
{
    public IntReactiveProperty Number { get; private set; } = new IntReactiveProperty(0);
    public Vector3 Position => transform.position;

    public void Initialize(int number, int angle)
    {
        Number.Value = number;
        transform.RotateAround(
            transform.root.position,
            Vector3.forward,
            angle
        );
    }

    void Start()
    {
        var text = GetComponentInChildren<Text>();
        Number.Subscribe(x =>
        {
            text.text = x.ToString();
        });
        Debug.Log("eye created");
    }
}
