using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayCS : MonoBehaviour {

    [SerializeField]
    ComputeShader _ComputeShader;

    ComputeBuffer _IntBuffer;

    const int totalCount = 10;
    const int ThreadSize = 8;
    const int GroupSize = totalCount / ThreadSize + 1;

    // Use this for initialization
    void Start()
    {


        Debug.Log(GroupSize);
        // init buffer
        _IntBuffer = new ComputeBuffer(totalCount, sizeof(int));

        // ComputeShader
        int kernelId = _ComputeShader.FindKernel("Test");
        //uint threadSizeX, threadSizeY, threadSizeZ;
        //_ComputeShader.GetKernelThreadGroupSizes(kernelId, out threadSizeX, out threadSizeY, out threadSizeZ);
        _ComputeShader.SetBuffer(kernelId, "_IntBuffer", _IntBuffer);
        _ComputeShader.Dispatch(kernelId, GroupSize, 1, 1);

        // 計算結果を表示
        int[] result = new int[totalCount];
        _IntBuffer.GetData(result);

        for (int i = 0; i < totalCount; i++)
        {
            Debug.Log(result[i]);
        }

    }
}
