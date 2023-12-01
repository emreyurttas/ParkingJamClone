using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObstacleControl : MonoBehaviour
{
    [SerializeField] Transform _pivot;
    public void OnCrashObstacle ()
    {
        _pivot.DOLocalRotate(new Vector3(20, 0, 0), 0.1f).SetEase(Ease.OutBack).OnComplete(RestartLocalRot);

        void RestartLocalRot ()
        {
            _pivot.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack);
        }
    }
}
