using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Waypoints : MonoBehaviour //31
{
    private Vector3 direccion;
    private PlayerController player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    //nos indica en que punto se encuentra
    private int indiceActual = 0;
    private bool aplicarFuerza;
    public bool agitando; //55

    public int vidas = 3;
    //porque puede morir con player pisandole la cabeza
    public Vector2 posicionCabeza;
    public float velocidadDesplazamiento;
    //lista con los puntos que va a recorrer nuestro enemigo
    public List<Transform> puntos = new List<Transform>();
    public bool esperando; //36
    public float tiempoDeEspera; //36
    public float fuerzaImpacto; //55

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    //para el rollito de cambiarle el nombre cada vez que se genera un enemy
    private void Start()
    {
        if (gameObject.CompareTag("Enemigo"))
            gameObject.name = "Spider";
    }

    private void FixedUpdate()
    {
        MovimientoWaypoints();
        if (gameObject.CompareTag("Enemigo"))
        {
            CambiarEscalaEnemigo();
        }

        //53
        if (aplicarFuerza)
        {
            //55 modificamos 25 por fuerzaImpacto y colocamos 15 en el inspector
            //rb.AddForce((transform.position - player.transform.position).normalized * 25, ForceMode2D.Impulse);
            rb.AddForce((transform.position - player.transform.position).normalized * fuerzaImpacto, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el objeto con el que colisionamos tiene la etiqueta "Player" y nosotros tenemos la etiqueta "Enemigo"
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemigo"))
        {
            // si los pies del player estan por encima de la cabeza de la spider
            if(player.transform.position.y - 0.7f > transform.position.y + posicionCabeza.y)
            {
                //aplicamos impuslo al player despues de saltar sobre cabeza de spider
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;

                Destroy(this.gameObject, 0.2f);
                Debug.Log("Destruimos Araña");
            }
            else
            {
                //consultar esta linea, parece que el player recibe daño desde la direcc. de spider
                player.RecibirDaño(-(player.transform.position - transform.position).normalized);
            }
        }
        //36
        else if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            //para que se mueva junto con la plataforma debe ser un objeto hijo
            if (player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;
            }
        }
    }

    //para que cuando nos bajemos de la plataforma dejemos de ser hijo de la plataforma 36
    //y dejar de tener el movimiento de la plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            player.transform.parent = null;
        }
    }

    private void CambiarEscalaEnemigo()
    {
        if (direccion.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direccion.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void MovimientoWaypoints()
    {
        //obtenemos direccion a la cual se va a mover
        direccion = (puntos[indiceActual].position - transform.position).normalized;

        //Mueve el objeto hacia el punto de destino actual
        //param: la posición actual del objeto, la posición del destino, y la distancia máxima que el objeto puede moverse en este frame
        //se multiplica para que el movimiento sea suave y basado en el tiempo
        //asignamos a la posicion del objeto con la asignacion
        if(!esperando) //36
            transform.position = (Vector2.MoveTowards(transform.position, puntos[indiceActual].position, velocidadDesplazamiento * Time.deltaTime));

        //para evitar bloqueos verificamos si la distancia entre la posición actual del objeto y el punto de destino actual es menor o igual a 0.7
        //en caso de ser asi activamos la corutina
        if (Vector2.Distance(transform.position, puntos[indiceActual].position) <= 0.7f)
        {
            if(!esperando) //si no esta esperando llamamos a la corutina 36
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

    public void RecibirDaño()
    {
        if (vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza = true;
            vidas--;
        }
        else
        {
            //55 comentamos y llamamos a ultimoAgitarCamara
            //StartCoroutine(AgitarCamara(0.1f));
            StartCoroutine(UltimoAgitarCamara(0.1f));

            //53 pegamos en Morir() para corregir tembleke continuo cuando matar bixo
            /*velocidadDesplazamiento = 0;
            rb.velocity = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
            */
        }
    }

    //53 para corregir tembleke continuo cuando matar bixo
    private void Morir()
    {
        if (vidas <= 0)
        {
            velocidadDesplazamiento = 0;
            rb.velocity = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }

    private IEnumerator AgitarCamara(float tiempo)
    {
        //55 ponemos contenido dentro de if
        if (!agitando)
        {
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(tiempo);
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            //53 para corregir tembleke continuo cuando matar bixo
            //Morir(); comentado en el 55
            //55
            agitando = false;
        }
    }
    
    //55 copiamos funcion y pegamos
    private IEnumerator UltimoAgitarCamara(float tiempo)
    {
        //55 ponemos contenido dentro de if
        if (!agitando)
        {
            transform.localScale = Vector3.zero; //para reducir su tamaño y dar sensacion
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(tiempo);
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            //53 para corregir tembleke continuo cuando matar bixo
            Morir();
            //55
            agitando = false;
        }
    }

    private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
