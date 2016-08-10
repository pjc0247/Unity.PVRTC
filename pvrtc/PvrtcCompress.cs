using UnityEngine;
using System.Collections;
using System;

public class PvrtcCompress  {

	private int GetMortonNumber(int x, int y)
	{
		return MortonTable.MORTON_TABLE[x >> 8] << 17 | MortonTable.MORTON_TABLE[y >> 8] << 16 | MortonTable.MORTON_TABLE[x & 0xFF] << 1 | MortonTable.MORTON_TABLE[y & 0xFF];
	}

	private void GetMinMaxColors(Color[] pixels, int width, int startX, int startY, ref Color32 minColor, ref Color32 maxColor)
	{
		minColor = Color.white; // white is same as all 255
		maxColor = Color.clear; // clear is same as all 0

		for (int x = startX; x < startX + 4; x++)
		{
			for (int y = startY; y < startY + 4; y++) 
			{

                Color32 currentColor = pixels[x * width + y]; //bitmap.GetPixel(x, y);
				if ( currentColor.r < minColor.r ) { minColor.r = currentColor.r; } 
				if ( currentColor.g < minColor.g ) { minColor.g = currentColor.g; } 
				if ( currentColor.b < minColor.b ) { minColor.b = currentColor.b; } 
				if ( currentColor.r > maxColor.r ) { maxColor.r = currentColor.r; } 
				if ( currentColor.g > maxColor.g ) { maxColor.g = currentColor.g; } 
				if ( currentColor.b > maxColor.b ) { maxColor.b = currentColor.b; } 
			}
		}
		/*
		byte[] inset = new byte[3];
		inset[0] = (byte)(( maxColor.r - minColor.r ) >> this.insetShift); 
		inset[1] = (byte)(( maxColor.g - minColor.g ) >> this.insetShift); 
		inset[2] = (byte)(( maxColor.b - minColor.b ) >> this.insetShift);

		minColor.r = (byte)(( minColor.r + inset[0] <= 255 ) ? minColor.r + inset[0] : 255); 
		minColor.g = (byte)(( minColor.g + inset[1] <= 255 ) ? minColor.g + inset[1] : 255); 
		minColor.b = (byte)(( minColor.b + inset[2] <= 255 ) ? minColor.b + inset[2] : 255);

		maxColor.r = (byte)(( maxColor.r >= inset[0] ) ? maxColor.r - inset[0] : 0); 
		maxColor.g = (byte)(( maxColor.g >= inset[1] ) ? maxColor.g - inset[1] : 0); 
		maxColor.b = (byte)(( maxColor.b >= inset[2] ) ? maxColor.b - inset[2] : 0);
		*/
	}

	private void GetMinMaxColorsWithAlpha(Texture2D bitmap, int startX, int startY, ref Color32 minColor, ref Color32 maxColor)
	{
		minColor = Color.white; // white is same as all 255
		maxColor = Color.clear; // clear is same as all 0

		for (int x = startX; x < startX + 4; x++)
		{
			for (int y = startY; y < startY + 4; y++)
			{
				Color32 currentColor = bitmap.GetPixel(x, y);
				if ( currentColor.r < minColor.r ) { minColor.r = currentColor.r; }
				if ( currentColor.g < minColor.g ) { minColor.g = currentColor.g; }
				if ( currentColor.b < minColor.b ) { minColor.b = currentColor.b; }
				if ( currentColor.a < minColor.a ) { minColor.a = currentColor.a; }
				if ( currentColor.r > maxColor.r ) { maxColor.r = currentColor.r; }
				if ( currentColor.g > maxColor.g ) { maxColor.g = currentColor.g; }
				if ( currentColor.b > maxColor.b ) { maxColor.b = currentColor.b; }
				if ( currentColor.a > maxColor.a ) { maxColor.a = currentColor.a; }
			}
		}
		/*
		byte[] inset = new byte[4];
		inset[0] = (byte)(( maxColor.r - minColor.r ) >> this.insetShift); 
		inset[1] = (byte)(( maxColor.g - minColor.g ) >> this.insetShift); 
		inset[2] = (byte)(( maxColor.b - minColor.b ) >> this.insetShift);
		inset[3] = (byte)(( maxColor.a - minColor.a ) >> this.insetShift);

		minColor.r = (byte)(( minColor.r + inset[0] <= 255 ) ? minColor.r + inset[0] : 255); 
		minColor.g = (byte)(( minColor.g + inset[1] <= 255 ) ? minColor.g + inset[1] : 255); 
		minColor.b = (byte)(( minColor.b + inset[2] <= 255 ) ? minColor.b + inset[2] : 255);
		minColor.a = (byte)(( minColor.a + inset[3] <= 255 ) ? minColor.a + inset[3] : 255);
		
		maxColor.r = (byte)(( maxColor.r >= inset[0] ) ? maxColor.r - inset[0] : 0); 
		maxColor.g = (byte)(( maxColor.g >= inset[1] ) ? maxColor.g - inset[1] : 0); 
		maxColor.b = (byte)(( maxColor.b >= inset[2] ) ? maxColor.b - inset[2] : 0);
		maxColor.a = (byte)(( maxColor.a >= inset[3] ) ? maxColor.a - inset[3] : 0);
		*/
	}

