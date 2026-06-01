using UnityEngine;

public class BolitaController : MonoBehaviour
{
    [Header("Movimiento")]
    public float fuerzaMovimiento = 8f;
    public float velocidadMaxima = 4f;
    public float drag = 10f;

    [Header("Rodadura")]
    public float multiplicadorGiro = 0.5f;
    public Transform visual;

    [Header("Arduino")]
    public float normalizacionGiro = 80f;

    private Vector3 velocidad;
    private float radio;
    private Collider miCollider;
    private Collider[] buffer = new Collider[16];

    void Start()
    {
        miCollider = GetComponent<Collider>();

        SphereCollider sc = GetComponent<SphereCollider>();

        if (sc != null)
        {
            radio = sc.radius * Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y,
                transform.lossyScale.z
            );
        }
        else
        {
            radio = 0.5f;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        float ix = 0f;
        float iz = 0f;

        // Teclado
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            ix = -1f;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            ix = 1f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            iz = 1f;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            iz = -1f;

        // MPU6050
        ix += Mathf.Clamp(MPU6050Reader.x / normalizacionGiro, -1f, 1f);
        iz += Mathf.Clamp(MPU6050Reader.y / normalizacionGiro, -1f, 1f);

        ix = Mathf.Clamp(ix, -1f, 1f);
        iz = Mathf.Clamp(iz, -1f, 1f);

        // Movimiento
        velocidad.x += ix * fuerzaMovimiento * dt;
        velocidad.z += iz * fuerzaMovimiento * dt;

        // Drag
        velocidad.x = Mathf.MoveTowards(
            velocidad.x,
            0f,
            drag * dt
        );

        velocidad.z = Mathf.MoveTowards(
            velocidad.z,
            0f,
            drag * dt
        );

        // Limitar velocidad
        Vector3 velH = new Vector3(
            velocidad.x,
            0f,
            velocidad.z
        );

        if (velH.magnitude > velocidadMaxima)
        {
            velH = velH.normalized * velocidadMaxima;
            velocidad.x = velH.x;
            velocidad.z = velH.z;
        }

        // Mover
        transform.position += new Vector3(
            velocidad.x,
            0f,
            velocidad.z
        ) * dt;

        ResolverPenetraciones();

        AplicarRodadura(dt);
    }

    void AplicarRodadura(float dt)
    {
        if (visual == null)
            return;

        Vector3 velHorizontal = new Vector3(
            velocidad.x,
            0f,
            velocidad.z
        );

        if (velHorizontal.magnitude < 0.01f)
            return;

        Vector3 ejeRotacion =
            Vector3.Cross(
                Vector3.up,
                velHorizontal.normalized
            );

        float angulo =
            (velHorizontal.magnitude / radio) *
            Mathf.Rad2Deg *
            dt *
            multiplicadorGiro;

        visual.Rotate(
            ejeRotacion,
            angulo,
            Space.World
        );
    }

    void ResolverPenetraciones()
    {
        int cantidad = Physics.OverlapSphereNonAlloc(
            transform.position,
            radio,
            buffer,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < cantidad; i++)
        {
            Collider col = buffer[i];

            if (col == null)
                continue;

            if (col == miCollider)
                continue;

            if (Physics.ComputePenetration(
                miCollider,
                transform.position,
                transform.rotation,
                col,
                col.transform.position,
                col.transform.rotation,
                out Vector3 direccion,
                out float distancia))
            {
                distancia = Mathf.Clamp(
                    distancia,
                    0f,
                    0.2f
                );

                transform.position +=
                    direccion *
                    (distancia + 0.001f);
            }
        }
    }
}