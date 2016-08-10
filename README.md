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

External Links
----
* PVRTC Encode/Decode impl (C#) : https://bitbucket.org/Agent_007/pvrtc-encoder-decoder-for-unity
* impl (C++) : https://bitbucket.org/jthlim/pvrtccompressor
