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
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _shell = GetComponent<ShellComponent>();
        _cameraPlane = new Plane(Vector3.forward, -1f);
    }
    Vector3 lastV;
    private void Update()
    {
        GrabUpdate();
        //_rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 1);


        var vpPos = Camera.main.WorldToViewportPoint(_rb.position);

        if (vpPos.x > 1 || vpPos.y > 1 || vpPos.x < 0 || vpPos.y < 0)
        {
            _rb.position = new Vector3(0, 0, _rb.position.z) + (Vector3)Random.insideUnitCircle;
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
        _rb.isKinematic = _grabbed;

        if (_grabbed)
        {
            _rb.MovePosition(Vector3.Lerp(_rb.position, mouseInWorld, Time.fixedDeltaTime * 10f));
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetComponent<Collider>(out var col))
            {
                if (col.bounds.Contains(mouseInWorld))
                    _grabbed = true;
            }
        }

        var releasedButton = Input.GetMouseButtonUp(0);
        if (releasedButton && _grabbed)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10f);
            _grabbed = false;
        }
    }
}
