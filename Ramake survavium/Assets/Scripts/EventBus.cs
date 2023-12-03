using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static Action OnExitActivated;

    public static Action OnBearDied;

    public static Action OnPlayerDied;

    public static Action OnCutSceneEnd;

    public static Action OnEnd;

    public static Action OnPlayerRespawn;
}
