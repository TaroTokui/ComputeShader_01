using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Instancing : MonoBehaviour {

    // ==============================
    #region // Defines
        
    const int ThreadBlockSize = 256;

    struct CubeData
    {
        public Vector3 Position;
        public Vector3 Albedo;
    }

    #endregion // Defines

    // ==============================
    #region // Serialize Fields
        
    // cubeの数
    [SerializeField]
    int _instanceCountX = 100;
    [SerializeField]
    int _instanceCountY = 100;
    
    [SerializeField]
    ComputeShader _ComputeShader;
    
    // instancingするmesh
    [SerializeField]
    Mesh _CubeMesh;
    
    [SerializeField]
    Material _CubeMaterial;
    
    [SerializeField]
    Vector3 _CubeMeshScale = new Vector3(1f, 1f, 1f);
    
    /// 表示領域の中心座標
    [SerializeField]
    Vector3 _BoundCenter = Vector3.zero;
    
    /// 表示領域のサイズ
    [SerializeField]
    Vector3 _BoundSize = new Vector3(300f, 300f, 300f);
    
    /// アニメーションの位相
    [Range(-Mathf.PI, Mathf.PI)]
    [SerializeField]
    float _Phi = Mathf.PI;
    
    /// アニメーションの周期
    [Range(0.01f, 100)]
    [SerializeField]
    float _Lambda = 1;
    
    /// アニメーションの大きさ
    [SerializeField]
    float _Amplitude = 1;

    /// アニメーションの速さ
    [SerializeField]
    [Range(0, 10)]
    float _Speed = 1;

    #endregion // Serialize Fields

    // ==============================
    #region // Private Fields

    ComputeBuffer _CubeDataBuffer;
    
    /// GPU Instancingの為の引数
    uint[] _GPUInstancingArgs = new uint[5] { 0, 0, 0, 0, 0 };
    
    /// GPU Instancingの為の引数バッファ
    ComputeBuffer _GPUInstancingArgsBuffer;

    // instanceの合計数
    int _instanceCount;

    #endregion // Private Fields


    // --------------------------------------------------
    #region // MonoBehaviour Methods
        
    void Start()
    {
        _instanceCount = _instanceCountX * _instanceCountY;

        // バッファ生成
        _CubeDataBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(CubeData)));
        _GPUInstancingArgsBuffer = new ComputeBuffer(1, _GPUInstancingArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        var cubeDataArr = new CubeData[_instanceCount];

        // 初期化
        int kernelId = _ComputeShader.FindKernel("Init");
        _ComputeShader.SetInt("_Width", _instanceCountX);
        _ComputeShader.SetInt("_Height", _instanceCountY);
        _ComputeShader.SetBuffer(kernelId, "_CubeDataBuffer", _CubeDataBuffer);
        _ComputeShader.Dispatch(kernelId, (Mathf.CeilToInt(_instanceCount / ThreadBlockSize) + 1), 1, 1);
        
    }
    
    void Update()
    {
        // ComputeShader
        int kernelId = _ComputeShader.FindKernel("Update");
        _ComputeShader.SetFloat("_Time", Time.time / 5.0f * _Speed);
        _ComputeShader.SetFloat("_Phi", _Phi);
        _ComputeShader.SetFloat("_Lambda", _Lambda);
        _ComputeShader.SetFloat("_Amplitude", _Amplitude);
        _ComputeShader.SetBuffer(kernelId, "_CubeDataBuffer", _CubeDataBuffer);
        _ComputeShader.Dispatch(kernelId, (Mathf.CeilToInt(_instanceCount / ThreadBlockSize) + 1), 1, 1);

        // GPU Instaicing
        _GPUInstancingArgs[0] = (_CubeMesh != null) ? _CubeMesh.GetIndexCount(0) : 0;
        _GPUInstancingArgs[1] = (uint)_instanceCount;
        _GPUInstancingArgsBuffer.SetData(_GPUInstancingArgs);
        _CubeMaterial.SetBuffer("_CubeDataBuffer", _CubeDataBuffer);
        _CubeMaterial.SetVector("_CubeMeshScale", _CubeMeshScale);
        Graphics.DrawMeshInstancedIndirect(_CubeMesh, 0, _CubeMaterial, new Bounds(_BoundCenter, _BoundSize), _GPUInstancingArgsBuffer);
    }
    
    void OnDestroy()
    {
        if (this._CubeDataBuffer != null)
        {
            this._CubeDataBuffer.Release();
            this._CubeDataBuffer = null;
        }
        if (this._GPUInstancingArgsBuffer != null)
        {
            this._GPUInstancingArgsBuffer.Release();
            this._GPUInstancingArgsBuffer = null;
        }
    }

    #endregion // MonoBehaviour Method
}
