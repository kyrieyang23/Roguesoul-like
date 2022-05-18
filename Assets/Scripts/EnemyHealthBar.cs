using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    float timeUntilBarIsHidden = 0;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        // slider.gameObject.SetActive(true);
    }

    public void SetHealth(int health)
    {
        Debug.Log(health+"EEEE");
        slider.value = health;
        timeUntilBarIsHidden = 3;
    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    private void Update()
    {
        timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;

        if (slider != null)
        {
            if (timeUntilBarIsHidden <= 0)
            {
                timeUntilBarIsHidden = 0;
                slider.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("hearerere");
                if (!slider.gameObject.activeInHierarchy)
                {
                    Debug.Log("hearerere");
                    slider.gameObject.SetActive(true);
                }
            }

            if (slider.value <= 0)
            {
                Destroy(slider.gameObject);
            }
        }
    }
}
