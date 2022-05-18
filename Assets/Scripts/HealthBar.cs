using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    public async void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
    public void SetCurrentHealth(int currentHealth)
    {
        slider.value = currentHealth;
        Debug.Log(slider.value);
    }
}
