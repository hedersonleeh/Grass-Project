using System;
public class SpinProjectile : WindyProjectile
{
    public double rx; // spin axis vector component
    public double ry; // spin axis vector component
    public double rz; // spin axis vector component
    public double omega; // angular velocity, m/s
    public double radius; // sphere radius, m

    public SpinProjectile(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass,
                          double area, double density, double cd, double rx, double ry, double rz, double omega,
                          double radius) : base(x0, y0, z0, vx0, vy0, vz0, time, mass, area, density, cd)
    {
        this.rx = rx;
        this.ry = ry;
        this.rz = rz;
        this.omega = omega;
        this.radius = radius;
    }
    public override void UpdateLocationAndVelocity(double dt)
    {
        ODESolver.RungeKutta4(this, dt);
    }
    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {
        // The getRightHandSide() method returns the right-hand
        // sides of the six first-order projectile ODEs
        // q[0] = vx = dxdt
        // q[1] = x
        // q[2] = vy = dydt
        // q[3] = y
        // q[4] = vz = dzdt
        // q[5] = z

        double[] dQ = new double[6];
        double[] newQ = new double[6];

        for (int i = 0; i < 6; i++)
        {
            newQ[i] = q[i] + qScale * deltaQ[i];
        }
        // Declare some convenience variables representing
        // the intermediate values of velocity.
        double vx = newQ[0];
        double vy = newQ[2];
        double vz = newQ[4];

        double vax = vx - WindX;
        double vay = vy - WindY;
        double vaz = vz;

        double va = Math.Sqrt((vax * vax + (vay * vay) + (vaz * vaz))) + 1.0e-8;

        double Fd = 0.5 * density * area * Cd * va * va;

        double Fdx = -Fd * vax / (va);
        double Fdy = -Fd * vay / (va);
        double Fdz = -Fd * vaz / (va);

        double v = Math.Sqrt((vx * vx + (vy * vy) + (vz * vz))) + 1.0e-8;

        // Evaluate the Magnus force terms.
        double Cl = radius * omega / v;
        double Fm = 0.5 * GetDensity() * GetArea() * Cl * v * v;
        double Fmx = (vy * rz - ry * vz) * Fm / v;
        double Fmy = -(vx * rz - rx * vz) * Fm / v;
        double Fmz = (vx * ry - rx * vy) * Fm / v;

        //  Compute the right - hand sides of the six ODEs.
        dQ[0] = ds * (Fdx + Fmx) / GetMass();
        dQ[1] = ds * vx;
        dQ[2] = ds * (Fdy + Fmy) / GetMass();
        dQ[3] = ds * vy;
        dQ[4] = ds * (G + (Fdz + Fmz) / GetMass());
        dQ[5] = ds * vz;

        return dQ;
    }
}
public class WindyProjectile : DragProjectile
{
    public double WindX;
    public double WindY;
    public WindyProjectile(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass, double area, double density, double cd) : base(x0, y0, z0, vx0, vy0, vz0, time, mass, area, density, cd)
    {
    }
    public override void UpdateLocationAndVelocity(double dt)
    {
        ODESolver.RungeKutta4(this, dt);
    }
    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {
        // The getRightHandSide() method returns the right-hand
        // sides of the six first-order projectile ODEs
        // q[0] = vx = dxdt
        // q[1] = x
        // q[2] = vy = dydt
        // q[3] = y
        // q[4] = vz = dzdt
        // q[5] = z

        double[] dQ = new double[6];
        double[] newQ = new double[6];

        for (int i = 0; i < 6; i++)
        {
            newQ[i] = q[i] + qScale * deltaQ[i];
        }
        // Declare some convenience variables representing
        // the intermediate values of velocity.
        double vx = newQ[0];
        double vy = newQ[2];
        double vz = newQ[4];

        double vax = vx - WindX;
        double vay = vy - WindY;
        double vaz = vz;

        double va = Math.Sqrt((vax * vax + (vay * vay) + (vaz * vaz))) + 1.0e-8;

        double Fd = 0.5 * density * area * Cd * va * va;

        dQ[0] = -ds * Fd * vax / (mass * va);
        dQ[1] = ds * vx;

        dQ[2] = -ds * Fd * vay / (mass * va);
        dQ[3] = ds * vy;

        dQ[4] = ds * (G - Fd * vaz / (mass * va));
        dQ[5] = ds * vz;

        return dQ;
    }

}
public class DragProjectile : SimpleProjectile
{
    protected double mass;
    protected double area;
    protected double density;
    protected double Cd;

    public DragProjectile(double x0, double y0, double z0, double vx0, double vy0, double vz0, double time, double mass, double area, double density, double cd) :
                          base(x0, y0, z0, vx0, vy0, vz0, time)
    {
        this.mass = mass;
        this.area = area;
        this.density = density;
        Cd = cd;
    }

    public double GetMass() => mass;
    public double GetArea() => area;
    public double GetDensity() => density;
    public double GetCd() => Cd;

    public override void UpdateLocationAndVelocity(double dt)
    {
        ODESolver.RungeKutta4(this, dt);
    }

    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {
        // The getRightHandSide() method returns the right-hand
        // sides of the six first-order projectile ODEs
        // q[0] = vx = dxdt
        // q[1] = x
        // q[2] = vy = dydt
        // q[3] = y
        // q[4] = vz = dzdt
        // q[5] = z

        double[] dQ = new double[6];
        double[] newQ = new double[6];

        for (int i = 0; i < 6; i++)
        {
            newQ[i] = q[i] + qScale * deltaQ[i];
        }
        // Declare some convenience variables representing
        // the intermediate values of velocity.
        double vx = newQ[0];
        double vy = newQ[2];
        double vz = newQ[4];

        double v = Math.Sqrt((vx * vx) + (vy * vy) + (vz * vz)) + 1.0e-8;

        double Fd = 0.5 * density * area * Cd * v * v;

        dQ[0] = -ds * Fd * vx / (mass * v);
        dQ[1] = ds * vx;

        dQ[2] = -ds * Fd * vy / (mass * v);
        dQ[3] = ds * vy;

        dQ[4] = ds * (G - Fd * vz / (mass * v));
        dQ[5] = ds * vz;

        return dQ;
    }
}

