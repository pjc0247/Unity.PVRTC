using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class RPVRTC {

  public static SignalEnumerator<Texture2D> Compress24(Texture2D tex)
  {
      var signal = new SignalEnumerator<Texture2D>();
      StartCoroutine(_Compress24(tex, signal));
      return signal;
  }
  private static IEnumerator _Compress24(Texture2D tex, SignalEnumerator<Texture2D> signal)
  {
      var width = tex.width;
      var height = tex.height;
      var resultTexture = new Texture2D(width, height, TextureFormat.PVRTC_RGB4, false, true);
      byte[] compressed = null;
      Color[] pixels = tex.GetPixels();
  
      yield return new AsyncExecutor<object>(x =>
      {
          PvrtcCompress compressor = new PvrtcCompress();
          compressed = compressor.EncodeRgb4Bpp(pixels, width, height);
      });
  
      resultTexture.LoadRawTextureData(compressed);
      resultTexture.Apply();
      
      signal.Notify(resultTexture);
  }

}
