namespace CycladeBindingsEditor
{
    public class ButtonMethodInfo
    {
        public string FieldName;
        public bool IsArray;

        public ButtonMethodInfo(string fieldName, bool isArray)
        {
            FieldName = fieldName;
            IsArray = isArray;
        }
    }
}