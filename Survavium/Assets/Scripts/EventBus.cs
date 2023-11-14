using System;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static Action OnExitActivated;

    public static Action OnBearDied;

    public static Action OnPlayerDied;

    public static Action OnCutSceneEnd;

    public static Action OnEnd;
}
