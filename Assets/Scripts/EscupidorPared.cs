using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscupidorPared : MonoBehaviour
{
    //COPIADO DE SKELETONMAGE Y WAYPOINT

    private Vector3 direccion;
    private PlayerController player; //27
    private Rigidbody2D rb; //27
    private SpriteRenderer sp; //27
    private Animator anim; //27
    //private CinemachineVirtualCamera cm; //27
    private bool aplicarFuerza; //27
    //nos indica en que punto se encuentra
    private int indiceActual = 0;

    public float distanciaDeteccionEscupitajo = 11; //27
    public GameObject escupitajo; //27
    public float fuerzaLanzamiento = 1; //27
    public float velocidadMovimiento; //27
    public int vidas = 3; //27
    public bool lanzandoBola; //27
    //lista con los puntos que va a recorrer nuestro enemigo
    public List<Transform> puntos = new List<Transform>();
    public bool esperando; //36
    public float tiempoDeEspera; //36
    public float velocidadDesplazamiento;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        //cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "EscupidorPared";
    }

    // Update is called once per frame
    void Update()
    {
        //waypoint
        MovimientoWaypoints();

        //calculamos direccion entre skeleton y player para obtener un vector que siempre apunte al player 27
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionEscupitajo;
        //para ver que se esta detectando la direccion correctamente
        Debug.DrawRay(transform.position, direccion, Color.red);

        //usamos metodo de Vector2 que permite calcular dist. entre 2 vectores
        float distanciaActual = Vector2.Distance(transform.position, player.transform.position);

        //
        if (distanciaActual <= distanciaDeteccionEscupitajo)
        {
            //anim.SetBool("caminando", false);

            Vector2 direccionNormalizada = direccion.normalized;
            if (!lanzandoBola)
            {
                //llamamos a una corrutina que lanza Bolas cada x tiempo
                StartCoroutine(LanzarBola(direccion, distanciaActual));
            }
        }
        
    }

    private IEnumerator LanzarBola(Vector2 direccionEscupitajo, float distancia)
    {
        lanzandoBola = true;
        //anim.SetBool("atacando", true);
        yield return new WaitForSeconds(0.45f); //tiempo entre cada escupitajo
        //anim.SetBool("atacando", false);
        //53 para mejorar la direccion de la escupitajo
        direccionEscupitajo = (player.transform.position - transform.position).normalized * distanciaDeteccionEscupitajo;
        direccionEscupitajo = direccionEscupitajo.normalized;

        //instanciamos la escupitajo en la posición actual del objeto
        //el 3er param es una rotacion poero nosotros le metemos quaternion
        GameObject escupitajoGO = Instantiate(escupitajo, transform.position, Quaternion.identity);
        //a la escupitajo le pasamos la direccion que hemos definido aqui
        // Se obtiene el componente escupitajo del objeto recién instanciado y se le asigna la dirección de la escupitajo.
        escupitajoGO.transform.GetComponent<EscupitajoPared>().direccionEscupitajo = direccionEscupitajo;
        //lo mismo pero con el esqueleto
        // Se obtiene el componente escupitajo del objeto recién instanciado y se le asigna el objeto esqueleto (this.gameObject).
        escupitajoGO.transform.GetComponent<EscupitajoPared>().esqueleto = this.gameObject;

        // Se aplica una velocidad a la escupitajo multiplicando la dirección por la fuerza de lanzamiento.
        escupitajoGO.transform.GetComponent<Rigidbody2D>().velocity = direccionEscupitajo * fuerzaLanzamiento;

        //GPT
        yield return new WaitForSeconds(2.0f);
        //anim.SetBool("atacando", false);

        lanzandoBola = false;
    }

    private void MovimientoWaypoints()
    {
        //obtenemos direccion a la cual se va a mover
        direccion = (puntos[indiceActual].position - transform.position).normalized;

        //Mueve el objeto hacia el punto de destino actual
        //param: la posición actual del objeto, la posición del destino, y la distancia máxima que el objeto puede moverse en este frame
        //se multiplica para que el movimiento sea suave y basado en el tiempo
        //asignamos a la posicion del objeto con la asignacion
        if (!esperando) //36
            transform.position = (Vector2.MoveTowards(transform.position, puntos[indiceActual].position, velocidadDesplazamiento * Time.deltaTime));

        //para evitar bloqueos verificamos si la distancia entre la posición actual del objeto y el punto de destino actual es menor o igual a 0.7
        //en caso de ser asi activamos la corutina
        if (Vector2.Distance(transform.position, puntos[indiceActual].position) <= 0.7f)
        {
            if (!esperando) //si no esta esperando llamamos a la corutina 36
                StartCoroutine(Espera());
        }
    }

    private IEnumerator Espera()
    {
        esperando = true; //36
        // Pausa la ejecución de la rutina hasta el próximo frame. En este contexto,
        // parece que simplemente se está esperando hasta el próximo frame antes de ejecutar las siguientes líneas de código.
        //yield return null; //cambiamos en el 36
        yield return new WaitForSeconds(tiempoDeEspera); //modificado en el 36
        esperando = false; //36

        indiceActual++;

        //si sobrepasamos reiniciamos el indiceActual
        if (indiceActual >= puntos.Count)
            indiceActual = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionEscupitajo);
    }
}
