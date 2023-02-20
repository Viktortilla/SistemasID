using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Peticiones01 : MonoBehaviour
{
    [SerializeField] RawImage[] YourRawImage;
    [SerializeField] RawImage CuadroDeImagen;
    [SerializeField] int userId = 1;
    [SerializeField] string myApiPath = "https://my-json-server.typicode.com/HeroFromThePast/FakeApi2023";
    [SerializeField] string rickYMortyApi = "https://rickandmortyapi.com/api";
    int[] downloadedDeck = new int[5];
    int imageIndex = 0;
    
    [SerializeField] int[] numeroDelPersonaje = {1,2,3,4,5};
    public void PresionarBoton()
    {
        StartCoroutine(ConsultarRickyMorty());
        StartCoroutine(ConsultarJsonRYM());
        StartCoroutine(ConsultarJsonRYMManual());
    }
    IEnumerator ConsultarRickyMorty()
    {
        UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi );//leer URL
        yield return www.Send();//Esperar respuesta 

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }//errores de salida
        else//en caso de que funcione 
        {
            

            if (www.responseCode == 200) //funciona
            {
                Debug.Log(www.GetResponseHeader("content-type"));//pregunta el tipo, en este caso application Json
                Debug.Log(www.downloadHandler.text);// resibiras informacion como la direccion url de los personajes, episodios, etc...
                

            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }//errores 

        }
    }
    
    IEnumerator ConsultarJsonRYM()
    {
        UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi+"/character" );//leer URL, recuerda estar en la seccion correcta /usuarios /ejemplos etc
        yield return www.Send();
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            
            if (www.responseCode == 200)
            {
                
                //toma la informacion del Json y conectala con el personaje modelo 
                ListaDePersonajesModelo personajesDelJson = JsonUtility.FromJson<ListaDePersonajesModelo>(www.downloadHandler.text);
                
                //preguntemos cuantos personajes hay 
                Debug.Log(personajesDelJson.info.count);
                
                //consultar nombres (por protecciones de la api solo te mandaara los primeros 20)
                foreach (PersonajeModelo personajeTemporalDelJson in personajesDelJson.results)
                {
                    Debug.Log("Name"+personajeTemporalDelJson.name);
                    
                }
                //pediremos solamente al primer personaje y mostraremos su imagen
                foreach (PersonajeModelo personajeTemporalDelJson in personajesDelJson.results)
                {
                    Debug.Log("Name "+personajeTemporalDelJson.name);
                    Debug.Log("ImageURL "+personajeTemporalDelJson.image);
                    StartCoroutine(MostrarUnaImagen(personajeTemporalDelJson.image));
                    break;//si no lo detienes las imagenes se colocaran en orden de llegada quedando la ultima en llegar

                }

                

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
    IEnumerator ConsultarJsonRYMManual()
    {
        UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi+"/character" );//leer URL, recuerda estar en la seccion correcta /usuarios /ejemplos etc
        yield return www.Send();
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            
            if (www.responseCode == 200)
            {
                
                //toma la informacion del Json y conectala con el personaje modelo 
                ListaDePersonajesModelo personajesDelJson = JsonUtility.FromJson<ListaDePersonajesModelo>(www.downloadHandler.text);
                
                //preguntemos cuantos personajes hay 
                Debug.Log(personajesDelJson.info.count);
                
                //consultar nombres (por protecciones de la api solo te mandaara los primeros 20)
                foreach (PersonajeModelo personajeTemporalDelJson in personajesDelJson.results)
                {
                    Debug.Log("Name"+personajeTemporalDelJson.name);
                    
                }
                //pediremos solamente al primer personaje y mostraremos su imagen
                foreach (PersonajeModelo personajeTemporalDelJson in personajesDelJson.results)
                {
                    Debug.Log("Name "+personajeTemporalDelJson.name);
                    Debug.Log("ImageURL "+personajeTemporalDelJson.image);
                    StartCoroutine(MostrarUnaImagen(personajeTemporalDelJson.image));
                    break;//si no lo detienes las imagenes se colocaran en orden de llegada quedando la ultima en llegar

                }
                

                for (int i = 0; i < numeroDelPersonaje.Length; i++)
                {
                    Debug.Log(personajesDelJson.results[numeroDelPersonaje[i]].name);
                    Debug.Log(personajesDelJson.results[numeroDelPersonaje[i]].id);
                    
                    StartCoroutine(DownloadImage(personajesDelJson.results[numeroDelPersonaje[i]].image,i));
                }
                
                

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
    IEnumerator MostrarUnaImagen(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else CuadroDeImagen.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
    IEnumerator DownloadImage(string MediaUrl, int imageIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else YourRawImage[imageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    IEnumerator GetPlayerInfo()
    {
        
        UnityWebRequest www = UnityWebRequest.Get(myApiPath + "/users/" + userId);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            string json = www.downloadHandler.text;

            if (www.responseCode == 200) //funciona
            {
                UserJsonData user = JsonUtility.FromJson<UserJsonData>(json);
                Debug.Log(user.name);
                Debug.Log(user.deck);

                
                for (int i = 0; i < user.deck.Length; i++)
                {
                    StartCoroutine(GetCharacters(user.deck[i], i));
                }
                

               

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


    IEnumerator GetCharacters(int charId, int imageIndex)
    {
        UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi + "/character/" + charId);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;

            if (www.responseCode == 200) //funciona
            {
                Character character = JsonUtility.FromJson<Character>(json);
                Debug.Log(character.name);

                StartCoroutine(DownloadImage(character.image, imageIndex));

               
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
//Clases modelo para consultar Json
/*
public class UserJsonData
{
    public int id;
    public string name;
    public int[] deck;
}





*/
[System.Serializable]//esto es para que nos permita consultar la informacion
public class ListaDePersonajesModelo
{
    public ListaDeInformacionDePersonajesModelo info;
    public List<PersonajeModelo> results;

}
[System.Serializable]//esto es para que nos permita consultar la informacion
public class ListaDeInformacionDePersonajesModelo
{
    public int count;
    public int pages;
    public string next;
    public string prev;
}//los nombres de las propiedades deben ser iguales a los de la api 
[System.Serializable]//esto es para que nos permita consultar la informacion
public class PersonajeModelo
{
    public int id;
    public string name;
    public string species;
    public string image;
}//esta clase debe tener las propiedades de los personajes en la api
//los nombres de las propiedades deben ser iguales a los de la api 