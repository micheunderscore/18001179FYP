using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour {
    public UnityEvent<Collider> onTriggerEnter;
    public void OnTriggerStay(Collider col) {
        if (onTriggerEnter != null && col.tag != transform.tag) onTriggerEnter.Invoke(col);
    }
}
