using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using NJsonSchema.Infrastructure;

namespace PsGui.Projection
{
    public class BasicProjector
    {
        private readonly string[] _primitiveTypes =
        {
            "System.Boolean",
            "System.Byte",
            "System.SByte",
            "System.Char",
            "System.Decimal",
            "System.Double",
            "System.Single",
            "System.Int32",
            "System.UInt32",
            "System.Int64",
            "System.UInt64",
            "System.Int16",
            "System.UInt16",
            "System.String"
        };

        private string TryGetPropertyValue(PSMemberInfo psPropertyInfo)
        {
            try
            {
                return _primitiveTypes.Contains(psPropertyInfo.TypeNameOfValue) ? psPropertyInfo.Value?.ToString() : null;
            }
            catch(GetValueInvocationException)
            {
                return null;
            }
        }

        private string RegisterColumn(ICollection<string> columns, string name)
        {
            if (!columns.Contains(name))
            {
                columns.Add(name);
            }
            return name;
        }

        public Projection Project(Collection<PSObject> objects)
        {
            var columns = new List<string>();
            var id = 0;
            var projection = new Projection
            {
                Metadata = new Metadata(),
                ProjectedObjects = objects.Select(o => o != null ? new ProjectedObject
                {
                    Id = id++,
                    IsNull = false,
                    Value = _primitiveTypes.Any(t => o.TypeNames.Contains(t)) ? o.BaseObject.ToString() : null,
                    Type = string.Join(",", o.TypeNames),
                    Properties = o?.Properties.Select(p => new Property
                    {
                        IsPrimitive = _primitiveTypes.Contains(p.TypeNameOfValue),
                        Value = TryGetPropertyValue(p),
                        IsSettable = p.IsSettable,
                        Name = RegisterColumn(columns, p.Name),
                        Type = p.TypeNameOfValue
                    }).ToList(),
                    Actions = o?.Methods
                        .Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_"))
                        .Select(m => new Action
                        {
                            Name = m.Name
                        }).ToList() 
                } : 
                new ProjectedObject
                {
                    IsNull = true,
                    Id = id++
                }).ToList()
            };
            projection.Metadata.Columns = columns;
            return projection;
        }
    }



    public class Metadata
    {
        public List<string> Columns { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsSettable { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
    }

    public class ProjectedObject
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public bool IsNull { get; set; }
        public string Value { get; set; }
        public List<Property> Properties { get; set; }
        public List<Action> Actions { get; set; }
    }

    public class Projection
    {
        public Metadata Metadata { get; set; }
        public List<ProjectedObject> ProjectedObjects { get; set; }
    }


}
