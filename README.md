# Unity.PVRTC
runtime texture compression

Usage
----
```cs
var www = new WWW("http://my_image.com");
yield return www;

var comp = RPVRTC.Compress24(www.texture);
yield return comp;

// DONE!
Texture2D compressedTexture = comp.data;
```
