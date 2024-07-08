using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bat : MonoBehaviour
{
    private CinemachineVirtualCamera cm; //para when pegue al player la camara tiemble 24
    private SpriteRenderer sp; //para hacer efecto de recibir daño 24
    private PlayerController player; //24
    private Rigidbody2D rb; //24
    private bool aplicarFuerza; //para ejercer retroceso al bat con nuestro ataque 24

    public float velocidadDeMovimiento = 3; //24
    public float radioDeDeteccion = 15; //24
    public float distanciaDeteccionJugador = 25; //me
    public LayerMask layerJugador; //24

    //necesitamos saber cual es la cabeza del muercielago para nuestros ataques 24
    public Vector2 posicionCabeza;

    //public bool enCabeza; //pa saber si le dimos en la cabeza 24, comentamos para hacerlo de otra manera
    public int vidas = 3; //hay que atacarle 3 veces pa matarlo 24
    public string nombre; //para que siempre se cree con el mismo nombre 24

    //GPT
    public float upwardForce = 10f;
    public float downwardForce = 10f;
    private bool playerDetected = false;

    //GPT PARABOLA
    public float speed = 2.0f;
    public float height = 2.0f;
    public float distance = 2.0f;
    private Vector3 startPosition;
    private bool isMoving = false;
    public float dashDistance; // Distancia que avanzará hacia el jugador
    public float dashSpeed; // Velocidad del avance hacia el jugador


    private void Awake() //24
    {
        //le damos valor al virtual camera
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>(); //24
        sp = GetComponent<SpriteRenderer>(); //24
        rb = GetComponent<Rigidbody2D>(); //24
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); //24
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = nombre; //para que siempre se cree con el mismo nombre en el inspector 24
        startPosition = transform.position; // Guarda la posición inicial del pájaro
    }

    //para dibujar un area en el inspector y ver la zona de agro 24
    //se dibuja cnd tienes seleccionado el elemento
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //param: pos. del bat, radio de deteccion
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccion);
        //cubo sobre el bat para detectar si el player le cae
        Gizmos.color = Color.green;
        //param: centro y tamaño del cubo
        Gizmos.DrawCube((Vector2)transform.position + posicionCabeza, new Vector2(1, 0.5f) * 0.7f);

        //area de deteccion de la bolaMago
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
    }

    // Update is called once per frame
    void Update()
    {
        //para la direccion entre player y bat 24
        Vector2 direccion = player.transform.position - transform.position;
        //para la distancia entre player y bat 24
        float distancia = Vector2.Distance(transform.position, player.transform.position);

        //validamos si jugador esta dentro de ese rango de deteccion 24
        if(distancia <= radioDeDeteccion)
        {
            //el bat camina hacia la direccion que le digamos (player) 24
            rb.velocity = direccion.normalized * velocidadDeMovimiento;
            CambiarVista(direccion.normalized.x); //para que el bat mire pa donde queramos
            //StartCoroutine(StartFlight()); ////GPT
            StartCoroutine(MoveParabolically(direccion));  //GPT
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        //caminamos pal player
        if(distancia <= distanciaDeteccionJugador)
        {
            Vector2 movimiento = new Vector2(direccion.x, 0);
            movimiento = movimiento.normalized;
            rb.velocity = new Vector2(movimiento.x * velocidadDeMovimiento, rb.velocity.y);
            rb.velocity = direccion.normalized * velocidadDeMovimiento;
            CambiarVista(direccion.normalized.x); //para que el bat mire pa donde queramos
        }
    }

    //GPT
    private IEnumerator MoveParabolically(Vector2 direccion)
    {
        isMoving = true;
        float elapsedTime = 0;

        //parábola
        while (elapsedTime < distance / speed)
        {
            elapsedTime += Time.deltaTime;
            //t va desde 0 hasta 1
            float t = elapsedTime * speed / distance;

            float x = t * distance;
            float y = height * Mathf.Sin(t * Mathf.PI);

            transform.position = startPosition + new Vector3(x, -y, 0);

            yield return null; // Espera hasta el siguiente frame
        }

        // Movimiento hacia el jugador
        //Vector3 targetPosition = player.position;
        Vector3 directionNormalizada = direccion.normalized;
        Vector3 dashTarget = transform.position + directionNormalizada * dashDistance;

        elapsedTime = 0;
        while (elapsedTime < dashDistance / dashSpeed)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, dashTarget, elapsedTime * dashSpeed / dashDistance);

            yield return null; // Espera hasta el siguiente frame
        }


        isMoving = false;
    }

    //GPT
    private IEnumerator StartFlight()
    {
        // Habilita la física para que el pájaro caiga
        rb.isKinematic = false;

        // Simula el inicio del vuelo aplicando una fuerza hacia arriba
        rb.AddForce(Vector3.up * upwardForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.0f);
        rb.AddForce(Vector3.down * upwardForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1.0f);
    }

    //copiamos de PlayerController 24
    //para cambiar la vista a donde mira el bat
    private void CambiarVista(float direccionX)
    {
        if (direccionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direccionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    //para detectar collision entre player y bat 24
    //PARA MI VIDEOJUEGO NO QUIERO QUE MI PLAYER MATE AL BICHO SALTANDO SOBRE SU CABEZA

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.RecibirDaño((transform.position - player.transform.position).normalized);

            //validamos si las piernas de player estan sobre cabeza de bat
            //ver 24 en el min. 35:47
            //if (enCabeza)
            //if (transform.position.y + posicionCabeza.y < player.transform.position.y - 0.7f)
            //{
            //    player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
            //    StartCoroutine(AgitarCamara(0.1f));
            //    Destroy(gameObject, 0.2f);
            //}
        }
        else
        {
            //player.RecibirDaño((transform.position - player.transform.position).normalized);
        }
    }
    //24
    //método especial que se utiliza para realizar operaciones de física.A diferencia del método Update, que se llama una vez por cada frame,
    //FixedUpdate se llama en intervalos de tiempo fijos y se utiliza comúnmente para actualizar simulaciones físicas.
    private void FixedUpdate()
    {
        //fuerza que le aplicamos al bat
        if (aplicarFuerza)
        {
            //queremos que la fuerza se aplique empujando al bat patrás
            //param: direccion y tipo de impulso al rb
            rb.AddForce((transform.position - player.transform.position).normalized *100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    //el daño que recibe el bat 24
    public void RecibirDaño()
    {
        StartCoroutine(AgitarCamara(0.1f)); //mov. de camara durante el golpe
        if (vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            StartCoroutine(AgitarCamara(0.1f)); //mov. de camara durante el golpe
            aplicarFuerza = true;
            vidas--;
        }
        else
        {
            //53 comentamos para corregir mov de camara tras matar bixo
            //Destroy(gameObject, 0.2f);
        }
    }

    //53 para corregir mov de camara tras matar bixo
    private void Morir()
    {
        if (vidas <= 0)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    

    //param: tiempo de espera 24
    private IEnumerator AgitarCamara(float tiempo)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        //53
        Morir();
    }

    //24
    private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
