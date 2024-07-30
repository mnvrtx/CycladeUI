using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CycladeBase.Utils;
using CycladeUI.Popups.PrefEditor.Elements;
using Solonity.View.Utils;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#else
using UnityEngine.UI;
#endif
using UnityEngine;

namespace CycladeUI.Popups.PrefEditor
{
    public class PrefBlock : MonoBehaviour
    {
        public PrefEditorPopup prefPopup;
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Text title;
#else
        public Text title;
#endif
        public ViewInstances<PrefElement> prefElements;

        public GameObject ExpandImage;
        private bool _expanded;

        private readonly List<PrefBlock> _myInnerBlocks = new();

        private void Awake()
        {
            prefElements.Initialize();
        }

        public void Initialize(string titleStr, object obj, bool expanded = true)
        {
            title.text = titleStr;
            _expanded = expanded;

            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var info in fields) 
                ProcessProp(obj, info);

            UpdateExpanded();
        }

        public void U_FlipExpand()
        {
            _expanded = !_expanded;
            UpdateExpanded();
        }

        public void ResetProps()
        {
            foreach (var element in prefElements.Instances)
            {
                element.type.GetSelected<PrefBaseChange>().ResetProperty();
            }
        }

        private void UpdateExpanded()
        {
            foreach (var element in prefElements.Instances)
                element.SetActive(_expanded);
            foreach (var prefBlock in _myInnerBlocks)
                prefBlock.SetActive(_expanded);
            
            ExpandImage.transform.rotation = _expanded ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 90);
        }

        private void ProcessProp(object obj, FieldInfo fieldInfo)
        {
            var propName = fieldInfo.Name;

            var type = fieldInfo.FieldType;
            
            var typeStr = type.FullName;
            
            if (type.IsEnum)
                typeStr = "Enum";

            var haveSerializable = type.GetCustomAttribute<SerializableAttribute>() != null;
            if (type.IsClass && haveSerializable && typeStr != "System.String")
                typeStr = "Serializable";
            
            var val = fieldInfo.GetValue(obj);
            
            if (val == null)
                return;

            switch (typeStr)
            {
                case "Serializable":
                {
                    var block = prefPopup.prefBlocks.GetNew(transform);
                    block.Initialize(propName, val);
                    block.transform.SetSiblingIndex(1);
                    _myInnerBlocks.Add(block);
                    break;
                }
                case "Enum":
                {
                    var element = prefElements.GetNew();
                    var pref = element.type.SelectAndGet<PrefEnumChange>(PrefElement.Enum);
                    
                    pref.Obj = obj;
                    pref.FieldInfo = fieldInfo;
                    
                    pref.title.text = propName;
                    
                    var enumValues = type.GetFields().Where(fi => fi.IsLiteral).Select(q => q.Name).ToList();

                    foreach (var enumValue in enumValues)
                    {
#if CYCLADEUI_TEXT_MESH_PRO
                        var optionData = new TMP_Dropdown.OptionData(enumValue);
#else
                        var optionData = new Dropdown.OptionData(enumValue);
#endif
                        pref.dropdown.options.Add(optionData);
                    }

                    var enVal = (int)Convert.ChangeType(val, typeof(int));
                    pref.InitialValue = enVal;
                    pref.dropdown.value = enVal;
                    
                    break;
                }
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                {
#if CYCLADEUI_TEXT_MESH_PRO
                    var integerNumber = TMP_InputField.ContentType.IntegerNumber;
#else
                    var integerNumber = InputField.ContentType.IntegerNumber;
#endif
                    AddPrefValChange(obj, fieldInfo, propName, val, type, typeStr, integerNumber);
                    break;
                }
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                {
#if CYCLADEUI_TEXT_MESH_PRO
                    var decimalNumber = TMP_InputField.ContentType.DecimalNumber;
#else
                    var decimalNumber = InputField.ContentType.DecimalNumber;
#endif
                    AddPrefValChange(obj, fieldInfo, propName, val, type, typeStr, decimalNumber);
                    break;
                }
                case "System.String":
                {
#if CYCLADEUI_TEXT_MESH_PRO
                    var contentType = TMP_InputField.ContentType.Standard;
#else
                    var contentType = InputField.ContentType.Standard;
#endif
                    AddPrefValChange(obj, fieldInfo, propName, val, type, typeStr, contentType);
                    break;
                }
                case "System.Boolean":
                {
                    var element = prefElements.GetNew();
                    var pref = element.type.SelectAndGet<PrefBoolChange>(PrefElement.Bool);
                    pref.title.text = propName;
                    pref.SetFlag((bool)val);
                    
                    pref.Obj = obj;
                    pref.FieldInfo = fieldInfo;
                    break;
                }
            }
        }

        private void AddPrefValChange(object obj, FieldInfo fieldInfo, string propName, object val, Type type, string typeStr,
#if CYCLADEUI_TEXT_MESH_PRO
                                      TMP_InputField.ContentType contentType
#else
                                      InputField.ContentType contentType
#endif
            )
        {
            var element = prefElements.GetNew();
            var pref = element.type.SelectAndGet<PrefValueChange>(PrefElement.Value);
            pref.title.text = propName;
            var strVal = val.ToString();
            pref.input.text = strVal;
            pref.input.contentType = contentType;
            pref.placeholder.text = $"{type.Name} value...";

            pref.Obj = obj;
            pref.FieldInfo = fieldInfo;
            pref.TypeStr = typeStr;
            pref.InitialValue = strVal;
        }

    }
}