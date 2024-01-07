using UnityEngine;

public class SpringTest : MonoBehaviour
{
    [SerializeField] private Transform ObjToMove;
    [SerializeField] private LineRenderer line;
    private SpringODE spring;
   [Min(0.1f)] public float mass = 1; //mass at the on of spring
    public float mu = .5f; //daminping Coefficient
    public float k = 20; //spring constanst
    [SerializeField] private Transform initialPos = null;
    private void Awake()
    {
        spring = new SpringODE(mass, mu, k, initialPos.position.y - 9);
        line.positionCount = 2;

    }

    private void FixedUpdate()
    {
        if (spring != null)
            spring.UpdatePositionAndVelocity(Time.deltaTime);


    }
    private void Update()
    {

        var y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        line.SetPositions(new Vector3[] { ObjToMove.position, initialPos.position });
        if (Input.GetMouseButton(0))
        {
            ObjToMove.position = new Vector3(ObjToMove.position.x, y);

            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            spring.SetQ(y, 1);
        }
        if (spring == null) return;
        spring.Mass = mass;
        spring.Mu = mu;
        spring.K = k;
        ObjToMove.position = new Vector3(ObjToMove.position.x, (float)spring.GetX());
        Debug.Log("CurrentVelocity: " + spring.GetVx());

    }
}