	public byte[] EncodeRgba4Bpp(Texture2D bitmap)
	{
		if (bitmap.height != bitmap.width) Debug.LogError ("Texture isn't square!");
		if (!((bitmap.height & (bitmap.height - 1)) == 0)) Debug.LogError ("Texture resolution must be 2^N!");

		int size = bitmap.width;
		int blocks = size / 4;
		int blockMask = blocks-1;
		
		PvrtcPacket[] packets = new PvrtcPacket[blocks * blocks];
		for (int i = 0; i < packets.Length; i++)
		{
			packets[i] = new PvrtcPacket();
		}
		
		for (int y = 0; y < blocks; ++y)
		{
			for (int x = 0; x < blocks; ++x)
			{
				Color32 minColor = Color.white; // white is same as all 255, should be same as Color.max 
				Color32 maxColor = Color.clear; // clear is same as all 0, should be same as Color.min
				this.GetMinMaxColorsWithAlpha(bitmap, 4*x, 4*y, ref minColor, ref maxColor);

				PvrtcPacket packet = packets[this.GetMortonNumber(x, y)];
				packet.SetPunchthroughAlpha(false);
				packet.SetColorA(minColor);
				packet.SetColorB(maxColor);
			}
		}

		int currentFactorIndex = 0;
		
		for (int y = 0; y < blocks; ++y)
		{
			for (int x = 0; x < blocks; ++x)
			{
				currentFactorIndex = 0;
				
				uint modulationData = 0;
				
				for(int py = 0; py < 4; ++py)
				{
					int yOffset = (py < 2) ? -1 : 0;
					int y0 = (y + yOffset) & blockMask;
					int y1 = (y0+1) & blockMask;
					
					for(int px = 0; px < 4; ++px)
					{
						int xOffset = (px < 2) ? -1 : 0;
						int x0 = (x + xOffset) & blockMask;
						int x1 = (x0+1) & blockMask;
						
						PvrtcPacket p0 = packets[this.GetMortonNumber(x0, y0)];
						PvrtcPacket p1 = packets[this.GetMortonNumber(x1, y0)];
						PvrtcPacket p2 = packets[this.GetMortonNumber(x0, y1)];
						PvrtcPacket p3 = packets[this.GetMortonNumber(x1, y1)];

						byte[] currentFactors = PvrtcPacket.BILINEAR_FACTORS[currentFactorIndex];
						
						Vector4Int ca = 	p0.GetColorRgbaA() * currentFactors[0] +
											p1.GetColorRgbaA() * currentFactors[1] +
											p2.GetColorRgbaA() * currentFactors[2] +
											p3.GetColorRgbaA() * currentFactors[3];
						
						Vector4Int cb = 	p0.GetColorRgbaB() * currentFactors[0] +
											p1.GetColorRgbaB() * currentFactors[1] +
											p2.GetColorRgbaB() * currentFactors[2] +
											p3.GetColorRgbaB() * currentFactors[3];
						
						Color32 pixel = (Color32)bitmap.GetPixel(4*x + px, 4*y + py);
						Vector4Int d = cb - ca;
						Vector4Int p = new Vector4Int(pixel.r*16, pixel.g*16, pixel.b*16, pixel.a*16);
						Vector4Int v = p - ca;
						
						// PVRTC uses weightings of 0, 3/8, 5/8 and 1
						// The boundaries for these are 3/16, 1/2 (=8/16), 13/16
						int projection = (v % d) * 16; //Mathf.RoundToInt(Vector4.Dot(v, d)) * 16;
						int lengthSquared = d % d; //Mathf.RoundToInt(Vector4.Dot(d,d));
						if(projection > 3*lengthSquared) modulationData++;
						if(projection > 8*lengthSquared) modulationData++;
						if(projection > 13*lengthSquared) modulationData++;
						
						modulationData = RotateRight(modulationData, 2);
						
						currentFactorIndex++;
					}
				}
				
				PvrtcPacket packet = packets[this.GetMortonNumber(x, y)];
				packet.SetModulationData(modulationData);
			}
		}

		byte[] returnValue = new byte[size * size /2];

		// Create final byte array from PVRTC packets
		for (int i = 0; i < packets.Length; i++)
		{
			byte[] tempArray = packets[i].GetAsByteArray();
			Buffer.BlockCopy(tempArray, 0, returnValue, 8*i, 8);
		}
		
		return returnValue;
	}


