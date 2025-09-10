using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** Script para cargar escenas de Unity, ademas de realizar limpiezas iniciales
 * donde sea necesario.
 * Esteban.Hernandez
 */
public class SceneLoader : MonoBehaviour {

	void Start(){

	}
	public void myOwnLoadScene(int index)
	{
		SceneManager.LoadScene(index);
	}
	
	public void FinishGame(){
		Application.Quit ();
	}

}
