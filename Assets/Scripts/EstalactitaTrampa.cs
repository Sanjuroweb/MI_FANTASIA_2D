using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstalactitaTrampa : MonoBehaviour
{

    private PlayerController player; //27
    private Rigidbody2D rb; //27
    private SpriteRenderer sp; //27
    private Animator anim; //27
    private Collider2D col; //me
    //private CinemachineVirtualCamera cm; //27
    private bool aplicarFuerza; //27

    public float distanciaDeteccionJugador = 17; //27
    public float distanciaDeteccionBola = 11; //27
    public float fuerzaLanzamiento = 1; //27
    public float velocidadMovimiento; //27
    public float tiempoDeCaida = 0.5f; //me


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
        //cada vez que creamos un objeto skeleton en la escena le cambiamos el nombre 27
        gameObject.name = "Estalactita";
        rb.gravityScale = 0;
        col = GetComponent<Collider2D>(); //me
    }

    // Update is called once per frame
    void Update()
    {
        //calculamos direccion entre skeleton y player para obtener un vector que siempre apunte al player 27
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionBola;
        //para ver que se esta detectando la direccion correctamente
        Debug.DrawRay(transform.position, direccion, Color.red);

        //usamos metodo de Vector2 que permite calcular dist. entre 2 vectores
        float distanciaActual = Vector2.Distance(transform.position, player.transform.position);

        //
        if (distanciaActual <= distanciaDeteccionBola)
        {
            //ActivarCaida();
            //Invoke("DetenerCaida", tiempoDeCaida);
            anim.SetBool("caer", true);
        }
    }

    //de la trampa
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().RecibirDaño(-(collision.transform.position - transform.position).normalized);
        }
    }

    public void DesactivarCollider2D()
    {
        if (col != null)
        {
            col.enabled = false;
        }
    }

    //
    private void ActivarCaida()
    {
        Debug.Log("Player activa trampa");
        rb.gravityScale = 1;
        GetComponent<Rigidbody2D>().AddForce(Vector2.down * 0.5f, ForceMode2D.Impulse);
    }

    private void DetenerCaida()
    {
        rb.gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
        //area de deteccion de la bolaMago
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionBola);
    }
}
