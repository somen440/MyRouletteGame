using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Ball : MonoBehaviour, IRollable
{
    // [SerializeField] について
    // ・ Unity Editor から弄りたい VS クラスとして外部公開したくない
    // @see https://qiita.com/makopo/items/8ef280b00f1cc18aec91

    /// <summary>
    /// 回転する角度のベース
    /// </summary>
    [SerializeField] float angle = 2.0f;

    /// <summary>
    /// 回転状態
    /// </summary>
    public ReactiveProperty<RollState> State { get; } = new ReactiveProperty<RollState>(RollState.None);

    /// <summary>
    /// 位置
    /// </summary>
    public ReactiveProperty<Vector3> Position { get; } = new ReactiveProperty<Vector3>();

    /// <summary>
    /// 回転するフレーム数
    /// </summary>
    private FloatReactiveProperty rollCount = new FloatReactiveProperty(5.0f);

    /// <summary>
    /// 親を起点としてぐるぐる回す
    /// </summary>
    public void Roll()
    {
        State.Value = RollState.Rolling;
    }

    void Start()
    {
        rollCount.Value = (new System.Random()).Next(50, 150) / 10;
        rollCount.Where(x => x == 0)
            .Subscribe(x =>
            {
                State.Value = RollState.Stoped;
                rollCount.Value = (new System.Random()).Next(50, 150) / 10;
            });
        Position.Value = transform.position;

        Debug.Log("Ball Start End");
    }

    void Update()
    {
        if (State.Value != RollState.Rolling)
        {
            return;
        }
        transform.RotateAround(
            transform.root.position,
            Vector3.forward,
            angle * rollCount.Value
        );
        rollCount.Value = Mathf.Max(0, rollCount.Value - Time.deltaTime);
        Position.Value = transform.position;
    }
}