	public byte[] EncodeRgb4Bpp(Color[] pixels, int width, int height)
	{
		if (height != width) Debug.LogError ("Texture isn't square!");
		if (!((height & (height - 1)) == 0)) Debug.LogError ("Texture resolution must be 2^N!");

		int size = width;
		int blocks = size / 4;
		int blockMask = blocks-1;
		
		PvrtcPacket[] packets = new PvrtcPacket[blocks * blocks];
		for (int i = 0; i < packets.Length; i++)
		{
			packets[i] = new PvrtcPacket();
		}

		for(int y = 0; y < blocks; ++y)
		{
			for(int x = 0; x < blocks; ++x)
			{
				Color32 minColor = Color.white; // white is same as all 255, should be same as Color.max 
				Color32 maxColor = Color.clear; // clear is same as all 0,   should be same as Color.min

				this.GetMinMaxColors(pixels, width, 4*x, 4*y, ref minColor, ref maxColor);

				PvrtcPacket packet = packets[this.GetMortonNumber(x, y)];
				packet.SetPunchthroughAlpha(false);
				packet.SetColorA(minColor.r, minColor.g, minColor.b);
				packet.SetColorB(maxColor.r, maxColor.g, maxColor.b);
			}
		}

		int currentFactorIndex = 0;
		
		for(int y = 0; y < blocks; ++y)
		{
			for(int x = 0; x < blocks; ++x)
			{
				currentFactorIndex = 0;
				
				uint modulationData = 0;
				
				for(int py = 0; py < 4; ++py)
				{
					int yOffset = (py < 2) ? -1 : 0;
					int y0 = (y + yOffset) & blockMask;
					int y1 = (y0+1) & blockMask;
					
					for(int px = 0; px < 4; ++px)
					{
						int xOffset = (px < 2) ? -1 : 0;
						int x0 = (x + xOffset) & blockMask;
						int x1 = (x0+1) & blockMask;
						
						PvrtcPacket p0 = packets[this.GetMortonNumber(x0, y0)];
						PvrtcPacket p1 = packets[this.GetMortonNumber(x1, y0)];
						PvrtcPacket p2 = packets[this.GetMortonNumber(x0, y1)];
						PvrtcPacket p3 = packets[this.GetMortonNumber(x1, y1)];

						byte[] currentFactors = PvrtcPacket.BILINEAR_FACTORS[currentFactorIndex];

						Vector3Int ca = 	p0.GetColorRgbA() * currentFactors[0] +
											p1.GetColorRgbA() * currentFactors[1] +
											p2.GetColorRgbA() * currentFactors[2] +
											p3.GetColorRgbA() * currentFactors[3];
						
						Vector3Int cb = 	p0.GetColorRgbB() * currentFactors[0] +
											p1.GetColorRgbB() * currentFactors[1] +
											p2.GetColorRgbB() * currentFactors[2] +
											p3.GetColorRgbB() * currentFactors[3];

                        Color32 pixel = pixels[(4 * x + px) * width + 4 * y + py];
						//Color32 pixel = bitmap.GetPixel(4*x + px, 4*y + py);

						Vector3Int d = cb - ca;
						Vector3Int p = new Vector3Int(pixel.r*16, pixel.g*16, pixel.b*16);
						Vector3Int v = p - ca;
						
						// PVRTC uses weightings of 0, 3/8, 5/8 and 1
						// The boundaries for these are 3/16, 1/2 (=8/16), 13/16
						int projection = (v % d) * 16; // Mathf.RoundToInt(Vector3.Dot(v, d)) * 16;
						int lengthSquared = d % d;//Mathf.RoundToInt(Vector3.Dot(d,d));
						if(projection > 3*lengthSquared) modulationData++;
						if(projection > 8*lengthSquared) modulationData++;
						if(projection > 13*lengthSquared) modulationData++;

						modulationData = RotateRight(modulationData, 2);

						currentFactorIndex++;
					}
				}
				
				PvrtcPacket packet = packets[this.GetMortonNumber(x, y)];
				packet.SetModulationData(modulationData);
			}
		}

		byte[] returnValue = new byte[size * size /2];

		// Create final byte array from PVRTC packets
		for (int i = 0; i < packets.Length; i++)
		{
			byte[] tempArray = packets[i].GetAsByteArray();
			Buffer.BlockCopy(tempArray, 0, returnValue, 8*i, 8);
		}

		return returnValue;
	}

	private static uint RotateRight(uint value, int count)
	{
		return (value >> count) | (value << (32 - count));
	}
}
