using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;


public class main_menu : MonoBehaviour
{
    public TMP_InputField inputField1;
    public TMP_InputField inputField2;
    public TMP_InputField inputField3;
    public TMP_InputField inputField4;
    public TMP_InputField inputField5;
    public TMP_InputField inputField6;
    public TMP_InputField inputField7;
    public TMP_InputField inputField8;
    public float NumberOfParticles;
    public float Particle_Raduis;
    public float Bound_Damping;
    public float Viscosity;
    public float Gas_Constant;
    public float Resting_Density;
    public float Gravity;
    public float Wind_Speed;


    public void exitButton(){
        Application.Quit();
        Debug.Log("game closed");
    }

    public void startButton(){

        if (float.TryParse(inputField1.text, out NumberOfParticles) && 
            float.TryParse(inputField2.text, out Particle_Raduis)&&
            float.TryParse(inputField3.text, out Bound_Damping) &&
            float.TryParse(inputField4.text, out Viscosity) && 
            float.TryParse(inputField5.text, out Gas_Constant) && 
            float.TryParse(inputField6.text, out Resting_Density) &&
            float.TryParse(inputField7.text, out Gravity) && 
            float.TryParse(inputField8.text, out Wind_Speed))
        {

            Staticdata.numToSpawn=NumberOfParticles;
            Staticdata.particleRadius = Particle_Raduis;
            Staticdata.boundDamping = Bound_Damping;
            Staticdata.viscosity = Viscosity;
            Staticdata.gasConstant = Gas_Constant;
            Staticdata.restingDensity = Resting_Density;
            Staticdata.gravity = Gravity;
            Staticdata.windSpeedValue = Wind_Speed;
            
            SceneManager.LoadScene("Simulator");

        }
        else
        {
            Debug.LogError("Invalid input. Please enter valid numbers.");
        }
    }
}
