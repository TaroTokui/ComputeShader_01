﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCS : MonoBehaviour {

    [SerializeField]
    ComputeShader _ComputeShader;

    ComputeBuffer _IntBuffer;

    const int totalCount = 10;
    const int ThreadSize = 8;
    const int GroupSize = totalCount / ThreadSize + 1;

    // Use this for initialization
    void Start () {
        
        // init buffer
        _IntBuffer = new ComputeBuffer(totalCount, sizeof(int));

        // ComputeShader
        int kernelId = _ComputeShader.FindKernel("Test");
        _ComputeShader.SetBuffer(kernelId, "_IntBuffer", _IntBuffer);
        _ComputeShader.Dispatch(kernelId, GroupSize, 1, 1);

        // 計算結果を表示
        int[] result = new int[totalCount];
        _IntBuffer.GetData(result);

        for(int i=0; i<totalCount; i++)
        {
            Debug.Log(result[i]);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        // 使ったbufferを開放する
        if (this._IntBuffer != null)
        {
            this._IntBuffer.Release();
            this._IntBuffer = null;
        }
    }
}
