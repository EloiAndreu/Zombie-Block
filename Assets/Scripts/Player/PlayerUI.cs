using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//COMPORTAMENT DE LA INTERFIÍCI DEL JUGADOR
public class PlayerUI : MonoBehaviour
{
    public TMP_Text munitionText;
    public List<GameObject> gunImatges;

    void Start(){
        munitionText.gameObject.SetActive(false);

        AmagarTotesLesGunImatges();
    }

    void Update(){
        //Si el jugador ha agafat un objecte... 
        //Mostrem el text de munició i la imatge de l'arma corresponent 
        if(GetComponent<Pickup>().obj != null){
            
            GameObject obj = GetComponent<Pickup>().obj;
            if(obj.GetComponent<Gun>()){
                
                munitionText.gameObject.SetActive(true);

                Gun gun = obj.GetComponent<Gun>();
                int gunID = gun.gunID;
                gunImatges[gunID].SetActive(true);

                float maxBales = gun.maxBales;
                float currentBales = gun.currentBales;

                munitionText.text = currentBales + " / " + maxBales;
            }
        }
        else{ //Si el jugador no té cap objecte... amaguem el text i la imatge
            munitionText.gameObject.SetActive(false);

            AmagarTotesLesGunImatges();
        }
    }

    void AmagarTotesLesGunImatges(){
        for(int i=0; i<gunImatges.Count; i++){
            gunImatges[i].SetActive(false);
        }
    }
}
