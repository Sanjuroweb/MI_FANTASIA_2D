using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    private CinemachineVirtualCamera cm; //para when pegue al player la camara tiemble 24
    private SpriteRenderer sp; //para hacer efecto de recibir da�o 24
    private PlayerController player; //24
    private Rigidbody2D rb; //24
    private bool aplicarFuerza; //para ejercer retroceso al bat con nuestro ataque 24

    public float velocidadDeMovimiento = 3; //24
    public float radioDeDeteccion = 15; //24
    public LayerMask layerJugador; //24

    //necesitamos saber cual es la cabeza del muercielago para nuestros ataques 24
    public Vector2 posicionCabeza;

    //public bool enCabeza; //pa saber si le dimos en la cabeza 24, comentamos para hacerlo de otra manera
    public int vidas = 3; //hay que atacarle 3 veces pa matarlo 24
    public string nombre; //para que siempre se cree con el mismo nombre 24

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
        //param: centro y tama�o del cubo
        Gizmos.DrawCube((Vector2)transform.position + posicionCabeza, new Vector2(1, 0.5f) * 0.7f);
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
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        //para saber si player cae sobre cabeza de bat 24
        //parametros: pos. de la cabeza, tama�o de la caja sobre la cabeza, angulo y layerMask del player
        //, comentamos para hacerlo de otra manera
        //enCabeza = Physics2D.OverlapBox((Vector2)transform.position + posicionCabeza, new Vector2(1, 0.5f) * 0.7f, 0, layerJugador);
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //validamos si las piernas de player estan sobre cabeza de bat
            //ver 24 en el min. 35:47
            //if (enCabeza)
            if(transform.position.y + posicionCabeza.y < player.transform.position.y - 0.7f)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;
                StartCoroutine(AgitarCamara(0.1f));
                Destroy(gameObject, 0.2f);
            }
        }
        else
        {
            player.RecibirDa�o((transform.position - player.transform.position).normalized);
        }
    }
    //24
    //m�todo especial que se utiliza para realizar operaciones de f�sica.A diferencia del m�todo Update, que se llama una vez por cada frame,
    //FixedUpdate se llama en intervalos de tiempo fijos y se utiliza com�nmente para actualizar simulaciones f�sicas.
    private void FixedUpdate()
    {
        //fuerza que le aplicamos al bat
        if (aplicarFuerza)
        {
            //queremos que la fuerza se aplique empujando al bat patr�s
            //param: direccion y tipo de impulso al rb
            rb.AddForce((transform.position - player.transform.position).normalized *100, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    //el da�o que recibe el bat 24
    public void RecibirDa�o()
    {
        StartCoroutine(AgitarCamara(0.1f)); //mov. de camara durante el golpe
        if (vidas > 0)
        {
            StartCoroutine(EfectoDa�o());
            StartCoroutine(AgitarCamara(0.1f)); //mov. de camara durante el golpe
            aplicarFuerza = true;
            vidas--;
        }
        else
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
    }

    //24
    private IEnumerator EfectoDa�o()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
