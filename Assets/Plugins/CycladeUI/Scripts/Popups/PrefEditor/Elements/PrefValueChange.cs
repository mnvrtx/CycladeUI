using System;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#else
using UnityEngine.UI;
#endif
using UnityEngine;

namespace CycladeUI.Popups.PrefEditor.Elements
{
    public class PrefValueChange : PrefBaseChange
    {
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_InputField input;
        public TMP_Text placeholder;
#else
        public InputField input;
        public Text placeholder;
#endif

        [NonSerialized] public string TypeStr;
        [NonSerialized] public string InitialValue;

        public override void ResetProperty()
        {
            input.text = InitialValue;
            U_OnChange();
        }

        public void U_OnChange()
        {
            try
            {
                var strVal = input.text;
                if (string.IsNullOrWhiteSpace(strVal) && TypeStr != "System.String")
                    return;

                object finalVal;

                switch (TypeStr)
                {
                    case "System.Byte":
                        finalVal = Convert.ToByte(strVal);
                        break;
                    case "System.SByte":
                        finalVal = Convert.ToSByte(strVal);
                        break;
                    case "System.Int16":
                        finalVal = Convert.ToInt16(strVal);
                        break;
                    case "System.UInt16":
                        finalVal = Convert.ToUInt16(strVal);
                        break;
                    case "System.Int32":
                        finalVal = Convert.ToInt32(strVal);
                        break;
                    case "System.UInt32":
                        finalVal = Convert.ToUInt32(strVal);
                        break;
                    case "System.Int64":
                        finalVal = Convert.ToInt64(strVal);
                        break;
                    case "System.UInt64":
                        finalVal = Convert.ToUInt64(strVal);
                        break;
                    case "System.Single":
                        finalVal = Convert.ToSingle(strVal);
                        break;
                    case "System.Double":
                        finalVal = Convert.ToDouble(strVal);
                        break;
                    case "System.Decimal":
                        finalVal = Convert.ToDecimal(strVal);
                        break;
                    case "System.String":
                        finalVal = strVal;
                        break;
                    default:
                        throw new Exception($"not defined type: {TypeStr}");
                }

                FieldInfo.SetValue(Obj, finalVal);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ResetProperty();
            }
        }
    }
}