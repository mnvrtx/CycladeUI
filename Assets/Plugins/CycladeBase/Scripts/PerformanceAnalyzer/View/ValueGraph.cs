using System;
using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeBase.PerformanceAnalyzer.View
{
    public class ValueGraph : MonoBehaviour
    {
        /* ----- TODO: ----------------------------
         * Add summaries to the variables.
         * Add summaries to the functions.
         * Check if we should add a "RequireComponent" for "FpsMonitor".
         * --------------------------------------*/

        [SerializeField] private Image m_imageGraph;

        [SerializeField] private Shader ShaderFull;

        public Color TopValueColor = new Color32(220, 41, 30, 255);
        public Color MiddleValueColor = new Color32(243, 232, 0, 255);
        public Color LowValueColor = new Color32(118, 212, 58, 255);

        internal bool InternalActive;

        private const int resolution = 128;

        private G_GraphShader m_shaderGraph;

        private float[] m_valueArray;

        private float m_highestValueRaw;
        private float m_highestValue;

        public Text text;

        private string _valueTitle;
        private ValSuffix _valSuffix;
        
        public float MiddleValueThreshold = 10;
        public float LowValueThreshold = 5;
        public bool TopValueIsGood;
        
        private float _maxValueCanShow = -1;

        public void Initialize(string valueTitle, ValSuffix valSuffix)
        {
            _valueTitle = valueTitle;
            _valSuffix = valSuffix;
        }

        void OnEnable()
        {
            m_shaderGraph = new G_GraphShader
            {
                Image = m_imageGraph
            };

            //UpdateParameters
            m_shaderGraph.Image.material = new Material(ShaderFull);
            
            // m_shaderGraph.ArrayMaxSize = G_GraphShader.ArrayMaxSizeLight;
            // m_shaderGraph.Image.material = new Material(ShaderLight);

            m_shaderGraph.InitializeShader();

            //create points
            
            m_shaderGraph.ShaderArrayValues = new float[resolution];

            m_valueArray = new float[resolution];

            for (int i = 0; i < resolution; i++)
            {
                m_shaderGraph.ShaderArrayValues[i] = 0;
            }

            m_shaderGraph.GoodColor = TopValueIsGood ? LowValueColor : TopValueColor;
            m_shaderGraph.CautionColor = MiddleValueColor;
            m_shaderGraph.CriticalColor = TopValueIsGood ? TopValueColor : LowValueColor;

            m_shaderGraph.UpdateColors();

            m_shaderGraph.UpdateArray();
        }

        // void Update()
        // {
        //     int fps = (int) (1 / Time.unscaledDeltaTime);
        //     Process(fps);
        // }

        public void Process(float value)
        {
            ProcessAndFindMinMax(value, out var currentMaxValue, out var currentMinValue);

            var suffixFunc = _valSuffix.ValSuffixFunc;
            text.text = $"{_valueTitle}: {suffixFunc.Invoke(value)}\nL: {suffixFunc.Invoke(currentMinValue)}\nH: {suffixFunc.Invoke(currentMaxValue)}";

            ProcessInternal(currentMaxValue);
        }
        
        public void ProcessLong(long longValue)
        {
            var fValue = Convert.ToSingle(longValue);
                
            ProcessAndFindMinMax(fValue, out var currentMaxValue, out var currentMinValue);

            var suffixFunc = _valSuffix.LongValSuffixFunc;
            text.text = $"{_valueTitle}: {suffixFunc.Invoke(longValue)}\nL: {suffixFunc.Invoke((long)currentMinValue)}\nH: {suffixFunc.Invoke((long)currentMaxValue)}";

            ProcessInternal(currentMaxValue);
        }

        private void ProcessAndFindMinMax(float value, out float currentMaxValue, out float currentMinValue)
        {
            currentMaxValue = 0;
            currentMinValue = float.MaxValue;

            for (int i = 0; i <= resolution - 1; i++)
            {
                if (i >= resolution - 1)
                {
                    m_valueArray[i] = value;
                }
                else
                {
                    m_valueArray[i] = m_valueArray[i + 1];
                }

                // Store the min/max value 
                if (currentMaxValue < m_valueArray[i])
                {
                    currentMaxValue = m_valueArray[i];
                }
                
                if (currentMinValue > m_valueArray[i])
                {
                    currentMinValue = m_valueArray[i];
                }
            }
        }

        private void ProcessInternal(float currentMaxValue)
        {
            if (_maxValueCanShow < 0)
            {
                _maxValueCanShow = MiddleValueThreshold * 2;
            }

            m_highestValueRaw = m_highestValueRaw < 1 || m_highestValueRaw <= currentMaxValue ? currentMaxValue : m_highestValueRaw - 1;

            m_highestValue = m_highestValueRaw > _maxValueCanShow ? _maxValueCanShow : m_highestValueRaw;
            for (int i = 0; i <= resolution - 1; i++) 
                m_shaderGraph.ShaderArrayValues[i] = Mathf.Clamp01(m_valueArray[i] / m_highestValue);

            m_shaderGraph.UpdatePoints();

            m_shaderGraph.Average = currentMaxValue;
            m_shaderGraph.UpdateAverage();

            m_shaderGraph.GoodThreshold = (float) MiddleValueThreshold / m_highestValue;
            m_shaderGraph.CautionThreshold = (float) LowValueThreshold / m_highestValue;
            m_shaderGraph.UpdateThresholds();
        }
    }
}