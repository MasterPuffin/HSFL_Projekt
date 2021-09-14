using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreRotation : MonoBehaviour {
    // Start is called before the first frame update
    private float offset = 1f;
    private Transform parent;

    private void Start() {
        parent = transform.parent;
    }

    void Update() {
        transform.rotation = Quaternion.Euler(0.0f, parent.rotation.eulerAngles.y, 0.0f);
        transform.position = new Vector3(parent.position.x, parent.position.y - offset, parent.position.z);
    }
}