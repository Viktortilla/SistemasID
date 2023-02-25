using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using  System.Linq;

public class Actividad2 : MonoBehaviour
{
    [SerializeField] private string ServerApiURL;
    
    [SerializeField] private GameObject pantalla1;
    [SerializeField] private GameObject pantalla2;
    
    [SerializeField] private GameObject usernameText;//text
    [SerializeField] private GameObject listScoreText;//text
    [SerializeField] private GameObject erroresText;//text
    
    
    public Cliente clienteTemporal = new Cliente();
    public string Token { get; set; }
    public string Username { get; set; }

    void Start()
    {
        
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
        
        pantalla1.SetActive(true);
        pantalla2.SetActive(false);
        
        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
            erroresText.GetComponent<TMP_Text>().text="No hay token";
            //Ir a Login
        }
        else
        {
            Debug.Log(Token);
            Debug.Log(Username);
            StartCoroutine(GetPerfil());
        }
        
        
    }

    public void CambioDeEsena()
    {
        
        if (pantalla1.activeSelf==true)
        {
            
            pantalla1.SetActive(false);
            pantalla2.SetActive(true);
        }
        else
        {
            
            pantalla1.SetActive(true);
            pantalla2.SetActive(false);
        }
        
    }
    
    public void Registrar()
    {
        
        Cliente cliente = new Cliente();
        cliente.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        cliente.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(cliente);
        Debug.Log("data de registro = "+postData);
        StartCoroutine(SignIn(postData));
    }

    public void Ingresar()
    {
        Cliente cliente = new Cliente();
        cliente.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        cliente.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(cliente);
        StartCoroutine(LogIn(postData));
    }
    public void AddScore()
    {
        
        Cliente cliente = new Cliente();
        cliente.username = Username;
        cliente.data.score=int.Parse(GameObject.Find("AdicionarScore").GetComponent<TMP_InputField>().text);
        string postData = JsonUtility.ToJson(cliente);
        Debug.Log(postData);
        StartCoroutine(AddScore(postData));
    }
    
    IEnumerator SignIn(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "/api/usuarios",postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
            erroresText.GetComponent<TMP_Text>().text="NETWORK ERROR :" + www.error;
        }
        else
        {
            Debug.Log("handler = "+www.downloadHandler.text);
            erroresText.GetComponent<TMP_Text>().text =www.downloadHandler.text;

            if (www.responseCode == 200)
            {

                AutentificacionJsonData jsonData = JsonUtility.FromJson<AutentificacionJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " se regitro con id " + jsonData.usuario._id);
                erroresText.GetComponent<TMP_Text>().text =jsonData.usuario.username + " \n id = " + jsonData.usuario._id;
                //Proceso de autenticacion

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
    IEnumerator LogIn(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "/api/auth/login", postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
            erroresText.GetComponent<TMP_Text>().text="NETWORK ERROR :" + www.error;
        }
        else
        {
            Debug.Log("handler = "+www.downloadHandler.text);
            erroresText.GetComponent<TMP_Text>().text =www.downloadHandler.text;

            if (www.responseCode == 200)
            {

                AutentificacionJsonData jsonData = JsonUtility.FromJson<AutentificacionJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " inicio sesion");
                erroresText.GetComponent<TMP_Text>().text = jsonData.usuario.username + " inicio sesion \n cargando...";
                

                Token = jsonData.token;
                Username = jsonData.usuario.username;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);
                
                
                clienteTemporal.username = jsonData.usuario.username;
                
                
                Invoke("CambioDeEsena",2);
                usernameText.GetComponent<TMP_Text>().text=" usuario = "+jsonData.usuario.username;

                


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
    IEnumerator AddScore(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "/api/usuarios/", postData);
        www.method = "PATCH";
        www.SetRequestHeader("x-token", Token);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
            erroresText.GetComponent<TMP_Text>().text="NETWORK ERROR :" + www.error;
        }
        else
        {
            Debug.Log("handler = "+www.downloadHandler.text);
            erroresText.GetComponent<TMP_Text>().text =www.downloadHandler.text;

            if (www.responseCode == 200)
            {

                AutentificacionJsonData jsonData = JsonUtility.FromJson<AutentificacionJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " - nuevo score incluido, Score = "+ jsonData.usuario.data.score);

                StartCoroutine(ListScore());

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

                AutentificacionJsonData jsonData = JsonUtility.FromJson<AutentificacionJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " Sigue con la sesion inciada");
                usernameText.GetComponent<TMP_Text>().text=" usuario = "+jsonData.usuario.username;
                StartCoroutine(ListScore());
                CambioDeEsena();
                

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
    IEnumerator ListScore()
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
                
                //Debug.Log("A");
                
                ListaDeClientes usuarios = JsonUtility.FromJson<ListaDeClientes>(www.downloadHandler.text);
                

                
                List<Cliente> lista = usuarios.usuarios;
                List<Cliente> listaOrdenada = lista.OrderByDescending(u => u.data.score).ToList<Cliente>();

                listScoreText.GetComponent<TMP_Text>().text = "";
                

                foreach (Cliente i in listaOrdenada)
                {
                    listScoreText.GetComponent<TMP_Text>().text += i.username + " score = " + i.data.score+"\n";
                    Debug.Log(i.username + " score = " + i.data.score);
                }
                
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
public class Cliente
{
    public string _id;
    public string username;
    public string password;
    public DataDelCliente data;

    public Cliente()
    {
        data = new DataDelCliente();
    }
    public Cliente(string username, string password)
    {
        this.username = username;
        this.password = password;
        data = new DataDelCliente();
    }
}

[System.Serializable]
public class DataDelCliente
{
    public int score;// si no hay nada es cero 
}

public class AutentificacionJsonData
{
    public Cliente usuario;
    public string token;
}
[System.Serializable]
public class ListaDeClientes
{
    public List<Cliente> usuarios;
}

