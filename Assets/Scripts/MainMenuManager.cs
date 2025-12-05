using UnityEngine;
public class MainMenuManager : MonoBehaviour
{
   private string FadeOut = "FadeOut";
   private Animator menuAnim;

   void Awake() => menuAnim = GetComponent<Animator>();
   public void MenuFade() => menuAnim.SetTrigger(FadeOut);
}

