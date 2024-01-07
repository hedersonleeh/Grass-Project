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
        _cameraPlane = new Plane(Camera.main.transform.forward, -1f);
    }
    Vector3 lastV;
    private void Update()
    {
        var mousePosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        _cameraPlane.Raycast(ray, out var distance);
        var mouseInWorld = ray.GetPoint(distance);

        if (Input.GetMouseButton(0))
            _rb.MovePosition(Vector3.Lerp(_rb.position, mouseInWorld, Time.fixedDeltaTime * 10f));
        _rb.isKinematic = Input.GetMouseButton(0);
        //_rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 1);
        springX.Mass = mass;
        springX.K = k;
        springX.Mu = mu;

        springY.Mass = mass;
        springY.K = k;
        springY.Mu = mu;
        var releasedButton = Input.GetMouseButtonUp(0);
        if (releasedButton)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10f);

        }
       

        springX.UpdatePositionAndVelocity(Time.fixedDeltaTime);
        springY.UpdatePositionAndVelocity(Time.fixedDeltaTime);

        for (int i = 0; i < _shell.RedererList.Length; i++)
        {


            if (_rb.velocity.magnitude < 7f)
            {
                //velocity -= velocity*.1f * Time.deltaTime;
                var VX = (float)springX.GetVx();
                var VY = (float)springY.GetVx();
                _shell.RedererList[i].material.SetVector("_DisplacementDirection", new Vector3(VX, VY, 0));

            }

            else
            {
                springX.SetQ(-lastV.x, 0);
                springY.SetQ(-lastV.y, 0);
                _shell.RedererList[i].material.SetVector("_DisplacementDirection", -_rb.velocity.normalized);

            }
            lastV = _rb.velocity.normalized;

        }



    }
}
