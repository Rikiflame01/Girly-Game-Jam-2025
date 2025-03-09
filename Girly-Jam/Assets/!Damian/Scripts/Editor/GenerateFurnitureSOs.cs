using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateFurnitureSOs
{
    [MenuItem("Tools/Generate Furniture SOs")]
    public static void Generate()
    {
        string spriteFolder = "Assets/Sprites";
        string targetFolder = "Assets/FurnitureSOs";

        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            AssetDatabase.CreateFolder("Assets", "FurnitureSOs");
        }

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { spriteFolder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (Path.GetExtension(path).ToLower() != ".png")
            {
                continue;
            }

            string filename = Path.GetFileNameWithoutExtension(path);
            int dollarIndex = filename.IndexOf("$");
            if (dollarIndex == -1)
            {
                Debug.LogError($"Invalid filename: no $ found in {path}");
                continue;
            }

            string namePart = filename.Substring(0, dollarIndex).Trim();
            string costPart = filename.Substring(dollarIndex + 1).Trim();

            if (string.IsNullOrEmpty(namePart))
            {
                Debug.LogError($"Empty name in filename: {path}");
                continue;
            }
            if (!int.TryParse(costPart, out int cost))
            {
                Debug.LogError($"Invalid cost in filename: {path}");
                continue;
            }

            string assetPath = $"{targetFolder}/{namePart}.asset";
            Furniture existing = AssetDatabase.LoadAssetAtPath<Furniture>(assetPath);

            if (existing != null)
            {
                existing.itemName = namePart;
                existing.cost = cost;
                EditorUtility.SetDirty(existing); 
            }
            else
            {
                Furniture furniture = ScriptableObject.CreateInstance<Furniture>();
                furniture.itemName = namePart;
                furniture.cost = cost;
                AssetDatabase.CreateAsset(furniture, assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Furniture SOs generated successfully.");
    }
}