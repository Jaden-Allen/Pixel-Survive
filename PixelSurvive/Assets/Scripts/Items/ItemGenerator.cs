using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemGenerator
{
    
}
public static class VoxelData
{
    
    public static Vector3[] voxelVerts (ItemData data) {

        Vector3[] verts = new Vector3[8] {
        new Vector3(0.0f, 0.0f, 0.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(1.0f, 0.0f, 0.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(1.0f, 1.0f, 0.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(0.0f, 1.0f, 0.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(0.0f, 0.0f, 1.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(1.0f, 0.0f, 1.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(1.0f, 1.0f, 1.0f * data.thickness) * (data.size / data.texture.width),
        new Vector3(0.0f, 1.0f, 1.0f * data.thickness) * (data.size / data.texture.width)
        };
        return verts;

    }

    public static readonly int[,] voxelTris = new int[6, 6] {
        { 0,3,1,1,3,2 }, // Front face
        { 5,6,4,4,6,7 }, // Back face
        { 3,7,2,2,7,6 }, // Top face
        { 1,5,0,0,5,4 }, // Bottom face
        { 4,7,0,0,7,3 }, // Left Face
        { 1,2,5,5,2,6 }, // Right Face
    };
    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0,0,-1),
        new Vector3(0,0,1),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(-1,0,0),
        new Vector3(1,0,0)
    };

    public static Vector2[] voxelUvs (Vector3 pos, Texture2D texture) {
        Vector2[] _uvs = new Vector2[6]
        {
            new Vector2((pos.x + 0) / texture.width, (pos.y + 0) / texture.height),
            new Vector2((pos.x + 0) / texture.width, (pos.y + 1) / texture.height),
            new Vector2((pos.x + 1) / texture.width, (pos.y + 0) / texture.height),
            new Vector2((pos.x + 1) / texture.width, (pos.y + 0) / texture.height),
            new Vector2((pos.x + 0) / texture.width, (pos.y + 1) / texture.height),
            new Vector2((pos.x + 1) / texture.width, (pos.y + 1) / texture.height)
        };

        return _uvs;
    }
    public static Mesh CreateItemMesh(ItemData data)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        bool[,] voxelMap = new bool[data.texture.width,data.texture.height];

        int vertexIndex = 0;
        for (int y = 0; y < data.texture.height; y++)
        {
            for (int x = 0; x < data.texture.width; x++)
            {
                Color color = data.texture.GetPixel(x, y);
                
                if (color.a != 0)
                {
                    voxelMap[x,y] = true;
                }
                else
                {
                    voxelMap[x,y] = false;
                }
            }
        }


        bool loadAll = false;
        if (loadAll)
        {
            for (int y = 0; y < data.texture.height; y++)
            {
                for (int x = 0; x < data.texture.width; x++)
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    Color color = data.texture.GetPixel(x, y);

                    if (color.a != 0)
                    {
                        for (int p = 0; p < 6; p++)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Vector3[] voxelVerts = VoxelData.voxelVerts(data);
                                int triangleIndex = voxelTris[p, i];
                                vertices.Add(voxelVerts[triangleIndex] + (pos * (data.size / data.texture.width)));
                                triangles.Add(vertexIndex);

                                Vector2[] _uvs = voxelUvs(pos, data.texture);
                                uvs.Add(_uvs[i]);
                                vertexIndex++;
                            }

                        }
                    }

                }
            }
        }
        else
        {
            for (int y = 0; y < data.texture.height; y++)
            {
                for (int x = 0; x < data.texture.width; x++)
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    Color color = data.texture.GetPixel(x, y);

                    if (color.a != 0)
                    {
                        for (int p = 0; p < 6; p++)
                        {
                            if (p == 0 || p == 1)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    Vector3[] voxelVerts = VoxelData.voxelVerts(data);
                                    int triangleIndex = voxelTris[p, i];
                                    vertices.Add(voxelVerts[triangleIndex] + (pos * (data.size / data.texture.width)));
                                    triangles.Add(vertexIndex);

                                    Vector2[] _uvs = voxelUvs(pos, data.texture);
                                    uvs.Add(_uvs[i]);
                                    vertexIndex++;
                                }
                            }
                            else
                            {
                                if (!CheckVoxel(pos + faceChecks[p], voxelMap, data))
                                {
                                    for (int i = 0; i < 6; i++)
                                    {
                                        Vector3[] voxelVerts = VoxelData.voxelVerts(data);
                                        int triangleIndex = voxelTris[p, i];
                                        vertices.Add(voxelVerts[triangleIndex] + (pos * (data.size / data.texture.width)));
                                        triangles.Add(vertexIndex);

                                        Vector2[] _uvs = voxelUvs(pos, data.texture);
                                        uvs.Add(_uvs[i]);
                                        vertexIndex++;
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        Debug.Log(data.typeId + " { Vertice Count: " + vertices.Count + ", Triangle Count: " + triangles.Count / 3 + " }");
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
    static bool CheckVoxel(Vector2 pos, bool[,] voxelMap, ItemData data)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        if (x < 0 || x > data.texture.width - 1 || y < 0 || y > data.texture.height - 1)
        {
            return false;
        }
        return voxelMap[x, y];
    }
    public static GameObject CreateItemObjectFromData(ItemStack itemStack, Material mat, Transform parent, bool inHand)
    {
        GameObject _item;
        Gun gun;
        if (itemStack.data.item.itemData.customGeometry != null)
        {
            _item = GameObject.Instantiate(itemStack.data.item.itemData.customGeometry, parent);
            gun = _item.GetComponent<Gun>();
            if (gun != null)
            {
                if (itemStack.gunData.magazineAttatchment!= null)
                {
                    gun.magazine = itemStack.gunData.magazineAttatchment;
                }
                if (itemStack.gunData.topRailAttatchment != null)
                {
                    gun.upperRail = itemStack.gunData.topRailAttatchment;
                }
                if (itemStack.gunData.bottomRailAttatchment != null)
                {
                    gun.lowerRail = itemStack.gunData.bottomRailAttatchment;
                }
                if (itemStack.gunData.muzzleAttachment != null)
                {
                    gun.muzzle = itemStack.gunData.muzzleAttachment;
                }
                gun.GenerateWeapon(false);

            }
            _item.name = itemStack.data.item.itemData.typeId;
            _item.transform.localScale = new Vector3(itemStack.data.item.itemData.size, itemStack.data.item.itemData.size, itemStack.data.item.itemData.size);
        }
        else
        {
            _item = new GameObject(itemStack.data.item.itemData.typeId);

            MeshFilter meshFilter = _item.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = _item.AddComponent<MeshRenderer>();

            Material _mat = new Material(mat);

            _mat.mainTexture = itemStack.data.item.itemData.texture;
            meshRenderer.material = _mat;

            meshFilter.mesh = CreateItemMesh(itemStack.data.item.itemData);

            _item.transform.parent = parent;
        }

        if (inHand)
        {
            _item.transform.localPosition = itemStack.data.item.itemData.renderOffset;
            _item.transform.localEulerAngles = itemStack.data.item.itemData.renderRotation;
            _item.layer = 7;
        }
        else
        {
            _item.transform.localPosition = Vector3.zero;
        }

        return _item;
    }
    public static GameObject CreateItemObjectFromData(ItemStack itemStack, Material mat, Transform parent)
    {
        GameObject _item;
        Gun gun;
        if (itemStack.data.item.itemData.customGeometry != null)
        {
            _item = GameObject.Instantiate(itemStack.data.item.itemData.customGeometry, parent);
            gun = _item.GetComponent<Gun>();
            if (gun != null)
            {
                if (itemStack.gunData.magazineAttatchment != null)
                {
                    gun.magazine = itemStack.gunData.magazineAttatchment;
                }
                if (itemStack.gunData.topRailAttatchment != null)
                {
                    gun.upperRail = itemStack.gunData.topRailAttatchment;
                }
                if (itemStack.gunData.bottomRailAttatchment != null)
                {
                    gun.lowerRail = itemStack.gunData.bottomRailAttatchment;
                }
                if (itemStack.gunData.muzzleAttachment != null)
                {
                    gun.muzzle = itemStack.gunData.muzzleAttachment;
                }
                gun.GenerateWeapon(false);

            }
            _item.name = itemStack.data.item.itemData.typeId;
            _item.transform.localScale = new Vector3(itemStack.data.item.itemData.size, itemStack.data.item.itemData.size, itemStack.data.item.itemData.size);
            _item.transform.localPosition = Vector3.zero;
        }
        else
        {
            _item = new GameObject(itemStack.data.item.itemData.typeId);

            MeshFilter meshFilter = _item.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = _item.AddComponent<MeshRenderer>();

            Material _mat = new Material(mat);

            _mat.mainTexture = itemStack.data.item.itemData.texture;
            meshRenderer.material = _mat;

            meshFilter.mesh = CreateItemMesh(itemStack.data.item.itemData);

            _item.transform.parent = parent;
            _item.transform.localPosition = new Vector3(-0.25f, 0f, 0f);
        }
        

        return _item;
    }
    public static GameObject CreateItemObjectFromData(ItemStack itemStack, Material mat, Transform parent, bool inHand, bool isRightHand)
    {
        GameObject _item;
        Gun gun;
        if (itemStack.data.item.itemData.customGeometry != null)
        {
            _item = GameObject.Instantiate(itemStack.data.item.itemData.customGeometry, parent);
            gun = _item.GetComponent<Gun>();
            if (gun != null)
            {
                if (itemStack.gunData.magazineAttatchment != null)
                {
                    gun.magazine = itemStack.gunData.magazineAttatchment;
                }
                if (itemStack.gunData.topRailAttatchment != null)
                {
                    gun.upperRail = itemStack.gunData.topRailAttatchment;
                }
                if (itemStack.gunData.bottomRailAttatchment != null)
                {
                    gun.lowerRail = itemStack.gunData.bottomRailAttatchment;
                }
                if (itemStack.gunData.muzzleAttachment != null)
                {
                    gun.muzzle = itemStack.gunData.muzzleAttachment;
                }
                gun.GenerateWeapon(false);

            }
            _item.name = itemStack.data.item.itemData.typeId;
            _item.transform.localScale = new Vector3(itemStack.data.item.itemData.size, itemStack.data.item.itemData.size, itemStack.data.item.itemData.size);
        }
        else
        {
            _item = new GameObject(itemStack.data.item.itemData.typeId);

            MeshFilter meshFilter = _item.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = _item.AddComponent<MeshRenderer>();

            Material _mat = new Material(mat);

            _mat.mainTexture = itemStack.data.item.itemData.texture;
            meshRenderer.material = _mat;

            meshFilter.mesh = CreateItemMesh(itemStack.data.item.itemData);

            _item.transform.parent = parent;
        }

        if (inHand)
        {
            if (isRightHand)
            {
                _item.transform.localPosition = itemStack.data.item.itemData.renderOffset;
                _item.transform.localEulerAngles = itemStack.data.item.itemData.renderRotation;
                _item.layer = 7;
            }
            else
            {
                _item.transform.localPosition = new Vector3(-1 * itemStack.data.item.itemData.renderOffset.x, itemStack.data.item.itemData.renderOffset.y, itemStack.data.item.itemData.renderOffset.z);
                _item.transform.localEulerAngles = itemStack.data.item.itemData.renderRotation;
                _item.layer = 7;
            }
            
        }
        else
        {
            _item.transform.localPosition = Vector3.zero;
        }

        return _item;
    }

}