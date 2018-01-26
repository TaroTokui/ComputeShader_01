using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ArrayCS : MonoBehaviour {

    struct MyData
    {
        public Vector3 Thread;
        public Vector3 Group;
        public float Index;
    }

    [SerializeField]
    ComputeShader _ComputeShader;

    ComputeBuffer _Vector3Buffer;
    ComputeBuffer _MyDataBuffer;

    const int totalCount = 10;

    // Use this for initialization
    void Start()
    {
        //------------------------------------------------------------------
        // Vector型を渡す

        Debug.Log("--------Vector3--------");

        // init buffer
        _Vector3Buffer = new ComputeBuffer(totalCount, Marshal.SizeOf(typeof(Vector3)));

        // ComputeShader
        int kernelId = _ComputeShader.FindKernel("Test");
        
        // スレッド数を取得
        uint threadSizeX, threadSizeY, threadSizeZ;
        _ComputeShader.GetKernelThreadGroupSizes(kernelId, out threadSizeX, out threadSizeY, out threadSizeZ);
        int GroupSize = totalCount / (int)threadSizeX + 1;

        _ComputeShader.SetFloat("_FloatValue", 2.0f);
        _ComputeShader.SetBuffer(kernelId, "_Vector3Buffer", _Vector3Buffer);
        _ComputeShader.Dispatch(kernelId, GroupSize, 1, 1);

        // 計算結果を表示
        Vector3[] result = new Vector3[totalCount];
        _Vector3Buffer.GetData(result);

        for (int i = 0; i < totalCount; i++)
        {
            Debug.Log(result[i]);
        }
        //------------------------------------------------------------------



        //------------------------------------------------------------------
        // 自分で定義した構造体を渡す
        // init buffer

        Debug.Log("--------MyDataBuffer--------");

        _MyDataBuffer = new ComputeBuffer(totalCount, Marshal.SizeOf(typeof(MyData)));

        // ComputeShader
        kernelId = _ComputeShader.FindKernel("StructureTest");

        // スレッド数を取得
        _ComputeShader.GetKernelThreadGroupSizes(kernelId, out threadSizeX, out threadSizeY, out threadSizeZ);
        GroupSize = totalCount / (int)threadSizeX + 1;
        
        _ComputeShader.SetBuffer(kernelId, "_MyDataBuffer", _MyDataBuffer);
        _ComputeShader.Dispatch(kernelId, GroupSize, 1, 1);

        // 計算結果を表示
        MyData[] result2 = new MyData[totalCount];
        _MyDataBuffer.GetData(result2);

        for (int i = 0; i < totalCount; i++)
        {
            Debug.Log(result2[i].Thread + ", " + result2[i].Group + ", " + result2[i].Index);
        }
        //------------------------------------------------------------------
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (this._Vector3Buffer != null)
        {
            this._Vector3Buffer.Release();
            this._Vector3Buffer = null;
        }

        if (this._MyDataBuffer != null)
        {
            this._MyDataBuffer.Release();
            this._MyDataBuffer = null;
        }
    }
}
