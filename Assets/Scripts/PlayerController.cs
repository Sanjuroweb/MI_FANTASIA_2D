using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject ultimoEnemigo; //53

    private int direccionX; //47

    private Rigidbody2D rb;
    private Animator anim;
    public Vector2 direccion;
    public Vector2 direccionRaw; //me
    public GameObject bulletPref; //me
    private CinemachineVirtualCamera cm; //17
    private Vector2 direccionMovimiento; // le damos el valor en la funcion caminar 19
    private Vector2 direccionDaño; //30
    private bool bloqueado; //el valoro se lo vamos a adar en las animaciones 25
    private GrayCamera gc; //30
    private SpriteRenderer sprite; //30

    private float velocidadDeMovimientoAuxiliar; //50
    private CapsuleCollider2D collider; //50

    [Header("Estadisticas")]
    public float velocidaDeMovimiento = 10;
    public float fuerzaDeSalto = 5;
    public float velocidadDash =20; //16
    public float velocidadDeslizar; //22
    public int vidas = 3; //30
    public float tiempoInmortalidad; //30

    [Header("Colisiones")]
    public LayerMask layerPiso;
    public float radioDeColision;
    public Vector2 abajo;
    public Vector2 derecha; //tiene valor en el inspector 22
    public Vector2 izquierda; //tiene valor en el inspector 22
    public float abuelitaLoca;

    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true; //para controlar el salto en el aire
    public bool puedeDash; //16
    public bool haciendoDash;
    public bool tocadoPiso;
    public bool haciendoShake; //17
    public bool estaAtacando; //19
    public bool enMuro; //22
    public bool muroDerecho; //para verificar si es muro derecho 22
    public bool muroIzquierdo; //para verificar si es muro derecho 22
    public bool agarrarse; //22
    public bool saltarDeMuro; //22
    public bool esInmortal; //30
    public bool aplicarFuerza; //30
    public bool terminandoMapa; //47
    public bool enEscalera;
    private bool agachandose; //50
    private bool subirEscalera;
    private float gravedadInicial; //internet
    private bool escalando;//internet
    private bool lanzandoBola; //me
    private bool botonPresionado;

    [Header("Escalar")]
    [SerializeField] private float velocidadEscalar;
   
    [Header("Movimientos")]
    public float x;  //cuando pulsamos izquierda es negativo, y a derecha es`positivo
    public float y; //cuando pulsamos abajo es negativo, y a arriba es`positivo

    [SerializeField] private ParticleSystem particulas; //me, www.youtube.com/watch?v=sxU0LpR1CY8

    //configuraciones iniciales, inicializar variables, o para realizar otras tareas que deben llevarse a cabo antes de que comience el juego.
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sprite = GetComponent<SpriteRenderer>();

        velocidadDeMovimientoAuxiliar = velocidaDeMovimiento; //50
        collider = GetComponent<CapsuleCollider2D>();
    }

    //internet
    private void escalarEscalera()
    {
        if (collider.CompareTag("Escalera"))
        {
            Debug.Log("TOCA ESCALERA");
        }
        //if ((direccion.y != 0 || escalando) && (collider.IsTouchingLayers(LayerMask.GetMask("Escaleras"))))
        //if ((direccion.y != 0 || escalando) && (collider.CompareTag("Escalera")))
        if ((direccion.y != 0 || escalando) && (enEscalera))
        {
            Vector2 velocidadSubida = new Vector2(rb.velocity.x, direccion.y * velocidadEscalar);
            rb.velocity = velocidadSubida;
            rb.gravityScale = 0;
            escalando = true;
            Debug.Log("PAD HACIA ARRIBA Y ENESCALERA");
            Debug.Log(rb.velocity);
        }
        else
        {
            rb.gravityScale = 3;
            escalando = false;
        }
    }

    //me
    private void LanzarBola()
    {
            Instantiate(bulletPref, transform.position, Quaternion.identity);
            Debug.Log("Lanzando bola");
            //hud.municion--;
            //GameObject.Find("SoundManager").GetComponent<soundManager>().PlayAudio("disparoPlayer");
    }

    //creamos funcion para añadirlo al evento dentro de atacarA 25
    public void SetBloqueadoTrue()
    {
        bloqueado = true;
    }

    //30
    public void Morir()
    {
        if (vidas > 0)
            return;

        //42 para que salga el gameover cuando perdemos
        GameManager.instance.GameOver();
        //se ejecuta si es igual a 0, enabled es de unity y desactiva el componente
        this.enabled = false;
    }

    //30
    public void RecibirDaño()
    {
        StartCoroutine(ImpactoDaño(Vector2.zero));
    }

    //30
    public void RecibirDaño(Vector2 direccionDaño)
    {
        StartCoroutine(ImpactoDaño(direccionDaño));
    }

    //30
    private IEnumerator ImpactoDaño(Vector2 direccionDaño)
    {
        if (!esInmortal)
        {
            //StartCoroutine(Inmortalidad());
            vidas--;
            //activamos el componente que lo vuelve gris
            gc.enabled = true;
            float velocidadAuxiliar = velocidaDeMovimiento;
            this.direccionDaño = direccionDaño;
            aplicarFuerza = true;
            //ralentizamos el tiempo
            Time.timeScale = 0.4f;
            //hacemos efecto de la onda expansiva y agitamos camara
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(AgitarCamara(0.2f));
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;
            gc.enabled = false;

            ActualizarVidasUI(1);

            //para eliminar 1 a 1 las 3 pocimas de vida del UI
            //le ponemos -1 para que sea 0, 1, 2
            //lo comentamos en el 46 para meterlo en la funcion de abajo ActualizarVidasUI()
            /*for(int i = GameManager.instance.vidasUI.transform.childCount - 1; i >= 0; i--)
            {
                if (GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }*/

            velocidaDeMovimiento = velocidadAuxiliar;
            Morir();
        }
    }

    //49
    public void MostrarVidasUI()
    {
        for (int i = 0; i < GameManager.instance.vidasUI.transform.childCount; i++)
        {
            GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    //46
    public void ActualizarVidasUI(int vidasADescontar)
    {
        int vidasDescontadas = vidasADescontar;

        //lo hemos cortado y pegado de ImpactoDaño en el 46
        //para eliminar 1 a 1 las 3 pocimas de vida del UI
        //le ponemos -1 para que sea 0, 1, 2
        for (int i = GameManager.instance.vidasUI.transform.childCount - 1; i >= 0; i--)
        {
            if (GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy && vidasDescontadas != 0)
            {
                GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                vidasDescontadas--;
                //break;
            }
            else
            {
                if (vidasDescontadas == 0)
                    break;
            }
        }
    }

    //30
    private void FixedUpdate()
    {
        if (aplicarFuerza)
        {
            //a 0 para que no se contraresten
            velocidaDeMovimiento = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-direccionDaño * 25, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }

        //escalarEscalera(); //internet
    }

    //30
    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad());
    }


    //30
    private IEnumerator Inmortalidad()
    {
        esInmortal = true;

        float tiempoTranscurrido = 0;

        while(tiempoTranscurrido < tiempoInmortalidad)
        {
            sprite.color = new Color(1, 1, 1, .5f);
            yield return new WaitForSeconds(tiempoInmortalidad / 20);
            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(tiempoInmortalidad / 20);
            tiempoTranscurrido += tiempoInmortalidad / 10;
        }

        esInmortal = false;
    }

    //47 cuando creamos referencia en el inspector no se podia porque teniamos como parametro un Vector2
    //lo hemos cambiado a int el tipo de dato del parametro
    public void MovimientoFinalMapa(int direccionX)
    {
        terminandoMapa = true;
        this.direccionX = direccionX;
        anim.SetBool("caminar", true);

        if (this.direccionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        else if (this.direccionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Start is called before the first frame update SE LLAMA DESPUES DE Awake()
    //se usa en el primer fotograma
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        //47 hemos metido aqui dentro el movimiento y agarre
        if (!terminandoMapa)
        {
            Movimiento();
            Agarres();
            probarControles();

            escalarEscalera();
            if (Input.GetKey(KeyCode.JoystickButton2))
            {
                if (!botonPresionado)
                {
                    botonPresionado = true;
                    LanzarBola();
                }
            }
            else
            {
                botonPresionado = false;
            }
        }
        else
        {
            rb.velocity = (new Vector2(direccionX * velocidaDeMovimiento, rb.velocity.y));
        }

        //53 para lo del retroceso de la collision
        if (!esInmortal && ultimoEnemigo != null)
        {
            Physics2D.IgnoreCollision(ultimoEnemigo.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            ultimoEnemigo = null;
        }
    }

    //53 para el retroceso
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (esInmortal)
            {
                ultimoEnemigo = collision.gameObject;
                Physics2D.IgnoreCollision(ultimoEnemigo.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            }
        }

        
    }

    //yo
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Escalera"))
        {
            Debug.Log("TOCA ESCALERA");
        }
    }*/

    //50
    private void Agacharse()
    {
        agachandose = true;
        collider.offset = new Vector2(-0.003442526f, -0.1217294f);
        collider.size = new Vector2(1.006073f, 1.006073f);
        anim.SetBool("agachado", true);
        velocidaDeMovimiento = velocidadDeMovimientoAuxiliar / 3;
    }

    //realiza la animacion de atacar en funcion de la direccion, dentro de Movimiento() 19
    //el vector direccion es la dir a donde vamos a atacar
    private void Atacar(Vector2 direccion)
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton0)) //Atacarr
        {
            if(!estaAtacando && !haciendoDash)
            {
                estaAtacando = true;

                //las variables del animator
                //cambiamos los valores por los del vector
                anim.SetFloat("ataqueX", direccion.x);
                anim.SetFloat("ataqueY", direccion.y);

                anim.SetBool("atacar", true);
            }
        }
    }

    //19
    public void FinalizarAtaque()
    {
        anim.SetBool("atacar", false);
        bloqueado = false; //25
        estaAtacando = false;
    }

    //funcion para darle la direccion de ataque 19
    //direccion, es la que obtenemos a cada momento dentro de Movimiento() en el Update()
    //direccionMovimiento, le damos el valor en la funcion Caminar() mediante la funcion DireccionAtaque()
    private Vector2 DireccionAtaque(Vector2 direccionMovimiento, Vector2 direccion)
    {
        if (rb.velocity.x == 0 && direccion.y != 0)
            return new Vector2(0, direccion.y);

        return new Vector2(direccionMovimiento.x, direccion.y);
    }

    private IEnumerator AgitarCamara() //17
    {
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(0.3f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private IEnumerator AgitarCamara(float tiempo) //17
    {
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    //para el rodar 16
    private void Dash(float x, float y)
    {
        anim.SetBool("dash", true);
        //tenemos que obter pos. del player respecto a la cam. 16
        Vector3 posicionJugador = Camera.main.WorldToViewportPoint(transform.position);
        Camera.main.GetComponent<RippleEffect>().Emit(posicionJugador);
        StartCoroutine(AgitarCamara());

        puedeDash = true; //16
        rb.velocity = Vector2.zero; // lo frenamos para que no des nos desmadre 16
        //normalizamos el vector y lo multip. por velocidad 16
        rb.velocity += new Vector2(x, y).normalized * velocidadDash;
        StartCoroutine(PreparaDash()); //llamamos a la corroutina 16
    }

    //preparamos una corrutina 16
    private IEnumerator PreparaDash()
    {
        StartCoroutine(DashSuelo());

        rb.gravityScale = 0; //cambiamos escala gravedad de player
        haciendoDash = true;

        //esperar un tiempo mientras hace el dash
        yield return new WaitForSeconds(0.3f);  //al final del video 16 ajustamos a 0.3 en el animator

        //despues la gravedad debe de afectarle nuevamente
        //cambio el valor a 1 porque no funcionaba y lo dijeron en los foros
        //rb.gravityScale = 3;
        rb.gravityScale = 1;
        haciendoDash = false;
        FinalizarDash(); //lo pusimos para mejorar el dash al final del 16

        //estando en el suelo da problemas por eso llamamos una corrutina
    }

    //creamos otra corrutina
    private IEnumerator DashSuelo()
    {
        yield return new WaitForSeconds(0.1f); //esperamos un tiempo
        if (enSuelo) //comprabamos, es la finalidad de la corrutina
            puedeDash = false;
    }

    public void FinalizarDash() //el evento en el animator para finalizar el dash 16
    {
        anim.SetBool("dash", false);
    }

    private void TocarPiso() //16
    {
        puedeDash = false;
        haciendoDash = false;
        anim.SetBool("saltar", false);
    }

    //esta funcion esta en el Update() y en cada frame se ejecuta su interior
    public void Movimiento()
    {
        //con el getAxis se pueden tomar valores entre el 0 y el -1
        //da como resultado 1 o -1 segun la direccion que vayas
        //da el valor de las teclas de direccion o jostick
        x = Input.GetAxis("Horizontal"); 
        y = Input.GetAxis("Vertical");

        //con el GetAxisRaw se va del 0 al -1 sin valores intermedios, o del 0 al 1 ---- 16
        //el raw no hace suavizado de movimiento
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        //te da la direccion del muñeco!!!
        direccion = new Vector2(x, y);
        direccionRaw = new Vector2(xRaw, yRaw);

        //Caminar(direccion);
        Caminar();
        //direccionMovimiento la sacamos de Caminar() 19
        Atacar(DireccionAtaque(direccionMovimiento, direccionRaw));

        //50
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Agacharse();
        }
        else if(agachandose)
        {
            collider.offset = new Vector2(-0.003442526f, -0.01574576f);
            collider.size = new Vector2(1.006073f, 1.548508f);
            velocidaDeMovimiento = velocidadDeMovimientoAuxiliar;
            anim.SetBool("agachado", false);
            agachandose = false;
        }

        //22
        if(enSuelo && !haciendoDash)
        {
            saltarDeMuro = false;
        }

        //22
        agarrarse = enMuro && Input.GetKey(KeyCode.JoystickButton7);
        subirEscalera = enEscalera && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.JoystickButton5));

        //22 l
        //enSuelo añadido en 25 para corregir la doble animacion al pegarse al muro
        //al final ponemos agarrarse 25
        if (agarrarse && !enSuelo) 
        {
            anim.SetBool("escalar", true);
            if (rb.velocity == Vector2.zero)
            //if ((rb.velocity.x < 10 && rb.velocity.x > -10) || rb.velocity == Vector2.zero) //LINEA PROPIA para probar que al pegarse al muro no haga 2 animaciones
            {
                //dentro de escalar tenemos que decirle cual animacion hacer si agarrarse o escalar
                anim.SetFloat("velocidad", 0);
            }
            else
            //else if(agarrarse && rb.velocity.y > 1) //LINEA PROPIA para probar que al pegarse al muro no haga 2 animaciones
            {
                //hara anim de escalar mientras velocidad es 1
                anim.SetFloat("velocidad", 1);
            }
        }
        else
        {
            anim.SetBool("escalar", false);
            //le damos valor por defecto a la velocidad
            anim.SetFloat("velocidad", 0);

        //me, para subir escalera
        } if (subirEscalera && !enSuelo) 
        {
            anim.SetBool("escalar", true);
            if (rb.velocity == Vector2.zero)
            //if ((rb.velocity.x < 10 && rb.velocity.x > -10) || rb.velocity == Vector2.zero) //LINEA PROPIA para probar que al pegarse al muro no haga 2 animaciones
            {
                //dentro de escalar tenemos que decirle cual animacion hacer si agarrarse o escalar
                anim.SetFloat("velocidad", 0);
            }
            else
            //else if(agarrarse && rb.velocity.y > 1) //LINEA PROPIA para probar que al pegarse al muro no haga 2 animaciones
            {
                //hara anim de escalar mientras velocidad es 1
                anim.SetFloat("velocidad", 1);
            }
        }
        else
        {
            anim.SetBool("escalar", false);
            //le damos valor por defecto a la velocidad
            anim.SetFloat("velocidad", 0);
        }

        //22
        if (agarrarse && !haciendoDash)
        {
            //la gravedad no afectará al personaje verticalmente mientras esté agarrado a la pared
            rb.gravityScale = 0;
            if(x > 0.2f || x < -0.2f) //ya sea que nos movemos a dcha o izda limitaremos la velocidad en y
                rb.velocity = new Vector2(rb.velocity.x, 0);
            
            //si player mueve hacia arriba asignamos 0.5f al modificador
            float modificadorVelocidad = y > 0 ? 0.5f : 1;
            //actualizamos la velocidad del rb
            rb.velocity = new Vector2(rb.velocity.x, y * (velocidaDeMovimiento * modificadorVelocidad));

            //para cambiar vista a donde mira el player en funcion de si esta en muro dcho o izdo
            if(muroIzquierdo && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }else if(muroDerecho && transform.localScale.x < 0){
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.gravityScale = 3;

        //me, para subir escalera
        }if (subirEscalera && !haciendoDash)
        {
            //la gravedad no afectará al personaje verticalmente mientras esté agarrado a la pared
            rb.gravityScale = 0;
            if(x > 0.2f || x < -0.2f) //ya sea que nos movemos a dcha o izda limitaremos la velocidad en y
                rb.velocity = new Vector2(rb.velocity.x, 0);
            
            //si player mueve hacia arriba asignamos 0.5f al modificador
            float modificadorVelocidad = y > 0 ? 0.5f : 1;
            //actualizamos la velocidad del rb
            rb.velocity = new Vector2(rb.velocity.x, y * (velocidaDeMovimiento * modificadorVelocidad));

            //para cambiar vista a donde mira el player en funcion de si esta en muro dcho o izdo
            if(muroIzquierdo && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }else if(muroDerecho && transform.localScale.x < 0){
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.gravityScale = 3;
        }

        //22
        if(enMuro && !enSuelo)
        {
            //para que al saltar de un muro a otro haga la animacion de escalar 25
            anim.SetBool("escalar", true);

            if (x != 0 && !agarrarse)
                DeslizarPared();
        }

        MejorarSalto();

        if (Input.GetKeyDown(KeyCode.JoystickButton1)) //Saltar
        {
            if(enSuelo)
            {
                anim.SetBool("saltar", true); //modificamos el booleano saltar para las animaciones 13
                Saltar();
            }

            //22
            if(enMuro && !enSuelo)
            {
                anim.SetBool("escalar", false);
                anim.SetBool("saltar", true);
                SaltarDesdeMuro();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) //Saltar
        {
            if (enSuelo)
            {
                anim.SetBool("saltar", true); //modificamos el booleano saltar para las animaciones 13
                Saltar();
            }

            //22
            if (enMuro && !enSuelo)
            {
                anim.SetBool("escalar", false);
                anim.SetBool("saltar", true);
                SaltarDesdeMuro();
            }
        }

        //tenemos que llamar al emit() del efecto rippley 15
        //para capar el dash, que solo se haga si ha tocado piso 25
        if (Input.GetKeyDown(KeyCode.JoystickButton3) && !haciendoDash && !puedeDash)
        {
            //accedemos a la camara principal y al effecto con la posicion actual 15, en el 16 no aparece esta linea, la comento porsiaca
            //Camera.main.GetComponent<RippleEffect>().Emit(transform.position);
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if(enSuelo && !tocadoPiso) //16
        {
            anim.SetBool("escalar", false); //22
            TocarPiso();
            tocadoPiso = true;
        }

        if (!enSuelo && tocadoPiso)
            tocadoPiso = false;

        float velocidad;
        if (rb.velocity.y > 0) //14
            velocidad = 1;
        else
            velocidad = -1;

        if (!enSuelo)
        {
            //anim.SetBool("caer", false); //13 lo quitamos en el 14
            anim.SetFloat("velocidadVertical", velocidad);
        }
        else
        {
            if(velocidad == -1)
            FinalizarSalto();
        }

        //else
        //{
        //    anim.SetBool("saltar", true); //13 lo sustutiumos por un evento
        //}
    }

    //22
    private void DeslizarPared()
    {
        if (puedeMover)
            rb.velocity = new Vector2(rb.velocity.x, -velocidadDeslizar);
    }

    //22
    private void SaltarDesdeMuro()
    {
        StopCoroutine(DeshabilitarMovimiento(0));
        StartCoroutine(DeshabilitarMovimiento(0.1f));

        Vector2 direccionMuro = muroDerecho ? Vector2.left : Vector2.right;

        //se cambia direccion.x por direccionMuro.x 25
        //para cambiar la direccion del player al saltar de un muro
        if (direccionMuro.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }else if(direccionMuro.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        //float horizontal = Input.GetAxis("Horizontal"); //me
        //float vertical = Input.GetAxis("Vertical"); //me

        // Crear una dirección de salto basada en los valores del joystick
        //Vector2 direccionSalto = new Vector2(horizontal, vertical); //me

        anim.SetBool("saltar", true);
        anim.SetBool("escalar", false);
        Saltar((Vector2.up + direccionMuro), true);
        //Saltar(direccionSalto, true); //me

        saltarDeMuro = true;

        //particulas.Play(); //me
    }

    //22
    private IEnumerator DeshabilitarMovimiento(float tiempo)
    {
        puedeMover = false;
        yield return new WaitForSeconds(tiempo);
        puedeMover = true;
    }
 

    //add event que creamos para poner variables a nuestro gusto
    public void FinalizarSalto() //13
    {
        anim.SetBool("saltar", false);
        //anim.SetBool("caer", false); lo eliminamos en el 14
    }

    //Time.deltaTime es un valor útil para suavizar movimientos y hacer que el comportamiento de tu juego
    //sea independiente de la velocidad del hardware en el que se esté ejecutando.
    private void MejorarSalto()
    {
        // Si la velocidad es menor que cero estoy cayendo
        if (rb.velocity.y < 0)
        {
            //para mejorar la apariencia del salto y darle una fisica mas natural
            rb.velocity += Vector2.up * Physics2D.gravity.y * (5f - 1) * Time.deltaTime;
        }
        else if((rb.velocity.y > 0) && !Input.GetKey(KeyCode.JoystickButton1))
        {
            //para mejorar la apariencia del salto y darle una fisica mas natural
            rb.velocity += Vector2.up * Physics2D.gravity.y * (5f - 1) * Time.deltaTime;
        }
        else if ((rb.velocity.y > 0) && !Input.GetKey(KeyCode.Space))
        {
            //para mejorar la apariencia del salto y darle una fisica mas natural
            rb.velocity += Vector2.up * Physics2D.gravity.y * (5f - 1) * Time.deltaTime;
        }
    }

    public void Agarres() //para verificar que el object esta en el suelo y asi solo poder saltar cuando se esta en el suelo
    {
        //hacemos casting porque transform es un vector de 3 vectores
        //el tercer parametro para el suelo, se especifica en cual layer se detectan las colisiones
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioDeColision, layerPiso);
        //En C#, una expresión que devuelve un Collider2D se evaluará como true si el Collider2D es diferente de null, y como false si es null.

        //35 tenemos que verificar que no sea una plataforma, para que no escale en las plataformas 35
        Collider2D collisionDerecha = Physics2D.OverlapCircle((Vector2)transform.position + derecha, radioDeColision, layerPiso);
        Collider2D collisionIzquierda = Physics2D.OverlapCircle((Vector2)transform.position + izquierda, radioDeColision, layerPiso);
        //me
        Collider2D collision = Physics2D.OverlapCircle((Vector2)transform.position, radioDeColision, layerPiso);

        //tenemos que verificar si tiene el tag plataforma para que no escale 35
        if(collisionDerecha != null)
        {
            if (collisionDerecha.CompareTag("Muro"))
            {
                enMuro = true;
                muroDerecho = true;
                //tenemos que ver si el muro con el que estoy colisionando es a la derecha o a la izquierda 22
                //muroDerecho = Physics2D.OverlapCircle((Vector2)transform.position + derecha, radioDeColision, layerPiso);
            }
        }
        else if(collisionIzquierda != null)
        {
            if (collisionIzquierda.CompareTag("Muro"))
            {
                enMuro = true;
                muroIzquierdo = true;
                //tenemos que ver si el muro con el que estoy colisionando es a la derecha o a la izquierda 22
                //muroIzquierdo = Physics2D.OverlapCircle((Vector2)transform.position + izquierda, radioDeColision, layerPiso);
            }
        }
        else
        {
            enMuro = false;
            muroDerecho = false;
            muroIzquierdo = false;
        }

        //me
        if(collision != null)
        {
            if (collision.CompareTag("Escalera"))
            {
                enEscalera = true;
                Debug.Log("Toca Escalera desde funcion agarres()");
            }
        }
        else
        {
            enEscalera = false;
        }

        //tenemos que ver si el muro con el que estoy colisionando es a la derecha o a la izquierda 22
        //muroDerecho = Physics2D.OverlapCircle((Vector2)transform.position + derecha, radioDeColision, layerPiso);
        //muroIzquierdo = Physics2D.OverlapCircle((Vector2)transform.position + izquierda, radioDeColision, layerPiso);

        //lo borramos en el 35 
        //enMuro = muroDerecho || muroIzquierdo; 
    }

    public void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); //obtenemos velocidad del rigidbody
        //rb.velocity += direccion * fuerzaDeSalto;
        rb.velocity += Vector2.up * fuerzaDeSalto; //sumamos la velocidad del rb al producto del 
        //Vector2.up es un atajo para representar el vector (0, 1) en un sistema de coordenadas 2D.
    }

    //22
    public void Saltar(Vector2 direccionSalto, bool muro)
    {
        //rb.velocity = new Vector2(rb.velocity.x, 0); //obtenemos velocidad del rigidbody
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y); //obtenemos velocidad del rigidbody
        rb.velocity += direccionSalto * fuerzaDeSalto; //sumamos la velocidad del rb al producto del 
    }

    //para poder caminar y para las animaciones
    public void Caminar()
    {
        //22, lo que esta dentro del else lo metimos en el 22
        //estaAtacando 25
        if (puedeMover && !haciendoDash && !estaAtacando)
        {
            if (saltarDeMuro) //es la veloc que obtine cuando salta del muro
            {
                //Lerp interpola entre 2 vectores, o sea la velocidad actual y la otra con un tiempo que esta definifo en el 3er parametro
                rb.velocity = Vector2.Lerp(rb.velocity,
                    (new Vector2(direccionRaw.x * velocidaDeMovimiento, rb.velocity.y)), Time.deltaTime / 2);
            }
            else
            {
                //para que se pueda mover hay que darle valores al velocity del rigid body
                //rb.velocity = new Vector2(direccion.x * velocidaDeMovimiento, rb.velocity.y); //eliminamos en el 22

                if (direccionRaw != Vector2.zero && !agarrarse)
                {
                    //animaciones de caer y caminar
                    if (!enSuelo)
                    {
                        //51
                        if (agachandose)
                            anim.SetBool("caminar", true);
                        else
                            anim.SetBool("saltar", true); // si no esta en suelo hago animacion caer, para animaciones 13
                    }
                    else
                    {
                        anim.SetBool("caminar", true); //13
                    }

                    rb.velocity = (new Vector2(direccionRaw.x * velocidaDeMovimiento, rb.velocity.y)); //22

                    //para que mire a la izquierda si la direccion x es negativa
                    if (direccionRaw.x < 0 && transform.localScale.x > 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.left, direccionRaw); //19
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }

                    else if (direccionRaw.x > 0 && transform.localScale.x < 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.right, direccionRaw); //19
                                                                                         //para que mire a la derecha si la direccion x es positiva
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                        //Mathf.Abs es para buscar el valor absoluto,  es para pasar de negativo a positivo sin cambiar el valor
                    }
                }
                else
                {
                    if (direccionRaw.y > 0 && direccionRaw.x == 0) //19
                    {
                        direccionMovimiento = DireccionAtaque(direccionRaw, Vector2.up);
                    }
                    //si no estoy moviendome no activo la anim de caminar
                    anim.SetBool("caminar", false); //13
                }
            }
        }
        //el valor de bloqueado se lo vamos a adar en las animaciones 25
        else
        {
            if (bloqueado)
            {
                FinalizarAtaque();
            }
        }
    }

    private void probarControles()
    {
        if (Input.GetKey(KeyCode.JoystickButton0))
        {
            Debug.Log("JoystickButton0");
        } 
        if (Input.GetKey(KeyCode.JoystickButton1))
        {
            Debug.Log("JoystickButton1");
        } 
        if (Input.GetKey(KeyCode.JoystickButton2))
        {
            Debug.Log("JoystickButton2");
        } 
        if (Input.GetKey(KeyCode.JoystickButton3))
        {
            Debug.Log("JoystickButton3");
        } 
        if (Input.GetKey(KeyCode.JoystickButton4))
        {
            Debug.Log("JoystickButton4");
        } 
        if (Input.GetKey(KeyCode.JoystickButton5))
        {
            Debug.Log("JoystickButton5");
        } 
        if (Input.GetKey(KeyCode.JoystickButton6))
        {
            Debug.Log("JoystickButton6");
        } 
        if (Input.GetKey(KeyCode.JoystickButton7))
        {
            Debug.Log("JoystickButton7");
        } 
        if (Input.GetKey(KeyCode.JoystickButton8))
        {
            Debug.Log("JoystickButton8");
        } 
        if (Input.GetKey(KeyCode.JoystickButton9))
        {
            Debug.Log("JoystickButton9");
        } 
        if (Input.GetKey(KeyCode.JoystickButton10))
        {
            Debug.Log("JoystickButton10");
        }
        if (Input.GetKey(KeyCode.JoystickButton10))
        {
            Debug.Log("JoystickButton10");
        }
        if (Input.GetKey(KeyCode.JoystickButton11))
        {
            Debug.Log("JoystickButton11");
        }
        if (Input.GetKey(KeyCode.JoystickButton12))
        {
            Debug.Log("JoystickButton12");
        }
        if (Input.GetKey(KeyCode.JoystickButton13))
        {
            Debug.Log("JoystickButton13");
        }
        if (Input.GetKey(KeyCode.JoystickButton14))
        {
            Debug.Log("JoystickButton14");
        }
        if (Input.GetKey(KeyCode.JoystickButton15))
        {
            Debug.Log("JoystickButton15");
        }
        if (Input.GetKey(KeyCode.JoystickButton16))
        {
            Debug.Log("JoystickButton16");
        }
        if (Input.GetKey(KeyCode.JoystickButton17))
        {
            Debug.Log("JoystickButton17");
        }
        if (Input.GetKey(KeyCode.JoystickButton18))
        {
            Debug.Log("JoystickButton18");
        }
        if (Input.GetKey(KeyCode.Keypad0))
        {
            Debug.Log("Keypad0");
        }
        if (Input.GetKey(KeyCode.Keypad1))
        {
            Debug.Log("Keypad1");
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            Debug.Log("Keypad2");
        }
        if (Input.GetKey(KeyCode.Keypad3))
        {
            Debug.Log("Keypad3");
        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            Debug.Log("Keypad4");
        }
        if (Input.GetKey(KeyCode.Keypad5))
        {
            Debug.Log("Keypad5");
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            Debug.Log("Keypad6");
        }
        if (Input.GetKey(KeyCode.Keypad7))
        {
            Debug.Log("Keypad7");
        }
        if (Input.GetKey(KeyCode.Keypad8))
        {
            Debug.Log("Keypad8");
        }
        if (Input.GetKey(KeyCode.Keypad9))
        {
            Debug.Log("Keypad9");
        }
        
    }
}
