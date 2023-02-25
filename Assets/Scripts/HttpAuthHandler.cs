using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using  System.Linq;

public class HttpAuthHandler : MonoBehaviour
{

    [SerializeField]
    private string ServerApiURL;
    
    [SerializeField]
    private GameObject canva1;
    [SerializeField]
    private GameObject canva2;
    
    [SerializeField]
    private GameObject Scoretext;
    
    [SerializeField]
        private GameObject UsernameText;
    

    public string Token { get; set; }
    public string Username { get; set; }

    // Start is called before the first frame update
    void Start()
    {
       // List<User> lista = new List<User>();
       // List<User> listaOrdenada = lista.OrderByDescending(u => u.data.score).ToList<User>();
       //Scoretext.GetComponent<TMP_Text>().text="hola bb \n sisas";

        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
            //Ir a Login
        }
        else
        {
            Debug.Log(Token);
            Debug.Log(Username);
            StartCoroutine(GetPerfil());
            
        }
        

    }

    public void Registrar()
    {
        User user = new User();
        user.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        user.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Registro(postData));
    }

    public void Ingresar()
    {
        User user = new User();
        user.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        user.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Login(postData));
    }
    IEnumerator Registro(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "/api/usuarios",postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " se regitro con id " + jsonData.usuario._id);
                //Proceso de autenticacion
                canva1.gameObject.SetActive(false);
                canva2.gameObject.SetActive(true);
                UsernameText.GetComponent<TMP_Text>().text+=" "+Username;
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
    IEnumerator Login(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "/api/auth/login", postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " inicio sesion");

                Token = jsonData.token;
                Username = jsonData.usuario.username;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);
                
                canva1.SetActive(false);
                canva2.SetActive(true);
                
                UsernameText.GetComponent<TMP_Text>().text+=" "+Username;

            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
    IEnumerator GetPerfil()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiURL + "/api/usuarios/"+Username);
        www.SetRequestHeader("x-token", Token);


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " Sigue con la sesion inciada");
                //Cambiar de escena
                
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
    IEnumerator ListScores()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiURL + "/api/usuarios");
        www.SetRequestHeader("x-token", Token);


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                Scoretext.GetComponent<TMP_Text>().text="hola bb";
                
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
    IEnumerator ModificarScore(string postData,string usuario )
    {
        
        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "/api/usuarios/"+usuario, postData);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");


        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {
                User usuarioTemp = JsonUtility.FromJson<User>(www.downloadHandler.text);

                usuarioTemp.data.score=int.Parse(GameObject.Find("AdicionarScore").GetComponent<TMP_InputField>().text);
                Scoretext.GetComponent<TMP_Text>().text=""+usuarioTemp.data.score;
                
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
}


[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public string password;
    public UserData data;

    public User()
    {
        
    }
    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[SerializeField]
public class UserData
{
    public int score;// si no hay nada es cero 
}

public class AuthJsonData
{
    public User usuario;
    public string token;
}

