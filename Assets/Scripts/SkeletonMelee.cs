using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonMelee : MonoBehaviour
{
    private PlayerController player; //27
    private Rigidbody2D rb; //27
    private SpriteRenderer sp; //27
    private Animator anim; //27
    private CinemachineVirtualCamera cm; //27
    private bool aplicarFuerza; //27

    public float distanciaDeteccionJugador = 17; //27
    public float distanciaDeteccionFlecha = 11; //27
    //public GameObject flecha; //27
    public float fuerzaLanzamiento = 1; //27
    public float velocidadMovimiento; //27
    public float velocidadMovimientoAtacando; //27
    public int vidas = 3; //27
    public bool lanzandoFlecha; //27
    public bool vivo;

    //Unity 2D - Enemigo Basico 2D (video de YT) === enemB

    public int rutina;
    public float cronometro;
    public Animator ani;
    public int direccionB;
    public float speed_walk;
    public float speed_run;
    public GameObject target;
    public bool atacando;
    public GameObject Hit;
    public float rangoAtaque;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        gameObject.name = "SkeletonMelee";
        vivo = true;
        ani = GetComponent<Animator>(); //enemB
        target = GameObject.Find("Player"); //enemB
    }

    void Update()
    {
        Vector2 direccion = (player.transform.position - transform.position).normalized * distanciaDeteccionFlecha;
        Debug.DrawRay(transform.position, direccion, Color.red);
        float distanciaActual = Vector2.Distance(transform.position, player.transform.position);
        DistanciaAgro(distanciaActual, direccion);

        //Comportamientos(); //enemB
    }

    public void DistanciaAgro(float distanciaActual, Vector2 direccion)
    {
        if (distanciaActual <= distanciaDeteccionJugador)
        {
            Vector2 movimiento = new Vector2(direccion.x, 0);
            Debug.Log("AGRO");
            if (distanciaActual > rangoAtaque && !atacando)
            {
                Debug.Log("caminando pal player");
                
                movimiento = movimiento.normalized;
                rb.velocity = new Vector2(movimiento.x * velocidadMovimiento, rb.velocity.y);
                anim.SetBool("caminando", true);
                CambiarVista(movimiento.x);
            }
            else
            {
                //rb.velocity = new Vector2(movimiento.x * velocidadMovimientoAtacando, rb.velocity.y);
                //StartCoroutine(StopMovimiento());
                anim.SetBool("atacando", true);
            }
        }
        else
        {
            Debug.Log("Fuera de AGRO");
            anim.SetBool("caminando", false);
            Comportamientos(); //enemB
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionJugador);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccionFlecha);
    }

    private void CambiarVista(float direccionX)
    {
        if (direccionX < 0 && transform.localScale.x > 0 && vivo)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direccionX > 0 && transform.localScale.x < 0 && vivo)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    //enemB
    public void Comportamientos()
    {
        //ani.SetBool("run", false);
        cronometro += 1 * Time.deltaTime;
        if(cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
        }

        switch(rutina)
        {
        case 0:
                anim.SetBool("caminando", false); 
                break;

        case 1:
                direccionB = Random.Range(0, 2);
                rutina++;
                break;

        case 2:

                switch(direccionB)
                {
                    case 0:
                        transform.rotation = Quaternion.Euler(0,0,0);
                        transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
                        break;

                    case 1:
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                        transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
                        break;
                }
                anim.SetBool("caminando", true); 
                break;
        }
    }

    public void RecibirDaño()
    {
        if (vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            //StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza = true;
            vidas--;
        }
        else
        {
            //StartCoroutine(AgitarCamara(0.1f));
            //53 lo quitamos de aqui pa ponerlo en la funcion Morir() y corregir el tembleque tras matar skeleton
            vivo = false;
            velocidadMovimiento = 0;
            rb.velocity = Vector2.zero;
            anim.SetBool("recibeDmg", true);
            Destroy(this.gameObject, 5f);
            //he descomentado esto para enemB, habría que respetar el mover camara, YA SE VERÁ!!!
        }
    }

    private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        velocidadMovimiento = 0;
        anim.SetBool("recibeDmg", true);
        yield return new WaitForSeconds(1f);
        sp.color = Color.white;
        velocidadMovimiento = 2;
        anim.SetBool("recibeDmg", false);
    }

    private IEnumerator StopMovimiento()
    {
        
        velocidadMovimiento = 0;
        yield return new WaitForSeconds(1f);
        velocidadMovimiento = 2;

    }

    //enemB
    public void ColliderWeaponTrue()
    {
        Hit.GetComponent<BoxCollider2D>().enabled = true;
    }

    //enemB
    public void ColliderWeaponFalse()
    {
        Hit.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void FinalAnim()
    {
        anim.SetBool("atacando", false);
    }
}
