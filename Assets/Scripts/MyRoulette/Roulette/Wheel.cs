using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Wheel : MonoBehaviour
{
    [SerializeField] int eyeNum = 6;

    private const string EYE_RESOURCE_PATH = "Eye";
    private const int CIRCLE_ANGLE = 360;

    private IRollable rollableTarget = null;
    private List<Eye> eyeList = new List<Eye>();
    private int closestNumber = 0;

    public ReactiveProperty<RollState> State { get; } = new ReactiveProperty<RollState>(RollState.None);
    public IntReactiveProperty ClosestNumber = new IntReactiveProperty(0);

    public void Run()
    {
        rollableTarget.Roll();
    }

    // Start is called before the first frame update
    void Start()
    {
        rollableTarget = transform.GetComponentInChildren<IRollable>();
        rollableTarget.State
            .Where(x => x != State.Value)
            .Subscribe(x => State.Value = x);

        for (var i = 0; i < eyeNum; i++)
        {
            var eyeObj = Instantiate((GameObject)Resources.Load(EYE_RESOURCE_PATH));
            eyeObj.transform.parent = transform;

            var eye = eyeObj.GetComponent<Eye>();
            eye.Initialize(
                number: (i + 1) * 10,
                angle: CIRCLE_ANGLE / eyeNum * i
            );
            eyeList.Add(eye);
        }

        rollableTarget.Position
            .Subscribe(x =>
            {
                ClosestNumber.Value = eyeList
                    .OrderBy(y => (x - y.Position).sqrMagnitude)
                    .First()
                    .Number.Value;
            });

        Debug.Log("Wheel Start End");
    }
}
