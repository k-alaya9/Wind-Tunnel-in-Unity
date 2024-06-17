using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UImanager : MonoBehaviour
{

    public Slider particleRadiusSlider;
    public Slider boundDampingSlider;
    public Slider viscositySlider;
    public Slider gasConstantSlider;
    public Slider restingDensitySlider;
    public Slider gravitySlider;
    public Slider windSpeedSlider;
    public TextMeshProUGUI particleCountText;
    public TextMeshProUGUI particleRadiusText;
    public TextMeshProUGUI boundDampingText;
    public TextMeshProUGUI viscosityText;
    public TextMeshProUGUI gasConstantText;
    public TextMeshProUGUI restingDensityText;
    public TextMeshProUGUI gravityText;
    public TextMeshProUGUI windSpeedText;

    public spawner Spawner;
    void Start()
    {
         // Initialize slider values and add listeners
        particleRadiusSlider.minValue = 0.1f;
        particleRadiusSlider.maxValue = 10f;

        boundDampingSlider.minValue = -1f;
        boundDampingSlider.maxValue = 1f;

        viscositySlider.minValue = 0f;
        viscositySlider.maxValue = 10f;

        gasConstantSlider.minValue = 0f;
        gasConstantSlider.maxValue = 10f;

        restingDensitySlider.minValue = 0f;
        restingDensitySlider.maxValue = 1000f;

        gravitySlider.minValue = -20f;
        gravitySlider.maxValue = 20f;


        windSpeedSlider.minValue = 0f;
        windSpeedSlider.maxValue = 1000f;
        windSpeedSlider.value = spawner.WindSpeedValue;
                // Initialize slider values and add listeners
        particleRadiusSlider.value = Spawner.ParticleRadius;
        boundDampingSlider.value = Spawner.BoundDamping;
        viscositySlider.value = Spawner.Viscosity;
        gasConstantSlider.value = Spawner.GasConstant;
        restingDensitySlider.value = Spawner.RestingDensity;
        gravitySlider.value = Spawner.Gravity;

        particleRadiusSlider.onValueChanged.AddListener(UpdateParticleRadius);
        boundDampingSlider.onValueChanged.AddListener(UpdateBoundDamping);
        viscositySlider.onValueChanged.AddListener(UpdateViscosity);
        gasConstantSlider.onValueChanged.AddListener(UpdateGasConstant);
        restingDensitySlider.onValueChanged.AddListener(UpdateRestingDensity);
        gravitySlider.onValueChanged.AddListener(UpdateGravity);
        windSpeedSlider.onValueChanged.AddListener(UpdateWindSpeed);
        
    }
    void Update()
    {
         if (Spawner != null)
        {
            particleCountText.text = "Particle Count: " + Spawner.numParticles;
            particleRadiusText.text = "Particle Radius: " + Spawner.ParticleRadius;
            boundDampingText.text = "Bound Damping: " + Spawner.BoundDamping;
            viscosityText.text = "Viscosity: " + Spawner.Viscosity;
            gasConstantText.text = "Gas Constant: " + Spawner.GasConstant;
            restingDensityText.text = "Resting Density: " + Spawner.RestingDensity;
            gravityText.text = "Gravity: " + Spawner.Gravity;
            windSpeedText.text = "Wind Speed: " + spawner.WindSpeedValue;
        }
    }
       public void UpdateParticleRadius(float value)
    {
        if (Spawner != null)
        {
            Spawner.ParticleRadius = value;
        }
    }

    public void UpdateBoundDamping(float value)
    {
        if (Spawner != null)
        {
            Spawner.BoundDamping = value;
        }
    }

    public void UpdateViscosity(float value)
    {
        if (Spawner != null)
        {
            Spawner.Viscosity = value;
        }
    }

    public void UpdateGasConstant(float value)
    {
        if (Spawner != null)
        {
            Spawner.GasConstant = value;
        }
    }

    public void UpdateRestingDensity(float value)
    {
        if (Spawner != null)
        {
            Spawner.RestingDensity = value;
        }
    }

    public void UpdateGravity(float value)
    {
        if (Spawner != null)
        {
            Spawner.Gravity = value;
        }
    }
   public void UpdateWindSpeed(float value)
    {
        if (Spawner != null)
        {
            spawner.WindSpeedValue = value;
        }
    }
}
