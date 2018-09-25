using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjects {

    public static GameObject tGameObject()
    {
        Vector3[] vertices = {
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 0.5f, 1f),
            new Vector3(-1f, 0.5f, 1f)
        };

        int[] triangles = {
            1, 3, 0,
            4, 2, 1,
            3, 2, 5,
            1, 0, 4,
            5, 0, 3,
            1, 2, 3,
            4, 5, 2,
            5, 4, 0 };

        Vector2[] uvs =
        {
            new Vector2(0.292869f, 0.000141f),
            new Vector2(0.585597f, 0.000141f),
            new Vector2(0.585597f, 0.292869f),
            new Vector2(0.585596f, 0.585597f),
            new Vector2(0.292869f, 0.292869f),
            new Vector2(0.000141f, 0.292869f),
        };

        Mesh mesh = new Mesh();
        GameObject go = new GameObject();
        MeshFilter mf = go.AddComponent<MeshFilter>();

        mesh.name = "tGameObject.Name";
        mf.mesh = mesh;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return go;
    }

    public static Material tMaterial()
    {
        int main_texture_id = Shader.PropertyToID("_MainTex");

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.red;

        mat.SetTexture(main_texture_id, tTexture());

        return mat;
    }

    public static Texture2D tTexture()
    {
        int width = 256;
        int height = 256;

        int wBlocks = 8;
        int hBlocks = 8;

        Texture2D texture = new Texture2D(width, height);

        for(int widthIdx = 0; widthIdx < width; ++widthIdx)
        {
            for (int heightIdx = 0; heightIdx < height; ++heightIdx)
            {
                if(((widthIdx / wBlocks) + (heightIdx / hBlocks)) % 2 == 0)
                {
                    texture.SetPixel(widthIdx, heightIdx, Color.black);
                }
                else
                {
                    texture.SetPixel(widthIdx, heightIdx, Color.white);
                }
            }
        }

        texture.Apply();

        return texture;
    }
}
