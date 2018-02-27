using System;

namespace XPatchLib
{
    internal class CombineDriveInfo : CombineBase
    {
        internal static string NameName = "Name";

        public CombineDriveInfo(TypeExtend pType) : base(pType)
        {
        }

        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            CombineCore combineCore = new CombineCore(Type);
            object newValue = null;
            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (string.Equals(pName, pReader.Name))
                {
                    pReader.Read();
                    continue;
                }

                if (pReader.NodeType == NodeType.Element)
                {
                    if (!string.Equals(pReader.Name, NameName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    newValue =
                        new CombineCore(TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                            .Combine(pReader, null, NameName);
                }
                pReader.Read();
            }

            return Type.CreateInstance(newValue);
        }
    }
    internal class CombineFileSystemInfo : CombineBase
    {
        internal static string FullPathName = "FullPath";
        internal static string OriginalPathName = "OriginalPath";

        public CombineFileSystemInfo(TypeExtend pType) : base(pType)
        {
        }

        protected override object CombineAction(ITextReader pReader, object pOriObject, string pName)
        {
            CombineCore combineCore = new CombineCore(Type);
            object newValue = null;
            while (!pReader.EOF)
            {
                if (pReader.Name.Equals(pName, StringComparison.OrdinalIgnoreCase) &&
                    pReader.NodeType == NodeType.EndElement)
                    break;

                //pReader.MoveToElement();

                if (string.Equals(pName, pReader.Name))
                {
                    pReader.Read();
                    continue;
                }

                if (pReader.NodeType == NodeType.Element)
                {
                    if (!string.Equals(pReader.Name, OriginalPathName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    newValue =
                        new CombineCore(TypeExtendContainer.GetTypeExtend(pReader.Setting, typeof(string), null, null))
                            .Combine(pReader, null, OriginalPathName);
                }
                pReader.Read();
            }

            return Type.CreateInstance(newValue);
        }
    }
}