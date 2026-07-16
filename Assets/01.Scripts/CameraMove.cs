using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 0.25f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    private Transform _target;
    private void LateUpdate()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if(playerObj != null)
        {
            _target = playerObj.transform;
        }
        if (_target == null) return;

        Vector3 desiredPosition = _target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position,desiredPosition,smoothSpeed);

        transform.position = smoothedPosition;
    }
}