public class SimpleProjectile : ODE
{
    public static double G = -9.81;

    public SimpleProjectile(double x0, double y0, double z0,
                            double vx0, double vy0, double vz0, double time) : base(6)
    {
        // Load the initial position, velocity, and time
        // values into the s field and q array from the
        // ODE class.
        SetS(time);
        SetQ(vx0, 0);
        SetQ(x0, 1);
        SetQ(vy0, 2);
        SetQ(y0, 3);
        SetQ(vz0, 4);
        SetQ(z0, 5);



    }
    //this methods return the location, velocity,
    //and time values.

    public double GetVx()
    {
        return GetQ(0);
    }
    public double GetVy()
    {
        return GetQ(2);
    }
    public double GetVz()
    {
        return GetQ(4);
    }

    public double GetX()
    {
        return GetQ(1);
    }
    public double GetY()
    {
        return GetQ(3);

    }
    public double GetZ()
    {
        return GetQ(5);

    }
    public double GetTime()
    {
        return GetS();
    }
    public virtual void UpdateLocationAndVelocity(double dt)
    {
        double time = GetS();
        double vx0 = GetQ(0);
        double x0 = GetQ(1);
        double vy0 = GetQ(2);
        double y0 = GetQ(3);
        double vz0 = GetQ(4);
        double z0 = GetQ(5);

        double x = x0 + vx0 * dt;
        double y = y0 + vy0 * dt;
        double vz = vz0 + G * dt;
        double z = z0 + vz0 * dt - (.5 * G * dt * dt);

        //Update Time
        time = time + dt;
        SetS(time);
        SetQ(x, 1);
        SetQ(y, 3);
        SetQ(vz, 4);
        SetQ(z, 5);

    }
    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {
        return new double[1];
    }
}
public class SpringODE : ODE
{
    private double mass; //mass at the on of spring
    private double mu; //daminping Coefficient
    private double k; //spring constanst
    private double x0; //initial spring deflection
    private double time; //independent variable
                         // SpringODE constructor.

    public double Mass
    {
        get { return mass; }
        set { mass = value; }
    }

    public double Mu { get => mu; set => mu = value; }
    public double K { get => k; set => k = value; }

    // Call the ODE constructor indicating that there
    // will be two coupled first-order ODEs.
    public SpringODE(double mass, double mu, double k, double x0) : base(2)
    {


        // Initialize fields declared in the class.
        this.mass = mass;
        this.Mu = mu;
        this.K = k;
        this.x0 = x0;
        time = 0.0;
        // Set the initial conditions of the dependent
        // variables.
        // q[0] = vx
        // q[1] = x;
        SetQ(0.0, 0);
        SetQ(x0, 1);
    }

    public double GetVx()
    {
        return GetQ(0);
    }
    public double GetX()
    {
        return GetQ(1);
    }

    public double GetTime()
    {
        return GetS();
    }
    // This method updates the velocity and position
    // of the spring using a 4th order Runge-Kutta
    // solver to integrate the equations of motion.
    public void UpdatePositionAndVelocity(double dt)
    {
        ODESolver.RungeKutta4(this, dt);
    }

    public override double[] GetRightHandSide(double s, double[] q, double[] deltaQ, double ds, double qScale)
    {
        // sides of the two first-order damped spring ODEs
        // q[0] = vx
        // q[1] = x

        // dq[0] = d(vx) =F+F+ dt*(-mu*dxdt - k*x)/mass
        // dq[1] = d(x) = dt*(v)

        double[] dq = new double[4]; // right-hand side values
        double[] newQ = new double[4]; // intermediate dependent
                                       // variable values.
                                       // Compute the intermediate values of the
                                       // dependent variables.
        for (int i = 0; i < 2; ++i)
        {
            newQ[i] = q[i] + qScale * deltaQ[i];
        }
        // Compute right-hand side values.
        double G = 1;
        dq[0] = ds * G - ds * (Mu * newQ[0] + K * newQ[1]) / mass;
        dq[1] = ds * (newQ[0]);
        return dq;

    }
}
public abstract class ODE
{
    private int numEqns; // number of equations to solve
    private double[] q; // array of dependent variables
    private double s; // independent variable

    protected ODE(int numEqns)
    {
        this.numEqns = numEqns;
        q = new double[numEqns];
    }
    // These methods return the number of equations or
    // the value of the dependent or independent variables.
    public int GetNumEqns()
    {
        return numEqns;
    }
    public double GetS()
    {
        return s;
    }
    public double GetQ(int index)
    {
        return q[index];
    }
    public double[] GetAllQ()
    {
        return q;
    }
    // These methods change the value of the dependent
    // or independent variables.
    public void SetS(double value)
    {
        s = value;
        return;
    }
    public void SetQ(double value, int index)
    {
        q[index] = value;
        return;


    }
    // This method returns the right-hand side of the
    // ODEs. It is declared abstract to force subclasses
    // to implement their own version of the method.
    public abstract double[] GetRightHandSide(double s,
    double[] q, double[] deltaQ, double ds, double qScale);
}
