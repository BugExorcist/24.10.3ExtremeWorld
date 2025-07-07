using UnityEditor;
using UnityEngine;

public class PostProcessor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;
        textureImporter.isReadable = false;
        if (assetPath.StartsWith("Assets/UI"))
        {   //�ر������mipmap ���������������ΪSprite
            textureImporter.mipmapEnabled = false;
            textureImporter.textureType = TextureImporterType.Sprite;
        }
        else if (assetPath.StartsWith("Assets/Units"))
        {   //����������Ϊ�Զ�ѹ��
            textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
        }
        else if (assetPath.StartsWith("Assets/FX/Textures"))
        {
            if (textureImporter.textureFormat == TextureImporterFormat.AutomaticTruecolor
                || textureImporter.textureFormat == TextureImporterFormat.Automatic16bit
                || textureImporter.textureFormat == TextureImporterFormat.Alpha8
                || textureImporter.textureFormat == TextureImporterFormat.ARGB16
                || textureImporter.textureFormat == TextureImporterFormat.RGB24
                || textureImporter.textureFormat == TextureImporterFormat.ARGB32
                || textureImporter.textureFormat == TextureImporterFormat.RGBA32
                || textureImporter.textureFormat == TextureImporterFormat.RGB16
                || textureImporter.textureFormat == TextureImporterFormat.RGBA16
                )
            {
                textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
            }
        }
    }
}
