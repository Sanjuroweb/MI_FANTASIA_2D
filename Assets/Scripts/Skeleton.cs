using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skeleton : MonoBehaviour //arranca en el 27
{
    private PlayerController player; //27
    private Rigidbody2D rb; //27
    private SpriteRenderer sp; //27
    private Animator anim; //27
    private CinemachineVirtualCamera cm; //27
    private bool aplicarFuerza; //27

    public float distanciaDeteccionJugador = 17; //27
    public float distanciaDeteccionFlecha = 11; //27
    public GameObject flecha; //27
    public float fuerzaLanzamiento = 5; //27
    public float velocidadMovimiento; //27
    public int vidas = 3; //27
    public bool lanzandoFlecha; //27

    //inicializamos variables 27
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //cada vez que creamos un objeto skeleton en la escena le cambiamos el nombre 27
        gameObject.name = "Skeleton";
    }

    // Update is called once per frame
    void Update()
    {
        //calculamos direccion entre skeleton y player 27
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionFlecha;
        //para ver que se esta detectando la direccion correctamente
        Debug.DrawRay(transform.position, direccion, Color.red);

        //usamos metodo de Vector2 que permite calcular dist. entre 2 vectores
        float distanciaActual = Vector2.Distance(transform.position, player.transform.position);

        //
        if(distanciaActual <= distanciaDeteccionFlecha)
        {
            //paramos el enemy
            rb.velocity = Vector2.zero;
            anim.SetBool("caminando", false);

            Vector2 direccionNormalizada = direccion.normalized;
            //para que mire a dcha e izda segun direcc. a donde camina
            CambiarVista(direccionNormalizada.x);
            if (!lanzandoFlecha)
            {
                //llamamos a una corrutina que lanza flechas cada x tiempo
                StartCoroutine(LanzarFlecha(direccion, distanciaActual));
            }
        }
        else
        {
            if(distanciaActual <= distanciaDeteccionJugador)
            {
                Debug.Log("caminando pal player");
                //creamos el vector del movimiento del enemy, solo se mueve en x
                Vector2 movimiento = new Vector2(direccion.x, 0);
                movimiento = movimiento.normalized;
                //le aplicamos fuerza al enemy
                rb.velocity = movimiento * velocidadMovimiento;
                anim.SetBool("caminando", true);
                CambiarVista(movimiento.x);
            }
            else
            {
                anim.SetBool("caminando", false);
            }
        }
    }

    private void CambiarVista(float direccionX)
    {
        if(direccionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }else if(direccionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
        //area de deteccion de la flecha
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionFlecha);
    }

    private IEnumerator LanzarFlecha(Vector2 direccionFlecha, float distancia)
    {
        lanzandoFlecha = true;
        anim.SetBool("disparando", true);
        yield return new WaitForSeconds(1.42f); //tiempo entre cada flecha
        anim.SetBool("disparando", false);
        direccionFlecha = direccionFlecha.normalized;

        //instanciamos la flecha en la posición actual del objeto
        //el 3er param es una rotacion poero nosotros le metemos quaternion
        GameObject flechaGO = Instantiate(flecha, transform.position, Quaternion.identity);
        //a la flecha le pasamos la direccion que hemos definido aqui
        // Se obtiene el componente Flecha del objeto recién instanciado y se le asigna la dirección de la flecha.
        flechaGO.transform.GetComponent<Flecha>().direccionFlecha = direccionFlecha;
        //lo mismo pero con el esqueleto
        // Se obtiene el componente Flecha del objeto recién instanciado y se le asigna el objeto esqueleto (this.gameObject).
        flechaGO.transform.GetComponent<Flecha>().esqueleto = this.gameObject;

        // Se aplica una velocidad a la flecha multiplicando la dirección por la fuerza de lanzamiento.
        flechaGO.transform.GetComponent<Rigidbody2D>().velocity = direccionFlecha * fuerzaLanzamiento;
        lanzandoFlecha = false;
    }

    public void RecibirDaño()
    {
        if(vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza = true;
            vidas--;
        }
        else
        {
            StartCoroutine(AgitarCamara(0.1f));
            velocidadMovimiento = 0;
            rb.velocity = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }

    private IEnumerator AgitarCamara(float tiempo)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

    private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.RecibirDaño((transform.position - player.transform.position).normalized);
        }
    }

    //método especial que se utiliza para realizar operaciones de física.A diferencia del método Update, que se llama una vez por cada frame,
    //FixedUpdate se llama en intervalos de tiempo fijos y se utiliza comúnmente para actualizar simulaciones físicas.
    private void FixedUpdate()
    {
        if (aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
}
