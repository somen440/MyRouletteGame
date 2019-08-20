using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum RollState
{
    None = 0,
    Rolling = 1,
    Stoped = 2,
}

interface IRollable
{
    /// <summary>
    /// 回転させる
    /// </summary>
    void Roll();

    /// <summary>
    /// 回転状態か
    /// </summary>
    ReactiveProperty<RollState> State { get; }

    /// <summary>
    /// 位置
    /// </summary>
    ReactiveProperty<Vector3> Position { get; }
}
