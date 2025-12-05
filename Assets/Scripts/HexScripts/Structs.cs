using System;
using UnityEngine;

[Serializable] public struct RendererMatIndex
{
    [SerializeField] public Renderer[] rendererList;
    [SerializeField] public ushort[] materialIndexNumber;
    public RendererMatIndex(Renderer[] rendererList, ushort[] materialIndexNumber)
    {
        this.rendererList = rendererList;
        this.materialIndexNumber = materialIndexNumber;
    }
}