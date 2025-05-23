using UnityEditor.SceneManagement;
using UnityEditor;
using Common.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class MapTools
{
    [MenuItem("��ͼ����/�������͵�")]
    public static void ExportTelePorters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if(current.isDirty)
        {   //��鵱ǰ��ͼ�Ƿ񱣴�
            EditorUtility.DisplayDialog("��ʾ", "���ȱ��浱ǰ����", "ȷ��");
            return;
        }

        List<TeleporterObject>allTeleporters = new List<TeleporterObject>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("���� {0} û���ҵ�!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("����", string.Format("��ͼ��{0} �����õ�Teleporter:[{1}] �в�����", map.Value.Resource, teleporter.ID), "ȷ��");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("����", string.Format("��ͼ��{0} �����õ�Teleporter:[{1}] MapID:{2} ����", map.Value.Resource, teleporter.ID, def.MapID), "ȷ��");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("��ʾ", "���͵㵼�����", "ȷ��");
    }

    [MenuItem("��ͼ����/����ˢ�ֵ�")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {   //��鵱ǰ��ͼ�Ƿ񱣴�
            EditorUtility.DisplayDialog("��ʾ", "���ȱ��浱ǰ����", "ȷ��");
            return;
        }

        if (DataManager.Instance.SpawnPoints == null)
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("���� {0} û���ҵ�!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }

            foreach (var spawnPoint in spawnPoints)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(spawnPoint.ID))
                    DataManager.Instance.SpawnPoints[map.Value.ID][spawnPoint.ID] = new SpawnPointDefine();

                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][spawnPoint.ID];
                def.ID = spawnPoint.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(spawnPoint.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(spawnPoint.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("��ʾ", "ˢ�ֵ㵼�����", "ȷ��");
    }

    [MenuItem("��ͼ����/���ɵ�������")]
    public static void GenerateNavDate()
    {
        //Material red = new Material(Shader.Find("Particle/ParticleAdditive"));
        //red.color = Color.red;
        //red.SetColor("_TintColor", Color.red);
        //red.enableInstancing  = true;
        GameObject go = GameObject.Find("MiniMapBoudingBox");
        if (go != null)
        {
            GameObject root = new GameObject("Root");
            BoxCollider bound = go.GetComponent<BoxCollider>();
            float step = 1f;
            for(float x = bound.bounds.min.x; x < bound.bounds.max.x; x += step)
            {
                for (float z = bound.bounds.min.z; z < bound.bounds.max.z; z += step)
                {
                    for (float y = bound.bounds.max.y; y < bound.bounds.max.y + 5f; y += step)
                    {
                        var pos = new Vector3(x, y, z);
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                        {
                            if (hit.hit)
                            {
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name = "Hit_" + hit.mask;
                                //box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform, true);
                                box.transform.position = pos;
                                box.transform.localScale = Vector3.one * 0.9f;
                            }
                        }
                    }
                }
            }
        }
    }
}