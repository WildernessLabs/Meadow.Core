using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace Meadow
{
    public abstract class ConfigurableObject
    {
        private string m_parentname;
        private bool m_isArrayElement = false;

        protected IConfiguration? ConfigurationRoot { get; private set; }
        protected string ConfigurationRootPath { get; }

        protected ConfigurableObject()
        {
            PathTypeName = this.GetType().Name.Replace("Settings", string.Empty);
            SetConfigRoot();
        }

        protected ConfigurableObject(object? parent)
        {
            PathTypeName = this.GetType().Name.Replace("Settings", string.Empty);

            if (parent != null)
            {
                if (parent is ConfigurableObject)
                {
                    var p = parent as ConfigurableObject;
                    if (!string.IsNullOrEmpty(p.m_parentname))
                    {
                        m_parentname = $"{p.m_parentname}:{parent.GetType().Name}";
                    }
                    else
                    {
                        m_parentname = parent.GetType().Name;
                    }
                }
                else
                {
                    if (!parent.Equals(string.Empty))
                    {
                        m_parentname = parent.GetType().Name;
                    }
                }
                if (m_parentname.EndsWith("Settings")) m_parentname = m_parentname.Substring(0, m_parentname.Length - 8);
            }

            SetConfigRoot();
        }

        protected ConfigurableObject(object? parent, string? configRootPath)
            : this(parent)
        {
            ConfigurationRootPath = configRootPath;

            if (parent == null && configRootPath != null)
            {
                var idx = configRootPath.LastIndexOf(':');
                if (idx > 0)
                {
                    if (int.TryParse(configRootPath.Substring(idx + 1), out int i))
                    {
                        m_isArrayElement = true;
                    }
                }
            }
        }

        private void SetConfigRoot()
        {
            var b = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            b.AddJsonFile("appsettings.json", true);
            this.ConfigurationRoot = b.Build();
        }

        public float GetConfiguredFloat([CallerMemberName] string? name = null, float defaultValue = 0f)
        {
            if (string.IsNullOrWhiteSpace(name)) return defaultValue;
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return Convert.ToSingle(stringVal);
        }

        public bool GetConfiguredBool([CallerMemberName] string? name = null, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(name)) return defaultValue;
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return Convert.ToBoolean(stringVal);
        }

        public int GetConfiguredInt([CallerMemberName] string? name = null, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(name)) return defaultValue;
            var stringVal = GetConfiguredValue(name);
            if (stringVal == null) return defaultValue;
            return Convert.ToInt32(stringVal);
        }

        private string PathTypeName { get; }

        public string? GetConfiguredValue([CallerMemberName] string? name = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            string key;

            if (ConfigurationRoot == null) return null;

            if (ConfigurationRootPath != null)
            {
                if (m_parentname != null)
                {
                    key = $"{ConfigurationRootPath}:{m_parentname}:{PathTypeName}:{name}".Replace('.', ':');
                }
                else
                {
                    // if we're in an array, we may have no type name in the key
                    if (m_isArrayElement)
                    {
                        key = $"{ConfigurationRootPath}:{name}".Replace('.', ':');
                    }
                    else
                    {
                        key = $"{ConfigurationRootPath}:{PathTypeName}:{name}".Replace('.', ':');
                    }
                }
            }
            else if (m_parentname != null)
            {
                key = $"{m_parentname}:{PathTypeName}:{name}".Replace('.', ':');
            }
            else
            {
                key = $"{PathTypeName}:{name}".Replace('.', ':');
            }

            // ReSharper disable once PossibleNullReferenceException
            return this.ConfigurationRoot[key] ?? null;
        }
    }
}
