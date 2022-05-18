using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharactorCollision : MonoBehaviour
{
    public CapsuleCollider charactorCollider;
    public CapsuleCollider charactorBlockerCollider;

    void Start()
    {
        Physics.IgnoreCollision(charactorCollider,charactorBlockerCollider,true);
    }


}
