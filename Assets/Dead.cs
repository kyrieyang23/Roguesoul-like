using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : MonoBehaviour
{
    Animator animator;
    public Material dissolve;
    string t = "Vector1_b7409059c6ee40c690f5431d86a5fe7a";
    private float dt = 0;
    public float speed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.CrossFade("Dead_01", 0.2f);
        // Debug.Log(dissolve.shader.GetPropertyName(3));
    }

    // Update is called once per frame
    void Update()
    {
        if(dt < 1)
        {
            dt += Time.deltaTime * speed;
            dissolve.SetFloat(t, dt);
        }
        else
        {
            dissolve.SetFloat(t, 0);
            Destroy(gameObject);
        }
        
    }

}
