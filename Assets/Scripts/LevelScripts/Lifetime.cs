using UnityEngine;
public class Lifetime : MonoBehaviour
{
    float time = 8, timer = 0;
    void Start() =>  timer = 0; 
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > time) Destroy(this.gameObject);
    }
}
