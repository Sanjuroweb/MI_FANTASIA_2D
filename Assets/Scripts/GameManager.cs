using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour //30
{
    private bool ejecutando; //46
    private bool cargandoNivel; //49
    private int indiceNivelInicio; //49

    public static GameManager instance;
    public GameObject vidasUI;
    public PlayerController player; //37
    public TextMeshProUGUI textoMonedas; //37 HECHO POR MI, cambie el tipo de Text a TextMeshProUGUI porque en el inspector no me dejaba asignar al GameManager
    public int monedas;
    public TextMeshProUGUI guardarPartidaTexto; //46

    public GameObject panelPausa; //42
    public GameObject panelGameOver; //42
    public GameObject panelCarga; //42

    //49 para corregir el proble que al cambiar de nivel la camara no le sigue
    public CinemachineConfiner cinemachineConfiner;

    public bool avanzandoNivel;
    public int nivelActual; //47
    public List<Transform> posicionesAvance = new List<Transform>(); //47
    public List<Transform> posicionesRetroceder = new List<Transform>(); //47
    public List<Collider2D> areasCamara = new List<Collider2D>(); //49 para lo del seguimiento de la camara
    public GameObject panelTransicion; //47


    //hacemos un patron Singleton
    //Awake es un método especial en Unity que se llama cuando se inicializa un objeto antes de que se ejecute el método Start.
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        //46 recordar mirar el video y como borrar los datos del jugador desde unity
        //edit/clear all PlayerPrefs
        //comentado en el 49
        /*if(PlayerPrefs.GetInt("vidas") != 0)
            CargarPartida();
        */
    }

    //49
    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "nivel1")
        {
            nivelActual = PlayerPrefs.GetInt("indiceNivelInicio");
            indiceNivelInicio = PlayerPrefs.GetInt("indiceNivelInicio");
            PosicionInicialJugador(indiceNivelInicio);
            cinemachineConfiner.m_BoundingShape2D = areasCamara[indiceNivelInicio];
        }
        else if(SceneManager.GetActiveScene().name == "LevelSelect")
        {
            PosicionInicialJugador(0);
        }
    }

    //47 lo llamaremos desde jugador
    public void ActivarPanelTransicion()
    {
        panelTransicion.GetComponent<Animator>().SetTrigger("ocultar");
    }

    //49
    private void PosicionInicialJugador(int indiceNivelInicio)
    {
        player.transform.position = posicionesAvance[indiceNivelInicio].transform.position;
    }

    //49
    public void SetIndiceNivelInicio(int indiceNivelInicio)
    {
        this.indiceNivelInicio = indiceNivelInicio;
        PlayerPrefs.SetInt("indiceNivelInicio", indiceNivelInicio);
    }
    
    //47
    public void CambiarPosicionJugador()
    {
        if (avanzandoNivel)
        {
            if(nivelActual + 1 < posicionesAvance.Count)
            {
                Debug.Log(nivelActual);
                player.transform.position = posicionesAvance[nivelActual + 1].transform.position;
                cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual + 1];
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("caminar", false);
                player.terminandoMapa = false;
            }
        }
        else
        {
            if (posicionesRetroceder.Count > nivelActual - 1)
            {
                Debug.Log(nivelActual);
                player.transform.position = posicionesRetroceder[nivelActual - 1].transform.position;
                cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual - 1];
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("caminar", false);
                player.terminandoMapa = false;
            }
        }
    }

    //46
    public void GuardarPartida()
    {
        float x, y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int vidas = player.vidas;
        //string nombreEscena = SceneManager.GetActiveScene().name; //48
        int nombreEscena = nivelActual; //49

        //en el video se comenta que esta sacado de la web de unity
        //se usa para guardar los datos del player en memoria
        PlayerPrefs.SetInt("monedas", monedas);
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetInt("vidas", vidas);
        //PlayerPrefs.SetString("nombreEscena", nombreEscena);  //48
        PlayerPrefs.SetInt("nivel", nombreEscena);  //49
        PlayerPrefs.SetInt("indiceNivelInicio", indiceNivelInicio);  //49

        //disparamos el texto de guardado
        if (!ejecutando)
            StartCoroutine(MostrarTextoGuardado());
    }

    //46 para que se muestre el texto de partida guardad solo por un ratito
    private IEnumerator MostrarTextoGuardado()
    {
        ejecutando = true; //para que no se superponga la ejecucion de corrutinas
        guardarPartidaTexto.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        guardarPartidaTexto.gameObject.SetActive(false);
        ejecutando = false;
    }

    //48
    public void CargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    //46 cargamos los datos guardados en PlayerPrefs
    //tenemos que llamarla desde el Awake()
    public void CargarPartida()
    {
        monedas = PlayerPrefs.GetInt("monedas");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.vidas = PlayerPrefs.GetInt("vidas");
        textoMonedas.text = monedas.ToString();
        nivelActual = PlayerPrefs.GetInt("nivel");
        cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual]; //49
        indiceNivelInicio = PlayerPrefs.GetInt("indiceNivelInicio"); //49

        //48 en el inspector cambio la referencia en el Canvas/Panel_Game_Over/Button_Continuar
        /*if (PlayerPrefs.GetString("nombreEscena") == string.Empty)
            SceneManager.LoadScene("LevelSelect");
        else
            SceneManager.LoadScene(PlayerPrefs.GetString("nombreEscena"));
        */

        int vidasADescontar = 3 - player.vidas; //46

        player.MostrarVidasUI();
        player.ActualizarVidasUI(vidasADescontar); //46
    }

    //37
    public void ActualizarContadorMonedas()
    {
        monedas++;
        textoMonedas.text = monedas.ToString();
    }

    //42
    public void PausarJuego()
    {
        //para que el juego quede pausado
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }
    
    //42
    public void DesPausarJuego()
    {
        //para que el juego continue
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    //42
    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    
    //42
    public void CargarSelector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }

    //49
    public void CargarEscena(string escenaACargar)
    {
        StartCoroutine(CargarEscenaCorrutina(escenaACargar));
    }

    //42
    public IEnumerator CargarEscenaCorrutina(string escenaACargar)
    {
        cargandoNivel = true; //49
        //SceneManager.LoadScene(escenaACargar); borrado en el 49

        //49 para que sea sincrono
        panelCarga.SetActive(true);

        AsyncOperation asyncload = SceneManager.LoadSceneAsync(escenaACargar);

        //wait until the asynchronous scene fully loads
        while (asyncload.isDone)
        {
            //cuando tengamos una escena mas grande durará más tiempo la pantalla de carga
            yield return null;
            //para hacer que la pantalla de carga dure mas tiempo artificialmente
            //yield return new WaitForSeconds(1);
        }
        //PosicionInicialJugador(indiceNivelInicio); //comentada en el 49
        cargandoNivel = false;
    } 
    
    //42
    public void GameOver()
    {
        panelGameOver.SetActive(true);

        //49
        //el if que habia aqui lo cortamos y pegamos en la funcion de abajo ContinuarJuego()
    }

    //49
    public void ContinuarJuego()
    {
        //49
        if (PlayerPrefs.GetFloat("x") != 0.0f)
        {
            player.enabled = true;
            CargarPartida();
            panelGameOver.SetActive(false);
        }
    }
    
    //42
    public void SalirDelJuego()
    {
        //para salir del juego
        Application.Quit();
    }

    //42
    public void CargarEscenaSelector()
    {
        StartCoroutine(CargarEscena());
    }

    //42
    private IEnumerator CargarEscena()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncload = SceneManager.LoadSceneAsync("LevelSelect");

        //wait until the asynchronous scene fully loads
        while (asyncload.isDone)
        {
            //cuando tengamos una escena mas grande durará más tiempo la pantalla de carga
            yield return null;
            //para hacer que la pantalla de carga dure mas tiempo artificialmente
            //yield return new WaitForSeconds(1);
        }
    }
}
