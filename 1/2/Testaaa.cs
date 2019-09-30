using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testaaa : MonoBehaviour
{
    //public Terrain terrain;

    public Tree TestTreeObject;
    public TreeInstance treeInstance;
    public TreePrototype treePrototype;
    public TerrainData terrainData;

    public GameObject Tree;

    public List<Tree> treelist;

    private void Awake()
    {
        if (TestTreeObject != null)
        {
            Debug.Log(TestTreeObject.name);

            //var treeData = TestTreeObject as TreeEditor
        }

        TestTreeObject = transform.GetComponent<Tree>();

        //terrain = transform.GetComponent<Terrain>();
        //tree = transform.GetComponent<Tree>();
        //treeInstance = transform.GetComponent<TreeInstance>();
        //treePrototype = transform.GetComponent<TreePrototype>();
        terrainData = transform.GetComponent<TerrainData>();
        //tree = transform.GetComponent<Tree>();

        //if (TestTreeObject != null)
        //{
        //    Debug.Log(TestTreeObject.name);

        //    var treeData = TestTreeObject.data as TreeEditor;

        //    if (treeData != null)
        //    {
        //        var root = treeData.root;

        //        Debug.Log(string.Format("unique ID = {0}", root.uniqueID));
        //        Debug.Log(string.Format("seed = {0}", root.seed));
        //        Debug.Log(string.Format("distribution frequency = {0}", root.distributionFrequency));

        //        var branchGroups = treeData.branchGroups;

        //        Debug.Log(string.Format("{0} branch groups", branchGroups.Length));
        //    }
        //}
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //TreeSeting();
        }
    }

    private void SetTree()
    {
        var data = TestTreeObject.data;

        //TestTreeObject = Tree.GetComponent<Tree>().branch = som

        //data;

        //for (var treeNode : Transform in treeObject.transform)
        //{
        //    for (var branch : Transform in treeNode)
        //    {
        //        for (var leaf : Transform in branch)
        //        {
        //            // use only one of the following lines
        //            leaf.gameObject.active = false; // method 1: disables just the game object
        //            leaf.gameObject.SetActiveRecursively(false); // method 2 : disables the whole hierachy (if leaf has children)
        //            leaf.renderer.enabled = false; // method 3: just disables the renderer, not the object, making it invisible (assuming the leaf has a renderer attached)
        //        }
        //    }
        //}
    }
}

//public void refreshTreeList()
//{
//    for (int i = 0; i < treeColliders.Count; i++)
//    {
//        Destroy(treeColliders[i]);
//    }
//    treeColliders.Clear();
//    Terrain terrain = Terrain.activeTerrain;
//    TerrainData data = terrain.terrainData;
//    trees = data.treeInstances;
//    if (trees.Length > 0)
//    {
//        for (int i = 0; i < trees.Length; i++)
//        {
//            GameObject cube = (GameObject)Instantiate(treeCollider);
//            Vector3 position = Vector3.Scale(trees[i].position, data.size) + terrain.transform.position;
//            cube.transform.position = position;
//            cube.transform.SetParent(this.transform);
//            cube.GetComponent<instanceReference>().tree = trees[i];
//            cube.GetComponent<instanceReference>().treeIndex = i;
//            treeColliders.Add(cube);
//        }
//    }
//}

//private void TreeSeting()
//{
//    Vector3 spawnPos = new Vector3();
//    spawnPos.x = Mathf.InverseLerp(-2048, 2048, transform.position.x); //Set the min and max global values for both X and Z. X and Z can either be grabbed from an object or random.
//    spawnPos.z = Mathf.InverseLerp(-2048, 2048, transform.position.z);
//    spawnPos.y = 0f; //The Y position seems to be automatically handled. The trees are always on the terrain.
//    TreeInstance newTree = new TreeInstance();
//    newTree.color = new Color(1, 1, 1);
//    newTree.lightmapColor = new Color(1, 1, 1);
//    newTree.heightScale = 1;
//    newTree.widthScale = 1;
//    newTree.prototypeIndex = 1; //There are 2 entries here by default. Index 0 is an oak tree, index 1 is a pine tree.
//    newTree.position = spawnPos;
//    newTree.rotation = Random.Range(0f, 180f); //Completely optional for making the trees a little more varied
//    Terrain.activeTerrain.AddTreeInstance(newTree); //Causes a spike in processing time, as all trees and grasses reset. My testing puts this at 50ms +/- 20ms, but only for 1 frame.
//    treelist.refreshTreeList(); //This causes our script for finding trees to refresh. No additional processing time noted in Profiler.

//    //var data = tree.data;

//    //var treein = transform.GetComponent<TreeInstance>();

//    //var prototype = transform.GetComponent<TreePrototype>();

//    //treein.prototypeIndex

//    //    for (float x = 0; x < terrainData.heightmapHeight; x++)
//    //    {
//    //        for (float z = 0; z < terrainData.heightmapHeight; z++)
//    //        {
//    //            Terrain terrain = GetComponent<Terrain>();
//    //            int r = UnityEngine.Random.Range(0, 500);
//    //            if (r == 0)
//    //            {
//    //                TreeInstance treeTemp = new TreeInstance();

//    //                treeTemp.position = new Vector3(x / terrainData.heightmapHeight, 0, z / terrainData.heightmapHeight);

//    //                treeTemp.prototypeIndex = 0;
//    //                treeTemp.widthScale = 1f;
//    //                treeTemp.heightScale = 1f;
//    //                treeTemp.color = Color.white;
//    //                treeTemp.lightmapColor = Color.white;
//    //                terrain.AddTreeInstance(treeTemp);
//    //                terrain.Flush();
//    //            }
//    //        }
//    //    }
//    //}
//}

//private void Tree()
//{
//    //var baseTree = GetComponent<Tree>();

//    //var tmp = baseTree.data as

//    //_treeController = baseTree.data as TreeEditor.TreeData;

//    //_root = _treeController.root as TreeEditor.TreeGroupRoot;

//    //_root.seed = Random.Range(0, 9999999);

//    //_root.UpdateSeed();
//}