using UnityEngine;
public static class ExtensionMethods
{//Eine methode von Unity um für alle Objekte einen Nullcheck zu machen
	public static T Ref<T>(this T obj) where T : Object => obj == null ? null : obj;
	public static float AspectRatio(this Texture texture) => texture.width / texture.height;
	public static float AtLeast(this float v, float minVal) => Mathf.Max(v, minVal);// Bottom wird geclampt
}