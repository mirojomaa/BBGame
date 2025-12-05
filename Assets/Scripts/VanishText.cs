using UnityEngine;
public class VanishText : MonoBehaviour
{  
   [SerializeField] private float duration;
   private float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > duration)
        {
            timer = 0;
           gameObject.SetActive(false);
        }
    }
}