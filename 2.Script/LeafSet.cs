using FIMSpace.Jiggling;
using UnityEngine;

public class LeafSet : MonoBehaviour
{
    public int line = 0;

    public LeafMake[] leafMakes;

    [Tooltip("성장중")]
    public float satisfyBonScale_0 = 0f;

    [Tooltip("성장후")]
    public float satisfyBonScale_1 = 0f;

    public float changeSizes = 0f;

    public void ChildLeafGet()
    {
        leafMakes = gameObject.GetComponentsInChildren<LeafMake>();
    }

    public bool ChildLeafSet(float bonScale)
    {
        if (leafMakes[0].Checkbonreafon(bonScale, satisfyBonScale_0, satisfyBonScale_1))
        {
            changeSizes = leafMakes[0].changeSize;
            for (int i = 0; i < leafMakes.Length; i++)
            {
                //leafMakes[i].satisfyBonScale_0 = this.satisfyBonScale_0;
                //leafMakes[i].satisfyBonScale_1 = this.satisfyBonScale_1;
                leafMakes[i].changeSize = changeSizes;
                leafMakes[i].GetComponent<FJiggling_Grow>().checkSize = changeSizes;
                leafMakes[i].GetComponent<MainGrowing>().enabled = true;
                leafMakes[i].transform.parent.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
            return true;
        }

        return false;
    }
}