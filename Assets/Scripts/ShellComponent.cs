using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellComponent : MonoBehaviour
{
    [SerializeField] private Material _ShellMaterial;
    [SerializeField] private int _layerCount;
    [SerializeField] private int _density = 100;
    [SerializeField] private float _tickness;
    [SerializeField] private float _minNoise, _maxNoise;
    [SerializeField, Range(0f, 1f)] private float _furLength;
    [SerializeField, Range(0f, 3f)] private float _furDistanceAttenuation;
    [SerializeField, Range(0f, 10f)] private float _curvature;

    [SerializeField, Range(0.0f, 1.0f)] private float displacementStrength = 0.1f;
    [SerializeField, Range(0.0f, 5.0f)] private float occlusionAttenuation = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float occlusionBias = 0.0f;


    [SerializeField] private Color _furColor;

    public MeshRenderer[] RedererList { get; private set; }

    private void OnEnable()
    {
        CreateShells();

    }

    private void CreateShells()
    {
        var shellParent = new GameObject("Shell");
        shellParent.transform.parent = transform;
        shellParent.transform.localPosition = Vector3.zero;
        shellParent.transform.localRotation = Quaternion.identity;
        shellParent.transform.localScale = Vector3.one;
        RedererList = new MeshRenderer[_layerCount];
        for (int i = 0; i < _layerCount; i++)
        {
            var obj = new GameObject("Shell_Layer_" + i, typeof(MeshFilter), typeof(MeshRenderer)).GetComponent<MeshRenderer>();
            RedererList[i] = obj;
            obj.transform.parent = shellParent.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            obj.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            obj.material = _ShellMaterial;
            obj.material.SetFloat("_LayerIndex", i);
            obj.material.SetFloat("_LayerCount", _layerCount);
            obj.material.SetFloat("_Index", i);
            obj.material.SetFloat("_Tickness", _tickness);
            obj.material.SetFloat("_Density", _density);
            obj.material.SetColor("_FurBaseColor", _furColor);
            obj.material.SetFloat("_MinNoise", _minNoise);
            obj.material.SetFloat("_MaxNoise", _maxNoise);
            obj.material.SetFloat("_FurDistanceAttenuation", _furDistanceAttenuation);
            obj.material.SetFloat("_DisplacementStrength", displacementStrength);
            obj.material.SetFloat("_OcclusionAttenuation", occlusionAttenuation);
            obj.material.SetFloat("_OcclusionBias", occlusionBias);
            obj.material.SetFloat("_Furlength", _furLength);
            obj.material.SetFloat("_Curvature", _curvature);
        }

    }
    private void Update()
    {
        if (Time.time % 2 == 0)
            UpdateMaterialProperties();
    }
    private void UpdateMaterialProperties()
    {
        for (int i = 0; i < RedererList.Length; i++)
        {
            RedererList[i].material.SetFloat("_LayerIndex", i);
            RedererList[i].material.SetFloat("_LayerCount", _layerCount);
            RedererList[i].material.SetFloat("_Index", i);
            RedererList[i].material.SetFloat("_Tickness", _tickness);
            RedererList[i].material.SetFloat("_Density", _density);
            RedererList[i].material.SetColor("_FurBaseColor", _furColor);
            RedererList[i].material.SetFloat("_MinNoise", _minNoise);
            RedererList[i].material.SetFloat("_MaxNoise", _maxNoise);
            RedererList[i].material.SetFloat("_FurDistanceAttenuation", _furDistanceAttenuation);
            RedererList[i].material.SetFloat("_DisplacementStrength", displacementStrength);
            RedererList[i].material.SetFloat("_OcclusionAttenuation", occlusionAttenuation);
            RedererList[i].material.SetFloat("_OcclusionBias", occlusionBias);
            RedererList[i].material.SetFloat("_Furlength", _furLength);
            RedererList[i].material.SetFloat("_Curvature", _curvature);
        }
    }



}
