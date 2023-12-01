using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VehicleControl : MonoBehaviour
{
    [SerializeField] Transform _pivot;
    [SerializeField] VehicleDirection _direction;
    bool _isSelected;
    Vector3 _mouseSelectPos;
    BoxCollider _boxCollider;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (_isSelected)
        {
            OnControlMouseMovement();
        }   
    }

    public void Crash ()
    {
        _pivot.DOShakeRotation(0.5f, new Vector3(0, 0, 10));
    }

    private void OnControlMouseMovement()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 velocity = mousePos - _mouseSelectPos;
        int pixelDistance = 100;

        if (_direction == VehicleDirection.Horizontal)
        {
            if (Mathf.Abs(velocity.x) > pixelDistance)
            {
                _isSelected = false;

                if (velocity.x > 0)
                    OnMoveForward();
                else
                    OnMoveBack();
            }
        }
        if (_direction == VehicleDirection.Vertical)
        {
            if (Mathf.Abs(velocity.y) > pixelDistance)
            {
                _isSelected = false;

                if (velocity.y > 0)
                    OnMoveForward();
                else
                    OnMoveBack();
            }
        }
    }

    private void OnMoveForward ()
    {
        OnControlRaycast(transform.forward, true);
    }

    private void OnMoveBack ()
    {
        OnControlRaycast(-transform.forward, false);
    }

    private void OnControlRaycast (Vector3 dir, bool moveForward)
    {
        Ray rayRight = new Ray(transform.position + transform.right * (_boxCollider.size.x / 2.01f), dir);
        Ray rayLeft = new Ray(transform.position - transform.right * (_boxCollider.size.x / 2.01f), dir);
        RaycastHit hitRight;
        RaycastHit hitLeft;
        RaycastHit hit;

        if (Physics.Raycast(rayRight,out hitRight, 50f) & Physics.Raycast(rayLeft, out hitLeft, 50f))
        {
            if (hitRight.distance < hitLeft.distance)
                hit = hitRight;
            else
                hit = hitLeft;

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Exit"))
            {
                Vector3 target = Vector3.zero;
                if (moveForward)
                    target = transform.position + transform.forward * hit.distance;
                else
                    target = transform.position - transform.forward * hit.distance;
                OnExitCar(target, hit.transform.GetComponent<ExitControl>().ExitPoints);
            }
            else
            {
                Vector3 target = Vector3.zero;
                if (moveForward)
                    target = transform.position + transform.forward * (hit.distance - (_boxCollider.size.z / 2));
                else
                    target = transform.position - transform.forward * (hit.distance - (_boxCollider.size.z / 2));
                OnMoveCar(target, hit.collider.gameObject);
            }
        }
    }

    private void OnCrashCar (GameObject targetObj)
    {
        if(targetObj.layer == LayerMask.NameToLayer("Obstacle"))
        {
            targetObj.GetComponent<ObstacleControl>().OnCrashObstacle();
        }
        if (targetObj.layer == LayerMask.NameToLayer("Vehicle"))
        {
            targetObj.GetComponent<VehicleControl>().Crash();
        }
    }

    private void OnMoveCar (Vector3 target, GameObject crashObj)
    {
        float distance = Vector3.Distance(transform.position, target);
        transform.DOMove(target, distance * 0.05f).SetEase(Ease.InOutCubic).OnComplete(() => OnCrashCar(crashObj));
    }

    private void OnExitCar (Vector3 exitPos,Transform[] points)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(exitPos);
        foreach (var item in points)
        {
            path.Add(item.position);
        }

        _boxCollider.enabled = false;
        transform.DOLocalPath(path.ToArray(), 1f * path.Count).SetEase(Ease.Linear).OnWaypointChange(WaypointChange).OnComplete(ComplatePath);

        void WaypointChange (int index)
        {
            if (index == path.Count)
                return;
            transform.DOLookAt(path[index], 0.3f);
        }

        void ComplatePath ()
        {
            GameManager.Instance.OnDestroyVehicle(this);
        }
    }

    private void OnMouseDown()
    {
        _isSelected = true;
        _mouseSelectPos = Input.mousePosition;
    }
}
