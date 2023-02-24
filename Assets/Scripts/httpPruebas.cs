using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class httpPruebas : MonoBehaviour
{
    [SerializeField] private string ServerApiURL;

     
    
    
    void Start()
    {
        
    }

    public void Registrar()
    {
        UserPrueba usuario = new UserPrueba();

        usuario.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        usuario.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(usuario);
        
        StartCoroutine(Registro(postData));
    }

    IEnumerator Registro(string postData)
    {
        
        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL+"/api/usuarios",postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type","application/json");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
           
            Debug.Log(www.downloadHandler.text);

            string json = www.downloadHandler.text;

            if (www.responseCode == 200) //funciona
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);
                
                Debug.Log(jsonData.usuario.username + " se registro con id "+ jsonData.usuario._id);
                //como obtenemos el token?

            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }

        }
    }
}

[System.Serializable]
public class UserPrueba
{
    public string _id;
    public string username;
    public string password;
    public int score;

    public UserPrueba()
    {
        
    }
    public UserPrueba(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
    
}

public class AuthJsonDataPrueba
{
    public UserPrueba usuario;
    public string token; 
}
