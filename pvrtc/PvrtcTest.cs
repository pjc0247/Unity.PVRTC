using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class PvrtcTest : MonoBehaviour {

	// Original textures
	public Texture2D globeTexture; // 24 bit RGB texture

	public Texture2D alphaTexture; // 32 bit RGBA texture


	// Compressed PVRTC textures
	private Texture2D pvrtcGlobeTexture = null;

	private Texture2D pvrtcAlphaTexture = null;


	// Decompressed PVRTC textures
	private Texture2D decompressedGlobeTexture = null;

	private Texture2D decompressedAlphaTexture = null;

	// Use this for initialization
	void Start () {
		this.CreatePvrtcAndUncompressedTextures();
	} 
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// Don't do drawing if textures aren't ready yet
		if (this.pvrtcGlobeTexture == null || this.pvrtcAlphaTexture == null || this.decompressedGlobeTexture == null || this.decompressedAlphaTexture == null) 
		{
			return;
		}

		// First the 24 bit RGB globe texture
		GUI.Label(new Rect(40, 10, 150, 30), "RGB 24 bit Globe");
		GUI.DrawTexture(new Rect(30, 35, this.globeTexture.width, this.globeTexture.height), this.globeTexture);

		// Then RGB 4 bit PVRTC version of globe texture
		GUI.Label(new Rect(30, 180, 150, 30), "RGB 4 bit PVRTC Globe");
		GUI.DrawTexture(new Rect(30, 205, this.pvrtcGlobeTexture.width, this.pvrtcGlobeTexture.height), this.pvrtcGlobeTexture);

		// Finally decompressed globe texture
		GUI.Label(new Rect(15, 350, 250, 30), "RGB 24 bit Globe (decompressed)");
		GUI.DrawTexture(new Rect(30, 375, this.decompressedGlobeTexture.width, this.decompressedGlobeTexture.height), this.decompressedGlobeTexture);


		// First the 32 bit RGBA alpha texture
		GUI.Label(new Rect(245, 10, 150, 30), "RGBA 32 bit Alpha");
		GUI.DrawTexture(new Rect(230, 35, this.alphaTexture.width, this.alphaTexture.height), this.alphaTexture);
		
		// Then RGBA 4 bit PVRTC version of alpha texture
		GUI.Label(new Rect(240, 305, 200, 30), "RGBA 4 bit PVRTC Alpha");
		GUI.DrawTexture(new Rect(230, 330, this.pvrtcAlphaTexture.width, this.pvrtcAlphaTexture.height), this.pvrtcAlphaTexture);

		// Finally decompressed alpha texture
		GUI.Label(new Rect(240, 595, 250, 30), "RGBA 32 bit Alpha (decompressed)");
		GUI.DrawTexture(new Rect(230, 620, this.decompressedAlphaTexture.width, this.decompressedAlphaTexture.height), this.decompressedAlphaTexture);
	}

	private void CreatePvrtcAndUncompressedTextures()
	{
        /*
		PvrtcCompress compressor = new PvrtcCompress();

		// First compress globe texture to RGB 4 bit PVRTC
		//byte[] globeTextureData = compressor.EncodeRgb4Bpp(this.globeTexture);

		// Create new RGB 4 bit PVRTC texture from compressed texture data
		this.pvrtcGlobeTexture = new Texture2D(this.globeTexture.width, this.globeTexture.height, TextureFormat.PVRTC_RGB4, false, true);
		this.pvrtcGlobeTexture.LoadRawTextureData(globeTextureData);
		this.pvrtcGlobeTexture.Apply();


		// Then compress alpha texture to RGBA 4 bit PVRTC
		byte[] alphaTextureData = compressor.EncodeRgba4Bpp(this.alphaTexture);
		
		// Create new RGBA 4 bit PVRTC texture from compressed texture data
		this.pvrtcAlphaTexture = new Texture2D(this.alphaTexture.width, this.alphaTexture.height, TextureFormat.PVRTC_RGBA4, false, true);
		this.pvrtcAlphaTexture.LoadRawTextureData(alphaTextureData);
		this.pvrtcAlphaTexture.Apply();


		PvrtcDecompress decompressor = new PvrtcDecompress();

		// Decompress RGB 4 bit PVRTC to 24 bit texture
		this.decompressedGlobeTexture = decompressor.DecodeRgb4Bpp(globeTextureData, this.globeTexture.width);
		this.decompressedGlobeTexture.Apply();

		// Decompress RGBA 4 bit PVRTC to 32 bit texture
		this.decompressedAlphaTexture = decompressor.DecodeRgba4Bpp(alphaTextureData, this.alphaTexture.width);
		this.decompressedAlphaTexture.Apply();
        */
	}
}
