using UnityEngine;

public class FurBall : MonoBehaviour
{
    Rigidbody _rb;
    SpringODE springX;
    SpringODE springY;
    ShellComponent _shell;

    [SerializeField, Min(0.1f)] private float _springForce = 1.0f;
    [SerializeField] private float mu = 0.0f;
    [SerializeField] private float k = 0.0f;
    [SerializeField] private float mass = 1.0f;
    private Plane _cameraPlane;
    private void Awake()
    {
        springX = new SpringODE(mass, mu, k, transform.position.x);
        springY = new SpringODE(mass, mu, k, transform.position.z);

        springX.Mass = mass;
        springX.K = k;
        springX.Mu = mu;
        springY.Mass = mass;
        springY.K = k;
        springY.Mu = mu;

        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.constraints = RigidbodyConstraints.FreezePositionZ;
        _rb.angularDrag = 6f;
        _shell = GetComponent<ShellComponent>();
        _cameraPlane = new Plane(Vector3.forward, -1f);
    }
    Vector3 lastV;
    private void Update()
    {
        GrabUpdate();
        //_rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 1);

        var vpPos = Camera.main.WorldToViewportPoint(_rb.position);
        var max = 1;
        var min = 0f;
        if (vpPos.x > max || vpPos.y > max || vpPos.x < min || vpPos.y < min)
        {
            vpPos.x = Mathf.Clamp(vpPos.x, min, max);
            vpPos.y = Mathf.Clamp(vpPos.y, min, max);
            _rb.position = Camera.main.ViewportToWorldPoint(vpPos);
        }

    }
    private void FixedUpdate()
    {
        springX.Mass = mass;
        springX.K = k;
        springX.Mu = mu;

        springY.Mass = mass;
        springY.K = k;
        springY.Mu = mu;



        springX.UpdatePositionAndVelocity(Time.fixedDeltaTime);
        springY.UpdatePositionAndVelocity(Time.fixedDeltaTime);
        var velocityMag = _rb.velocity.magnitude;
        for (int i = 0; i < _shell.RedererList.Length; i++)
        {


            if (velocityMag < 5f)
            {
                //velocity -= velocity*.1f * Time.deltaTime;
                var VX = (float)springX.GetVx();
                var VY = (float)springY.GetVx();
                _shell.RedererList[i].material.SetVector("_DisplacementDirection", new Vector3(VX, VY, 0) + Physics.gravity.normalized);

            }

            else
            {
                springX.SetQ(-lastV.x, 0);
                springY.SetQ(-lastV.y, 0);
                _shell.RedererList[i].material.SetVector("_DisplacementDirection", -_rb.velocity.normalized + Physics.gravity.normalized);

            }
            lastV = _rb.velocity.normalized;

        }
    }
    bool _grabbed;
    private void GrabUpdate()
    {
        var mousePosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        _cameraPlane.Raycast(ray, out var distance);
        var mouseInWorld = ray.GetPoint(distance);
        if (Physics.Raycast(ray, out var hit))
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.TryGetComponent<Collider>(out var col))
                {
                    if (col == GetComponent<Collider>())
                        _grabbed = true;
                }
            }

        }
        _rb.isKinematic = _grabbed;

        if (_grabbed)
        {
            _rb.MovePosition(Vector3.Lerp(_rb.position, mouseInWorld, Time.fixedDeltaTime * 10f));
        }




        var releasedButton = Input.GetMouseButtonUp(0);
        if (releasedButton && _grabbed)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10f);
            _grabbed = false;
        }
    }
}
