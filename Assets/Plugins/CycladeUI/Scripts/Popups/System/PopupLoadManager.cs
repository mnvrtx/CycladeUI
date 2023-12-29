using System;
using CycladeBase.Utils.Logging;
using UnityEngine;

namespace CycladeUI.Popups.System
{
    public static class PopupLoadManager
    {
        private static readonly Log log = new(nameof(PopupLoadManager));

        public static BasePopup Load(Type type, string path)
        {
            var formatPath = FormatPath(path);
            var popup = Resources.Load<BasePopup>(formatPath);

            if (popup == null)
                throw new Exception($"Failed to load popup {type.Name} in path '{formatPath}(raw: {path})'");

            return popup;
        }

        public static ResourceRequest LoadAsync(Type type, string path)
        {
            var formatPath = FormatPath(path);
            var request = Resources.LoadAsync<BasePopup>(formatPath);
            
            if (request == null)
                throw new Exception($"Failed to load popup {type.Name} in path '{formatPath}(raw: {path})'");

            return request;
        }

        public static string FormatPath(string path)
        {
            string prefixToRemove = "Resources/";
            string extensionToRemove = ".prefab";

            path = path.Split(prefixToRemove)[1];

            if (path.EndsWith(extensionToRemove)) 
                path = path.Substring(0, path.Length - extensionToRemove.Length);

            return path;
        }
    }
}