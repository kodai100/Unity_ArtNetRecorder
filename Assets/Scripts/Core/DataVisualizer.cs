using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class DataVisualizer : MonoBehaviour, IDisposable
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private ComputeShader dmxTextureBufferCompute;

    private ComputeBuffer dmxComputeBuffer;
    private RenderTexture dmxBuffer;
    private int kernelIndex;

    private int maxUniverseNum;

    public void Open()
    {
        rawImage.enabled = true;
    }

    public void Close()
    {
        rawImage.enabled = false;
    }
    
    public void Initialize(int maxUniverseNum = 64)
    {
        this.maxUniverseNum = maxUniverseNum;
        
        kernelIndex = dmxTextureBufferCompute.FindKernel("CSMain");
        dmxComputeBuffer = new ComputeBuffer(maxUniverseNum * 512, sizeof(float));    // universe * 
        dmxTextureBufferCompute.SetBuffer(kernelIndex, "_Buffer", dmxComputeBuffer);
        
        dmxBuffer = CreateRenderTexture(512, maxUniverseNum);
        dmxTextureBufferCompute.SetTexture(kernelIndex, "_Result", dmxBuffer);

        rawImage.texture = dmxBuffer;
    }

    // Update is called once per frame
    public void Exec(float[] dmxRaw)
    {
        
        dmxComputeBuffer.SetData(dmxRaw);
        dmxTextureBufferCompute.Dispatch(kernelIndex, 512 / 32, maxUniverseNum / 32, 1);
        
    }
    
    private RenderTexture CreateRenderTexture(int width, int height)
    {
        var renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        renderTexture.graphicsFormat = GraphicsFormat.R16_SFloat;
        renderTexture.enableRandomWrite = true;
        renderTexture.hideFlags = HideFlags.DontSave;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.wrapMode = TextureWrapMode.Repeat;
        renderTexture.Create();
        return renderTexture;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Dispose()
    {
        dmxComputeBuffer?.Release();
    }
}
